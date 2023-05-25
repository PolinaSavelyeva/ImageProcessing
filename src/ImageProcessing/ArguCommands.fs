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
    | CPU -> Process.CPU
    | NvidiaGPU -> Process.GPU Brahma.FSharp.Platform.Nvidia
    | IntelGPU -> Process.GPU Brahma.FSharp.Platform.Intel
    | AmdGPU -> Process.GPU Brahma.FSharp.Platform.Amd
    | AnyGPU -> Process.GPU Brahma.FSharp.Platform.Any

type CLIArguments =
    | [<Mandatory; AltCommandLine("-in")>] InputPath of inputPath: string
    | [<Mandatory; AltCommandLine("-out")>] OutputPath of outputPath: string
    | [<AltCommandLine("-agent"); EqualsAssignment>] AgentsSupport of Process.AgentsSupport
    | [<AltCommandLine("-unit"); EqualsAssignment>] ProcessingUnit of ArguProcessingUnits
    | [<Mandatory; MainCommand>] Transformations of list<Process.Transformations>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputPath _ -> "Path to a file or a directory where the images will be processed from."
            | OutputPath _ -> "Path to a file or a directory where the images will be saved."
            | AgentsSupport _ -> "Process files using different agents strategy."
            | ProcessingUnit _ -> "Process files using CPU or GPU."
            | Transformations _ -> "List of available transformations."
