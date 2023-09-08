module Process

open Agents
open MyImage
open Kernels
open Brahma.FSharp

/// <summary>
/// Specifies the level of agents support
/// </summary>
type AgentsSupport =
    | Full // Uses a single agent to open, process and save
    | Partial // Uses different agents for each transformation and saving
    | PartialUsingComposition // Uses one agent for transformation and one for save
    | No // Uses naive image processing function

/// <summary>
/// Represents the available image transformations
/// </summary>
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

/// <summary>
/// Represents processing device
/// </summary>
type ProcessingUnits =
    | CPU
    | GPU of Platform

/// <summary>
/// Parses an image transformation and returns the corresponding CPU-based transformation function
/// </summary>
/// <param name="transformation">The transformation type to apply</param>
/// <returns>
/// A CPU-based transformation function corresponding to the specified transformation type
/// </returns>
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

/// <summary>
/// Generates GPU kernel functions for applying various image transformations
/// </summary>
/// <param name="clContext">The OpenCL context for GPU processing</param>
/// <param name="localWorkSize">The local work size for GPU computation</param>
/// <returns>
/// A function that takes a transformation type and returns the corresponding GPU
/// kernel function for applying the specified transformation
/// </returns>
let transformationsParserGPU (clContext: ClContext) (localWorkSize: int) =

    let applyFilterKernel = GPU.applyFilter clContext localWorkSize
    let flipKernel = GPU.flip clContext localWorkSize
    let rotateKernel = GPU.rotate clContext localWorkSize

    fun transformation ->
        match transformation with
        | Gauss -> applyFilterKernel gaussianBlurKernel
        | Sharpen -> applyFilterKernel sharpenKernel
        | Lighten -> applyFilterKernel lightenKernel
        | Darken -> applyFilterKernel darkenKernel
        | Edges -> applyFilterKernel edgesKernel
        | RotationR -> rotateKernel true
        | RotationL -> rotateKernel false
        | FlipV -> flipKernel true
        | FlipH -> flipKernel false

/// <summary>
/// Processes images located at the specified input path and saves the processed images to the specified output path
/// </summary>
/// <param name="inputPath">The path where the input images are located</param>
/// <param name="outputPath">The path where the processed images will be saved</param>
/// <param name="processingUnit">The GPU platform to be used for processing</param>
/// <param name="imageEditorsList">A list of functions to be applied to the images</param>
/// <param name="agentsSupport">Specifies the level of agent support</param>
let processImages inputPath outputPath processingUnit imageEditorsList agentsSupport =

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
                let parsingFunction = transformationsParserGPU clContext 64
                List.map parsingFunction imageEditorsList

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
