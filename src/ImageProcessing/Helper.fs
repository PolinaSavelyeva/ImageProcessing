module Helper

/// <summary>
/// Converts a 2D array to a flat 1D array
/// </summary>
/// <param name="array2D">The input 2D array to be flattened</param>
/// <returns>A 1D array containing all the elements of the input 2D array</returns>
let toFlatArray array2D =
    seq {
        for x in [ 0 .. (Array2D.length1 array2D) - 1 ] do
            for y in [ 0 .. (Array2D.length2 array2D) - 1 ] do
                yield array2D[x, y]
    }
    |> Array.ofSeq

/// <summary>
/// Generates a file path by combining an output directory and an image name
/// </summary>
/// <param name="outputDirectory">The directory where the file will be located</param>
/// <param name="imageName">The name of the file or image</param>
/// <returns>A full file path obtained by combining the output directory and image name</returns>
let generatePath outputDirectory (imageName: string) =
    System.IO.Path.Combine(outputDirectory, imageName)
