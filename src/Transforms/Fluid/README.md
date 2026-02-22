### Fluid Transform
This is a [liquid template language](https://shopify.github.io/liquid) transform for [Transformalize](https://github.com/dalenewman/Transformalize). 
Here is [a liquid cheat sheet](https://www.shopify.com/partners/shopify-cheat-sheet).

It's implemented in .NET using [fluid](https://github.com/sebastienros/fluid) by Sébastien Ros.

### Usage

Here's a test arrangement that reads 5 rows from the `bogus` provider
and uses a short-hand variation of the fluid transform to create the `Score` field.

The liquid template is:

 `{% assign x = Stars | times: Reviewers %}<span>{{ x }}</span>`

The arrangement is:

```xml
<add name="TestProcess" read-only="true">
   <connections>
      <add name="input" provider="bogus" seed="1" />
      <add name="output" provider="console" />
   </connections>
   <entities>
      <add name="Contact" size="5">
         <fields>
            <add name="FirstName" />
            <add name="LastName" />
            <add name="Stars" type="byte" min="1" max="5" />
            <add name="Reviewers" type="int" min="0" max="500" />
         </fields>
         <calculated-fields>
            <add name="Score" raw="true" t="fluid({% assign x = Stars | times: Reviewers %}&lt;span>{{ x }}&lt;/span>)" />
         </calculated-fields>
      </add>
   </entities>
</add>
```

This output is:

```bash
FirstName,LastName,Stars,Reviewers,Score
Justin,Konopelski,5,438,<span>2190</span>
Carmen,Bradtke,1,37,<span>37</span>
Bryan,Boehm,3,172,<span>516</span>
Caleb,Hane,4,78,<span>312</span>
Willie,Tromp,1,65,<span>65</span>
```

### Benchmark
```ini
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
```
| Method                    | Mean     | Error   | StdDev  | Ratio |
|-------------------------- |---------:|--------:|--------:|------:|
| &#39;10000 test rows&#39;         | 204.2 ms | 1.73 ms | 1.62 ms |  1.00 |
| &#39;10000 rows with 1 fluid&#39; | 221.3 ms | 2.12 ms | 1.88 ms |  1.08 |

