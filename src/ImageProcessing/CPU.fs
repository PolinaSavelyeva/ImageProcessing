module ImageProcessing.CPU

open MyImage

/// <summary>
/// Applies a filter to an input image
/// </summary>
/// <param name="filter">The filter kernel to apply to the image</param>
/// <param name="image">The input image to process</param>
/// <returns>A new processed image</returns>
let applyFilter filter (image: MyImage) =

    let filterDiameter = (Array2D.length1 filter) / 2
    let filter = Helper.toFlatArray filter

    let pixelProcessing p =

        let pw = p % image.Width
        let ph = p / image.Width

        let dataToHandle =
            [| for i in ph - filterDiameter .. ph + filterDiameter do
                   for j in pw - filterDiameter .. pw + filterDiameter do
                       if i < 0 || i >= image.Height || j < 0 || j >= image.Width then
                           float32 image.Data[p]
                       else
                           float32 image.Data[i * image.Width + j] |]

        Array.fold2 (fun acc x y -> acc + x * y) 0.0f filter dataToHandle

    MyImage(Array.mapi (fun p _ -> byte (pixelProcessing p)) image.Data, image.Width, image.Height, image.Name)

/// <summary>
/// Rotates an input image either clockwise or counterclockwise
/// </summary>
/// <param name="isClockwise">True to rotate the image clockwise or false for counterclockwise rotation</param>
/// <param name="image">The input image to rotate</param>
/// <returns>A new rotated image</returns>
let rotate (isClockwise: bool) (image: MyImage) =

    let buffer = Array.zeroCreate (image.Width * image.Height)
    let weight = System.Convert.ToInt32 isClockwise

    for j in 0 .. image.Width - 1 do
        for i in 0 .. image.Height - 1 do

            let pw = j * weight + (image.Width - 1 - j) * (1 - weight)
            let ph = i * (1 - weight) + (image.Height - 1 - i) * weight

            buffer[ph + pw * image.Height] <- image.Data[j + i * image.Width]

    MyImage(buffer, image.Height, image.Width, image.Name)

/// <summary>
/// Flips an input image either vertically or horizontally
/// </summary>
/// <param name="isVertical">True to flip the image vertically or false for horizontal flip</param>
/// <param name="image">The input image to flip</param>
/// <returns>A new flipped image</returns>
let flip (isVertical: bool) (image: MyImage) =

    let buffer = Array.zeroCreate (image.Height * image.Width)
    let weight = System.Convert.ToInt32 isVertical

    for j in 0 .. image.Width - 1 do
        for i in 0 .. image.Height - 1 do

            let pw = (image.Width - j - 1) * weight + j * (1 - weight)
            let ph = i * weight + (image.Height - i - 1) * (1 - weight)

            buffer[pw + ph * image.Width] <- image.Data[j + i * image.Width]

    MyImage(buffer, image.Width, image.Height, image.Name)
