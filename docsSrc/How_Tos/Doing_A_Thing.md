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

For detailed descriptions of all features visit [Api Reference]().


## Simple CPU Example

---

Open library and load image to process:

```fsharp
open ImageProcessing

let myImage = MyImage.load ("Full/Path/To/Images/Folder/image_name.jpg")
```

Create new function which sequentially applies blur filter and clockwise rotation to images and saves it on CPU:

```fsharp
let applyCustomFilterOnCPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> CPU.applyFilter gaussianBlurKernel
    let rotatedImage = blurredImage |> CPU.rotate true
    
    MyImage.save rotatedImage pathToSave
```

The result:

```fsharp
open ImageProcessing

let myImage = MyImage.load ("Full/Path/To/Images/Folder/image_name.jpg")

let applyCustomFilterOnCPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> CPU.applyFilter gaussianBlurKernel
    let rotatedImage = blurredImage |> CPU.rotate true
    
    MyImage.save rotatedImage pathToSave
 
let pathToSave = "Path/To/Directory/image_name.jpg"

applyCustomFilterOnCPU myImage pathToSave
```

## Simple GPU Example

---

Open library and load image to process:

```fsharp
open ImageProcessing

let myImage = MyImage.load ("Full/Path/To/Images/Folder/image_name.jpg")
```

Create new function which sequentially applies blur filter and clockwise rotation to images and saves it on GPU. But before it we need to do some steps for diagnosing graphical device.

Define the ``device`` value by specifying the brand of your GPU or whatever the program finds (embedded graphics cards are also suitable). And make OpenCL context of it:

```fsharp
let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)
```

Next, define new values for filter and rotation functions. This action is necessary because of compiling [kernel function]() once:

```fsharp
let applyFilterGPU = GPU.applyFilter clContext 64
let rotateGPU = GPU.rotate clContext 64
```

And the final function:

```fsharp
let applyCustomFilterOnGPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilterGPU gaussianBlurKernel
    let rotatedImage = blurredImage |> rotateGPU true
    
    MyImage.save rotatedImage pathToSave
```

The result:

```fsharp
open ImageProcessing

let myImage = MyImage.load ("Full/Path/To/Images/Folder/image_name.jpg")

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

let applyCustomFilterOnGPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilterGPU gaussianBlurKernel
    let rotatedImage = blurredImage |> rotateGPU true
    
    MyImage.save rotatedImage pathToSave
 
let pathToSave = "Path/To/Directory/image_name.jpg"

applyCustomFilterOnGPU myImage pathToSave
```
