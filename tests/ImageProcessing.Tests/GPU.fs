module GPU

open Helper
open Kernels
open Expecto
open Generators

let myConfig =
    { FsCheckConfig.defaultConfig with arbitrary = [ typeof<MyGenerators> ] }

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

let applyFilterGPU = GPU.applyFilter clContext 64
let rotateGPU = GPU.rotate clContext 64
let flipGPU = GPU.flip clContext 64

[<Tests>]
let tests =

    testList
        "GPUTests"
        [ (*testPropertyWithConfig myConfig "Vertical/horizontal flip on GPU is equal to vertical/horizontal flip on CPU on generated MyImage"
          <| fun myImage (rotation: bool) ->

              let expectedResult = CPU.flip rotation myImage
              let actualResult = flipGPU rotation myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testPropertyWithConfig myConfig "Clockwise/counterclockwise rotation on GPU is equal to clockwise/counterclockwise rotation on CPU on generated MyImage"
          <| fun myImage (rotation: bool) ->

              let expectedResult = CPU.rotate rotation myImage
              let actualResult = rotateGPU rotation myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testPropertyWithConfig myConfig "Application of the generated filters on GPU is equal to the application on CPU on generated MyImage"
          <| fun myImage (kernel : Kernel) ->

              let expectedResult = CPU.applyFilter kernel.Data myImage
              let actualResult = applyFilterGPU kernel.Data myImage

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

          testProperty "Application of the filter (darken) on GPU is equal to the application on CPU on real image"
          <| fun _ ->

              let expectedResult = CPU.applyFilter darkenKernel myImage1
              let actualResult = applyFilterGPU darkenKernel myImage1

              Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "
*) testCase "Application of the filter (sharpen) on GPU is equal to the application on CPU on real image"
   <| fun _ ->

       let expectedResult = CPU.applyFilter sharpenKernel myImage4
       let actualResult = applyFilterGPU sharpenKernel myImage4

       Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

   testCase "Application of the filter (lighten) on GPU is equal to the application on CPU on real image"
   <| fun _ ->

       let expectedResult = CPU.applyFilter lightenKernel myImage1
       let actualResult = applyFilterGPU lightenKernel myImage1

       Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

   testCase "Clockwise rotation on GPU is equal to clockwise rotation on CPU on real image"
   <| fun _ ->

       let expectedResult = CPU.rotate true myImage2
       let actualResult = rotateGPU true myImage2

       Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

   testCase "Four counterclockwise MyImage rotations is equal to the original on GPU"
   <| fun _ ->

       let result =
           myImage1 |> rotateGPU false |> rotateGPU false |> rotateGPU false |> rotateGPU false

       Expect.equal result.Data myImage1.Data $"Unexpected: %A{result.Data}.\n Expected: %A{myImage1.Data}. "
       Expect.equal result.Height myImage1.Height $"Unexpected: %A{result.Height}.\n Expected: %A{myImage1.Height}. "
       Expect.equal result.Width myImage1.Width $"Unexpected: %A{result.Width}.\n Expected: %A{myImage1.Width}. "

   testCase "Horizontal flip on GPU is equal to horizontal flip on CPU on real image"
   <| fun _ ->

       let expectedResult = CPU.flip false myImage3
       let actualResult = flipGPU false myImage3

       Expect.equal actualResult.Data expectedResult.Data $"Unexpected: %A{actualResult.Data}.\n Expected: %A{expectedResult.Data}. "

   testCase "Two vertical/horizontal MyImage flips is equal to the original on GPU"
   <| fun _ ->

       let result =
           myImage4 |> rotateGPU false |> rotateGPU false |> rotateGPU false |> rotateGPU false

       Expect.equal result.Data myImage4.Data $"Unexpected: %A{result.Data}.\n Expected: %A{myImage4.Data}. "
       Expect.equal result.Height myImage4.Height $"Unexpected: %A{result.Height}.\n Expected: %A{myImage4.Height}. "
       Expect.equal result.Width myImage4.Width $"Unexpected: %A{result.Width}.\n Expected: %A{myImage4.Width}. " ]
