module ArguCommands

open Argu

type ArguProcessingUnits =
    | CPU
    | NvidiaGPU
    | IntelGPU
    | AmdGPU
    | AnyGPU

let arguProcessingUnitsParser unit =
    match unit with
    | CPU -> ProcessAll.CPU
    | NvidiaGPU -> ProcessAll.GPU Brahma.FSharp.Platform.Nvidia
    | IntelGPU -> ProcessAll.GPU Brahma.FSharp.Platform.Intel
    | AmdGPU -> ProcessAll.GPU Brahma.FSharp.Platform.Amd
    | AnyGPU -> ProcessAll.GPU Brahma.FSharp.Platform.Any

type CLIArguments =
    | [<Mandatory; AltCommandLine("-in")>] InputPath of inputPath: string
    | [<Mandatory; AltCommandLine("-out")>] OutputPath of outputPath: string
    | [<AltCommandLine("-agent"); EqualsAssignment>] AgentsSupport of ProcessAll.AgentsSupport
    | [<AltCommandLine("-unit"); EqualsAssignment>] ProcessingUnit of ArguProcessingUnits
    | [<Mandatory; MainCommand>] Transformations of list<ProcessAll.Transformations>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputPath _ -> "Path to a file or a directory where the images will be processed from."
            | OutputPath _ -> "Path to a file or a directory where the images will be saved."
            | AgentsSupport _ -> "Process files using different agents strategy."
            | ProcessingUnit _ -> "Process files using CPU or GPU."
            | Transformations _ -> "List of available transformations."
