---
title: Structure
category: Explanations
categoryindex: 3
index: 1
---

# Structure of ImageProcessing

## Core Concepts

---

### 1. Agents
The code uses agents as a fundamental concept for concurrent and asynchronous processing of images. 
Agents are lightweight, concurrent execution units that can receive and process messages independently.

### 2. Image Processing
Image processing is a key operation performed on images, which involves applying various transformations. 
These transformations can include filters like Gaussian blur, sharpening, and flip operations (both horizontal and vertical).

### 3. CPU and GPU Processing
The code supports image processing on both CPU and GPU platforms. 
CPU processing is done using standard F# functions, while GPU processing utilizes the [Brahma.FSharp](https://github.com/YaccConstructor/Brahma.FSharp) library to leverage the power of Graphics Processing Units for parallel processing.


## Diagram

---

There is a UML diagram representing the dependencies between modules: 

![image](https://raw.githubusercontent.com/PolinaSavelyeva/ImageProcessing/main/resources/image_processing_uml.svg)
