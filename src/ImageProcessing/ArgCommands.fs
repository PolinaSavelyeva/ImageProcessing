module ArgCommands

open Argu

type ArgProcessingUnits =
    | CPU
    | NvidiaGPU
    | IntelGPU
    | AmdGPU
    | AnyGPU

let argProcessingUnitsParser unit =
    match unit with
    | CPU -> ImageProcessing.CPU
    | NvidiaGPU -> ImageProcessing.GPU Brahma.FSharp.Platform.Nvidia
    | IntelGPU -> ImageProcessing.GPU Brahma.FSharp.Platform.Intel
    | AmdGPU -> ImageProcessing.GPU Brahma.FSharp.Platform.Amd
    | AnyGPU -> ImageProcessing.GPU Brahma.FSharp.Platform.Any

type ClIArguments =
    | [<Mandatory; AltCommandLine("-in")>] InputPath of inputPath: string
    | [<Mandatory; AltCommandLine("-out")>] OutputPath of outputPath: string
    | [<AltCommandLine("-agent"); EqualsAssignment>] AgentsSupport of ImageProcessing.AgentsSupport
    | [<AltCommandLine("-unit"); EqualsAssignment>] ProcessingUnit of ArgProcessingUnits
    | [<Mandatory; MainCommand>] Transformations of list<ImageProcessing.Transformations>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputPath _ -> "path to a file or a directory where the images will be processed from."
            | OutputPath _ -> "path to a file or a directory where the images will be saved."
            | AgentsSupport _ -> "process files using different agents strategy."
            | ProcessingUnit _ -> "process files using CPU or GPU."
            | Transformations _ -> "list of available transformations."
