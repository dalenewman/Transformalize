# Transformalize Pipeline Composition Analysis

This document describes how Transformalize uses Autofac to compose its ETL pipelines. The process is divided into two main stages: **Configuration** and **Execution**.

## 1. Configuration Phase (`ConfigurationContainer`)

The `ConfigurationContainer` is responsible for taking a raw configuration string and turning it into a hydrated `Process` object.

### Key Responsibilities:
*   **Shorthand Expansion**: Transformalize uses a shorthand notation for transforms (`t="..."`) and validators (`v="..."`). This container uses `ShorthandCustomizer` and `TransformModule`/`ValidateModule` to expand these into full operation objects before the `Process` is instantiated.
*   **Parameter Resolution**: It handles the transformation of parameters. If a parameter has a transform applied to it in the configuration, `ConfigurationContainer` runs a "mini-process" to calculate the parameter's final value.
*   **Validation**: It triggers the `Cfg.Net` loading process which validates the configuration against the schema.

## 2. Execution Phase (`Container`)

The `Container` class takes a `Process` object and registers the necessary components to execute the ETL tasks.

### Context Hierarchy
Transformalize uses nested contexts to provide metadata and logging to different parts of the pipeline:
*   **`PipelineContext`**: The global context for the entire process.
*   **`ConnectionContext`**: Scoped to a specific `Connection`.
*   **`InputContext` / `OutputContext`**: Scoped to a specific `Entity` and its input or output requirements.

### Entity Pipeline Composition
For every entity defined in the process, an `IPipeline` is registered. The pipeline is composed of:
1.  **Input Reader**: Resolved as `IRead` or `IInputProvider` (provided by modules like `SqlServerModule`).
2.  **Transform Chain**: 
    *   `IncrementTransform`: Handles internal row indexing.
    *   `DefaultTransform`: Applies default values to fields.
    *   `SystemHashcodeTransform`: Generates hash codes for change detection.
    *   **Custom Transforms**: Resolved via `TransformFactory` based on the entity's configuration.
    *   `SystemFieldsTransform`: Adds system-level fields (like `TflKey`).
3.  **Validation Chain**: Resolved via `ValidateFactory`.
4.  **Output Writer**: Resolved as `IWrite` or `IOutputProvider`.
5.  **Updater**: Resolved as `IUpdate` for handling incremental updates.

### Process Controller
The `IProcessController` is the orchestrator. It:
*   Holds a list of all `IPipeline` objects.
*   Manages **Pre-Actions**: Such as `IInitializer` (for `init` mode) and `Before` actions.
*   Manages **Post-Actions**: Such as `Flatten` actions and `After` actions.
*   Handles **Delete Handlers**: Executed if the entity configuration specifies `Delete="true"`.

## 3. Provider Modules (e.g., `SqlServerModule`)

Provider modules are where the generic interfaces are mapped to specific technologies.

### Example: `SqlServerModule`
*   **`IConnectionFactory`**: Registers `SqlServerConnectionFactory` to manage `SqlConnection` objects.
*   **Readers**: Registers `AdoInputReader` for reading from SQL Server.
*   **Writers**: Registers `SqlServerWriter` which handles bulk inserts and updates.
*   **Schema**: Registers `AdoSchemaReader` for discovering table structures.
*   **Controllers**: Registers `AdoStarController` to manage the creation of star schema views and flattened tables in SQL Server.
*   **Delete Handling**: Sets up `AdoDeleter` and `AdoReader` for identifying and removing records that no longer exist in the source.

## Summary of the Flow

1.  **Load Config**: `ConfigurationContainer` parses text -> `Process` object.
2.  **Register Components**: `Container` + `ProviderModules` register all logic for that `Process`.
3.  **Execute**: `IProcessController.Execute()` is called, which runs pre-actions, iterates through entity pipelines, and runs post-actions.
