# Proposal: asynchronous ETL streaming

Status: implemented as an opt-in API for **1.5.0** (2026-09-11). See [implementation and migration guidance](async-streaming-migration.md) for the chosen contracts, provider coverage, compatibility fallbacks, cancellation, and transaction behavior. The original proposal below records the motivation and evaluation ideas; performance benchmarking and a producer/consumer queue remain future work.

## Motivation

The ADO asynchronous readers currently collect the query result into a `List<IRow>` before returning it. `DefaultPipeline.ReadAsync()` awaits that result before applying transforms, and `ExecuteAsync()` then passes it to the writer. Large queries therefore retain all input rows and delay the first output until reading finishes.

Relevant implementation:

- [AdoInputReader](../src/Providers/Ado/Transformalize.Provider.Ado/AdoInputReader.cs)
- [AdoReader](../src/Providers/Ado/Transformalize.Provider.Ado/AdoReader.cs)
- [DefaultPipeline](../src/Transformalize/Impl/DefaultPipeline.cs)

This opportunity primarily concerns async callers. The CLI currently calls synchronous `Execute()`, and synchronous ADO reads already support streaming when buffering is disabled. Changing the async path alone would not automatically accelerate ordinary CLI runs.

## Intended benefits

- Begin transforming and writing before the entire input query finishes.
- Bound input buffering by batch size or queue capacity, rather than total row count.
- Reduce allocation pressure and peak memory usage for large inputs.
- Explore overlapping input and output when measurements show a benefit.

Async enumeration alone does not introduce parallel processing or guarantee higher throughput. Providers, transforms, and output consumers can still retain rows internally; memory bounds must be assessed across the whole pipeline.

## Contract changes

The current reader and writer contracts use:

```csharp
Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default);
Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default);
```

Prefer adding optional streaming interfaces alongside existing contracts, rather than changing those signatures or adding required members to existing interfaces. Illustrative names only:

```csharp
public interface IReadStream {
    IAsyncEnumerable<IRow> ReadStreamAsync(CancellationToken token = default);
}

public interface IWriteStream {
    Task WriteStreamAsync(
        IAsyncEnumerable<IRow> rows,
        CancellationToken token = default);
}
```

Providers, pipelines, process controllers, and Autofac registration would need to expose and compose these capabilities. Existing `IRead`, `IWrite`, `IInputProvider`, `IOutputProvider`, `IPipeline`, and `IProcessController` implementations should remain usable. Decide explicitly how callers opt in and how a mixed pipeline falls back when a component lacks streaming support.

The core targets `netstandard2.0`. [Microsoft.Bcl.AsyncInterfaces](https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces) provides the async enumeration and disposal interfaces for that target. Evaluate that dependency against multi-targeting or a future target-framework change; choose a package version when implementing the proposal.

## Transform compatibility

Existing operations consume `IEnumerable<IRow>` and can perform work before, during, and after enumeration. Some support only the sequence overload: the Jint transform's single-row overload, for example, throws `NotImplementedException`.

Do not blindly invoke `Operate(IRow)` for every transform or restart `Operate(IEnumerable<IRow>)` for every batch. Either approach can change initialization, state, aggregation, row expansion, or finalization behavior. Audit operations and distinguish those safe to adapt per row or batch from those needing a streaming implementation or explicit materialization. Preserve one operation instance and its state for the intended execution lifetime.

## Execution considerations

- Start with a pull-based stream: the writer requests input as it consumes it. If a producer/consumer queue is later introduced, bound its capacity and preserve row order and ownership. Slow output must limit further input buffering.
- Keep the connection, command, and data reader alive throughout enumeration. Dispose them on completion, cancellation, early termination, and failure.
- Propagate cancellation through connection opening, reads, writes, queue waits, and enumeration disposal.
- Define failure and transaction behavior. Streaming allows writes before a later input-read failure; preserve or explicitly document rollback, partial commits, retries, counters, and completion actions.
- Check same-database input/output arrangements, connection-pool usage, locks, and provider restrictions while a reader remains open during writes.
- Keep JavaScript engines and other mutable operations isolated. Streaming does not make these components safe for concurrent use.

Wrapping a lazy synchronous enumerable in a completed task can reduce buffering, but its database reads still block during enumeration. It is not a substitute for true asynchronous I/O. Likewise, collecting an async stream back into a list at an adapter boundary loses its memory and first-output benefits.

## Suggested evaluation

1. Measure the existing synchronous and asynchronous paths on a representative large ADO query. Record peak memory, allocated bytes, time to first output, total duration, and rows per second.
2. Prototype an optional streaming ADO reader and writer with a minimal compatible transform chain. Keep existing contracts and default behavior intact.
3. Test ordering, slow consumers, early termination, cancellation, read/write failures, and cleanup. Include stateful transforms, validators, row-producing operations, and explicit buffering fallbacks.
4. Compare pull-based streaming with bounded batches or a bounded queue. Separate memory and latency improvements from any throughput gains.
5. Decide whether to extend the capability to other providers and expose it through the CLI. Revisit package compatibility and versioning before adopting public contracts.

Open decisions include interface names, opt-in behavior, batch sizing, mixed-provider fallbacks, transaction guarantees, transform adaptation, and the minimum supported target frameworks.

## Deferred to 2.0

A strict mode that throws instead of buffering when a component lacks the streaming contract. In 1.5.0 the compatibility adapters log at warning level and name the offending type, but they still materialize, because a minor release has to keep third-party `IRead`/`IWrite` implementations working. Removing `Task<IEnumerable<IRow>> ReadAsync(CancellationToken)` from `IRead`/`IInputProvider`, or giving it a default implementation that forwards to `ReadStreamAsync`, belongs in the same release.
