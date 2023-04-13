module GPU

open Kernels
open Expecto
open MyImage

let myConfig =
    { FsCheckConfig.defaultConfig with
        arbitrary = [ typeof<Generators.MyGenerators> ]
        maxTest = 10 }

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

let applyFilterGPU = GPU.applyFilter clContext 64
let rotateGPU = GPU.rotate clContext 64
let flipGPU = GPU.flip clContext 64

[<Tests>]
let tests =

    testList
        "GPUTests"
        [ testPropertyWithConfig myConfig "Vertical/horizontal flip on GPU is equal to vertical/horizontal flip on CPU on generated MyImage"
          <| fun myImage (rotation: bool) ->

              let expectedResult = CPU.flip rotation myImage
              let actualResult = flipGPU rotation myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testPropertyWithConfig myConfig "Clockwise/counterclockwise rotation on GPU is equal to clockwise/counterclockwise rotation on CPU on generated MyImage"
          <| fun myImage (rotation: bool) ->

              let expectedResult = CPU.rotate rotation myImage
              let actualResult = rotateGPU rotation myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testPropertyWithConfig myConfig "Application of the filter (darken) on GPU is equal to the application on CPU on generated MyImage"
          <| fun myImage ->

              let expectedResult = CPU.applyFilter darkenKernel myImage
              let actualResult = applyFilterGPU darkenKernel myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (gauss) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let myImage = load path

              let expectedResult = CPU.applyFilter gaussianBlurKernel myImage
              let actualResult = applyFilterGPU gaussianBlurKernel myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (edges) on CPU is equal to the application on GPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let myImage = load path

              let expectedResult = CPU.applyFilter edgesKernel myImage
              let actualResult = applyFilterGPU edgesKernel myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (sharpen) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/3.jpg"
              let myImage = load path

              let expectedResult = CPU.applyFilter sharpenKernel myImage
              let actualResult = applyFilterGPU sharpenKernel myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (lighten) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let myImage = load path

              let expectedResult = CPU.applyFilter lightenKernel myImage
              let actualResult = applyFilterGPU lightenKernel myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Clockwise rotation on GPU is equal to clockwise rotation on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let myImage = load path

              let expectedResult = CPU.rotate true myImage
              let actualResult = rotateGPU true myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Counterclockwise rotation on GPU is equal to counterclockwise rotation on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let myImage = load path

              let expectedResult = CPU.rotate false myImage
              let actualResult = rotateGPU false myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Two clockwise MyImage rotations is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

              let expectedResult = load actualResultPath

              let actualResult =
                  expectedResult |> rotateGPU true |> rotateGPU true |> rotateGPU true |> rotateGPU true

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testCase "Two counterclockwise MyImage rotations is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

              let expectedResult = load actualResultPath

              let actualResult =
                  expectedResult |> rotateGPU false |> rotateGPU false |> rotateGPU false |> rotateGPU false

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testCase "Vertical flip on GPU is equal to vertical flip on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let myImage = load path

              let expectedResult = CPU.flip true myImage
              let actualResult = flipGPU true myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Horizontal flip on GPU is equal to horizontal flip on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let myImage = load path

              let expectedResult = CPU.flip false myImage
              let actualResult = flipGPU false myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Two vertical/horizontal MyImage flips is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

              let expectedResult = load actualResultPath

              let resultsArray =
                  [| (expectedResult |> flipGPU true |> flipGPU true).Data; (expectedResult |> flipGPU false |> flipGPU false).Data |]

              Expect.allEqual resultsArray expectedResult.Data $"Unexpected: %A{resultsArray}.\n Expected: %A{expectedResult.Data}. "

          ]
