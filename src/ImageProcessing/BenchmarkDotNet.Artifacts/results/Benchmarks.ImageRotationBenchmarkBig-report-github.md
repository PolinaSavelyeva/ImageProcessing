``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT DEBUG
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT


```
|                      Method |      Mean |     Error |    StdDev |    Median |
|---------------------------- |----------:|----------:|----------:|----------:|
|                  ImageSharp | 18.094 ms | 0.1484 ms | 0.1239 ms | 18.113 ms |
|     ImageProcessingCompiled |  1.168 ms | 0.0407 ms | 0.1201 ms |  1.226 ms |
| ImageProcessingNoneCompiled | 10.916 ms | 0.2181 ms | 0.3197 ms | 10.842 ms |
|                 ImageMagick |  7.016 ms | 0.1044 ms | 0.0976 ms |  6.998 ms |
