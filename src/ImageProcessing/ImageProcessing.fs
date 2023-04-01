module ImageProcessing

open System
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats
open Brahma.FSharp

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

let loadAsMyImage (filePath: string) =

    let image = Image.Load<L8> filePath
    let buffer = Array.zeroCreate<byte> (image.Width * image.Height)
    image.CopyPixelDataTo(Span<byte> buffer)

    MyImage(buffer, image.Width, image.Height, System.IO.Path.GetFileName filePath)

let toFlatArray array2D =
    seq {
        for x in [ 0 .. (Array2D.length1 array2D) - 1 ] do
            for y in [ 0 .. (Array2D.length2 array2D) - 1 ] do
                yield array2D[x, y]
    }
    |> Array.ofSeq

let gaussianBlurKernel =
    array2D [ [ 1; 4; 6; 4; 1 ]; [ 4; 16; 24; 16; 4 ]; [ 6; 24; 36; 24; 6 ]; [ 4; 16; 24; 16; 4 ]; [ 1; 4; 6; 4; 1 ] ]
    |> Array2D.map (fun x -> (float32 x) / 256.0f)

let edgesKernel =
    array2D [ [ 0; 0; -1; 0; 0 ]; [ 0; 0; -1; 0; 0 ]; [ 0; 0; 2; 0; 0 ]; [ 0; 0; 0; 0; 0 ]; [ 0; 0; 0; 0; 0 ] ]
    |> Array2D.map float32

let darkenKernel = array2D [ [ 2 ] ] |> Array2D.map (fun x -> (float32 x) / 10.0f)

let lightenKernel = array2D [ [ 2 ] ] |> Array2D.map float32

let sharpenKernel =
    array2D [ [ 0; -1; 0 ]; [ -1; 5; -1 ]; [ 0; -1; -0 ] ] |> Array2D.map float32

let applyFilterToMyImage filter (myImage: MyImage) =

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

let rotateMyImage (isClockwise: bool) (myImage: MyImage) =

    let buffer = Array.zeroCreate (myImage.Width * myImage.Height)
    let weight = Convert.ToInt32 isClockwise

    for j in 0 .. myImage.Width - 1 do
        for i in 0 .. myImage.Height - 1 do
            buffer[(i * (1 - weight) + (myImage.Height - 1 - i) * weight)
                   + (j * weight + (myImage.Width - 1 - j) * (1 - weight)) * myImage.Height] <- myImage.Data[j + i * myImage.Width]

    MyImage(buffer, myImage.Height, myImage.Width, myImage.Name)

let saveMyImage (myImage: MyImage) filePath =

    let image = Image.LoadPixelData<L8>(myImage.Data, myImage.Width, myImage.Height)

    image.Save filePath

let listAllImages directory =

    let allowableExtensions =
        [| ".jpg"; ".jpeg"; ".png"; ".gif"; ".webp"; ".pbm"; ".bmp"; ".tga"; ".tiff" |]

    let allFilesSeq = System.IO.Directory.EnumerateFiles directory

    let allowableFilesSeq =
        Seq.filter (fun (path: string) -> Array.contains (System.IO.Path.GetExtension path) allowableExtensions) allFilesSeq

    printfn $"Images in %A{directory} directory : %A{allowableFilesSeq}"
    List.ofSeq allowableFilesSeq

let generatePath outputDirectory (imageName: string) =
    System.IO.Path.Combine(outputDirectory, imageName)

let applyFilterGPUKernel (clContext: ClContext) localWorkSize =

    let kernel =
        <@
            fun (range: Range1D) (image: ClArray<byte>) imageWidth imageHeight (filter: ClArray<float32>) filterDiameter (result: ClArray<byte>) ->
                let p = range.GlobalID0
                let pw = p % imageWidth
                let ph = p / imageWidth
                let mutable res = 0.0f

                for i in ph - filterDiameter .. ph + filterDiameter do
                    for j in pw - filterDiameter .. pw + filterDiameter do

                        let f = filter[(i - ph + filterDiameter) * (2 * filterDiameter + 1) + (j - pw + filterDiameter)]

                        if i < 0 || i >= imageHeight || j < 0 || j >= imageWidth then
                            res <- res + (float32 image[p]) * f
                        else
                            res <- res + (float32 image[i * imageWidth + j]) * f

                result[p] <- byte (int res)
        @>

    let kernel = clContext.Compile kernel

    fun (commandQueue: MailboxProcessor<_>) (filter: ClArray<float32>) filterDiameter (image: ClArray<byte>) imageHeight imageWidth (result: ClArray<byte>) ->

        let ndRange = Range1D.CreateValid(imageHeight * imageWidth, localWorkSize)

        let kernel = kernel.GetKernel()
        commandQueue.Post(Msg.MsgSetArguments(fun () -> kernel.KernelFunc ndRange image imageWidth imageHeight filter filterDiameter result))
        commandQueue.Post(Msg.CreateRunMsg<_, _> kernel)
        result

let applyFiltersGPU (clContext: ClContext) localWorkSize =
    let kernel = applyFilterGPUKernel clContext localWorkSize
    let queue = clContext.QueueProvider.CreateQueue()

    fun (filters: list<float32[,]>) (image: MyImage) ->

        let mutable input =
            clContext.CreateClArray<byte>(image.Data, HostAccessMode.NotAccessible)

        let mutable output = clContext.CreateClArray( image.Data.Length, HostAccessMode.NotAccessible, allocationMode = AllocationMode.Default )

        for filter in filters do

            let filterDiameter = (Array2D.length1 filter) / 2
            let filter = toFlatArray filter
            let clFilter = clContext.CreateClArray<float32>(filter, HostAccessMode.NotAccessible, DeviceAccessMode.ReadOnly)
            let oldInput = input

            input <- kernel queue clFilter filterDiameter input image.Height image.Width output
            output <- oldInput
            queue.Post(Msg.CreateFreeMsg clFilter)

        let result = Array.zeroCreate (image.Height * image.Width)

        let result = queue.PostAndReply(fun ch -> Msg.CreateToHostMsg(input, result, ch))
        queue.Post(Msg.CreateFreeMsg input)
        queue.Post(Msg.CreateFreeMsg output)
        MyImage(result, image.Width, image.Height, image.Name)
