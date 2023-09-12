# ImageProcessing

ImageProcessing is an easy-to-use F# package that utilizes [Brahma.FSharp](https://github.com/YaccConstructor/Brahma.FSharp) and [SixLabors.ImageSharph](https://github.com/SixLabors/ImageSharp). It offers two primary image processing options: CPU and GPU or agent-supported processing, all accessible within the included console application.
## Supported Features
- Loading images from a local source and saving them.
- Processing all images within a specified directory.
- Filtering using one of five kernels, including "Gaussian blur" and "edges".
- Other edits such as 90-degree rotation and flipping.
- Combinations of existing transformations in four different scenarios.
- Ability to utilize all of the features solely through the command line.

##  Documentation
See more information on [GitHub Pages](https://polinasavelyeva.github.io/ImageProcessing/).

## Requirements

Make sure the following requirements are installed on your system:

- [dotnet SDK](https://www.microsoft.com/net/download/core) 3.0 or higher (recommended 6.0+),
- [Mono](http://www.mono-project.com/) if you're on Linux or macOS
or
- [VSCode Dev Container](https://code.visualstudio.com/docs/remote/containers).

## Simple Usage

Before usage, go to specify directory:
```sh
$ cd /path/to/ImageProcessing/src/ImageProcessing
```
To process images from one directory and save them to another, you can use the following commands.

- #### Оne transformation applied to each image in the directory
```sh
$ dotnet run -in /input/path -out /output/path -agent=full -unit=cpu gauss
```
- #### List of transformations that are sequentially applied
```sh
$ dotnet run -in /input/path -out /output/path  -agent=no -unit=anygpu gauss sharpen
```

##  Examples

The final result for all types of transformations and filters:

|Original|Sharpen|
|:-------|:------|
| ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplash2.jpg)  | ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashSharpen.jpg) |

|Gauss|Edges|
|:-------|:------|
| ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashGauss.jpg) | ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashEdges.jpg) |

|Darken|Lighten|
|:-------|:------|
| ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashDarken.jpg)  | ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashLighten.jpg) |


|Rotation R|Rotation L|
|:-------|:------|
| ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashRotation.jpg) | ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashRotationF.jpg) |

|Flip H|Flip V|
|:-------|:------|
| ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashFlipF.jpg) | ![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/david-clode-78YxP3PP05A-unsplashFlip.jpg) |


### Template
To find more building and running options take a look at the [MiniScaffold](https://github.com/TheAngryByrd/MiniScaffold) template.
