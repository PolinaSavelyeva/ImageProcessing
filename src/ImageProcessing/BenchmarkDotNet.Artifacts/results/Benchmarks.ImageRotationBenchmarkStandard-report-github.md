``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT DEBUG
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT


```
|                      Method |        Mean |     Error |    StdDev |
|---------------------------- |------------:|----------:|----------:|
|                  ImageSharp |  8,555.5 μs |  71.97 μs |  60.10 μs |
|     ImageProcessingCompiled |    555.5 μs |  22.42 μs |  65.04 μs |
| ImageProcessingNoneCompiled | 10,310.7 μs | 198.02 μs | 228.04 μs |
|                 ImageMagick |  3,078.9 μs |  56.95 μs |  69.94 μs |
