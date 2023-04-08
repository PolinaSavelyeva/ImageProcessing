module Main

open Argu
open MyImage
open ArgCommands
open ImageProcessing

[<EntryPoint>]
let main argv =

    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some System.ConsoleColor.DarkYellow
        )

    let parser = ArgumentParser.Create<ClIArguments>(errorHandler = errorHandler)

    match parser.ParseCommandLine argv with

    | res when
        res.Contains(InputPath)
        && res.Contains(OutputPath)
        && res.Contains(AgentsSupport)
        && res.Contains(ProcessingUnit)
        && res.Contains(Transformations)
        ->

        let inputPath = res.GetResult(InputPath)
        let outputPath = res.GetResult(OutputPath)
        let processors = res.GetResult(Transformations)
        let unit = res.GetResult(ProcessingUnit)
        let agentsSupport = res.GetResult(AgentsSupport)

        processAllFiles inputPath outputPath (unit |> argProcessingUnitsParser) processors agentsSupport

    | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

    0
