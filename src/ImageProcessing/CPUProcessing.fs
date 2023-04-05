module CPUProcessing

open MyImage

let applyFilter filter (myImage: MyImage) =

    let filterDiameter = (Array2D.length1 filter) / 2
    let filter = toFlatArray filter

    let pixelProcessing p =

        let pw = p % myImage.Width
        let ph = p / myImage.Width

        let dataToHandle =
            [| for i in ph - filterDiameter .. ph + filterDiameter do
                   for j in pw - filterDiameter .. pw + filterDiameter do
                       if i < 0 || i >= myImage.Height || j < 0 || j >= myImage.Width then
                           float32 myImage.Data[p]
                       else
                           float32 myImage.Data[i * myImage.Width + j] |]

        Array.fold2 (fun acc x y -> acc + x * y) 0.0f filter dataToHandle

    MyImage(Array.mapi (fun p _ -> byte (pixelProcessing p)) myImage.Data, myImage.Width, myImage.Height, myImage.Name)

let rotate (isClockwise: bool) (myImage: MyImage) =

    let buffer = Array.zeroCreate (myImage.Width * myImage.Height)
    let weight = System.Convert.ToInt32 isClockwise

    for j in 0 .. myImage.Width - 1 do
        for i in 0 .. myImage.Height - 1 do

            let pw = j * weight + (myImage.Width - 1 - j) * (1 - weight)
            let ph = i * (1 - weight) + (myImage.Height - 1 - i) * weight

            buffer[ph + pw * myImage.Height] <- myImage.Data[j + i * myImage.Width]

    MyImage(buffer, myImage.Height, myImage.Width, myImage.Name)

let flip (isVertical: bool) (myImage: MyImage) =

    let buffer = Array.zeroCreate (myImage.Height * myImage.Width)
    let weight = System.Convert.ToInt32 isVertical

    for j in 0 .. myImage.Width - 1 do
        for i in 0 .. myImage.Height - 1 do

            let pw = (myImage.Width - j - 1) * weight + j * (1 - weight)
            let ph = i * weight + (myImage.Height - i - 1) * (1 - weight)

            buffer[pw + ph * myImage.Width] <- myImage.Data[j + i * myImage.Width]

    MyImage(buffer, myImage.Width, myImage.Height, myImage.Name)
