module ProcessAll

open Agents
open MyImage
open Kernels
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
    | GPU of Platform

let transformationsParserCPU transformation =
    match transformation with
    | Gauss -> CPU.applyFilter gaussianBlurKernel
    | Sharpen -> CPU.applyFilter sharpenKernel
    | Lighten -> CPU.applyFilter lightenKernel
    | Darken -> CPU.applyFilter darkenKernel
    | Edges -> CPU.applyFilter edgesKernel
    | RotationR -> CPU.rotate true
    | RotationL -> CPU.rotate false
    | FlipV -> CPU.rotate true
    | FlipH -> CPU.flip false

let transformationsParserGPU transformation =
    match transformation with
    | Gauss -> GPU.applyFilter gaussianBlurKernel
    | Sharpen -> GPU.applyFilter sharpenKernel
    | Lighten -> GPU.applyFilter lightenKernel
    | Darken -> GPU.applyFilter darkenKernel
    | Edges -> GPU.applyFilter edgesKernel
    | RotationR -> GPU.rotate true
    | RotationL -> GPU.rotate false
    | FlipV -> GPU.flip true
    | FlipH -> GPU.flip false

let processAllFiles inputPath outputPath processingUnit imageEditorsList agentsSupport =

    let listAllImages directory =

        let allowableExtensions =
            [| ".jpg"; ".jpeg"; ".png"; ".gif"; ".webp"; ".pbm"; ".bmp"; ".tga"; ".tiff" |]

        let allFilesSeq = System.IO.Directory.EnumerateFiles directory

        let allowableFilesSeq =
            Seq.filter (fun (path: string) -> Array.contains (System.IO.Path.GetExtension path) allowableExtensions) allFilesSeq

        List.ofSeq allowableFilesSeq

    let filesToProcess =
        if System.IO.File.Exists inputPath then
            [ inputPath ]
        else
            listAllImages inputPath

    let imageEditorsList =
        match processingUnit with
        | CPU -> List.map transformationsParserCPU imageEditorsList
        | GPU platform ->
            if ClDevice.GetAvailableDevices(platform) |> Seq.isEmpty then
                failwith $"No %A{platform} device was found. "
            else
                let clContext = ClContext(ClDevice.GetAvailableDevices(platform) |> Seq.head)
                List.map (fun n -> transformationsParserGPU n clContext 64) imageEditorsList

    match agentsSupport with
    | Full ->
        let imageEditor = List.reduce (>>) imageEditorsList

        let processorsArray =
            Array.init System.Environment.ProcessorCount (fun _ -> imageFullProcessor imageEditor outputPath)

        for file in filesToProcess do
            (Array.minBy (fun (p: MailboxProcessor<pathMessage>) -> p.CurrentQueueLength) processorsArray)
                .Post(Path file)

        for imgProcessor in processorsArray do
            imgProcessor.PostAndReply EOS
    | Partial ->
        let imageProcessor =
            List.foldBack imageProcessor imageEditorsList (imageSaver outputPath)

        for file in filesToProcess do
            imageProcessor.Post(Image(load file))

        imageProcessor.PostAndReply imageMessage.EOS
    | PartialUsingComposition ->
        let imageProcessor =
            imageProcessor (List.reduce (>>) imageEditorsList) (imageSaver outputPath)

        for file in filesToProcess do
            imageProcessor.Post(Image(load file))

        imageProcessor.PostAndReply imageMessage.EOS
    | No ->
        let imageProcessAndSave path =
            let image = load path
            let editedImage = image |> List.reduce (>>) imageEditorsList
            Helper.generatePath outputPath image.Name |> save editedImage

        List.iter imageProcessAndSave filesToProcess
