module ArgCommands

open Argu
open CPUImageProcessing

type Transformations =
    | Gauss
    | Sharpen
    | Lighten
    | Darken
    | Edges
    | RotationR
    | RotationL

let transformationsParser p =
    match p with
    | Gauss -> applyFilterTo2DArray gaussianBlurKernel
    | Sharpen -> applyFilterTo2DArray sharpenKernel
    | Lighten -> applyFilterTo2DArray lightenKernel
    | Darken -> applyFilterTo2DArray darkenKernel
    | Edges -> applyFilterTo2DArray edgesKernel
    | RotationR -> rotate2DArray true
    | RotationL -> rotate2DArray false

type ClIArguments =
    | [<Mandatory; AltCommandLine("-in")>] InputPath of inputPath: string
    | [<Mandatory; AltCommandLine("-out")>] OutputPath of outputPath: string
    | [<Mandatory; MainCommand>] Transform of list<Transformations>

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputPath _ -> "path to a file or a directory where the images will be processed from."
            | OutputPath _ -> "path to a file or a directory where the images will be saved."
            | Transform _ -> "list of available transformations."
