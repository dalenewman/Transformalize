# Transformalize.Provider.Ado

This is an ADO.NET data provider used in Transformalize. 
It uses [Dapper](https://github.com/DapperLib/Dapper). 
Below is a benchmark for inserting 1000 rows of [bogus](https://github.com/bchavez/Bogus) data 
into various [Docker](https://www.docker.com) hosted databases.

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3007/22H2/2022Update/SunValley2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
```
| Method     | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD |
|----------- |----------:|---------:|---------:|----------:|------:|--------:|
| baseline   |  25.29 ms | 0.175 ms | 0.146 ms |  25.29 ms |  1.00 |    0.00 |
| sqlite     |  73.74 ms | 1.365 ms | 1.277 ms |  73.34 ms |  2.91 |    0.05 |
| sqlserver  |  80.32 ms | 1.603 ms | 4.081 ms |  78.40 ms |  3.18 |    0.18 |
| postgresql | 446.57 ms | 4.631 ms | 4.332 ms | 446.73 ms | 17.66 |    0.19 |
| mysql      | 628.71 ms | 7.338 ms | 6.864 ms | 631.74 ms | 24.86 |    0.27 |
