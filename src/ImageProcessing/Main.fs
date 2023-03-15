namespace ImageProcessing

open Argu
open ArgCommands
open CPUImageProcessing

module Main =

    [<EntryPoint>]
    let main argv =

        (*let errorHandler =
            ProcessExiter(
                colorizer =
                    function
                    | ErrorCode.HelpText -> None
                    | _ -> Some System.ConsoleColor.DarkYellow
            )

        let parser = ArgumentParser.Create<ClIArguments>(errorHandler = errorHandler)

        match parser.ParseCommandLine argv with

        | res when res.Contains(InputPath) && res.Contains(OutputPath) && res.Contains(Transform) ->

            let inputPath = res.GetResult(InputPath)
            let outputPath = res.GetResult(OutputPath)
            let processor =
                res.GetResult(Transform) |> List.map transformationsParser

            if System.IO.File.Exists inputPath then
                let image = loadAsMyImage inputPath
                let processedImage = List.head processor image
                saveMyImage processedImage outputPath
            else
                processor |> processAllAsMyImage inputPath outputPath

        | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"*)

        let input = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input"
        let output = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output"
        printf $"{CPUImageProcessing.processAllAsMyImage input output [rotateMyImage true]}"

        0
