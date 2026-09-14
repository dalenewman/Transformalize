# Migrating to asynchronous streaming in 1.5.0

Version **1.5.0** adds opt-in asynchronous row streaming. All packable Transformalize projects, including provider, transform, logging, and Autofac packages, use this version. This is a minor release: existing contracts remain implementable, synchronous execution stays supported, and the existing buffered execution entry point remains available.

## Application calls

Import `Transformalize.Extensions`. Existing `IProcessController`, `IPipeline`, `IRead`, `IInputProvider`, `IWrite`, and `IOutputProvider` variables can use the extension methods without casts:

```csharp
using Transformalize.Contracts;
using Transformalize.Extensions;

using (var scope = container.CreateScope(process, logger)) {
    var controller = scope.Resolve<IProcessController>();
    await controller.ExecuteStreamAsync(cancellationToken);
}
```

For a read-only consumer, enumerate inside the scope:

```csharp
using (var scope = container.CreateScope(process, logger)) {
    var controller = scope.Resolve<IProcessController>();
    await foreach (var row in controller.ReadStreamAsync(cancellationToken)) {
        // Consume or send this row here; do not retain it unless needed.
    }
}
```

`ReadStreamAsync`, like `Read`, does not initialize output or run process pre/post actions. Use `ExecuteStreamAsync` for the complete lifecycle. Streams are lazy, ordered, and intended for one enumeration per execution. Do not use one controller or operation instance concurrently. Breaking an `await foreach` disposes its enumerator, releasing native input resources (including file handles and Elasticsearch scrolls). Manually acquired enumerators must be disposed with `await using`.

If a consumer needs an in-memory result, make that explicit:

```csharp
var rows = await controller.ReadStreamAsync(cancellationToken)
    .MaterializeAsync(cancellationToken);
```

| Previous call | 1.5.0 choice |
| --- | --- |
| `Execute()` / `Read()` | Continue using these for synchronous execution. |
| `await ExecuteAsync(token)` | Still supported with buffered behavior. Use `ExecuteStreamAsync(token)` to opt in. |
| `await ReadAsync(token)` returning `IEnumerable<IRow>` | Still supported with buffered behavior. Use `ReadStreamAsync(token)` and `await foreach` to opt in, or materialize explicitly. |
| `await WriteAsync(enumerable, token)` | Still supported for an existing synchronous sequence or batch. |
| Writing an async source | `await writer.WriteStreamAsync(source, token)` |
| Schema/map reads, initialization, actions, updates | Their async methods remain supported; they are not row-stream replacements. |

**Nothing is deprecated in this release.** No `Obsolete` attribute was added to `ReadAsync`, `ExecuteAsync`, or enumerable-based `WriteAsync`, so upgrading produces no new warnings and projects treating warnings as errors are unaffected. Streaming is purely additive: the buffered family stays fully supported, and `ExecuteAsync` is still implemented in terms of `ReadAsync`. `ReadAsync` also remains a required member of `IRead` and `IInputProvider` on `netstandard2.0`, so existing implementers have nothing to change. The preference for `ReadStreamAsync` is documented on those contract members rather than enforced by the compiler.

## Capabilities and fallbacks

The optional public contracts are `IReadStream`, `IWriteStream`, `IOperateStream`, and `IExecuteStream`. `IStreamingPipeline` and `IStreamingProcessController` combine streaming and existing contracts. Autofac exposes the combined controller interface and both named entity and process pipeline interfaces. Existing provider registrations continue working: the pipeline detects capabilities on the registered reader or writer instance. Autofac aliases share an instance within the execution scope.

| Component | Streaming behavior |
| --- | --- |
| `AdoInputReader`, `AdoReader` | Async open, command execution, and row reads. Connection, command, and reader live until enumeration finishes or is disposed. |
| `AdoStarParametersReader`, `AdoInputBatchReader` | Parameter rows stream from the database; batch reads hold at most `read-size` keys and stream matching rows inside a disposable transaction. |
| `ElasticReader` | Async search/scroll requests, one response page at a time (`read-size`, default 100). Next page is requested only after the current page is consumed. The latest scroll ID is cleared on completion, early disposal, cancellation, or mapping failure, using a separate cleanup token with a 10-second timeout. |
| `SolrInputReader` | Async cursor/offset pages (`read-size`, default 500), emitting rows before requesting the next page. Explicit `page`/`size` requests remain single-page reads. |
| CSV file/stream readers | Async record reads and streaming type conversion; no whole-result row list. Owned readers/files close when enumeration ends or is disposed. |
| JSON array file/stream readers | Incremental top-level array parsing with BOM encoding detection. Memory depends on parser buffers and individual JSON elements, not the whole array. |
| JSON Lines file/stream readers | Async line reads and one JSON document per line. JSON stream readers leave the caller's stream open; file readers own and close their files. |
| File and console readers | Async line reads, including command stdout. Early disposal/cancellation stops the owned command process. XML input remains one complete document row; line-pattern grouping retains the current logical record. |
| `LuceneReader` | Bounded `SearchAfter` pages of live matching documents; search/stored-field APIs are synchronous. The index reader is disposed with the enumerator. |
| Bogus, Excel, filesystem, default/parameter/internal readers | Lazy row enumeration with cancellation checks. Underlying generation/Excel/filesystem APIs are synchronous. Internal configuration rows already reside in memory; filesystem ordering still retains file metadata. |
| Composite, internal-key, and console provider wrappers | Forward async enumeration to their underlying readers. Parameter-row input requests just its first parent row and disposes the parent enumerator. |
| `ElasticQueryReader` | Incremental flattening without a complete output row list. Elastic.Transport still materializes the server's aggregation response; large aggregations are not bounded by `read-size`. |
| ADO connection with `buffer='true'` | Materializes all rows and closes input resources before yielding the first row. Default `false` streams. |
| `AdoEntityWriter` | Pulls at most `insert-size` input rows per batch, awaits matching, inserts and updates before requesting the next batch. |
| `AdoEntityInserter`, `AdoEntityUpdater` | Consume async batches with cancellable Dapper commands and a transaction per call. |
| `SqlServerWriter` | Async schema query and bulk copy; batches limited by `insert-size`, with async matching and updates. |
| ADO/internal output provider wrappers | Forward the stream to their underlying writer. |
| `InternalWriter` | Consumes incrementally but retains final configuration rows by design. |
| Base transforms and validators using their unchanged sequence implementation | Translate the base `Select(Operate)` behavior to async enumeration, preserving the run predicate and operation instance. |
| Built-in transforms and validators | All stream natively. Setup that is not available at construction (map loading, template compilation) runs once through the `Initialize()` hook or a dedicated `OperateStreamAsync`. No built-in operation materializes its input. |
| `geocode` / `place` transforms | Rate-limited parallel batches bounded by `update-size`; one batch is resident at a time. Row order within a batch is not preserved, as before. |
| `fromXml` / `toRow` / `fromLengths` | Row-expanding streams. Memory is bounded by the rows produced from a single input row, not the input. |
| Custom operations overriding `Operate(IEnumerable<IRow>)` | Materialize the input once and invoke `Operate(IEnumerable<IRow>)` once. Never invoke their single-row overload or restart the operation per batch. |
| Legacy reader/provider | Calls its existing async read once, then enumerates its result. It may buffer or perform synchronous I/O. |
| CSV, JSON, JSON Lines, file, console, trace, string, null and log writers | Consume the stream row by row and write as they go. |
| Solr, Elasticsearch and Lucene writers | Post batches bounded by `insert-size`; the parallel Solr writer takes its throttle before pulling the next batch, so at most `max-degree-of-parallelism` batches are resident. |
| GeoJSON writers | Stream natively, one feature at a time. The process writers emit the collection start and end on the first and last entity, so one document still spans several calls. |
| Excel and Razor writers | Implement `IWriteStream` but materialize inside it, on purpose: `SpreadsheetWriter` emits the workbook as a unit, and the Razor template receives the rows as a model it may enumerate more than once. The buffering is a requirement of those formats, and it is attributable to them rather than hidden in the adapter. |
| Legacy writer/provider | Materializes the entire incoming stream, then calls its existing async write once. This preserves whole-document output and writer initialization/finalization. |
| Custom legacy pipeline/controller | Execution falls back to its existing `ExecuteAsync`. |

Base classes check which type implements the sequence overload; a subclass overriding it gets the conservative fallback unless it also overrides `OperateStreamAsync`. This protects existing third-party subclasses, and no built-in operation depends on it — a unit test enforces that. For a custom operation, prefer the protected `Initialize()` hook for one-time setup and leave `Operate(IEnumerable<IRow>)` alone; that keeps the operation on the streaming path. `Initialize()` runs once per execution, before the first row, on both the synchronous and streaming paths, and may set `Run`. It is skipped entirely when the operation is already disabled. Implement `IOperateStream` directly when the operation filters, expands, batches, or holds a resource across enumeration. Explicit fallback changes input/operation timing: all input is read before that operation starts. Avoid relying on interleaved source side effects at a materialization boundary.

**No component shipped in this repository uses a compatibility adapter.** Every reader that produces rows implements `IReadStream`, every writer that consumes them implements `IWriteStream`, and every built-in transform and validator streams. A test linked into each streaming test project (`ShippedComponentsImplementTheStreamingContracts`) fails the build if a shipped reader or writer drops off the contract; the metadata-only input/output providers are exempt because their synchronous row method only throws. The adapters remain for third-party components, which is the compatibility promise of this minor release, and they now log at **warning** level naming the offending type, so a fallback can never happen silently. A strict mode that throws instead of buffering was considered and **deferred to 2.0**: it would break third-party providers that only implement the existing contracts, which a minor release must keep working. Third-party sequence overrides still buffer. All supported built-in row readers, transforms, validators, and row writers now expose streaming directly. The metadata-only ADO, Elasticsearch, Solr, and Lucene input-provider objects never supported row reads; Autofac registers separate entity readers for those providers. Custom readers without `IReadStream`, calculated-field output, and writers without `IWriteStream` retain compatibility adapters. Debug logs identify pipeline writer and base-operation materialization fallbacks. Native async enumeration does not guarantee asynchronous I/O in the database driver (SQLite, for example, may execute synchronously).

## Cancellation, failures, and transactions

Pass a request/job token through the whole execution. Native stream enumeration, ADO opens, reads, matching commands, writes, and bulk copy accept it. A token supplied through `WithCancellation` also reaches native readers. CSV and `StreamReader` APIs on `netstandard2.0` do not take a cancellation token: checks occur before/after each record or text-buffer read, so a pending text read may finish before cancellation is observed. Synchronous source APIs also observe cancellation between reads. Legacy components and existing lifecycle actions retain their own cancellation limitations. Transaction begin/commit/rollback and resource disposal use the synchronous APIs available on the current `netstandard2.0` target.

Native streaming data-read/write exceptions propagate. Successful completion actions, final updates, and end callbacks are skipped after an exception or cancellation. Async ADO key matching now propagates failures instead of returning an empty match set, which could otherwise cause duplicate inserts. There is no automatic retry.

There is **no process-wide transaction**. The ADO entity writer commits insert/update calls within each batch; SQL Server bulk copy uses internal batch transactions. A later read or write failure can leave earlier batches committed. Direct calls to the ADO streaming inserter/updater use one transaction across that call, rolling back on failure. Their counters advance after commit. External/custom writer counters and transaction guarantees remain the provider's responsibility. Resume/retry must account for previously committed work.

Streaming leaves an input reader open during output writes. For input and output against the same database, assess locks, connection-pool capacity, and driver restrictions; use separate connections. `buffer='true'` on the input is an explicit escape hatch when the source must close before output begins. Internal output and whole-document writers retain data; an async return type alone does not promise bounded process memory.

## OrchardCore.Transformalize follow-up

After publishing, update **every `Transformalize*` PackageReference to `1.5.0`**, including all `.Autofac` references, in `src/OrchardCore.Transformalize/OrchardCore.Transformalize.csproj` and any other project referencing these packages. The current project mixes versions from 1.0.0 through 1.4.4; using the aligned release avoids retaining an older provider through its wrapper.

The current call sites to review are:

- `Services/ArrangementRunService.cs` and `Services/ArrangementStreamService.cs`: call `ExecuteStreamAsync(token)` after creating the execution scope.
- `Services/LoadFormModifier.cs` and `Services/TransformalizeParametersModifier.cs`: opt into streaming where desired and pass the request token.
- `Services/PipelineAction.cs` and workflow/job entry points: carry the token through their service contracts to the execution call.

The run/stream services currently create a local scope inside the preparation block and retain only the controller. Keep that scope in a `using` covering preparation and **all** execution/consumption. For example, after adding a token to the service method:

```csharp
using var scope = await _container.CreateScopeAsync(process, _logger, streamWriter);
var controller = scope.Resolve<IProcessController>();
await controller.ExecuteStreamAsync(token);
```

Use `HttpContext.RequestAborted` for request work and the scheduler's cancellation token for jobs. Let cancellation propagate as cancellation; handle execution failures according to the existing application error policy. An HTTP response may already contain rows when a later failure occurs, so do not assume the status can still be changed after response streaming begins.

The service named `ArrangementStreamService` is not automatically a fully streaming HTTP exporter. Its registered output writer also needs `IWriteStream`; otherwise the compatibility adapter buffers before writing. Adapt any custom CSV/JSON/response writer to initialize once, consume the async sequence once, and finalize once. Verify ownership of the caller's `StreamWriter` and leave it open where the caller owns it.

Validate sync and streaming arrangements with internal output, CSV/JSON export, SQL Server input/output, Jint transforms/validators, row expansion, paging, cancellation, and large queries. Compare row values/order, actions, insert/update/delete counts, partial failures, and scope cleanup. Measure time to first output and peak memory on representative production providers before claiming throughput improvements.

## Framework and dependency

Core remains `netstandard2.0` and adds [Microsoft.Bcl.AsyncInterfaces 10.0.8](https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces/10.0.8), aligned with the existing 10.0.8 dependencies. Consumers need a compiler supporting C# async iterators for `await foreach`; older code can keep calling the existing APIs. See Microsoft's [async streams documentation](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/generate-consume-asynchronous-stream) for enumeration and cancellation syntax.

## Release verification

Validated on .NET 10 / macOS ARM64, 2026-09-11:

- Full solution build passed with zero warnings.
- Core: 132; ADO: 54; SQLite: 14; Jint: 14; CSV: 15; JSON: 18; Excel: 4; Bogus: 2; Fluid: 1; Razor: 1; GeoJson: 3; Geography: 7; Humanizer: 2; LambdaParser: 1; MySQL: 9; SQL Server: 19; PostgreSQL: 12; Solr: 16; Elasticsearch: 21; Lucene: 8 — all passed (353 tests).
- The MySQL, SQL Server, PostgreSQL, Solr, and Elasticsearch suites run against Docker testcontainers started per test assembly, on `Testcontainers 4.15.0`.
- The build produces no warnings. The previous `SSH.NET 2025.1.0` NU1903 advisory (GHSA-q939-rpr3-3284, high) came in transitively through `Testcontainers`; 4.14.0 and later depend on the patched `SSH.NET 2026.0.0`, so the bump clears it without a pin.
- SQLite, PostgreSQL, SQL Server, and MySQL Northwind scenarios each exercise both synchronous execution and the new streaming execution path.
- `NorthWindIntegrationMySqlAsync` deliberately uses different delta values than its synchronous counterpart. Both classes share the container's `northwind`/`northwindstar` databases, and MySQL's `ON UPDATE CURRENT_TIMESTAMP` only advances `modified_at` when a column actually changes value. Re-applying identical values would leave this arrangement's version field untouched, so no delta would be detected and the update assertions would fail depending on class order. SQL Server's `rowversion` and the PostgreSQL trigger advance on any update, so those pairs can share values.
- A core unit test asserts that no built-in transform or validator overrides `Operate(IEnumerable<IRow>)` without a native `OperateStreamAsync`, which would silently reintroduce whole-input buffering.
- SQLite transaction tests verify rollback on cancellation, source failure, and constraint failure; the entity-writer test verifies committed batches before a later input failure.
- All 63 SDK NuGet packages built into `artifacts/nuget/1.5.0`; their manifests use 1.5.0 for both package versions and dependencies on other Transformalize packages. No packages were published.

Regression tests also verify first-row delivery before EOF without creating later row objects, non-seekable JSON input, BOM/unicode parity, malformed JSON tails, search-page demand, scroll cleanup, and Lucene hit IDs/deleted documents.

This validation establishes behavior and bounded pull demand, not a throughput or peak-memory benchmark. The legacy non-SDK Oracle project is not part of the package build.
