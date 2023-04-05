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

        if System.IO.File.Exists inputPath then

            let image = load inputPath

            let processor =
                match unit with
                | CPU -> List.map transformationsParserCPU processors |> List.reduce (>>)

                | GPU ->
                    let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
                    let clContext = Brahma.FSharp.ClContext(device)

                    List.map (fun n -> transformationsParserGPU n clContext 64) processors |> List.reduce (>>)

            let processedImage = processor image
            save processedImage outputPath
        else
            processAllFiles inputPath outputPath unit processors agentsSupport

    | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

    0
