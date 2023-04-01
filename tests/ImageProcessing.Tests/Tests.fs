namespace Tests

open Expecto
open ImageProcessing

module RotationTests =
    [<Tests>]
    let tests =
        testList
            "RotationTests"
            [ testCase "360 degree MyImage clockwise rotation is equal to the original"
              <| fun _ ->
                  let actualResultPath =
                      __SOURCE_DIRECTORY__ + "/Images/input/bobby-milan-46dEIq91kHg-unsplash.jpg"

                  let expectedResult = loadAsMyImage actualResultPath

                  let actualResult =
                      expectedResult
                      |> rotateMyImage true
                      |> rotateMyImage true
                      |> rotateMyImage true
                      |> rotateMyImage true

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult}.\n Expected: %A{expectedResult}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              testCase "360 degree MyImage counterclockwise rotation is equal to the original"
              <| fun _ ->
                  let actualResultPath =
                      __SOURCE_DIRECTORY__ + "/Images/input/fallon-michael-g3czzez5lh4-unsplash.jpg"

                  let expectedResult = loadAsMyImage actualResultPath

                  let actualResult =
                      expectedResult
                      |> rotateMyImage false
                      |> rotateMyImage false
                      |> rotateMyImage false
                      |> rotateMyImage false

                  Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult}.\n Expected: %A{expectedResult}. "
                  Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
                  Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

              testProperty "360 degree byte array counter/clockwise rotation is equal to the original"
              <| fun (arr: byte[,]) ->
                  let flatArr = toFlatArray arr
                  let image = MyImage(flatArr, Array2D.length2 arr, Array2D.length1 arr, "Image")

                  let resultsArray =
                      [| (image |> rotateMyImage true |> rotateMyImage true |> rotateMyImage true |> rotateMyImage true)
                             .Data
                         (image |> rotateMyImage false |> rotateMyImage false |> rotateMyImage false |> rotateMyImage false)
                             .Data |]

                  Expect.allEqual resultsArray image.Data $"Unexpected: %A{resultsArray} and original {arr}.\n Expected equality. " ]