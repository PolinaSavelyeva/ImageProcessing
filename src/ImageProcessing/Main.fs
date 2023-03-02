namespace ImageProcessing

open Argu
open ArgCommands
open Helper
open ImageFolderProcessing
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
                res.GetResult(Transform) |> List.map transformationsParser |> funcComposition

            if System.IO.File.Exists inputPath then
                let image = loadAs2DArray inputPath
                let processedImage = processor image
                save2DArrayAsImage processedImage outputPath
            else
                processor |> processAllFiles inputPath outputPath

        | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

        0
