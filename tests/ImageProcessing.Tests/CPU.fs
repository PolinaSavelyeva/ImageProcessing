module CPU

open CPU
open Helper
open Expecto
open Generators

let myConfig =
    { FsCheckConfig.defaultConfig with
        arbitrary = [ typeof<MyGenerators> ]
        maxTest = 10 }

[<Tests>]
let tests =
    testList
        "CPUTests"
        [ testCase "360 degree MyImage counterclockwise rotation is equal to the original on CPU"
          <| fun _ ->

              let result =
                  myImage2 |> rotate false |> rotate false |> rotate false |> rotate false

              Expect.equal result.Data myImage2.Data $"Unexpected: %A{result.Data}.\n Expected: %A{myImage2.Data}. "
              Expect.equal result.Height myImage2.Height $"Unexpected: %A{result.Height}.\n Expected: %A{myImage2.Height}. "
              Expect.equal result.Width myImage2.Width $"Unexpected: %A{result.Width}.\n Expected: %A{myImage2.Width}. "

          testPropertyWithConfig myConfig "360 degree counter/clockwise rotation is equal to the original on CPU on generated MyImage"
          <| fun myImage ->

              let resultsArray =
                  [| (myImage |> rotate true |> rotate true |> rotate true |> rotate true).Data; (myImage |> rotate false |> rotate false |> rotate false |> rotate false).Data |]

              Expect.allEqual resultsArray myImage.Data $"Unexpected: %A{resultsArray} and original {myImage.Data}.\n Expected equality. "

          testCase "Two vertical MyImage flips is equal to the original on CPU"
          <| fun _ ->

              let result = myImage3 |> flip true |> flip true

              Expect.equal result.Data myImage3.Data $"Unexpected: %A{result.Data}.\n Expected: %A{myImage3.Data}. "
              Expect.equal result.Height myImage3.Height $"Unexpected: %A{result.Height}.\n Expected: %A{myImage3.Height}. "
              Expect.equal result.Width myImage3.Width $"Unexpected: %A{result.Width}.\n Expected: %A{myImage3.Width}. "

          testPropertyWithConfig myConfig "Two vertical/horizontal MyImage flips is equal to the original on CPU on generated MyImage"
          <| fun myImage ->

              let resultsArray =
                  [| (myImage |> flip true |> flip true).Data; (myImage |> flip false |> flip false).Data |]

              Expect.allEqual resultsArray myImage.Data $"Unexpected: %A{resultsArray} and original {myImage.Data}.\n Expected equality. " ]
