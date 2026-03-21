# Getting Started with Transformalize

This guide walks you through installing Transformalize, writing your first arrangement, and running incremental loads.

## Installation (not yet tested, coming soon...)

Download a self-contained binary for your platform from the [latest release](https://github.com/dalenewman/Transformalize/releases), or use the install script on macOS/Linux:

```bash
curl -fsSL https://raw.githubusercontent.com/dalenewman/Transformalize/master/scripts/install-tfl.sh | sh
```

Verify the installation:

```bash
tfl --version
```

See the [CLI distribution guide](./cli-distribution.md) for all platform options.

## Docker

A function in your profile like this should work on Mac and Linux terminals:

```bash
function tfl {
  local tty_flags=()
  if [[ -t 0 && -t 1 ]]; then
    tty_flags=(-it)
  fi
  docker run "${tty_flags[@]}" --rm -v "$(pwd)":"$(pwd)" -w "$(pwd)" ghcr.io/dalenewman/transformalize.cli:latest "$@"
}
```

> **Tip:** You'd have to `docker pull ghcr.io/dalenewman/transformalize.cli:latest` to update.

## Your First Arrangement: CSV to Console

Create a CSV file called `contacts.csv`:

```csv
Id,FirstName,LastName,Email
1,John,Doe,john@example.com
2,Jane,Smith,jane@example.com
3,Bob,Johnson,bob@example.com
```

Create an arrangement file called `csv-to-console.xml`:

```xml
<cfg name="CsvToConsole" read-only="true">
  <connections>
    <add name="input" provider="file" file="contacts.csv" />
    <add name="output" provider="console" />
  </connections>
  <entities>
    <add name="Contact">
      <fields>
        <add name="Id" type="int" primary-key="true" />
        <add name="FirstName" />
        <add name="LastName" />
        <add name="Email" />
      </fields>
    </add>
  </entities>
</cfg>
```

Run it:

```bash
tfl -a csv-to-console.xml
```

You should see the CSV rows printed to the console.

## Adding Transforms

Transforms modify field values during processing. Use the `t` attribute on fields or add `<calculated-fields>` for derived values.

```xml
<cfg name="CsvWithTransforms" read-only="true">
  <connections>
    <add name="input" provider="file" file="contacts.csv" />
    <add name="output" provider="console" />
  </connections>
  <entities>
    <add name="Contact">
      <fields>
        <add name="Id" type="int" primary-key="true" />
        <add name="FirstName" />
        <add name="LastName" />
        <add name="Email" t="tolower()" />
      </fields>
      <calculated-fields>
        <add name="FullName" t="copy(FirstName).append( ).append(LastName).trim()" />
        <add name="EmailDomain" t="copy(Email).split(@).last()" />
      </calculated-fields>
    </add>
  </entities>
</cfg>
```

Transforms are chained with the dot (`.`) separator. The `t` attribute is shorthand that compiles into transform operations at runtime.

## Generating Test Data with Bogus

You do not need a real data source to experiment. The Bogus provider generates fake data:

```xml
<cfg name="BogusExample" read-only="true">
  <connections>
    <add name="input" provider="bogus" seed="1" />
    <add name="output" provider="console" />
  </connections>
  <entities>
    <add name="Contact" size="10">
      <fields>
        <add name="Identity" type="int" />
        <add name="FirstName" />
        <add name="LastName" />
        <add name="Email" />
        <add name="Color" />
        <add name="Latitude" type="decimal" />
        <add name="Longitude" type="decimal" />
      </fields>
    </add>
  </entities>
</cfg>
```

See the [Bogus Provider](../src/Providers/Bogus/README.md) for more options.

## Writing to a Database

To write output to a SQLite database, change the output connection:

```xml
<cfg name="BogusToSqlite">
  <connections>
    <add name="input" provider="bogus" seed="1" />
    <add name="output" provider="sqlite" file="output.sqlite3" />
  </connections>
  <entities>
    <add name="Contact" size="100">
      <fields>
        <add name="Identity" type="int" primary-key="true" />
        <add name="FirstName" />
        <add name="LastName" />
        <add name="Email" />
      </fields>
    </add>
  </entities>
</cfg>
```

Run it once to initialize:

```bash
tfl init -a bogus-to-sqlite.xml
```

Transformalize creates the output table and inserts the rows.

## Running Incrementally

Incremental loading is the core strength of Transformalize. When reading from a relational database, add a `version` field to your entity to enable change detection:

```xml
<cfg name="Incremental">
  <connections>
    <add name="input" provider="sqlserver" database="Northwind" server="localhost" user="sa" password="secret" />
    <add name="output" provider="sqlite" file="northwind-star.sqlite3" />
  </connections>
  <entities>
    <add name="Order Details" version="RowVersion">
      <fields>
        <add name="OrderID" type="int" primary-key="true" />
        <add name="ProductID" type="int" primary-key="true" />
        <add name="UnitPrice" type="decimal" precision="19" scale="4" />
        <add name="Quantity" type="short" />
        <add name="Discount" type="single" />
        <add name="RowVersion" type="byte[]" length="8" />
      </fields>
    </add>
  </entities>
</cfg>
```

The first run initializes the output and loads all rows. Subsequent runs compare version values and process only new or changed records:

```bash
tfl init -a incremental.xml   # first run: full load
tfl run -a incremental.xml   # subsequent runs: incremental
```

## Using Schema Detection

If you are not sure what fields an entity has, run Transformalize in schema mode:

```bash
tfl schema -a incremental.xml
```

This connects to the input and outputs the detected field definitions, which you can copy into your arrangement.

## Next Steps

- See the [Transforms Reference](./transforms.md) for a complete list of built-in transforms.
- Read the [Architecture](./architecture.md) document to understand how the pipeline works.
- Explore the [provider READMEs](../src/Providers/) for provider-specific configuration options.
