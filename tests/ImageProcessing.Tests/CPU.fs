module CPU

open CPU
open Expecto
open MyImage

[<Tests>]
let tests =
    testList
        "CPUTests"
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
              let flatArr = Helper.toFlatArray arr
              let image = MyImage(flatArr, Array2D.length2 arr, Array2D.length1 arr, "Image")

              let resultsArray =
                  [| (image |> rotate true |> rotate true |> rotate true |> rotate true).Data; (image |> rotate false |> rotate false |> rotate false |> rotate false).Data |]

              Expect.allEqual resultsArray image.Data $"Unexpected: %A{resultsArray} and original {image.Data}.\n Expected equality. " ]
