# Transforms Reference

Transforms modify field values during processing. They are applied using the `t` shorthand attribute on fields and calculated fields. Multiple transforms can be chained with `.`:

```xml
<add name="Name" t="copy(FirstName).append( ).append(LastName).trim()" />
```

Each entry below shows a real XML configuration, its input and output, and a link to the test that verifies it.

---

## String Transforms

### append

Appends a literal value or field value to a string.

```xml
<rows>
  <add Field1='one' Field2='2' Field3='three' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).append( is 1)' />
  <add name='t2' t='copy(Field2).append( is two)' />
  <add name='t3' t='copy(Field3).append(Field2)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `one` | `copy(Field1).append( is 1)` | `one is 1` |
| `2` | `copy(Field2).append( is two)` | `2 is two` |
| `three` | `copy(Field3).append(Field2)` | `three2` |

Test: [TestAppend.cs — AppendWorks](../test/Test.Unit.Core/TestAppend.cs#L32)
Source: [AppendTransform.cs](../src/Transformalize/Transforms/AppendTransform.cs#L23)

---

### prepend

Prepends a literal value to a string.

```xml
<rows>
  <add Field1='world' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).prepend(hello )' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `world` | `copy(Field1).prepend(hello )` | `hello world` |

Test: [TestMiscTransforms.cs — PrependWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L30)
Source: [PrependTransform.cs](../src/Transformalize/Transforms/PrependTransform.cs#L24)

---

### trim

Removes leading and trailing whitespace. Often chained after other transforms.

```xml
<rows>
  <add input=' Wave 1' />
</rows>
<fields>
  <add name='input' t='trim()' />
</fields>
<calculated-fields>
  <add name='output' t='copy(input).startsWith(W)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| ` Wave 1` | `trim()` | `Wave 1` |

Test: [TrimThenStartsWith.cs — TryIt](../test/Test.Unit.Core/TrimThenStartsWith.cs#L32)
Source: [TrimTransform.cs](../src/Transformalize/Transforms/TrimTransform.cs#L23)

---

### toupper / tolower

Converts to uppercase or lowercase.

```xml
<rows>
  <add Field1='Hello World' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).toupper()' />
  <add name='t2' t='copy(Field1).tolower()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `Hello World` | `toupper()` | `HELLO WORLD` |
| `Hello World` | `tolower()` | `hello world` |

Test: [TestMiscTransforms.cs — UpperAndLowerWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L160)
Source: [ToUpperTransform.cs](../src/Transformalize/Transforms/ToUpperTransform.cs#L23), [ToLowerTransform.cs](../src/Transformalize/Transforms/ToLowerTransform.cs#L24)

---

### left / right

Takes characters from the left or right of a string.

```xml
<rows>
  <add Field1='abcdefgh' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).left(3)' />
  <add name='t2' t='copy(Field1).right(3)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `abcdefgh` | `left(3)` | `abc` |
| `abcdefgh` | `right(3)` | `fgh` |

Test: [TestMiscTransforms.cs — LeftAndRightWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L61)
Source: [LeftTransform.cs](../src/Transformalize/Transforms/LeftTransform.cs#L23), [RightTransform.cs](../src/Transformalize/Transforms/RightTransform.cs#L25)

---

### substring

Extracts a substring by start index and optional length.

```xml
<rows>
  <add name='dalenewman' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(name).substring(4)' />
  <add name='t2' t='copy(name).substring(4,3)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `dalenewman` | `substring(4)` | `newman` |
| `dalenewman` | `substring(4,3)` | `new` |

Test: [TestSubstring.cs — TrySubstring](../test/Test.Unit.Core/TestSubstring.cs#L33)
Source: [SubStringTransform.cs](../src/Transformalize/Transforms/SubStringTransform.cs#L24)

---

### replace

Replaces occurrences of one string with another. Also see [regexreplace](#regexreplace) for pattern-based replacement.

```xml
<parameters>
  <add name='replaced' value='TestEnvironments' t='replace(Test,Production).trimEnd(s).append(s)' />
</parameters>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `TestEnvironments` | `replace(Test,Production).trimEnd(s).append(s)` | `ProductionEnvironments` |

Test: [TransformParameters.cs — A](../test/Test.Unit.Core/TransformParameters.cs#L33)
Source: [ReplaceTransform.cs](../src/Transformalize/Transforms/ReplaceTransform.cs#L24)

---

### insert / remove

Inserts a string at a position or removes characters at a position.

```xml
<rows>
  <add Field1='abcdef' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).insert(3,XY)' />
  <add name='t2' t='copy(Field1).remove(2,2)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `abcdef` | `insert(3,XY)` | `abcXYdef` |
| `abcdef` | `remove(2,2)` | `abef` |

Test: [TestMiscTransforms.cs — InsertAndRemoveWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L127)
Source: [InsertTransform.cs](../src/Transformalize/Transforms/InsertTransform.cs#L23), [RemoveTransform.cs](../src/Transformalize/Transforms/RemoveTransform.cs#L23)

---

### padleft / padright

Pads a string to a total width with a specified character.

```xml
<rows>
  <add Field1='42' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).padleft(5,0)' />
  <add name='t2' t='copy(Field1).padright(5,.)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `42` | `padleft(5,0)` | `00042` |
| `42` | `padright(5,.)` | `42...` |

Test: [TestMiscTransforms.cs — PadLeftAndPadRightWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L94)
Source: [PadLeftTransform.cs](../src/Transformalize/Transforms/PadLeftTransform.cs#L24), [PadRightTransform.cs](../src/Transformalize/Transforms/PadRightTransform.cs#L23)

---

### ellipsis

Truncates a string to a maximum length, appending `...` (or a custom suffix) if it exceeds the limit.

```xml
<rows>
  <add desc='Once upon a time there was a dude' />
  <add desc='The end' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(desc).ellipsis(15)' />
  <add name='t2' t='copy(desc).ellipsis(5, blah blah blah)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `Once upon a time there was a dude` | `ellipsis(15)` | `Once upon a tim...` |
| `The end` | `ellipsis(15)` | `The end` |
| `Once upon a time there was a dude` | `ellipsis(5, blah blah blah)` | `Once blah blah blah` |

Test: [TestEllipsis.cs — TryEllipsis](../test/Test.Unit.Core/TestEllipsis.cs#L33)
Source: [EllipsisTransform.cs](../src/Transformalize/Transforms/EllipsisTransform.cs#L23)

---

### condense

Collapses repeated characters (default: spaces).

```xml
<rows>
  <add Field1='Some  duplicates spa ces' />
  <add Field1='O n e  S p a c ee' />
</rows>
<calculated-fields>
  <add name='cf1' t='copy(Field1).condense()' />
  <add name='cf2' t='copy(Field1).condense(e)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `Some  duplicates spa ces` | `condense()` | `Some duplicates spa ces` |
| `O n e  S p a c ee` | `condense(e)` | `O n e  S p a c e` |

Test: [TestCondense.cs — Condense](../test/Test.Unit.Core/TestCondense.cs#L32)
Source: [CondenseTransform.cs](../src/Transformalize/Transforms/CondenseTransform.cs#L22)

---

### format

Applies a format string using positional (`{0}`) or named (`{Field1}`) placeholders.

```xml
<rows>
  <add Field1='1' Field2='2' Field3='3' />
</rows>
<calculated-fields>
  <add name='Format' t='copy(Field1,Field2,Field3).format({0}-{1}+{2} ).trim()' />
  <add name='BetterFormat' t='format({Field1}-{Field2}+{Field3} ).trim()' />
  <add name='WithFormat' t='format({Field1:#.0} and {Field3:000.0000})' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1, 2, 3` | `format({0}-{1}+{2})` | `1-2+3` |
| `1, 2, 3` | `format({Field1}-{Field2}+{Field3})` | `1-2+3` |
| `1, 3` | `format({Field1:#.0} and {Field3:000.0000})` | `1 and 003.0000` |

Test: [TestFormat.cs — FormatTransformer](../test/Test.Unit.Core/TestFormat.cs#L32)
Source: [FormatTransform.cs](../src/Transformalize/Transforms/FormatTransform.cs#L25), [BetterFormat.cs](../src/Transformalize/Transforms/BetterFormat.cs#L26)

---

### tag

Wraps a value in an HTML/XML tag, with optional attributes.

```xml
<rows>
  <add Field1='1' />
</rows>
<calculated-fields>
  <add name='span' t='copy(Field1).tag(span)' />
  <add name='div' t='copy(Field1).tag(div,class:fun)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1` | `tag(span)` | `<span>1</span>` |
| `1` | `tag(div,class:fun)` | `<div class="fun">1</div>` |

Test: [TagTransform.cs — Tag](../test/Test.Unit.Core/TagTransform.cs#L32)
Source: [TagTransform.cs](../src/Transformalize/Transforms/TagTransform.cs#L26)

---

### formatxml

Pretty-prints XML with proper indentation.

```xml
<rows>
  <add xml='&lt;stuff value="1"&gt;&lt;things&gt;&lt;add item="1"/&gt;&lt;/things&gt;&lt;/stuff&gt;' />
</rows>
<calculated-fields>
  <add name='formatted' t='copy(xml).formatXml()' />
</calculated-fields>
```

| Input | Output |
|-------|--------|
| `<stuff value="1"><things><add item="1"/></things></stuff>` | Indented XML with newlines |

Test: [FormatXmlTransform.cs — FormatXmlTransform1](../test/Test.Unit.Core/FormatXmlTransform.cs#L55)
Source: [FormatXmlTransform.cs](../src/Transformalize/Transforms/FormatXmlTransform.cs#L28)

---

### concat

Concatenates multiple field values into one string.

```xml
<rows>
  <add Field1='a' Field2='b' Field3='c' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1,Field2,Field3).concat()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `a`, `b`, `c` | `concat()` | `abc` |

Test: [TestMiscTransforms.cs — ConcatWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L810)
Source: [ConcatTransform.cs](../src/Transformalize/Transforms/ConcatTransform.cs#L24)

---

### reverse

Reverses array elements. To reverse a string, split it first.

```xml
<rows>
  <add Field1='abc' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).split().reverse().join()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `abc` | `split().reverse().join()` | `cba` |

Test: [TestMiscTransforms.cs — ReverseWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L193)
Source: [ReverseTransform.cs](../src/Transformalize/Transforms/ReverseTransform.cs#L25)

---

## Copy and Coalesce

### copy

Copies the value of one or more fields into a calculated field.

```xml
<rows>
  <add Field1='1' Field2='2' />
</rows>
<calculated-fields>
  <add name='Field3' type='int' t='copy(Field2)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| Field2=`2` | `copy(Field2)` | `2` |

Test: [TestCopy.cs — CopyTransform1](../test/Test.Unit.Core/TestCopy.cs#L32)
Source: [CopyTransform.cs](../src/Transformalize/Transforms/CopyTransform.cs#L26)

---

### coalesce

Returns the first non-empty value from a list of fields.

```xml
<rows>
  <add Field1='' Field2='' Field3='found' />
</rows>
<fields>
  <add name='Field1' default='' />
  <add name='Field2' default='' />
  <add name='Field3' default='' />
</fields>
<calculated-fields>
  <add name='t1' t='copy(Field1,Field2,Field3).coalesce()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `""`, `""`, `found` | `coalesce()` | `found` |

Test: [TestMiscTransforms.cs — CoalesceWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L488)
Source: [CoalesceTransform.cs](../src/Transformalize/Transforms/CoalesceTransform.cs#L25)

---

## Math Transforms

### add / sum

Adds numeric values from multiple fields.

```xml
<rows>
  <add Field1='10.6954' Field2='129.992' Field3='7' />
</rows>
<calculated-fields>
  <add name='Add' type='decimal' t='copy(Field1,Field2,Field3).add()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `10.6954`, `129.992`, `7` | `add()` | `147.6874` |

Test: [MathTransforms.cs — DoMath](../test/Test.Unit.Core/MathTransforms.cs#L32)
Source: [AddTransform.cs](../src/Transformalize/Transforms/AddTransform.cs#L24)

---

### multiply

Multiplies values from multiple fields.

```xml
<rows>
  <add Field1='5' Field2='3' />
</rows>
<calculated-fields>
  <add name='t1' type='int' t='copy(Field1,Field2).multiply()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `5`, `3` | `multiply()` | `15` |

Test: [TestMiscTransforms.cs — MultiplyWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L456)
Source: [MultiplyTransform.cs](../src/Transformalize/Transforms/MultiplyTransform.cs#L25)

---

### ceiling / floor

Rounds up or down to the nearest integer.

```xml
<rows>
  <add Field1='10.6954' />
</rows>
<calculated-fields>
  <add name='Ceiling' type='double' t='copy(Field1).ceiling()' />
  <add name='Floor' type='double' t='copy(Field1).floor()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `10.6954` | `ceiling()` | `11` |
| `10.6954` | `floor()` | `10` |

Test: [MathTransforms.cs — DoMath](../test/Test.Unit.Core/MathTransforms.cs#L32)
Source: [CeilingTransform.cs](../src/Transformalize/Transforms/CeilingTransform.cs#L24), [FloorTransform.cs](../src/Transformalize/Transforms/FloorTransform.cs#L27)

---

### round

Rounds to a specified number of decimal places.

```xml
<rows>
  <add Field2='129.992' />
</rows>
<calculated-fields>
  <add name='Round' type='decimal' t='copy(Field2).round(1)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `129.992` | `round(1)` | `130.0` |

Test: [MathTransforms.cs — DoMath](../test/Test.Unit.Core/MathTransforms.cs#L32)
Source: [RoundTransform.cs](../src/Transformalize/Transforms/RoundTransform.cs#L24)

---

### roundto / roundupto / rounddownto

Rounds to the nearest multiple, or rounds up/down to the nearest multiple.

```xml
<rows>
  <add Field1='10.6954' Field3='7' />
</rows>
<calculated-fields>
  <add name='RoundTo5' type='double' t='copy(Field1).roundTo(5)' />
  <add name='RoundTo3' type='double' t='copy(Field1).roundTo(3)' />
  <add name='RoundUpTo5' type='double' t='copy(Field1).roundUpTo(5)' />
  <add name='RoundDownTo4' type='int' t='copy(Field3).roundDownTo(4)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `10.6954` | `roundTo(5)` | `10` |
| `10.6954` | `roundTo(3)` | `12` |
| `10.6954` | `roundUpTo(5)` | `15` |
| `7` | `roundDownTo(4)` | `4` |

Test: [MathTransforms.cs — DoMath](../test/Test.Unit.Core/MathTransforms.cs#L32)

---

### abs

Returns the absolute value.

```xml
<rows>
  <add Field2='129.992' />
</rows>
<calculated-fields>
  <add name='Abs' type='decimal' t='copy(Field2).abs()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `129.992` | `abs()` | `129.992` |

Test: [MathTransforms.cs — DoMath](../test/Test.Unit.Core/MathTransforms.cs#L32)
Source: [AbsTransform.cs](../src/Transformalize/Transforms/AbsTransform.cs#L24)

---

### opposite

Negates a numeric value.

```xml
<rows>
  <add Field1='5' />
</rows>
<calculated-fields>
  <add name='t1' type='int' t='copy(Field1).opposite()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `5` | `opposite()` | `-5` |

Test: [TestMiscTransforms.cs — OppositeWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L715)
Source: [OppositeTransform.cs](../src/Transformalize/Transforms/OppositeTransform.cs#L24)

---

### invert

Inverts a boolean value (logical NOT).

```xml
<rows>
  <add Field1='true' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).invert()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `true` | `invert()` | `false` |

Test: [TestMiscTransforms.cs — InvertWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L746)
Source: [InvertTransform.cs](../src/Transformalize/Transforms/InvertTransform.cs#L23)

---

## Date Transforms

### datepart

Extracts a part of a date (year, month, day, hour, weekofyear, etc.).

```xml
<rows>
  <add StartDate='2016-06-01' EndDate='2016-08-01' />
</rows>
<calculated-fields>
  <add name='StartYear' type='int' t='copy(StartDate).datepart(year)' />
  <add name='StartWeek' type='int' t='copy(StartDate).datepart(weekofyear)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `2016-06-01` | `datepart(year)` | `2016` |
| `2016-06-01` | `datepart(weekofyear)` | `23` |

Test: [DatePartTransform.cs — DatePart1](../test/Test.Unit.Core/DatePartTransform.cs#L32)
Source: [DatePartTransform.cs](../src/Transformalize/Transforms/DatePartTransform.cs#L24)

---

### datediff

Calculates the difference between two dates in the specified unit.

```xml
<rows>
  <add StartDate='2016-06-01' EndDate='2016-08-01' />
</rows>
<calculated-fields>
  <add name='Days' type='int' t='copy(StartDate,EndDate).datediff(day)' />
  <add name='Hours' type='int' t='copy(StartDate,EndDate).datediff(hour)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `2016-06-01`, `2016-08-01` | `datediff(day)` | `61` |
| `2016-06-01`, `2016-08-01` | `datediff(hour)` | `1464` |

Test: [DateDiffTransform.cs — DateDiff1](../test/Test.Unit.Core/DateDiffTransform.cs#L33)
Source: [DateDiffTransform.cs](../src/Transformalize/Transforms/Dates/DateDiffTransform.cs#L26)

---

### datemath

Applies date math expressions (`/M` to round to month, `+1h` to add one hour, etc.).

```xml
<rows>
  <add Date='2017-01-01 9 AM' />
</rows>
<calculated-fields>
  <add name='RoundToMonth' type='datetime' t='copy(Date).dateMath(/M)' />
  <add name='AddHourMinute' type='datetime' t='copy(Date).DateMath(+1h+1m)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `2017-01-01 9:00 AM` | `dateMath(/M)` | `2017-01-01 00:00:00` |
| `2017-01-01 9:00 AM` | `dateMath(+1h+1m)` | `2017-01-01 10:01:00` |

Test: [DateMath.cs — TrySomeDateMath](../test/Test.Unit.Core/DateMath.cs#L34)
Source: [DateMathTransform.cs](../src/Transformalize/Transforms/Dates/DateMathTransform.cs#L24)

---

### convert

Converts a value to the field's declared type. Supports custom date formats.

```xml
<rows>
  <add OwensDate='02152018040000' />
</rows>
<fields>
  <add name='OwensDate' type='datetime' t='convert(date,MMddyyyyHHmmss)' />
</fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `02152018040000` | `convert(date,MMddyyyyHHmmss)` | `2018-02-15 04:00:00` |

Test: [DateConvert.cs — TryConvertSpecificFormat](../test/Test.Unit.Core/DateConvert.cs#L33)
Source: [ConvertTransform.cs](../src/Transformalize/Transforms/ConvertTransform.cs#L25)

---

### specifykind

Sets the `DateTimeKind` (Unspecified, Local, or Utc) on a datetime value.

```xml
<rows>
  <add Date1='2019-05-05 1:05 PM' Date2='2019-05-05 1:05 PM' Date3='2019-05-05 1:05 PM' />
</rows>
<fields>
  <add name='Date1' type='datetime' t='specifyKind(unspecified)' />
  <add name='Date2' type='datetime' t='specifyKind(local)' />
  <add name='Date3' type='datetime' t='specifyKind(utc)' />
</fields>
```

| Input | Transform | Output Kind |
|-------|-----------|-------------|
| `2019-05-05 1:05 PM` | `specifyKind(unspecified)` | `Unspecified` |
| `2019-05-05 1:05 PM` | `specifyKind(local)` | `Local` |
| `2019-05-05 1:05 PM` | `specifyKind(utc)` | `Utc` |

Test: [TestSpecifyKind.cs — Try](../test/Test.Unit.Core/TestSpecifyKind.cs#L34)
Source: [SpecifyKindTransform.cs](../src/Transformalize/Transforms/Dates/SpecifyKindTransform.cs#L24)

---

### tounixtime

Converts a datetime to a Unix timestamp (seconds or milliseconds).

```xml
<rows>
  <add Date='2019-07-01 10:30 AM' />
</rows>
<calculated-fields>
  <add name='Seconds' type='long' t='copy(Date).timezone(Eastern Standard Time,UTC).tounixtime(seconds)' />
  <add name='Ms' type='long' t='copy(Date).timezone(Eastern Standard Time,UTC).tounixtime(ms)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `2019-07-01 10:30 AM EST` | `tounixtime(seconds)` | `1561991400` |
| `2019-07-01 10:30 AM EST` | `tounixtime(ms)` | `1561991400000` |

Test: [TestToUnixTime.cs — Test](../test/Test.Unit.Core/TestToUnixTime.cs#L34)
Source: [ToUnixTimeTransform.cs](../src/Transformalize/Transforms/ToUnixTimeTransform.cs#L24)

---

### totime

Converts a numeric value to a TimeSpan string.

```xml
<rows>
  <add intField='238' longField='500' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(intField).toTime(hour)' />
  <add name='t2' t='copy(longField).toTime(seconds)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `238` | `toTime(hour)` | `9.22:00:00` |
| `500` | `toTime(seconds)` | `00:08:20` |

Test: [TestToTimeTransform.cs — ToTime](../test/Test.Unit.Core/TestToTimeTransform.cs#L32)
Source: [ToTimeTransform.cs](../src/Transformalize/Transforms/ToTimeTransform.cs#L24)

---

### timeago

Produces a relative time description (e.g., "30 minutes ago").

```xml
<calculated-fields>
  <add name='utc1' t='copy(utcdate).timeAgo()' />
</calculated-fields>
```

Output varies based on the time difference: `30 minutes ago`, `2 hours`, `tomorrow`, etc.

Test: [TestTimeAgoTransform.cs — TimeSummary](../test/Test.Unit.Core/TestTimeAgoTransform.cs#L30)
Source: [RelativeTimeTransform.cs](../src/Transformalize/Transforms/Dates/RelativeTimeTransform.cs#L50)

---

## Type Conversion

### tobool

Converts a string value to boolean.

```xml
<rows>
  <add Field1='true' />
  <add Field1='false' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).tobool()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `true` | `tobool()` | `true` |
| `false` | `tobool()` | `false` |

Test: [TestMiscTransforms.cs — ToBoolWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L521)
Source: [ToBoolTransform.cs](../src/Transformalize/Transforms/ToBoolTransform.cs#L25)

---

### toyesno

Converts a boolean to `Yes` or `No`.

```xml
<rows>
  <add Field1='true' />
  <add Field1='false' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).tobool()' />
  <add name='t2' t='copy(t1).toyesno()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `true` | `tobool().toyesno()` | `Yes` |
| `false` | `tobool().toyesno()` | `No` |

Test: [TestMiscTransforms.cs — ToYesNoWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L554)
Source: [ToYesNoTransform.cs](../src/Transformalize/Transforms/ToYesNoTransform.cs#L23)

---

## Boolean / Comparison Transforms

### contains

Checks if a string contains a value. Returns `true` or `false`.

```xml
<rows>
  <add Tags='Tag1 Tag2' />
  <add Tags='Tag2 Tag3' />
</rows>
<calculated-fields>
  <add name='IsTag1' type='bool' t='copy(Tags).contains(Tag1)' />
  <add name='IsTag3' type='bool' t='copy(Tags).contains(Tag3)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `Tag1 Tag2` | `contains(Tag1)` | `true` |
| `Tag1 Tag2` | `contains(Tag3)` | `false` |
| `Tag2 Tag3` | `contains(Tag3)` | `true` |

Test: [TestContains.cs — Contains](../test/Test.Unit.Core/TestContains.cs#L32)
Source: [ContainsTransform.cs](../src/Transformalize/Transforms/ContainsTransform.cs#L23)

---

### startswith

Checks if a string starts with a value.

```xml
<rows>
  <add input=' Wave 1' />
  <add input='Flight 1' />
</rows>
<fields>
  <add name='input' t='trim()' />
</fields>
<calculated-fields>
  <add name='output' type='bool' t='copy(input).startsWith(W)' />
</calculated-fields>
```

| Input (after trim) | Transform | Output |
|---------------------|-----------|--------|
| `Wave 1` | `startsWith(W)` | `true` |
| `Flight 1` | `startsWith(W)` | `false` |

Test: [TrimThenStartsWith.cs — TryIt](../test/Test.Unit.Core/TrimThenStartsWith.cs#L32)
Source: [StartsWithTransform.cs](../src/Transformalize/Transforms/StartsWithTransform.cs#L24)

---

### endswith

Checks if a string ends with a value.

```xml
<rows>
  <add Field1='hello.com' />
  <add Field1='hello.org' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).endswith(.com)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `hello.com` | `endswith(.com)` | `true` |
| `hello.org` | `endswith(.com)` | `false` |

Test: [TestMiscTransforms.cs — EndsWithWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L352)
Source: [EndsWithTransform.cs](../src/Transformalize/Transforms/EndsWithTransform.cs#L23)

---

### in

Checks if a value is in a list.

```xml
<rows>
  <add Field1='1' />
  <add Field1='5' />
</rows>
<calculated-fields>
  <add name='In123' type='bool' t='copy(Field1).in(1,2,3)' />
  <add name='In456' type='bool' t='copy(Field1).in(4,5,6)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1` | `in(1,2,3)` | `true` |
| `1` | `in(4,5,6)` | `false` |
| `5` | `in(1,2,3)` | `false` |
| `5` | `in(4,5,6)` | `true` |

Test: [TestIn.cs — In](../test/Test.Unit.Core/TestIn.cs#L32)
Source: [InTransform.cs](../src/Transformalize/Transforms/InTransform.cs#L25)

---

### equals

Checks if all copied field values are equal.

```xml
<rows>
  <add Field1='11' Field2='12' Field3='13' />
  <add Field1='11' Field2='11' Field3='11' />
</rows>
<calculated-fields>
  <add name='AreEqual' type='bool' t='copy(Field1,Field2,Field3).equals()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `11`, `12`, `13` | `equals()` | `false` |
| `11`, `11`, `11` | `equals()` | `true` |

Test: [TestTransformEquals.cs — Run](../test/Test.Unit.Core/TestTransformEquals.cs#L32)
Source: [EqualsTransform.cs](../src/Transformalize/Transforms/EqualsTransform.cs#L25)

---

### any

Checks if any of the copied field values matches a value.

```xml
<rows>
  <add Field1='1' Field2='2' Field3='3' />
</rows>
<calculated-fields>
  <add name='Has1' type='bool' t='copy(*).any(1)' />
  <add name='Has4' type='bool' t='copy(Field1,Field2,Field3).any(4)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1`, `2`, `3` | `any(1)` | `true` |
| `1`, `2`, `3` | `any(4)` | `false` |

Test: [TestAny.cs — AnyWorks](../test/Test.Unit.Core/TestAny.cs#L32)
Source: [AnyTransform.cs](../src/Transformalize/Transforms/AnyTransform.cs#L26)

---

### isnumeric

Checks if a value is numeric.

```xml
<rows>
  <add Field1='123' />
  <add Field1='abc' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).isnumeric()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `123` | `isnumeric()` | `true` |
| `abc` | `isnumeric()` | `false` |

Test: [TestMiscTransforms.cs — IsNumericWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L286)
Source: [IsNumericTransform.cs](../src/Transformalize/Transforms/IsNumericTransform.cs#L23)

---

### isempty

Checks if a value is empty.

```xml
<rows>
  <add Field1='hello' />
  <add Field1='' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).isempty()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `hello` | `isempty()` | `false` |
| (empty) | `isempty()` | `true` |

Test: [TestMiscTransforms.cs — IsEmptyWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L319)
Source: [IsEmptyTransform.cs](../src/Transformalize/Transforms/IsEmptyTransform.cs#L23)

---

### ismatch

Tests a value against a regular expression.

```xml
<rows>
  <add Field1='12345' />
  <add Field1='abc' />
</rows>
<calculated-fields>
  <add name='t1' type='bool' t='copy(Field1).ismatch(^[0-9]+$)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `12345` | `ismatch(^[0-9]+$)` | `true` |
| `abc` | `ismatch(^[0-9]+$)` | `false` |

Test: [TestMiscTransforms.cs — IsMatchWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L777)
Source: [RegexIsMatchTransform.cs](../src/Transformalize/Transforms/RegexIsMatchTransform.cs#L28)

---

## Conditional Transforms

### iif

Inline if — evaluates a condition and returns one of two values. Supports `=`, `>`, `<`, `>=`, `<=`, `!=`, `^=` (starts with), `*=` (contains).

```xml
<rows>
  <add Field1='2' Field2='1' Field3='3' Field4='4' Field5='rockstar' Field6='rock' />
  <add Field1='2' Field2='2' Field3='3' Field4='4' Field5='rockstars' Field6='star' />
</rows>
<calculated-fields>
  <add name='Equal' t='iif(Field1=Field2,Field3,Field4)' />
  <add name='GreaterThan' t='iif(Field1 > Field2,Field3,Field4)' />
  <add name='StartsWith' t='iif(Field5 ^= Field6,Field3,Field4)' />
</calculated-fields>
```

| Row | Transform | Output |
|-----|-----------|--------|
| Row 1 (Field1=2, Field2=1) | `iif(Field1=Field2,Field3,Field4)` | `4` (not equal) |
| Row 2 (Field1=2, Field2=2) | `iif(Field1=Field2,Field3,Field4)` | `3` (equal) |
| Row 1 (Field1=2, Field2=1) | `iif(Field1 > Field2,Field3,Field4)` | `3` (greater) |

Test: [IIfTransform.cs — IIfTransform1](../test/Test.Unit.Core/IIfTransform.cs#L32)
Source: [IifTransform.cs](../src/Transformalize/Transforms/IifTransform.cs#L28)

---

## Regular Expression Transforms

### match

Extracts the first regex match.

```xml
<rows>
  <add Field1='Local/3114@tolocalext-fc5d,1' />
  <add Field1='SIP/Generic-Vitel_Outbound-0002b45a' />
</rows>
<calculated-fields>
  <add name='MatchField1' t='copy(Field1).match(3[0-9]{3}(?=@))' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `Local/3114@tolocalext-fc5d,1` | `match(3[0-9]{3}(?=@))` | `3114` |
| `SIP/Generic-Vitel_Outbound-0002b45a` | `match(3[0-9]{3}(?=@))` | `None` |

Test: [TestMatch.cs — Match](../test/Test.Unit.Core/TestMatch.cs#L32)
Source: [RegexMatchTransform.cs](../src/Transformalize/Transforms/RegexMatchTransform.cs#L29)

---

### matching

Extracts all regex matches and concatenates them.

```xml
<rows>
  <add Field1='Local/3114@tolocalext-fc5d,1' />
</rows>
<calculated-fields>
  <add name='Matching' t='copy(Field1).matching([0-9a-zA-Z])' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `Local/3114@tolocalext-fc5d,1` | `matching([0-9a-zA-Z])` | `Local3114tolocalextfc5d1` |

Test: [TestMatch.cs — Match](../test/Test.Unit.Core/TestMatch.cs#L32)
Source: [RegexMatchingTransform.cs](../src/Transformalize/Transforms/RegexMatchingTransform.cs#L25)

---

### matchcount

Counts regex matches.

```xml
<rows>
  <add Field1='SIP/Generic-Vitel_Outbound-0002b45a' />
</rows>
<calculated-fields>
  <add name='MatchCount' type='int' t='copy(Field1).matchCount(Vitel)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `SIP/Generic-Vitel_Outbound-0002b45a` | `matchCount(Vitel)` | `1` |

Test: [TestMatch.cs — Match](../test/Test.Unit.Core/TestMatch.cs#L32)
Source: [RegexMatchCountTransform.cs](../src/Transformalize/Transforms/RegexMatchCountTransform.cs#L24)

---

### regexreplace

Replaces matches of a regex pattern.

```xml
<rows>
  <add Field1='abc 123 def 456' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).regexreplace([0-9]+,#)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `abc 123 def 456` | `regexreplace([0-9]+,#)` | `abc # def #` |

Test: [TestMiscTransforms.cs — RegexReplaceWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L385)
Source: [RegexReplaceTransform.cs](../src/Transformalize/Transforms/RegexReplaceTransform.cs#L24)

---

## Array Transforms

### split / join

Splits a string into an array, and joins an array back into a string.

```xml
<rows>
  <add Input='1 2 3 4' />
</rows>
<calculated-fields>
  <add name='Joined' t='copy(Input).split( ).join(-)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1 2 3 4` | `split( ).join(-)` | `1-2-3-4` |

Test: [TestSplitAndJoin.cs — SplitBySpace](../test/Test.Unit.Core/TestSplitAndJoin.cs#L34)
Source: [SplitTransform.cs](../src/Transformalize/Transforms/SplitTransform.cs#L26), [JoinTransform.cs](../src/Transformalize/Transforms/JoinTransform.cs#L25)

---

### sort

Sorts array elements.

```xml
<rows>
  <add Input='3,,2,,1' />
</rows>
<calculated-fields>
  <add name='Joined' t='copy(Input).split(,,).sort().join(-)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `3,,2,,1` | `split(,,).sort().join(-)` | `--1-2-3` |

Test: [TestSplitAndJoin.cs — SplitByCommas](../test/Test.Unit.Core/TestSplitAndJoin.cs#L110)
Source: [SortTransform.cs](../src/Transformalize/Transforms/SortTransform.cs#L25)

---

### distinct

Removes duplicate array elements.

```xml
<rows>
  <add Words='One Two Three One' />
</rows>
<calculated-fields>
  <add name='DistinctWords' t='copy(Words).split( ).distinct().join( )' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `One Two Three One` | `split( ).distinct().join( )` | `One Two Three` |

Test: [TestDistinct.cs — TryDistinct](../test/Test.Unit.Core/TestDistinct.cs#L33)
Source: [DistinctTransform.cs](../src/Transformalize/Transforms/DistinctTransform.cs#L25)

---

### toarray

Converts multiple field values into an array. Often used with `join()`.

```xml
<rows>
  <add Input1='2' Input2='4' Input3='6' />
</rows>
<calculated-fields>
  <add name='Joined' t='copy(Input1,Input2,Input3).toArray().join(-)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `2`, `4`, `6` | `toArray().join(-)` | `2-4-6` |

Test: [TestToArrayAndJoin.cs — Join](../test/Test.Unit.Core/TestToArrayAndJoin.cs#L34)
Source: [ToArrayTransform.cs](../src/Transformalize/Transforms/ToArrayTransform.cs#L25)

---

### first / last

Gets the first or last element of an array.

```xml
<rows>
  <add Field1='a,b,c' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).split(\,).first()' />
  <add name='t2' t='copy(Field1).split(\,).last()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `a,b,c` | `split().first()` | `a` |
| `a,b,c` | `split().last()` | `c` |

Test: [TestMiscTransforms.cs — FirstAndLastWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L651)
Source: [FirstTransform.cs](../src/Transformalize/Transforms/FirstTransform.cs#L24), [LastTransform.cs](../src/Transformalize/Transforms/LastTransform.cs#L25)

---

### get

Gets an array element by index.

```xml
<rows>
  <add Field1='a,b,c' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).split(\,).get(1)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `a,b,c` | `split().get(1)` | `b` |

Test: [TestMiscTransforms.cs — GetWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L684)
Source: [GetTransform.cs](../src/Transformalize/Transforms/GetTransform.cs#L24)

---

### length / len

Gets the length of a string or the count of array elements.

```xml
<rows>
  <add Input='1 2 3 4' />
</rows>
<calculated-fields>
  <add name='StringLen' type='int' t='copy(Input).length()' />
  <add name='ArrayLen' type='int' t='copy(Input).split( ).length()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1 2 3 4` | `length()` | `7` (string length) |
| `1 2 3 4` | `split( ).length()` | `4` (array count) |

Test: [TestLength.cs — LengthOfStrings, LengthOfArrays](../test/Test.Unit.Core/TestLength.cs#L34)
Source: [LengthTransform.cs](../src/Transformalize/Transforms/LengthTransform.cs#L24)

---

### splitlength

Gets the number of parts when splitting by a separator.

```xml
<rows>
  <add Field1='a,b,c,d' />
</rows>
<calculated-fields>
  <add name='t1' type='int' t='copy(Field1).splitlength(\,)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `a,b,c,d` | `splitlength(,)` | `4` |

Test: [TestMiscTransforms.cs — SplitLengthWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L588)
Source: [SplitLengthTransform.cs](../src/Transformalize/Transforms/SplitLengthTransform.cs#L23)

---

### slice

Python-style array slicing with `start:end:step` syntax.

```xml
<rows>
  <add Field1='stranger things in the upside down' Field3='jobs.mineplex.com' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).slice(1:3, )' />
  <add name='t3' t='copy(Field3).slice(0:1,.)' />
  <add name='t4' t='copy(Field1).slice(2, )' />
  <add name='t5' t='copy(Field3).slice(-2,.)' />
  <add name='t6' t='copy(Field1).slice(::2, )' />
  <add name='t7' t='copy(Field1).slice(3:0:-1, )' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `stranger things in the upside down` | `slice(1:3, )` | `things in` |
| `jobs.mineplex.com` | `slice(0:1,.)` | `jobs` |
| `stranger things in the upside down` | `slice(2, )` | `in the upside down` |
| `jobs.mineplex.com` | `slice(-2,.)` | `mineplex.com` |
| `stranger things in the upside down` | `slice(::2, )` | `stranger in upside` |
| `stranger things in the upside down` | `slice(3:0:-1, )` | `the in things` |

Test: [TestSlice.cs — SliceWorks](../test/Test.Unit.Core/TestSlice.cs#L32)
Source: [SliceTransform.cs](../src/Transformalize/Transforms/SliceTransform.cs#L26)

---

## Encoding / Decoding

### htmlencode

HTML-encodes a string.

```xml
<rows>
  <add Field1='&lt;b&gt;bold&lt;/b&gt;' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).htmlencode()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `<b>bold</b>` | `htmlencode()` | `&lt;b&gt;bold&lt;/b&gt;` |

Test: [TestMiscTransforms.cs — HtmlEncodeAndDecodeWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L224)
Source: [HtmlEncodeTransform.cs](../src/Transformalize/Transforms/HtmlEncodeTransform.cs#L29)

---

### xmldecode

XML/HTML-decodes a string. Aliases: `htmldecode`.

```xml
<rows>
  <add Field1='stranger &amp;amp; things in the upside down' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).xmlDecode()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `stranger &amp; things` | `xmlDecode()` | `stranger & things` |

Test: [TestXmlDecode.cs — XmlDecodeWorks](../test/Test.Unit.Core/TestXmlDecode.cs#L32)
Source: [HtmlDecodeTransform.cs](../src/Transformalize/Transforms/HtmlDecodeTransform.cs#L29)

---

### urlencode

URL-encodes a string.

```xml
<rows>
  <add Field1='hello world' />
</rows>
<calculated-fields>
  <add name='t1' t='copy(Field1).urlencode()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `hello world` | `urlencode()` | `hello+world` |

Test: [TestMiscTransforms.cs — UrlEncodeAndDecodeWork](../test/Test.Unit.Core/TestMiscTransforms.cs#L255)
Source: [UrlEncodeTransform.cs](../src/Transformalize/Transforms/UrlEncodeTransform.cs#L29)

---

### hashcode

Computes a hash code for a string. Same input always produces the same output.

```xml
<rows>
  <add Field1='hello' />
  <add Field1='hello' />
</rows>
<calculated-fields>
  <add name='t1' type='int' t='copy(Field1).hashcode()' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `hello` | `hashcode()` | (consistent integer) |
| `hello` | `hashcode()` | (same integer) |

Test: [TestMiscTransforms.cs — HashCodeWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L619)
Source: [LeftTransform.cs](../src/Transformalize/Transforms/LeftTransform.cs#L23), [RightTransform.cs](../src/Transformalize/Transforms/RightTransform.cs#L25)

---

## Field-Producing Transforms

### fromsplit

Splits a field into multiple output fields.

```xml
<fields>
  <add name='Field1'>
    <transforms>
      <add method='fromsplit' separator=','>
        <fields>
          <add name='f1' />
          <add name='f2' />
          <add name='f3' />
        </fields>
      </add>
    </transforms>
  </add>
</fields>
```

| Input | Transform | f1 | f2 | f3 |
|-------|-----------|----|----|-----|
| `a,b,c` | `fromsplit(,)` | `a` | `b` | `c` |

Test: [TestMiscTransforms.cs — FromSplitWorks](../test/Test.Unit.Core/TestMiscTransforms.cs#L416)
Source: [FromSplitTransform.cs](../src/Transformalize/Transforms/FromSplitTransform.cs#L25)

---

### fromregex

Extracts regex groups into separate output fields.

```xml
<fields>
  <add name='address'>
    <transforms>
      <add method='fromregex' pattern='(.*)\s+(\d+)\s*(.*)'>
        <fields>
          <add name='street' />
          <add name='number' />
          <add name='extra' />
        </fields>
      </add>
    </transforms>
  </add>
</fields>
```

| Input | street | number | extra |
|-------|--------|--------|-------|
| `1ST AV 1011` | `1ST AV` | `1011` | (empty) |
| `1ST AV 402 APT 1` | `1ST AV` | `402` | `APT 1` |

Test: [TestFromRegex.cs — FromRegexWorks](../test/Test.Unit.Core/TestFromRegex.cs#L32)
Source: [FromRegexTransform.cs](../src/Transformalize/Transforms/FromRegexTransform.cs#L26)

---

### fromlengths

Parses fixed-width fields by character lengths.

```xml
<fields>
  <add name='line'>
    <transforms>
      <add method='fromlengths'>
        <fields>
          <add name='f1' length='1' />
          <add name='f2' length='2' />
          <add name='f3' length='3' type='int' />
          <add name='f4' length='4' />
          <add name='f5' length='5' />
          <add name='f6' length='6' />
        </fields>
      </add>
    </transforms>
  </add>
</fields>
```

| Input | f1 | f2 | f3 | f4 | f5 | f6 |
|-------|----|----|----|----|----|-----|
| `122333444455555666666` | `1` | `22` | `333` | `4444` | `55555` | `666666` |

Test: [FromLengthsTransform.cs — FromLengths](../test/Test.Unit.Core/FromLengthsTransform.cs#L32)
Source: [FromLengthsTranform.cs](../src/Transformalize/Transforms/FromLengthsTranform.cs#L26)

---

### fromxml

Parses XML elements into rows with fields extracted from attributes or elements.

```xml
<rows>
  <add id='1' xml='&lt;things&gt;&lt;add name="deez"/&gt;&lt;add name="nutz"/&gt;&lt;/things&gt;' />
  <add id='2' xml='&lt;things&gt;&lt;add name="got"/&gt;&lt;add name="eeee"/&gt;&lt;/things&gt;' />
</rows>
```

| Input id | Input xml | Output rows |
|----------|-----------|-------------|
| `1` | `<things><add name="deez"/><add name="nutz"/></things>` | 2 rows: `deez`, `nutz` |
| `2` | `<things><add name="got"/><add name="eeee"/></things>` | 2 rows: `got`, `eeee` |

Test: [FromXmlTransform.cs — Try1](../test/Test.Unit.Core/FromXmlTransform.cs#L32)
Source: [FromXmlTransform.cs](../src/Transformalize/Transforms/FromXmlTransform.cs#L29)

---

## Row-Producing Transforms

### torow

Expands an array into multiple rows (one per element).

```xml
<rows>
  <add Input1='2' Input2='4' Input3='6' />
</rows>
<calculated-fields>
  <add name='Value' t='copy(Input1,Input2,Input3).toArray().toRow()' />
</calculated-fields>
```

| Input | Transform | Output Rows |
|-------|-----------|-------------|
| `2`, `4`, `6` | `toArray().toRow()` | 3 rows: `2`, `4`, `6` |

Test: [TestToRow.cs — ToRow](../test/Test.Unit.Core/TestToRow.cs#L35)
Source: [ToArrayTransform.cs](../src/Transformalize/Transforms/ToArrayTransform.cs#L25)

---

## Row-Filtering Transforms

### include / exclude

Filters rows based on a field value. `include` keeps matching rows; `exclude` removes them.

```xml
<rows>
  <add Field1='1' />
  <add Field1='2' />
</rows>
<fields>
  <add name='Field1' t='include(1)' />
</fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1`, `2` | `include(1)` | Only row with `1` passes |
| `1`, `2` | `exclude(1)` | Only row with `2` passes |

Test: [FilterTransform.cs — IncludeTransform, ExcludeTransform](../test/Test.Unit.Core/FilterTransform.cs#L32)
Source: [FilterTransform.cs](../src/Transformalize/Transforms/FilterTransform.cs#L30)

---

## Utility Transforms

### map

Looks up a value in a named map. Unmapped values either pass through or return a configured default.

```xml
<maps>
  <add name='map'>
    <items>
      <add from='1' to='One' />
      <add from='2' to='Two' />
      <add from='*' to='None' />
    </items>
  </add>
</maps>
<calculated-fields>
  <add name='Map' t='copy(Field1).map(map)' />
</calculated-fields>
```

| Input | Transform | Output |
|-------|-----------|--------|
| `1` | `map(map)` | `One` |
| `2` | `map(map)` | `Two` |
| `4` | `map(map)` | `None` |

Test: [TestMap.cs — MapTransformAdd](../test/Test.Unit.Core/TestMap.cs#L32)
Source: [MapTransform.cs](../src/Transformalize/Transforms/MapTransform.cs#L25)

---

### log

Logs the current value during processing (useful for debugging). Does not change the value.

```xml
<calculated-fields>
  <add name='cf1' t='copy(Field1).condense().log()' />
</calculated-fields>
```

Test: [LogTransform.cs — Log](../test/Test.Unit.Core/LogTransform.cs#L32)
Source: [LogTransform.cs](../src/Transformalize/Transforms/LogTransform.cs#L24)

---

## Validators

Validators check field values and set a corresponding `{Field}Valid` boolean field. They can be applied to fields or calculated fields using `v` (validation) attributes or longhand `<transforms>`.

### required

Validates that a field is not empty.

```xml
<fields>
  <add name='Field1' v='required()' />
</fields>
```

| Input | Validator | Field1Valid |
|-------|-----------|-------------|
| `present` | `required()` | `true` |
| (empty) | `required()` | `false` |

Test: [Validation/TestRequired.cs — Run](../test/Test.Unit.Core/Validation/TestRequired.cs#L32)
Source: [RequiredValidator.cs](../src/Transformalize/Validators/RequiredValidator.cs#L24)

---

### contains (validator)

Validates that a field contains a substring.

```xml
<fields>
  <add name='Field1' v='contains(pre)' />
</fields>
```

| Input | Validator | Field1Valid |
|-------|-----------|-------------|
| `present` | `contains(pre)` | `true` |
| (empty) | `contains(pre)` | `false` |

Test: [Validation/TestContains.cs — Run](../test/Test.Unit.Core/Validation/TestContains.cs#L32)
Source: [ContainsTransform.cs](../src/Transformalize/Transforms/ContainsTransform.cs#L23)

---

### length

Validates that a field is exactly a given length.

```xml
<fields>
  <add name='text' v='length(7)' />
</fields>
```

| Input | Validator | textValid |
|-------|-----------|-----------|
| `perfect` (7 chars) | `length(7)` | `true` |
| `too-long` (8 chars) | `length(7)` | `false` |
| `short` (5 chars) | `length(7)` | `false` |

Test: [Validation/TestLength.cs — Run](../test/Test.Unit.Core/Validation/TestLength.cs#L32)
Source: [LengthValidator.cs](../src/Transformalize/Validators/LengthValidator.cs#L24)

---

### minlength / maxlength

Validates minimum or maximum string length.

```xml
<fields>
  <add name='Field1' v='maxLength(10)' />
  <add name='Field2' v='minLength(20)' />
</fields>
```

| Input | Validator | Valid |
|-------|-----------|-------|
| `0123456789` (10 chars) | `maxLength(10)` | `true` |
| `abcdefghijklmnopqrstuvwxyz` (26 chars) | `maxLength(10)` | `false` |
| `abcdefghijklmnopqrstuvwxyz` (26 chars) | `minLength(20)` | `true` |
| `0123456789` (10 chars) | `minLength(20)` | `false` |

Tests: [Validation/TestMaxLength.cs — Run](../test/Test.Unit.Core/Validation/TestMaxLength.cs#L32), [Validation/TestMinLength.cs — Run](../test/Test.Unit.Core/Validation/TestMinLength.cs#L32)

---

### matches

Validates that a field matches a regex pattern.

```xml
<fields>
  <add name='Field' v='matches([A-Z]{1})' />
</fields>
```

| Input | Validator | FieldValid |
|-------|-----------|------------|
| `dale` | `matches([A-Z]{1})` | `false` |
| `Dale` | `matches([A-Z]{1})` | `true` |

Test: [Validation/TestMatches.cs — Run](../test/Test.Unit.Core/Validation/TestMatches.cs#L32)
Source: [MatchValidator.cs](../src/Transformalize/Validators/MatchValidator.cs#L29)

---

### all

Validates that all copied field values match a given value.

```xml
<calculated-fields>
  <add name='All9' t='copy(Field1,Field2,Field3)' v='all(9)' />
</calculated-fields>
```

| Input | Validator | All9Valid |
|-------|-----------|-----------|
| `9`, `10`, `11` | `all(9)` | `false` |
| `9`, `9`, `9` | `all(9)` | `true` |

Test: [Validation/TestAll.cs — Run](../test/Test.Unit.Core/Validation/TestAll.cs#L32)
Source: [AllValidator.cs](../src/Transformalize/Validators/AllValidator.cs#L27)

---

### map (validator)

Validates that a field value exists in a named map.

```xml
<maps>
  <add name='map'>
    <items>
      <add from='1' />
      <add from='2' />
      <add from='3' />
    </items>
  </add>
</maps>
<fields>
  <add name='Field1' v='map(map)' />
</fields>
```

| Input | Validator | Field1Valid |
|-------|-----------|-------------|
| `2` | `map(map)` | `true` |
| `4` | `map(map)` | `false` |

Test: [Validation/TestMap.cs — MapWithOnlyFrom](../test/Test.Unit.Core/Validation/TestMap.cs#L32)
Source: [MapTransform.cs](../src/Transformalize/Transforms/MapTransform.cs#L25)

---

## Extension Transform Packages

Additional transforms are available as separate NuGet packages:

| Package | Method | Description |
|---------|--------|-------------|
| `Transformalize.Transform.Fluid` | `fluid(template)` | Liquid template rendering |
| `Transformalize.Transform.Jint` | `jint(script)` | JavaScript expression evaluation |
| `Transformalize.Transform.Humanizer` | `humanize()`, `dehumanize()` | Humanizer string transforms |
| `Transformalize.Transform.LambdaParser` | `eval(expression)` | Expression evaluation |
| `Transformalize.Transform.Razor` | `razor(template)` | Razor template rendering |
