``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT DEBUG
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT


```
|                      Method |        Mean |     Error |    StdDev |
|---------------------------- |------------:|----------:|----------:|
|                  ImageSharp |  1,702.3 μs |  33.97 μs |  67.05 μs |
|     ImageProcessingCompiled |    336.6 μs |   6.67 μs |  14.07 μs |
| ImageProcessingNoneCompiled | 10,126.3 μs | 188.35 μs | 147.05 μs |
|                 ImageMagick |    526.2 μs |   8.33 μs |   6.96 μs |
