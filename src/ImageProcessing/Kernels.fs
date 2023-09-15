module ImageProcessing.Kernels

/// <summary>
/// Represents a Gaussian blur kernel for filter applying on CPU  as a 2D array
/// </summary>
let gaussianBlurKernel =
    array2D [ [ 1; 4; 6; 4; 1 ]; [ 4; 16; 24; 16; 4 ]; [ 6; 24; 36; 24; 6 ]; [ 4; 16; 24; 16; 4 ]; [ 1; 4; 6; 4; 1 ] ]
    |> Array2D.map (fun x -> (float32 x) / 256.0f)

/// <summary>
/// Represents a edges kernel for filter applying on CPU as a 2D array
/// </summary>
let edgesKernel =
    array2D [ [ 0; 0; -1; 0; 0 ]; [ 0; 0; -1; 0; 0 ]; [ 0; 0; 2; 0; 0 ]; [ 0; 0; 0; 0; 0 ]; [ 0; 0; 0; 0; 0 ] ]
    |> Array2D.map float32

/// <summary>
/// Represents a darken kernel for filter applying on CPU as a 2D array
/// </summary>
let darkenKernel = array2D [ [ 2 ] ] |> Array2D.map (fun x -> (float32 x) / 10.0f)

/// <summary>
/// Represents a lighten kernel for filter applying on CPU as a 2D array
/// </summary>
let lightenKernel = array2D [ [ 2 ] ] |> Array2D.map float32

/// <summary>
/// Represents a sharpen kernel for filter applying on CPU as a 2D array
/// </summary>
let sharpenKernel =
    array2D [ [ 0; -1; 0 ]; [ -1; 5; -1 ]; [ 0; -1; -0 ] ] |> Array2D.map float32
