# Architectural Analysis: Transition from IRead/IWrite to IInputProvider/IOutputProvider

This document explores the ongoing architectural shift in Transformalize from the granular `IRead`/`IWrite` interfaces to the more holistic `IInputProvider`/`IOutputProvider` interfaces.

## 1. The Author's Intent

The original design of Transformalize separated concerns into very small, specific interfaces:
- **`IRead`**: Just reads rows.
- **`IWrite`**: Just writes rows.
- **`IOutputController`**: Handled initialization and lifecycle (Start/End).
- **`IInputProvider` (originally `IVersionDetector`)**: Handled change detection.

### The Problem
This granular approach led to "interface explosion" and complicated the Autofac registration logic. In `Container.cs`, you can see the complexity of stitching together a pipeline by checking for the existence of multiple different interfaces. Furthermore, provider-specific metadata (like getting the max surrogate key or handling bulk-copy options) was often leaked into the controllers or the writer implementations.

### The Goal: Holistic Providers
The move to `IInputProvider` and `IOutputProvider` aims to:
1.  **Simplify Registration**: Instead of registering 5 different interfaces, a provider can register one.
2.  **Encapsulate Provider Logic**: All logic related to a specific technology (SQL Server, Elasticsearch, etc.) is contained within that technology's provider implementation.
3.  **Standardize Metadata Management**: Interfaces like `GetMaxVersion()`, `GetMaxTflKey()`, and `GetNextTflBatchId()` are now part of the contract, ensuring all providers handle incremental loads consistently.
4.  **Support Schema Discovery**: `IInputProvider.GetSchema()` allows providers to report their structure, facilitating automatic configuration generation.

## 2. Interface Comparison

| Feature | Old (IRead/IWrite) | New (IInput/IOutputProvider) |
| :--- | :--- | :--- |
| **Data Flow** | `Read()`, `Write()` | `Read()`, `Write()` |
| **Change Detection** | `IInputProvider.GetMaxVersion()` (partial) | Integrated `GetMaxVersion()` |
| **Lifecycle** | `IOutputController` | `Start()`, `End()`, `Initialize()` |
| **Metadata** | Scattered in ADO extensions | `GetMaxTflKey()`, `GetNextTflBatchId()` |
| **Deletes** | `IEntityDeleteHandler` | `Delete()`, `ReadKeys()`, `Match()` |
| **Schema** | `ISchemaReader` | `GetSchema()` |

## 3. Provider Readiness Assessment

Based on a scan of the codebase, the transition is currently in a "hybrid" state.

### ✅ Fully or Mostly Transitioned
*   **ADO (SQL Server, MySQL, PostgreSql, Sqlite)**:
    *   `AdoOutputProvider` and `AdoInputProvider` (via `AdoInputVersionDetector`) are well-established.
    *   They still often wrap an internal `IWrite` (like `SqlServerWriter`) but are registered as `IOutputProvider`.
    *   Note: Many methods in `AdoOutputProvider` still throw `NotImplementedException` (like `Delete()` or `Match()`), indicating that the full transition of delete logic hasn't happened yet.
*   **Elasticsearch**:
    *   Has `ElasticInputProvider` and `ElasticOutputProvider` (via `ElasticOutputVersionDetector`).
    *   Implements `GetMaxVersion()` for change detection.

### ⚠️ Partially Transitioned (Halfway)
*   **Console**: Has `ConsoleInputProvider` and `ConsoleOutputProvider`, but they are relatively simple wrappers around `ConsoleReader`/`ConsoleWriter`.
*   **Razor**: Has a `RazorOutputController` that behaves like a provider but isn't fully integrated into the new interface.
*   **File/Json/Csv**: Most of these still rely heavily on `IRead` and `IWrite` directly. While some have "Controllers", they haven't fully embraced the `IInputProvider` contract for things like schema discovery.

### ❌ Not Transitioned (Legacy)
*   **Excel / OpenXml**: Rely entirely on `IRead`/`IWrite`.
*   **Bogus**: Purely a reader (`BogusReader`).
*   **Kml / GeoJson**: Purely writers (`IWrite`).
*   **Mail**: Purely a writer (`MailWriter`).

## 4. Pipeline Logic (`DefaultPipeline.cs`)

The `DefaultPipeline` currently supports both models to maintain backward compatibility:

```csharp
// Prefer IRead, fallback to InputProvider.Read()
var data = Reader == null ? InputProvider.Read() : Reader.Read();

// Prefer IWrite, fallback to OutputProvider.Write()
if (Writer == null) {
    OutputProvider.Write(Read());
} else {
    Writer.Write(Read());
}
```

This "Dual Support" allows the system to function while providers are being updated one by one.

## 5. Conclusion & Recommendations

The author's goal of phasing out `IRead`/`IWrite` in favor of providers is sound but incomplete. 

**Next Steps for Completion:**
1.  **Implement missing methods** in `AdoOutputProvider` (especially `Match` and `Delete`) to allow the removal of `IBatchReader` and `IEntityDeleteHandler`.
2.  **Migrate File-based providers** (CSV, JSON) to implement `IInputProvider.GetSchema()`, which would greatly improve the "init" mode experience for flat files.
3.  **Refactor `DefaultPipeline`** to *only* accept `IInputProvider` and `IOutputProvider`, providing a "GenericProvider" wrapper for legacy `IRead`/`IWrite` implementations.
