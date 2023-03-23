namespace ImageProcessing

open Argu
open ArgCommands
open CPUImageProcessing
open Streaming

module Main =

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
            && res.Contains(Transform)
            && res.Contains(AgentsSupport)
            ->

            let inputPath = res.GetResult(InputPath)
            let outputPath = res.GetResult(OutputPath)
            let processor = res.GetResult(Transform) |> List.map transformationsParser
            let agentsSupport = res.GetResult(AgentsSupport)

            if System.IO.File.Exists inputPath then
                let image = loadAsMyImage inputPath
                let processedImage = List.head processor image
                saveMyImage processedImage outputPath
            else
                processAllFiles inputPath outputPath processor agentsSupport

        | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

        0
