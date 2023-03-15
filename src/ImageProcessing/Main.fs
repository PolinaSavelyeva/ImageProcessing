namespace ImageProcessing

open Argu
open ArgCommands
open CPUImageProcessing

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

        | res when res.Contains(InputPath) && res.Contains(OutputPath) && res.Contains(Transform) ->

            let inputPath = res.GetResult(InputPath)
            let outputPath = res.GetResult(OutputPath)
            let processor =
                res.GetResult(Transform) |> List.map transformationsParser

            if System.IO.File.Exists inputPath then
                let image = loadAs2DArray inputPath
                let processedImage = List.head processor image
                save2DArrayAsImage processedImage outputPath
            else
                processor |> processAllFiles inputPath outputPath

        | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

        (*let input = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input"
        let output = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output"
        printf $"{Streaming.processAllFiles input output [rotate2DArray true; applyFilterTo2DArray darkenKernel]}"*)

        0
