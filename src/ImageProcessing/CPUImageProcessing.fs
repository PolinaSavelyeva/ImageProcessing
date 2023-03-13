module CPUImageProcessing

open System
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats

let loadAs2DArray (filePath: string) =

    let image = Image.Load<L8> filePath
    let height = image.Height
    let width = image.Width
    let result = Array2D.zeroCreate height width

    for i in 0 .. width - 1 do
        for j in 0 .. height - 1 do
            result[j, i] <- image.Item(i, j).PackedValue

    printfn $"H=%A{height} W=%A{width}"
    result

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

let applyFilterTo2DArray (filter: float32[,]) (image2DArray: byte[,]) =

    let height = Array2D.length1 image2DArray
    let width = Array2D.length2 image2DArray
    let filterDiameter = (Array2D.length1 filter) / 2
    let filter = toFlatArray filter

    let pixelProcessing px py =
        let dataToHandle =
            [| for i in px - filterDiameter .. px + filterDiameter do
                   for j in py - filterDiameter .. py + filterDiameter do
                       if i < 0 || i >= height || j < 0 || j >= width then
                           float32 image2DArray[px, py]
                       else
                           float32 image2DArray[i, j] |]

        Array.fold2 (fun acc x y -> acc + x * y) 0.0f filter dataToHandle

    Array2D.mapi (fun x y _ -> byte (pixelProcessing x y)) image2DArray

let rotate2DArray (isClockwise: bool) (image2DArray: byte[,]) =

    let height = Array2D.length1 image2DArray
    let width = Array2D.length2 image2DArray
    let buffer = Array2D.zeroCreate width height
    let weight = Convert.ToInt32 isClockwise

    for j in 0 .. width - 1 do
        for i in 0 .. height - 1 do
            buffer[j * weight + (width - 1 - j) * (1 - weight), i * (1 - weight) + (height - 1 - i) * weight] <- image2DArray[i, j]

    buffer

let save2DArrayAsImage (image2DArray: byte[,]) filePath =

    let height = Array2D.length1 image2DArray
    let width = Array2D.length2 image2DArray
    let image = Image.LoadPixelData<L8>(toFlatArray image2DArray, width, height)

    printfn $"H=%A{height} W=%A{width}"
    image.Save filePath

let listAllImages directory =

    let allowableExtensions =
        [| ".jpg"; ".jpeg"; ".png"; ".gif"; ".webp"; ".pbm"; ".bmp"; ".tga"; ".tiff" |]
    let allFilesSeq = System.IO.Directory.EnumerateFiles directory
    let allowableFilesSeq =
        Seq.filter (fun (path: string) -> Array.contains (System.IO.Path.GetExtension path) allowableExtensions) allFilesSeq

    printfn $"Images in %A{directory} directory : %A{allowableFilesSeq}"
    List.ofSeq allowableFilesSeq

let processAllFiles inputDirectory outputDirectory imageEditor =

    let generatePath (filePath: string) =
        System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileName filePath)

    let imageProcessAndSave path =
        let image = loadAs2DArray path
        let editedImage = imageEditor image
        generatePath path |> save2DArrayAsImage editedImage

    listAllImages inputDirectory |> List.map imageProcessAndSave |> ignore
