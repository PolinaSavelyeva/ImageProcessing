module MyImage

open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats

/// <summary>
/// Encapsulates an image, which includes both the byte pixel data and its associated attributes.
/// </summary>
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

/// <summary>
/// Loads the image located at the specified file path.
/// </summary>
/// <param name="filePath">The path where the image is located. The image name and extension are required.</param>
let load (filePath: string) =

    let image = Image.Load<L8> filePath
    let buffer = Array.zeroCreate<byte> (image.Width * image.Height)
    image.CopyPixelDataTo(System.Span<byte> buffer)

    MyImage(buffer, image.Width, image.Height, System.IO.Path.GetFileName filePath)

/// <summary>
/// Saves the image to the specified directory in the same extension as the input.
/// </summary>
/// <param name="image">Saved image.</param>
/// <param name="filePath">Path to the directory where the image will be saved.</param>
let save (image: MyImage) filePath =

    let image = Image.LoadPixelData<L8>(image.Data, image.Width, image.Height)

    image.Save filePath
