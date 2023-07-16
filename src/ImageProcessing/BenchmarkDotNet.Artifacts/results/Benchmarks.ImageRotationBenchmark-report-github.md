``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT DEBUG
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT


```
|     Method |     Mean |    Error |   StdDev |
|----------- |---------:|---------:|---------:|
| smallImage | 97.61 μs | 0.324 μs | 0.271 μs |
