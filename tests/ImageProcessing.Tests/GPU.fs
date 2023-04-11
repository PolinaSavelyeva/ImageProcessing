module GPU

open Kernels
open Expecto
open MyImage

let myConfig =
    { FsCheckConfig.defaultConfig with
        arbitrary = [ typeof<Generators.MyGenerators> ]
        maxTest = 5 }

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

[<Tests>]
let tests =

    testList
        "GPUTests"
        [ testPropertyWithConfig myConfig "Vertical/horizontal flip on GPU is equal to vertical/horizontal flip on CPU on generated MyImage"
          <| fun myImage (rotation: bool) ->

              let expectedResult = CPU.flip rotation myImage
              let actualResult = GPU.flip rotation clContext 64 myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testPropertyWithConfig myConfig "Clockwise/counterclockwise rotation on GPU is equal to clockwise/counterclockwise rotation on CPU on generated MyImage"
          <| fun myImage (rotation: bool) ->

              let expectedResult = CPU.rotate rotation myImage
              let actualResult = GPU.rotate rotation clContext 64 myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testPropertyWithConfig myConfig "Application of the filter (darken) on GPU is equal to the application on CPU on generated MyImage"
          <| fun myImage ->

              let expectedResult = CPU.applyFilter darkenKernel myImage
              let actualResult = GPU.applyFilter darkenKernel clContext 64 myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (gauss) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let image = load path

              let expectedResult = CPU.applyFilter gaussianBlurKernel image
              let actualResult = GPU.applyFilter gaussianBlurKernel clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (edges) on CPU is equal to the application on GPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let image = load path

              let expectedResult = CPU.applyFilter edgesKernel image
              let actualResult = GPU.applyFilter edgesKernel clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (sharpen) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/3.jpg"
              let image = load path

              let expectedResult = CPU.applyFilter sharpenKernel image
              let actualResult = GPU.applyFilter sharpenKernel clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Application of the filter (lighten) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let image = load path

              let expectedResult = CPU.applyFilter lightenKernel image
              let actualResult = GPU.applyFilter lightenKernel clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Clockwise rotation on GPU is equal to clockwise rotation on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let image = load path

              let expectedResult = CPU.rotate true image
              let actualResult = GPU.rotate true clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Counterclockwise rotation on GPU is equal to counterclockwise rotation on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let image = load path

              let expectedResult = CPU.rotate false image
              let actualResult = GPU.rotate false clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Two clockwise MyImage rotations is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

              let expectedResult = load actualResultPath

              let actualResult =
                  expectedResult
                  |> GPU.rotate true clContext 64
                  |> GPU.rotate true clContext 64
                  |> GPU.rotate true clContext 64
                  |> GPU.rotate true clContext 64

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testCase "Two counterclockwise MyImage rotations is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

              let expectedResult = load actualResultPath

              let actualResult =
                  expectedResult
                  |> GPU.rotate false clContext 64
                  |> GPU.rotate false clContext 64
                  |> GPU.rotate false clContext 64
                  |> GPU.rotate false clContext 64

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testCase "Vertical flip on GPU is equal to vertical flip on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let image = load path

              let expectedResult = CPU.flip true image
              let actualResult = GPU.flip true clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Horizontal flip on GPU is equal to horizontal flip on CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let image = load path

              let expectedResult = CPU.flip false image
              let actualResult = GPU.flip false clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Two vertical/horizontal MyImage flips is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

              let expectedResult = load actualResultPath

              let resultsArray =
                  [| (expectedResult |> GPU.flip true clContext 64 |> GPU.flip true clContext 64)
                         .Data
                     (expectedResult |> GPU.flip false clContext 64 |> GPU.flip false clContext 64)
                         .Data |]

              Expect.allEqual resultsArray expectedResult.Data $"Unexpected: %A{resultsArray}.\n Expected: %A{expectedResult.Data}. "

          ]
