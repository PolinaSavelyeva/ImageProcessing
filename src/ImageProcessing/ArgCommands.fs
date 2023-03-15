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
    | Gauss -> applyFilterToMyImage gaussianBlurKernel
    | Sharpen -> applyFilterToMyImage sharpenKernel
    | Lighten -> applyFilterToMyImage lightenKernel
    | Darken -> applyFilterToMyImage darkenKernel
    | Edges -> applyFilterToMyImage edgesKernel
    | RotationR -> rotateMyImage true
    | RotationL -> rotateMyImage false

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
