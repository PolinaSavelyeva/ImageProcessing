---
title: Getting Started
category: Tutorials
categoryindex: 1
index: 1
---

# Getting Started with ImageProcessing


## Prepearing

---

### Requirements

First of all, make sure the following requirements are installed on your system:

* [dotnet SDK](https://www.microsoft.com/net/download/core) 3.0 or higher (recommended 6.0+),
* [Mono](http://www.mono-project.com/) if you're on Linux or macOS or
* [VSCode Dev Container](https://code.visualstudio.com/docs/remote/containers).

To find more building and running options take a look at the [MiniScaffold](https://github.com/TheAngryByrd/MiniScaffold) template.

### Package Adding

Go to directory with your ``build.fsproj`` (or ``build.csproj``) file and install ImageProcessing using command line:

```shell
dotnet add package ImageProcessingByPolinaSavelyeva --version 1.0.0
```

For more information visit package main [GitHub page](https://github.com/PolinaSavelyeva/ImageProcessing/pkgs/nuget/ImageProcessing).

## Features

---

The following features are implemented, even for CPU and GPU:

* **Filters**
    * Gaussian Blur
    * Edges
    * Sharpen
    * Lighten
    * Darken


* **Transformations**
    * Clockwise rotation
    * Counterclockwise rotation
    * Vertical flip (Y-axis)
    * Horizontal flip (X-axis)


* **Multi-threaded processing tools**
    * Saving agent
    * Processing agent
    * Full processing agent, i.e saving plus processing
    * Events logger


* **Directory with pictures processing tool**

For detailed descriptions of all features above visit [Api Reference](https://polinasavelyeva.github.io/ImageProcessing/reference/index.html).

## Simple Usage

---

### Using CLI

Before usage, go to specify directory:

```sh
cd /path/to/ImageProcessing/src/ImageProcessing
```
To process images from one directory and save them to another, you can use the following commands.

* **Оne transformation applied to each image in the directory**

```sh
dotnet run -in /input/path -out /output/path -agent=full -unit=cpu gauss
```

* **List of transformations that are sequentially applied**

```sh
dotnet run -in /input/path -out /output/path  -agent=no -unit=anygpu gauss sharpen
```

You can find more details about CLI processing [here](https://polinasavelyeva.github.io/ImageProcessing/How_Tos/Using_A_CLI.html).

### Using Your Own Code

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

Create the same function for GPU. But before it we need to do some steps for diagnosing graphical device.

Define the ``device`` value by specifying the brand of your GPU or whatever the program finds (embedded graphics cards are also suitable). And make OpenCL context of it:

```fsharp
let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)
```

Next, define new values for filter and rotation functions. This action is necessary because of compiling [kernel function](https://polinasavelyeva.github.io/ImageProcessing/How_Tos/Making_A_Code.html#GPU-processing-kernels) once:

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

let applyCustomFilterOnCPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> CPU.applyFilter gaussianBlurKernel
    let rotatedImage = blurredImage |> CPU.rotate true
    
    MyImage.save rotatedImage pathToSave

let device = Brahma.FSharp.ClDevice.GetFirstAppropriateDevice()
let clContext = Brahma.FSharp.ClContext(device)

let applyFilterGPU = GPU.applyFilter clContext 64
let rotateGPU = GPU.rotate clContext 64

let applyCustomFilterOnGPU (image: MyImage) (pathToSave : string) = 
    let blurredImage = image |> applyFilterGPU gaussianBlurKernel
    let rotatedImage = blurredImage |> rotateGPU true
    
    MyImage.save rotatedImage pathToSave
 
let pathToSave = "Path/To/Directory/image_name.jpg"

applyCustomFilterOnCPU myImage pathToSave
applyCustomFilterOnGPU myImage pathToSave
```
