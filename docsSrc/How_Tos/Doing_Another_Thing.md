---
title: How To do a second thing
category: How To Guides
categoryindex: 2
index: 2
---

# How-To use CLI

## Overview

---

The CLI accepts several command-line arguments that control the behavior of the image processing application. 
Below is a list of available command-line arguments and their descriptions.

### InputPath

* **Syntax:** `-in <inputPath>`
* **Description:** Specifies the path to a file or directory where the images to be processed are located.

### OutputPath

* **Syntax:** `-out <outputPath>`
* **Description:** Specifies the path to a file or directory where the processed images will be saved.

### AgentsSupport

* **Syntax:** `-agent=<agentType>`
* **Description:** Allows you to specify the [agent strategy](https://polinasavelyeva.github.io/ImageProcessing/reference/process-agentssupport.html) to be used during image processing.  You can choose from the following options:
  * ``Full``, which uses a single agent for opening, processing and saving
  * ``Partial``, which uses different agents for each transformation and saving
  * ``PartialUsingComposition``, which uses one agent for transformation and one for saving
  * ``No``, which uses naive image processing function

### ProcessingUnit

* **Syntax:** `-unit=<unitType>`
* **Description:** Specifies the [processing unit](https://polinasavelyeva.github.io/ImageProcessing/reference/process-processingunits.html) to be used for image processing. You can choose from the following options:
  * ``CPU``
  * ``NvidiaGPU``
  * ``IntelGPU``
  * ``AmdGPU``
  * ``AnyGPU``

### Transformations

- **Syntax:** `<transformation1> <transformation2> ...`
- **Description:** Provides a list of [available transformations](https://polinasavelyeva.github.io/ImageProcessing/reference/process-transformations.html) to be applied during image processing. You can choose from the following options:
  * ``Gauss``
  * ``Sharpen``
  * ``Lighten``
  * ``Darken``
  * ``Edges``
  * ``RotationR``
  * ``RotationL``
  * ``FlipV``
  * ``FlipH``

## Usage

---

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

## Examples

---

Before usage, go to specify directory:

```sh
cd /path/to/ImageProcessing/src/ImageProcessing
```

1. Process images using the CPU with Gauss filter:
   ```sh
   dotnet run -in "input_directory" -out "output_directory" -unit=cpu gauss
   ```

2. Process images using an Nvidia GPU with full agent support and rotations:
   ```sh
   dotnet run -in "input_directory" -out "output_directory" -unit=NvidiaGPU -agent=Full -rotationl -rotationr
   ```
   
