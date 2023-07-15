module Benchmarks

open BenchmarkDotNet.Attributes

let smallImage = MyImage.load ("C:\Users\lissa\Документы\ImageProcessing\src\ImageProcessing\BenchmarkImages\300.jpg")
let standardImage = MyImage.load ("C:\Users\lissa\Документы\ImageProcessing\src\ImageProcessing\BenchmarkImages\689.jpg")
let bigImage = MyImage.load ("C:\Users\lissa\Документы\ImageProcessing\src\ImageProcessing\BenchmarkImages\1036.jpg")

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)
let rotateGPU = GPU.rotate clContext 64

type RotationRWithoutCompilation() =

    [<Benchmark>]
    member this.myImage1() = rotateGPU true smallImage

    member this.myImage2() = rotateGPU true standardImage

    member this.myImage3() = rotateGPU true bigImage

type RotationRWithCompilation() =

    [<Benchmark>]
    member this.myImage1() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true smallImage

    member this.myImage2() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true standardImage

    member this.myImage3() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true bigImage
