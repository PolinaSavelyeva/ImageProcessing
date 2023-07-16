module Benchmarks

open BenchmarkDotNet.Attributes
open System
open SixLabors.ImageSharp
open SixLabors.ImageSharp.Processing
open SixLabors.ImageSharp.PixelFormats
open ImageMagick

let smallMyImage =
    MyImage.load (__SOURCE_DIRECTORY__ + "/BenchmarkImages/300.jpg")

let standardMyImage =
    MyImage.load (__SOURCE_DIRECTORY__ + "/BenchmarkImages/689.jpg")

let bigMyImage =
    MyImage.load (__SOURCE_DIRECTORY__ + "/BenchmarkImages/1036.jpg")
let bigBigMyImage =
    MyImage.load (__SOURCE_DIRECTORY__ + "/BenchmarkImages/3024x4032.jpg")

let smallMagickImage = new MagickImage(__SOURCE_DIRECTORY__ + "/BenchmarkImages/300.jpg")
let standardMagickImage = new MagickImage(__SOURCE_DIRECTORY__ + "/BenchmarkImages/689.jpg")
let bigMagickImage = new MagickImage(__SOURCE_DIRECTORY__ + "/BenchmarkImages/1036.jpg")
let bigBigMagickImage = new MagickImage(__SOURCE_DIRECTORY__ + "/BenchmarkImages/3024x4032.jpg")

let smallImage = Image.Load<L8>(__SOURCE_DIRECTORY__ + "/BenchmarkImages/300.jpg")
let standardImage = Image.Load<L8>(__SOURCE_DIRECTORY__ + "/BenchmarkImages/689.jpg")
let bigImage = Image.Load<L8>(__SOURCE_DIRECTORY__ + "/BenchmarkImages/1036.jpg")
let bigBigImage = Image.Load<L8>(__SOURCE_DIRECTORY__ + "/BenchmarkImages/3024x4032.jpg")

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)
let rotateGPU = GPU.rotate clContext 64

type ImageRotationBenchmarkSmall() =

    [<Benchmark>]
    member this.ImageSharp() =
        smallImage.Mutate(fun x -> x.Rotate(90.0f) |> ignore)

    [<Benchmark>]
    member this.ImageProcessingCompiled() = rotateGPU true smallMyImage

    [<Benchmark>]
    member this.ImageProcessingNoneCompiled() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true smallMyImage

    [<Benchmark>]
    member this.ImageMagick() =
         smallMagickImage.Rotate(90.0)

type ImageRotationBenchmarkStandard() =

    [<Benchmark>]
    member this.ImageSharp() =
        standardImage.Mutate(fun x -> x.Rotate(90.0f) |> ignore)

    [<Benchmark>]
    member this.ImageProcessingCompiled() = rotateGPU true standardMyImage

    [<Benchmark>]
    member this.ImageProcessingNoneCompiled() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true standardMyImage

    [<Benchmark>]
    member this.ImageMagick() =
         standardMagickImage.Rotate(90.0)

type ImageRotationBenchmarkBig() =

    [<Benchmark>]
    member this.ImageSharp() =
        bigImage.Mutate(fun x -> x.Rotate(90.0f) |> ignore)

    [<Benchmark>]
    member this.ImageProcessingCompiled() = rotateGPU true bigMyImage

    [<Benchmark>]
    member this.ImageProcessingNoneCompiled() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true bigMyImage

    [<Benchmark>]
    member this.ImageMagick() =
         bigMagickImage.Rotate(90.0)

type ImageRotationBenchmarkBigBig() =

    [<Benchmark>]
    member this.ImageSharp() =
        bigBigImage.Mutate(fun x -> x.Rotate(90.0f) |> ignore)

    [<Benchmark>]
    member this.ImageProcessingCompiled() = rotateGPU true bigBigMyImage

    [<Benchmark>]
    member this.ImageProcessingNoneCompiled() =
        let rotateGPU = GPU.rotate clContext 64
        rotateGPU true bigBigMyImage

    [<Benchmark>]
    member this.ImageMagick() =
         bigBigMagickImage.Rotate(90.0)
