### Overview

This is a `Razor` provider and transform for 
[Transformalize](https://github.com/dalenewman/Transformalize) using [RazorEngine](https://github.com/Antaris/RazorEngine) 
for the .NET Framework and [RazorEngineCore](https://github.com/adoconnection/RazorEngineCore) 
for .NET Core.

### Write Usage

```xml
<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='Razor' template='template.cshtml' file='output.html' />
  </connections>
  <entities>
    <add name='Contact' size='1000'>
      <fields>
        <add name='Identity' type='int' primary-key='true' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>
```

This writes 1000 rows of bogus data to *output.html*.

The template *template.cshtml* is passed a [RazorModel](https://github.com/dalenewman/Transformalize.Provider.Razor/blob/master/src/Transformalize.Provider.Razor/RazorModel.cs).  The template in this example looks like this:

```html
@model Transformalize.Providers.Razor.RazorModel
<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Razor Output</title>
</head>
<body>
    <table>
        <thead>
            <tr>
                @foreach (var field in Model.Entity.Fields.Where(f => !f.System)) {
                    <th>@field.Label</th>
                }
            </tr>
        </thead>
        <tbody>
            @foreach (var row in Model.Rows) {
                <tr>
                    @foreach (var field in Model.Entity.Fields.Where(f => !f.System)) {
                        <td>@(row[field])</td>
                    }
                </tr>
                ++Model.Entity.Inserts;
            }
        </tbody>
    </table>
</body>
</html>
```

The table in *output.html* looks like this (clipped for brevity):

<table>
        <thead>
            <tr>
                    <th>Identity</th>
                    <th>FirstName</th>
                    <th>LastName</th>
                    <th>Stars</th>
                    <th>Reviewers</th>
            </tr>
        </thead>
        <tbody>
                <tr>
                        <td>1</td>
                        <td>Justin</td>
                        <td>Konopelski</td>
                        <td>3</td>
                        <td>153</td>
                </tr>
                <tr>
                        <td>2</td>
                        <td>Eula</td>
                        <td>Schinner</td>
                        <td>2</td>
                        <td>41</td>
                </tr>
                <tr>
                        <td>3</td>
                        <td>Tanya</td>
                        <td>Shanahan</td>
                        <td>4</td>
                        <td>412</td>
                </tr>
                <tr>
                        <td>4</td>
                        <td>Emilio</td>
                        <td>Hand</td>
                        <td>4</td>
                        <td>469</td>
                </tr>
                <tr>
                        <td>5</td>
                        <td>Rachel</td>
                        <td>Abshire</td>
                        <td>3</td>
                        <td>341</td>
                </tr>
    </tbody>
</table>

### Benchmark

*Note: Numbers get better with more records.*

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.18363
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
  [Host]       : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.4150.0
  LegacyJitX64 : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit LegacyJIT/clrjit-v4.8.4150.0;compatjit-v4.8.4150.0

Job=LegacyJitX64  Jit=LegacyJit  Platform=X64  
Runtime=Clr  

```
|                    Method |       Mean |    Error |   StdDev | Ratio | RatioSD |
|-------------------------- |-----------:|---------:|---------:|------:|--------:|
|         &#39;10000 test rows&#39; |   871.0 ms | 12.63 ms | 11.82 ms |  1.00 |    0.00 |
| &#39;10000 rows with 1 razor&#39; | 1,222.3 ms | 23.75 ms | 30.03 ms |  1.41 |    0.05 |


``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.201
  [Host]    : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT
  RyuJitX64 : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT

Job=RyuJitX64  Jit=RyuJit  Platform=X64  

```
|                    Method |     Mean |   Error |  StdDev | Ratio |
|-------------------------- |---------:|--------:|--------:|------:|
|         &#39;10000 test rows&#39; | 649.3 ms | 5.51 ms | 5.15 ms |  1.00 |
| &#39;10000 rows with 1 razor&#39; | 850.6 ms | 7.03 ms | 6.58 ms |  1.31 |