# ImageProcessing

ImageProcessing is an easy-to-use F# package that utilizes [Brahma.FSharp](https://github.com/YaccConstructor/Brahma.FSharp) and [SixLabors.ImageSharph](https://github.com/SixLabors/ImageSharp). It offers two primary image processing options: CPU and GPU or agent-supported processing, all accessible within the included console application.
### Supported Features
- Loading images from a local source and saving them.
- Processing all images within a specified directory.
- Filtering using one of five kernels, including "Gaussian blur" and "edges".
- Other edits such as 90-degree rotation and flipping.
- Combinations of existing transformations in four different scenarios.
- Ability to utilize all of the features solely through the command line.

###  Documentation
Detailed documentation, including additional conceptual explanations, is available for ImageProcessing package.

## Installation

| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `ImageProcessing`              | [![NuGet]()]() | 

### Requirements

Make sure the following requirements are installed on your system:

- [dotnet SDK](https://www.microsoft.com/net/download/core) 3.0 or higher (recommended 6.0+),
- [Mono](http://www.mono-project.com/) if you're on Linux or macOS
or
- [VSCode Dev Container](https://code.visualstudio.com/docs/remote/containers).

### Simple Usage

To process images you can use the following commands:
```sh
$ cd /path/to/ImageProcessing/src/ImageProcessing
$ dotnet run -in /input/path -out /output/path -agent=full -unit=cpu gauss
$ dotnet run -in /input/path -out /output/path  -agent=no -unit=anygpu gauss sharpen
```
The final result for all types of tranformations:
|Original|Sharpen|
|:-------|:------|
| ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplash2.jpg)  | ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashSharpen.jpg) |

|Gauss|Edges|
|:-------|:------|
| ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashGauss.jpg) | ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashEdges.jpg) |

|Darken|Lighten|
|:-------|:------|
| ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashDarken.jpg)  | ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashLighten.jpg) |


|Rotation R|Rotation L|
|:-------|:------|
| ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashRotation.jpg) | ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashRotationF.jpg) |

|Flip H|Flip V|
|:-------|:------|
| ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashFlipF.jpg) | ![image](https://github.com/PolinaSavelyeva/ImageProcessing/blob/package/resources/david-clode-78YxP3PP05A-unsplashFlip.jpg) |

### Template
To find more building and running options take a look at the [MiniScaffold](https://github.com/TheAngryByrd/MiniScaffold) template.
