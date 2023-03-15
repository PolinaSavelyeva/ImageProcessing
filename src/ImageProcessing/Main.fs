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

        | res when res.Contains(InputPath) && res.Contains(OutputPath) && res.Contains(Transform) ->

            let inputPath = res.GetResult(InputPath)
            let outputPath = res.GetResult(OutputPath)
            let processor = res.GetResult(Transform) |> List.map transformationsParser

            if System.IO.File.Exists inputPath then
                let image = loadAsMyImage inputPath
                let processedImage = List.head processor image
                saveMyImage processedImage outputPath
            else
                res.Contains(AgentsSupport) |> processAllFilesB inputPath outputPath processor

        | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

        (*let input = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input"
        let output = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output"
        let input1 = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input/bobby-milan-46dEIq91kHg-unsplash.jpg"
        let output1 = "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output/bobby-milan-46dEIq91kHg-unsplash.jpg"

        //let img = loadAsMyImage input1
        //let res = rotateMyImage true img
        //saveMyImage res output1
        let start = System.DateTime.Now
        printf $"{Streaming.processAllFilesB input output [applyFilterToMyImage gaussianBlurKernel; applyFilterToMyImage sharpenKernel]}"
        printfn
            $"TotalTime = %f{(System.DateTime.Now
                              - start)
                                 .TotalMilliseconds}"*)
        0
