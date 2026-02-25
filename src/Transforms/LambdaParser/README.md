### Overview

This adds an `eval` transform to Transformalize using [NReco.LambdaParser](https://github.com/nreco/lambdaparser).

### Usage

```xml
<cfg name="Test">
    <entities>
        <add name="Test">
            <rows>
                <add text="SomethingWonderful" number="2" />
            </rows>
            <fields>
                <add name="text" />
                <add name="number" type="int" />
            </fields>
            <calculated-fields>
                <add name="evaluated" t='eval(text + " " + number)' />
            </calculated-fields>
        </add>
    </entities>
</cfg>
```

This produces `SomethingWonderful 2`

### Benchmark

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3194)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 8.0.13 (8.0.1325.6609), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.13 (8.0.1325.6609), X64 RyuJIT AVX2

```
| Method             | Mean     | Error   | StdDev  | Ratio | RatioSD |
|------------------- |---------:|--------:|--------:|------:|--------:|
| &#39;5000 rows&#39;        | 108.9 ms | 1.91 ms | 2.13 ms |  1.00 |    0.03 |
| &#39;5000 rows 1 eval&#39; | 115.0 ms | 1.13 ms | 1.00 ms |  1.06 |    0.02 |
