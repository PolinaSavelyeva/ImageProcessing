---
title: How To do a first thing
category: How To Guides
categoryindex: 2
index: 1
---

# How-To make your own code

## Implemented Features

---

ImageProcessing supports the following features, which you can use when implementing your examples:

* **Filters for CPU/GPU**
    * Gaussian Blur
    * Edges
    * Sharpen
    * Lighten
    * Darken


* **Transformations for CPU/GPU**
    * Clockwise rotation
    * Counterclockwise rotation
    * Vertical flip (Y-axis)
    * Horizontal flip (X-axis)


* **Multithreaded processing tools**
    * Saving agent
    * Processing agent
    * Full processing agent, i.e saving plus processing
    * Events logger


* **Directory with pictures processing tool**

    Also you can specify:
  * Processing unit (CPU or GPU)
  * Agents support

For detailed descriptions of all features visit [Api Reference](https://polinasavelyeva.github.io/ImageProcessing/reference/index.html).



## Processing on CPU

---

### CPU filter kernels

Filter kernels are used to call ``applyFilter`` function. 
You can create 2D float32 array as new kernel or use implemented [kernels](https://polinasavelyeva.github.io/ImageProcessing/reference/global-kernels.html) such as:

* Gaussian Blur
* Edges
* Sharpen
* Lighten
* Darken

### Multi-threaded processing on CPU

You can use either single-threaded CPU-based image processing or multi-threaded image processing.
Multithreading is performed using logger and agent functions, which is specified in this [section](https://polinasavelyeva.github.io/ImageProcessing/reference/global-agents.html).

Four of them are implemented:

* Logger, which is used to inform user about status of operations, i.e work of another agents
* Saver agent, which is used to save images stored in queue and to send messages to logger
* Processing agent, which is used to process images stored in queue using specified transformation and to send messages to logger
* Full processing agent, which does both saving and processing 


### Simple CPU Example

Open library and load image to process:

```fsharp
open ImageProcessing.MyImage
open ImageProcessing.CPU

let myImage = load ("Full/Path/To/Images/Folder/image_name.jpg")
```

Create new function which sequentially applies blur filter and clockwise rotation to images and saves the result:

```fsharp
let applyCustomFilterOnCPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilter gaussianBlurKernel
    let rotatedImage = blurredImage |> rotate true
    
    save rotatedImage pathToSave
```

The result:

```fsharp
open ImageProcessing.MyImage
open ImageProcessing.CPU

let myImage = load ("Full/Path/To/Images/Folder/image_name.jpg")

let applyCustomFilterOnCPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilter gaussianBlurKernel
    let rotatedImage = blurredImage |> rotate true
    
    save rotatedImage pathToSave
 
let pathToSave = "Path/To/Directory/image_name.jpg"

applyCustomFilterOnCPU myImage pathToSave
```

## Processing on GPU

---

### GPU filter kernels

Filter kernels are used to call ``applyFilter`` function.
You can create 2D float32 array as new kernel or use implemented [kernels](https://polinasavelyeva.github.io/ImageProcessing/reference/global-kernels.html) such as:

* Gaussian Blur
* Edges
* Sharpen
* Lighten
* Darken

### GPU processing kernels

GPU kernels are used to call GPU-processing functions.
They have specific defining style, so for more information about how they work I recommended to visit [Brahma.Fsharp tutorial](https://yaccconstructor.github.io/Brahma.FSharp/Articles/Custom_Kernels.html).
But if you have no need in creating new GPU kernels just use implemented [ones](https://polinasavelyeva.github.io/ImageProcessing/reference/global-gpu.html), such as:

* ``applyFilterGPUKernel``
* ``rotateGPUKernel``
* ``flipGPUKernel``

All of them take ``clContext`` (which is device's environment abstraction) and ``localWorkSize`` (which shows the size of local work group) as input parameters.

### Multi-threaded processing on GPU

You can use either single-threaded GPU-based image processing or multi-threaded image processing.
Multithreading is performed using logger and agent functions, which specified in this [section](https://polinasavelyeva.github.io/ImageProcessing/reference/global-agents.html).

Four of them are implemented:

* Logger, which is used to inform user about status of operations, i.e work of another agents
* Saver agent, which is used to save images stored in queue and to send messages to logger
* Processing agent, which is used to process images stored in queue using specified transformation and to send messages to logger
* Full processing agent, which does both saving and processing

### Simple GPU Example

Open library and load image to process:

```fsharp
open ImageProcessing.MyImage
open ImageProcessing.GPU

let myImage = load ("Full/Path/To/Images/Folder/image_name.jpg")
```

Before creating our function we need to do some steps for diagnosing graphical device.

Define the ``device`` value by specifying the brand of your GPU or whatever the program finds (embedded graphics cards are also suitable). 
And make OpenCL context of it:

```fsharp
let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)
```

Next, define new values for filter and rotation functions. 
This action is necessary because of compiling [kernel function](https://polinasavelyeva.github.io/ImageProcessing/How_Tos/Doing_A_Thing.html#GPU-processing-kernels) once:

```fsharp
let applyFilterGPU = applyFilter clContext 64
let rotateGPU = rotate clContext 64
```

Create new function which sequentially applies blur filter and clockwise rotation to images and saves the result:

```fsharp
let applyCustomFilterOnGPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilterGPU gaussianBlurKernel
    let rotatedImage = blurredImage |> rotateGPU true
    
    save rotatedImage pathToSave
```

The result:

```fsharp
open ImageProcessing.MyImage
open ImageProcessing.GPU

let myImage = load ("Full/Path/To/Images/Folder/image_name.jpg")

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

let applyFilterGPU = applyFilter clContext 64
let rotateGPU = rotate clContext 64

let applyCustomFilterOnGPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilterGPU gaussianBlurKernel
    let rotatedImage = blurredImage |> rotateGPU true
    
    save rotatedImage pathToSave
 
let pathToSave = "Path/To/Directory/image_name.jpg"

applyCustomFilterOnGPU myImage pathToSave
```

## Processing of Multiple Images via Directories

---

### Process parameters

The ``processImage`` function is designed to process directories with various configuration options. 
It allows you to choose the type of [agent support](https://polinasavelyeva.github.io/ImageProcessing/reference/process-agentssupport.html) for processing:

* ``Full``, which uses a single agent for opening, processing and saving
* ``Partial``, which uses different agents for each transformation and saving
* ``PartialUsingComposition``, which uses one agent for transformation and one for saving
* ``No``, which uses naive image processing function

And define a list of [transformations](https://polinasavelyeva.github.io/ImageProcessing/reference/process-transformations.html) to apply to the image, and specify the [processing unit](https://polinasavelyeva.github.io/ImageProcessing/reference/process-processingunits.html) (CPU or GPU) for the operation. 

### Simple Directory Processing Example

Open library and define directories:

```fsharp
open ImageProcessing.Process
open ImageProcessing.AgentSupport

let inputDirectory = "Full/Path/To/Input/Images/Folder/"
let outputDirectory = "Full/Path/To/Output/Images/Folder/"
```

Define list of [transformations](https://polinasavelyeva.github.io/ImageProcessing/reference/process-transformations.html) and filters that will be used:

```fsharp
let imageEditorsList = [Darken; Edges; RotationL]
```

Note that transformations and filters will be applied sequentially, one-by-one on each image in the specific directory. Choose and define [processing unit](https://polinasavelyeva.github.io/ImageProcessing/reference/process-processingunits.html#CPU) and [multithreading mode](https://polinasavelyeva.github.io/ImageProcessing/reference/process-agentssupport.html).

```fsharp
let processingUnit = GPU Brahma.FSharp.Platform.Nvidia
let agentsSupport = AgentSupport.Full
```

The result:

```fsharp
open ImageProcessing.Process
open ImageProcessing.AgentSupport

let inputDirectory = "Full/Path/To/Input/Images/Folder/"
let outputDirectory = "Full/Path/To/Output/Images/Folder/"

let imageEditorsList = [Darken; Edges; RotationL]

let processingUnit = GPU Brahma.FSharp.Platform.Nvidia
let agentsSupport = AgentSupport.Full

processImages inputDirectory outputDirectory processingUnit imageEditorsList agentsSupport
```

