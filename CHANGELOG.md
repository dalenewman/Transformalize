# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.5.0] - 2026-09-11

### Added

- Opt-in asynchronous ETL streaming: `IReadStream`, `IWriteStream`, `IOperateStream`, `IExecuteStream`, `IStreamingPipeline`, and `IStreamingProcessController`, with extension methods for existing interface variables and Autofac registrations for streaming controllers/pipelines.
- Native ADO input/key streaming with cancellation and enumeration-scoped resource cleanup; bounded ADO entity writes and SQL Server bulk copy; incremental internal output; native Jint transforms preserving engine state and first-row error handling.
- Native streaming for Elasticsearch search/scroll, Solr paging, CSV records, JSON arrays/Lines, file/console input, specialized ADO readers, and internal/composite readers; lazy streams over synchronous Bogus, Excel, filesystem, and Lucene APIs. Elasticsearch scrolls and owned input resources are released on early exit/cancellation. Aggregation responses and whole-document rows retain their format-specific memory requirements.
- Streaming type conversion avoids buffering CSV/internal input while preserving first-row error handling; incremental JSON parsing preserves BOM encoding detection.
- Safe transform/validator adaptation: unchanged base row operations stream, while third-party sequence overrides materialize once and retain their full-sequence contract. Mixed providers use explicit compatibility adapters. Input `buffer=true` closes the native ADO reader before yielding output.
- **All built-in transforms and validators stream.** A protected `Initialize()` hook on `BaseTransform`/`BaseValidate` carries one-time setup (map loading, template compilation, connection resolution) that previously required overriding `Operate(IEnumerable<IRow>)` and therefore buffered the whole input. `filter`, `format`, `map`, `in`, `join`, `fromLengths`, `toRow`, `toUnixTime`, `fromXml`, `razor`, `fluid`, `geocode`, `place`, `adoRun`, and the `map` validator no longer materialize. A unit test enforces that no built-in operation relies on the fallback.
- **All built-in row writers stream.** CSV, JSON, JSON Lines, file, console, trace, string, null, and log writers now implement `IWriteStream` and write row by row; Solr, Elasticsearch, and Lucene writers consume batches bounded by `insert-size`. The parallel Solr writer takes its throttle before pulling the next batch, bounding resident batches at `max-degree-of-parallelism` instead of the whole input.
- **Nothing shipped in this repository buffers through a compatibility adapter.** The remaining GeoJSON writers (all eight), `MailWriter`, `ElasticPartialUpdater`, and `AdoCalculatedFieldUpdater` now implement `IWriteStream`. `ExcelWriter` and `RazorWriter` implement it too but materialize inside, on purpose, because their formats are finalized as one document; the buffering is now attributable to the format instead of hidden in the adapter. A test linked into every streaming test project fails if a shipped reader or writer drops off the contract.
- Streaming fallbacks now log at **warning** level naming the offending type, instead of debug, so a third-party component that buffers the whole input cannot do so silently.
- Sync/streaming parity and regression coverage for lifecycle, ordering, bounded demand, early termination, cancellation, failures, sequence expansion/finalization, Jint state, and ADO resource cleanup.
- `NorthWindIntegrationMySqlAsync`, the streaming counterpart to the existing MySQL Northwind integration test, so MySQL now covers both execution paths like SQLite, SQL Server, and PostgreSQL.
- [Streaming migration guide](docs/async-streaming-migration.md), including the OrchardCore.Transformalize follow-up, materialization boundaries, and partial-commit behavior.

### Changed

- **Retired the KML provider.** `src/Providers/Kml` and its `Transformalize.Provider.Kml` package are removed, along with `kml` in `Constants.ProviderDomain` and its case in `ProcessValidate`. Nothing in the repository referenced the project and no Autofac module registered it, so it could not be composed through the normal container; the last published version was 1.0.0. Arrangements using `provider="kml"` will now fail validation with an invalid-provider error. The `kml-` alias filter in `GeoJsonStreamWriter` is left in place so GeoJSON output is unchanged for arrangements that still carry those fields.

- Fixed `toUnixTime`: with `Run` disabled it was missing a `yield break`, so it enumerated its input twice and emitted every row twice.
- `action`, `connection`, `parameter`, and `script` transforms now honor `Run` like every other transform; their redundant sequence overrides ran even after the constructor disabled them.

- Aligned **all packable projects and their Autofac packages at 1.5.0**, plus the CLI and its Docker publish profile tags. Core remains `netstandard2.0`, with `Microsoft.Bcl.AsyncInterfaces 10.0.8` providing async enumeration contracts.
- Native streaming read/write failures propagate and skip successful completion callbacks. Streaming ADO insert/update counters advance after transaction commit. Earlier entity batches may remain committed after later failures; execution does not provide a process-wide transaction.
- Elasticsearch aggregation reads now handle the transport's `JsonElement` responses in both sync and streaming paths; streaming flattening retains only its current output row.
- Async ADO key matching now passes cancellation to commands and propagates failures after rollback, avoiding an empty match result that could cause duplicate inserts.
- ADO-generated filters now bind scalar, list, `LIKE`, and full-text values as typed command parameters instead of interpolating them into SQL.
- Updated the test projects' `Testcontainers` packages to 4.15.0, which resolves the transitive `SSH.NET` dependency to the patched 2026.0.0 and clears the NU1903 advisory (GHSA-q939-rpr3-3284). The solution now builds with no warnings.
- The PostgreSQL test project targets `net10.0` only, matching every other test project; it was the last one still multi-targeting `net8.0`.
- No public API is deprecated or removed. Synchronous methods, `ReadAsync`, `ExecuteAsync`, enumerable-based `WriteAsync`, and the non-row async methods remain fully supported and warning-free, and `ExecuteAsync` is still implemented in terms of `ReadAsync`. Streaming is opt-in through the new contracts and extension methods; prefer `ReadStreamAsync` with async enumeration, or explicit `MaterializeAsync`, for new code.

## [1.4.6] - 2026-09-10

### Changed

- **Lower allocation overhead in the ETL pipeline** (`Transformalize 1.4.6`): removed a duplicate storage-array allocation for every master row. Default-value processing now uses a direct loop for calculated fields and checks whitespace without creating trimmed strings.
- **Faster formatter setup** (`Transformalize 1.4.6`): reuse the compiled, fixed regex that identifies format placeholders. Each formatter still resolves its own format string and reads current row values.
- **Less repeated work in ADO updates** (`Provider.Ado 1.4.6`): collect and sort update-field metadata once per write call instead of once per row, in both synchronous and asynchronous writes. Parameter values are still read from each row.
- **Reuse JavaScript preparation** (`Transform.Jint 1.4.6`, `Validate.Jint 1.4.6`): prepare each operation's script once and reuse it while the source stays identical. If `Operation.Script` changes, prepare the new source before evaluating it. Row variables and results are evaluated on every call; engine state remains local to each transform or validator, and existing syntax-error handling is preserved.
- **Avoid unused JavaScript engines during startup** (`Transform.Jint 1.4.6`, `Validate.Jint 1.4.6`): discovering operation signatures no longer creates an engine. Engines are created only when a context is supplied for execution.
- Aligned the affected core, ADO, and Jint packages and their corresponding Autofac packages at **1.4.6**, so updating the Autofac packages brings in the optimized implementations. The CLI and Docker image tags also use **1.4.6**; Visual Studio Docker publish profiles now use .NET 10 images to match the CLI target.

### Added

- JavaScript regression coverage for changing row values, script-source changes between rows, independent engine state, and syntax-error handling after setup.

## [1.4.4] - 2026-07-23

### Added

- **Configurable output unit and precision for the `distance` transform** (`Transformalize 1.4.4`, `Transform.Geography 1.4.4`): the `distance` transform now accepts two optional parameters, `decimal-places` and `distance-unit`, exposed as `Operation.DecimalPlaces` (int, default `1`) and `Operation.DistanceUnit` (default `miles`; domain `miles,kilometers,meters,nauticalmiles`). These are translated into the `int decimalPlaces` and `Geolocation.DistanceUnit` arguments passed to `GeoCalculator.GetDistance`. Previously the transform always used the library defaults (1 decimal place, miles) with no way to override them. Shorthand usage: `distance(from-lat,from-lon,to-lat,to-lon,decimal-places,distance-unit)`, e.g. `distance(FromLat,FromLon,ToLat,ToLon,2,kilometers)`.
- **`Test.Unit.Geography` test project**: added unit tests for `DistanceTransform` covering the static distance calculation (unit conversions and decimal-place rounding) and end-to-end configuration wiring of the new `decimal-places`/`distance-unit` parameters through the `distance` shorthand.

## [1.4.3] - 2026-06-22

### Changed

- **Dependency alignment with OrchardCore 3.0.0** (`1.4.3`): bumped shared dependencies to match the versions OrchardCore 3.0.0 ships, so projects co-hosted with OrchardCore (e.g. `OrchardCore.Transformalize`) resolve a single, consistent version of each:
  - `System.Text.Json` 10.0.5 → 10.0.8 (`Provider.Json`, `Transform.Json`, `Provider.GeoJson`).
  - `Jint` 4.6.3 → 4.9.2 (`Transform.Jint`, `Validate.Jint`).
  - `MailKit` 4.16.0 → 4.17.0 (`Provider.Mail`).
  - `DocumentFormat.OpenXml` 3.4.1 → 3.5.1 (`Provider.OpenXml`).
  - `Lucene.Net*` 4.8.0-beta00016 → 4.8.0-beta00017 (`Provider.Lucene`).
- The corresponding `.Autofac` wrapper packages were revved alongside their base packages so consumers (which reference the `.Autofac` packages) pick up the updated dependencies.

### Security

- **Patched native SQLite** (`Provider.Sqlite 1.4.3`): pinned `SQLitePCLRaw.bundle_e_sqlite3` to `3.0.3` to resolve **CVE-2025-6965** (NU1903, high severity — memory corruption in SQLite < 3.50.2). `Microsoft.Data.Sqlite` still floors the bundle at the vulnerable, deprecated `2.1.11`, so an explicit pin to the `3.0.x` bundle (which uses `SourceGear.sqlite3` ≥ 3.50.4.5) is the only mitigation. Revisit when `Microsoft.Data.Sqlite` references a non-vulnerable bundle itself.

## [1.4.2] - 2026-05-21

### Changed

- **SQL Server FTS — CONTAINS is now the default** (`Ado 1.4.2`, `Transformalize 1.4.2`): When a field has a `search-type` and the filter `type='search'`, SQL Server now generates `CONTAINS()` by default (previously generated `CONTAINS` without a normalizer). Use `query-type='freetext'` to opt into `FREETEXT()` (natural-language, no operators).
- **CONTAINS auto-normalizer** (`Ado 1.4.2`): User input is automatically normalized before being passed to `CONTAINS()` so common syntax mistakes are fixed transparently:
  - Unquoted prefix terms (`chef*`) are auto-quoted (`"chef*"`).
  - Leading wildcards (`*chai`) are stripped; both-side wildcards (`*chai*`) strip the leading `*` and quote the remainder (`"chai*"`).
  - Bare multi-word input (`chai chang`) is joined with `AND` (`chai AND chang`).
  - Bare `NOT` is promoted to `AND NOT` (`chai NOT chang` → `chai AND NOT chang`).
  - Dangling leading operators (`OR chai`, `AND NOT chai`) are stripped.
  - Dangling trailing operators (`chai AND`, `something* AND somethingelse OR`) are stripped.
  - Consecutive operators (`chai AND OR chang`) keep the first and drop the second (`chai AND chang`).
  - Explicit `AND`, `OR`, `AND NOT`, and `NEAR` operators are preserved, with each operand still individually normalized.
- **`query-type` and `mode` domain validation** (`Transformalize 1.4.2`): `SearchType.QueryType` now validates against `plain,web,phrase,raw,contains,freetext`; `SearchType.Mode` validates against `boolean,natural,expansion`.

## [1.4.1] - 2026-05-19

### Fixed

- **SQL Server provider** (`1.4.1`): Port was ignored when building the connection string. The `DataSource` now uses `server,port` format when a non-zero port is configured.

## [1.4.0] - 2026-05-19

### Added

- **Native full-text search for ADO providers** (`Ado 1.4.0`, `Transformalize 1.4.0`): Fields tagged with a `search-type` whose name references a `<search-type>` element now generate native FTS predicates instead of `LIKE` when `type='search'` filters are applied.
  - **SQL Server** (`1.4.0`): `CONTAINS(field, 'query')` with optional `LANGUAGE 'lang'` via `analyzer`. Supports CONTAINS phrase (`"two words"`), boolean OR/AND, and prefix (`"word*"`) syntax.
  - **PostgreSQL** (`1.4.0`): `to_tsvector(lang, field) @@ tsquery(lang, 'query')`. The `query-type` attribute selects the tsquery function: `plain` (default, `plainto_tsquery`), `web` (`websearch_to_tsquery` — supports `"phrase"`, `-exclude`, `OR`), `phrase` (`phraseto_tsquery`), or `raw` (`to_tsquery` — supports `|`, `&`, `:*` prefix).
  - **MySQL** (`1.4.0`): `MATCH(field) AGAINST('query' mode)`. The `mode` attribute selects `boolean` (default — supports `"phrase"`, `+required`, `-excluded`, `word*`), `natural`, or `expansion`.
  - **SQLite** (`1.4.0`): FTS5 subquery `rowid IN (SELECT rowid FROM Entity_fts WHERE Entity_fts MATCH 'query')`. Supports FTS5 phrase, `OR`, `AND`, `NOT`, and prefix (`word*`) syntax.
  - When the filter value equals the wildcard (`*`) the FTS predicate is skipped and all rows are returned.
  - Negation (`operator='notequal'`) wraps the expression in `NOT (...)`.
  - Fields without a matching search type fall back to the previous `LIKE '%value%'` behaviour.
- See [`docs/full-text-search.md`](docs/full-text-search.md) for configuration details and index setup per provider.

## [1.3.0] - 2026-04-29

### Fixed

- **Ado provider** (`1.1.0`): Use `DateTime.UtcNow` instead of `DateTime.Now` when recording batch start/end timestamps in the output controller.
- **PostgreSql provider** (`1.1.0`): `Enclose()` now quotes identifiers containing hyphens in addition to spaces and reserved words.

## [1.2.1] - 2026-04-25

### Added

- Extend "map" input type to `Field` (was already added to `Parameter` in 1.2.0).
- Add `Constants.InputTypeDomain` so `Field` and `Parameter` share a single domain definition for `InputType`.

## [1.2.0] - 2026-04-25

### Added

- Add "map" option and associated validation to parameter input type.

### Maintenance

- Updated MailKit to remove vulnerability

## [1.1.0] - 2026-04-12

### Added

- `bucketize` transform: maps numeric values to labeled string buckets using a map. Use `MapItem.From`/`To` as inclusive range boundaries (`*` for unbounded) and `MapItem.Value` as the label. Values outside all defined ranges fall through as their string representation. Example: `copy(Age).bucketize(AgeGroups)`.

## [1.0.0] - 2026-03-19

Going to version 1.0.0 so Vlad doesn't get build warnings.

## [0.12.x-beta and earlier]

For changes prior to 1.0.0, see the [git history](https://github.com/dalenewman/Transformalize/commits/master).
