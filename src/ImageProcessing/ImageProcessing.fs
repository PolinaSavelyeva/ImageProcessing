module ImageProcessing

open MyImage
open Kernels
open Streaming
open Brahma.FSharp

type AgentsSupport =
    | Full // Uses a single agent to open, process and save
    | Partial // Uses different agents for each transformation and saving
    | PartialUsingComposition // Uses one agent for transformation and one for save
    | No // Uses naive image processing function

type Transformations =
    | Gauss
    | Sharpen
    | Lighten
    | Darken
    | Edges
    | RotationR // Clockwise rotation
    | RotationL // Counterclockwise rotation
    | FlipV // Vertical flip
    | FlipH // Horizontal flip

type ProcessingUnits =
    | CPU
    | GPU

let transformationsParserCPU t =
    match t with
    | Gauss -> CPUProcessing.applyFilter gaussianBlurKernel
    | Sharpen -> CPUProcessing.applyFilter sharpenKernel
    | Lighten -> CPUProcessing.applyFilter lightenKernel
    | Darken -> CPUProcessing.applyFilter darkenKernel
    | Edges -> CPUProcessing.applyFilter edgesKernel
    | RotationR -> CPUProcessing.rotate true
    | RotationL -> CPUProcessing.rotate false
    | FlipV -> CPUProcessing.rotate true
    | FlipH -> CPUProcessing.flip false

let transformationsParserGPU t =
    match t with
    | Gauss -> GPUProcessing.applyFilter gaussianBlurKernel
    | Sharpen -> GPUProcessing.applyFilter sharpenKernel
    | Lighten -> GPUProcessing.applyFilter lightenKernel
    | Darken -> GPUProcessing.applyFilter darkenKernel
    | Edges -> GPUProcessing.applyFilter edgesKernel
    | RotationR -> GPUProcessing.rotate true
    | RotationL -> GPUProcessing.rotate false
    | FlipV -> GPUProcessing.flip true
    | FlipH -> GPUProcessing.flip false

let processAllFiles inputDirectory outputDirectory processingUnit imageEditorsList agentsSupport =

    let listAllImages directory =

        let allowableExtensions =
            [| ".jpg"; ".jpeg"; ".png"; ".gif"; ".webp"; ".pbm"; ".bmp"; ".tga"; ".tiff" |]

        let allFilesSeq = System.IO.Directory.EnumerateFiles directory

        let allowableFilesSeq =
            Seq.filter (fun (path: string) -> Array.contains (System.IO.Path.GetExtension path) allowableExtensions) allFilesSeq

        printfn $"Images in %A{directory} directory : %A{allowableFilesSeq}"
        List.ofSeq allowableFilesSeq

    let filesToProcess = listAllImages inputDirectory

    let imageEditorsList =
        match processingUnit with
        | CPU -> List.map transformationsParserCPU imageEditorsList
        | GPU ->
            let device = ClDevice.GetFirstAppropriateDevice()
            let clContext = ClContext(device)

            List.map (fun n -> transformationsParserGPU n clContext 64) imageEditorsList

    match agentsSupport with
    | Full ->
        let imageEditor = List.reduce (>>) imageEditorsList

        let processorsArray =
            Array.init System.Environment.ProcessorCount (fun _ -> imageFullProcessor imageEditor outputDirectory)

        for file in filesToProcess do
            (Array.minBy (fun (p: MailboxProcessor<pathMessage>) -> p.CurrentQueueLength) processorsArray)
                .Post(Path file)

        for imgProcessor in processorsArray do
            imgProcessor.PostAndReply EOS

    | Partial ->
        let imageProcessor =
            List.foldBack imageProcessor imageEditorsList (imageSaver outputDirectory)

        for file in filesToProcess do
            imageProcessor.Post(Image(load file))

        imageProcessor.PostAndReply imageMessage.EOS
    | PartialUsingComposition ->
        let imageProcessor =
            imageProcessor (List.reduce (>>) imageEditorsList) (imageSaver outputDirectory)

        for file in filesToProcess do
            imageProcessor.Post(Image(load file))

        imageProcessor.PostAndReply imageMessage.EOS
    | No ->
        let imageProcessAndSave path =
            let image = load path
            let editedImage = image |> List.reduce (>>) imageEditorsList
            generatePath outputDirectory image.Name |> save editedImage

        List.iter imageProcessAndSave filesToProcess
