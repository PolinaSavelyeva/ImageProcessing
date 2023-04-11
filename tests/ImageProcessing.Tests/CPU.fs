module CPU

open CPU
open Expecto
open MyImage

let myConfig =
    { FsCheckConfig.defaultConfig with arbitrary = [ typeof<Generators.MyGenerators> ] }

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

          testPropertyWithConfig myConfig "360 degree counter/clockwise rotation is equal to the original on CPU on generated MyImage"
          <| fun myImage ->

              let resultsArray =
                  [| (myImage |> rotate true |> rotate true |> rotate true |> rotate true).Data; (myImage |> rotate false |> rotate false |> rotate false |> rotate false).Data |]

              Expect.allEqual resultsArray myImage.Data $"Unexpected: %A{resultsArray} and original {myImage.Data}.\n Expected equality. "

          testCase "Two vertical MyImage flips is equal to the original on CPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/1.jpg"

              let expectedResult = load actualResultPath

              let actualResult = expectedResult |> flip true |> flip true

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testCase "Two horizontal MyImage flips is equal to the original on CPU"
          <| fun _ ->
              let actualResultPath = __SOURCE_DIRECTORY__ + "/Images/input/2.jpg"

              let expectedResult = load actualResultPath

              let actualResult = expectedResult |> flip false |> flip false

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
              Expect.equal actualResult.Height expectedResult.Height $"Unexpected: %A{actualResult.Height}.\n Expected: %A{expectedResult.Height}. "
              Expect.equal actualResult.Width expectedResult.Width $"Unexpected: %A{actualResult.Width}.\n Expected: %A{expectedResult.Width}. "

          testPropertyWithConfig myConfig "Two vertical/horizontal MyImage flips is equal to the original on CPU on generated MyImage"
          <| fun myImage ->

              let resultsArray =
                  [| (myImage |> flip true |> flip true).Data; (myImage |> flip false |> flip false).Data |]

              Expect.allEqual resultsArray myImage.Data $"Unexpected: %A{resultsArray} and original {myImage.Data}.\n Expected equality. "

          ]
