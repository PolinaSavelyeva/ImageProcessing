module GPU

open Kernels
open Expecto
open MyImage

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

[<Tests>]
let tests =

    testList
        "GPUTests"
        [ testCase "Application of the filter (gauss) on GPU is equal to the application on CPU on real image"
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

          testCase "Vertical flip on the GPU is Vertical flip on the CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
              let image = load path

              let expectedResult = CPU.flip true image
              let actualResult = GPU.flip true clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Horizontal flip on the GPU is Horizontal flip on the CPU on real image"
          <| fun _ ->

              let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
              let image = load path

              let expectedResult = CPU.flip false image
              let actualResult = GPU.flip false clContext 64 image

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testCase "Two vertical MyImage flips is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

              let expectedResult = load actualResultPath

              let actualResult =
                  expectedResult
                  |> GPU.flip true clContext 64
                  |> GPU.flip true clContext 64
                  |> GPU.flip true clContext 64
                  |> GPU.flip true clContext 64

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testCase "Two horizontal MyImage flips is equal to the original on GPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

              let expectedResult = load actualResultPath

              let actualResult =
                  expectedResult
                  |> GPU.flip false clContext 64
                  |> GPU.flip false clContext 64
                  |> GPU.flip false clContext 64
                  |> GPU.flip false clContext 64

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          ]
