module MyImage

open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats

type MyImage =
    val Data: array<byte>
    val Width: int
    val Height: int
    val Name: string

    new(data, width, height, name) =
        { Data = data
          Width = width
          Height = height
          Name = name }

let load (filePath: string) =

    let image = Image.Load<L8> filePath
    let buffer = Array.zeroCreate<byte> (image.Width * image.Height)
    image.CopyPixelDataTo(System.Span<byte> buffer)

    MyImage(buffer, image.Width, image.Height, System.IO.Path.GetFileName filePath)

let save (myImage: MyImage) filePath =

    let image = Image.LoadPixelData<L8>(myImage.Data, myImage.Width, myImage.Height)

    image.Save filePath

let toFlatArray array2D =
    seq {
        for x in [ 0 .. (Array2D.length1 array2D) - 1 ] do
            for y in [ 0 .. (Array2D.length2 array2D) - 1 ] do
                yield array2D[x, y]
    }
    |> Array.ofSeq
