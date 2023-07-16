``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT DEBUG
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT


```
|                      Method |      Mean |     Error |    StdDev |    Median |
|---------------------------- |----------:|----------:|----------:|----------:|
|                  ImageSharp | 20.359 ms | 0.2005 ms | 0.1565 ms | 20.335 ms |
|     ImageProcessingCompiled |  8.058 ms | 0.4255 ms | 1.2545 ms |  8.659 ms |
| ImageProcessingNoneCompiled | 17.838 ms | 0.5313 ms | 1.5666 ms | 17.481 ms |
|                 ImageMagick | 98.143 ms | 1.9464 ms | 3.4089 ms | 96.219 ms |
