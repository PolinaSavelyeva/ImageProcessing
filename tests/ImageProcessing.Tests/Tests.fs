namespace Tests

open Expecto
open FsCheck
open MyImage

module RotationCPUTests =
    open CPUProcessing

    [<Tests>]
    let tests =
        testList
            "RotationTests"
            [ testCase "360 degree MyImage clockwise rotation is equal to the original on CPU"
              <| fun _ ->
                  let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

                  let expectedResult = load actualResultPath

                  let actualResult =
                      expectedResult |> rotate true |> rotate true |> rotate true |> rotate true

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              testCase "360 degree MyImage counterclockwise rotation is equal to the original on CPU"
              <| fun _ ->
                  let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

                  let expectedResult = load actualResultPath

                  let actualResult =
                      expectedResult |> rotate false |> rotate false |> rotate false |> rotate false

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              testProperty "360 degree byte array counter/clockwise rotation is equal to the original on CPU"
              <| fun (arr: byte[,]) ->
                  let flatArr = toFlatArray arr
                  let image = MyImage(flatArr, Array2D.length2 arr, Array2D.length1 arr, "Image")

                  let resultsArray =
                      [| (image |> rotate true |> rotate true |> rotate true |> rotate true).Data; (image |> rotate false |> rotate false |> rotate false |> rotate false).Data |]

                  Expect.allEqual resultsArray image.Data $"Unexpected: %A{resultsArray} and original {image.Data}.\n Expected equality. " ]

module FiltersGPUTests =
    open Kernels

    [<Tests>]
    let tests =

        let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
        let clContext = Brahma.FSharp.ClContext(device)

        let random = System.Random()

        testList
            "FilterTests"
            [ testCase "Application of the filter (gauss) on GPU is equal to the application on CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.applyFilter gaussianBlurKernel image
                  let actualResult = GPUProcessing.applyFilter gaussianBlurKernel clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "Application of the filter (edges) on CPU is equal to the application on GPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.applyFilter edgesKernel image
                  let actualResult = GPUProcessing.applyFilter edgesKernel clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "Application of the filter (sharpen) on GPU is equal to the application on CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/3.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.applyFilter sharpenKernel image
                  let actualResult = GPUProcessing.applyFilter sharpenKernel clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "Application of the filter (lighten) on GPU is equal to the application on CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.applyFilter lightenKernel image
                  let actualResult = GPUProcessing.applyFilter lightenKernel clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testProperty "Application of the filter (darken) on GPU is equal to the application on CPU on arrays2D"
              <| fun (height : PositiveInt, width : PositiveInt) ->

                  let arr = Array.init (height.Get * width.Get) (fun _ -> byte (random.Next(0, 255)))
                  let image = MyImage(arr, height.Get, width.Get, "Image")

                  let expectedResult = CPUProcessing.applyFilter darkenKernel image
                  let actualResult = GPUProcessing.applyFilter darkenKernel clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              ]

module RotationGPUTests =
    open Kernels

    [<Tests>]
    let tests =

        let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
        let clContext = Brahma.FSharp.ClContext(device)

        let random = System.Random()

        testList
            "RotationTests"
            [ testCase "Clockwise rotation on the GPU is equal to clockwise rotation on the CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.rotate true image
                  let actualResult = GPUProcessing.rotate true clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "Counterclockwise rotation on the GPU is equal to counterclockwise rotation on the CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.rotate false  image
                  let actualResult = GPUProcessing.rotate false clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "360 degree MyImage clockwise rotation is equal to the original on GPU"
              <| fun _ ->
                  let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

                  let expectedResult = load actualResultPath

                  let actualResult =
                      expectedResult |> GPUProcessing.rotate true clContext 64 |> GPUProcessing.rotate true clContext 64  |> GPUProcessing.rotate true clContext 64  |> GPUProcessing.rotate true clContext 64

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              testCase "360 degree MyImage counterclockwise rotation is equal to the original on GPU"
              <| fun _ ->
                  let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

                  let expectedResult = load actualResultPath

                  let actualResult =
                      expectedResult |> GPUProcessing.rotate false clContext 64 |> GPUProcessing.rotate false clContext 64  |> GPUProcessing.rotate false clContext 64  |> GPUProcessing.rotate false clContext 64

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              ]

module FlipGPUTests =
    open Kernels

    [<Tests>]
    let tests =

        let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
        let clContext = Brahma.FSharp.ClContext(device)

        let random = System.Random()

        testList
            "FlipTests"
            [ testCase "Vertical flip on the GPU is Vertical flip on the CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.flip true image
                  let actualResult = GPUProcessing.flip true clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "Horizontal flip on the GPU is Horizontal flip on the CPU on real image"
              <| fun _ ->

                  let path = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"
                  let image = load path

                  let expectedResult = CPUProcessing.flip false  image
                  let actualResult = GPUProcessing.flip false clContext 64 image

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

              testCase "Two vertical MyImage flips is equal to the original on GPU"
              <| fun _ ->
                  let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

                  let expectedResult = load actualResultPath

                  let actualResult =
                      expectedResult |> GPUProcessing.flip true clContext 64 |> GPUProcessing.flip true clContext 64  |> GPUProcessing.flip true clContext 64  |> GPUProcessing.flip true clContext 64

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              testCase "Two horizontal MyImage flips is equal to the original on GPU"
              <| fun _ ->
                  let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

                  let expectedResult = load actualResultPath

                  let actualResult =
                      expectedResult |> GPUProcessing.flip false clContext 64 |> GPUProcessing.flip false clContext 64  |> GPUProcessing.flip false clContext 64  |> GPUProcessing.flip false clContext 64

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              ]
