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
            let image = loadAsMyImage inputPath
            // TODO delete common part
            match unit with
            | CPU ->
                let processor = List.map transformationsParserCPU processors |> List.reduce (>>)
                let processedImage = processor image
                saveMyImage processedImage outputPath
            | GPU ->
                let device = ClDevice.GetFirstAppropriateDevice()
                let clContext = ClContext(device)

                let processor = List.map (transformationsParserGPU clContext 64) processors |> List.reduce (>>)
                let processedImage = processor image
                saveMyImage processedImage outputPath
        else
            processAllFiles inputPath outputPath unit processors agentsSupport

    | _ -> printfn $"Unexpected command.\n {parser.PrintUsage()}"

    //let device = ClDevice.GetFirstAppropriateDevice()
    //let context = ClContext(device)
    //let applyFiltersOnGPU = applyFiltersGPU context 64
    //let flipFunc = applyFiltersGPU context 64 gaussianBlurKernel

    //let filters = [ darkenKernel; darkenKernel ]
    //let image = loadAsMyImage "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input/bobby-milan-46dEIq91kHg-unsplash.jpg"
    //let res = applyFiltersOnGPU filters image
    //let res = flipFunc image
    //saveMyImage res "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output/1.jpg"
    //processAllFiles "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input" "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output" applyFiltersOnGPU Full
    //processAllFiles "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/input/1.jpg" "/Users/polinas/Documents/ImageProcessing/tests/ImageProcessing.Tests/Images/output/1.jpg" GPU [Lighten; RotationR; FlipH] No
    0
