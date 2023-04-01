module Main

open Argu
open ArgCommands
open Streaming
open ImageProcessing
open Brahma.FSharp


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

    let device = ClDevice.GetFirstAppropriateDevice()
    let context = ClContext(device)
    let applyFiltersOnGPU = applyFiltersGPU context 64

    let filters = [
        gaussianBlurKernel
        gaussianBlurKernel
        edgesKernel
    ]

    processAllFiles "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input" "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output" applyFiltersOnGPU Full

    0
