# Contributing to Transformalize

Thank you for considering contributing to Transformalize. This guide covers the basics of setting up a development environment, building, testing, and submitting changes.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (the CLI targets .NET 10; the core library targets .NET Standard 2.0)
- A C# editor such as Visual Studio, VS Code, or JetBrains Rider

## Building

From the repository root:

```bash
dotnet build
```

To build a specific project:

```bash
dotnet build src/Transformalize/Transformalize.csproj
dotnet build src/CLI/CLI.csproj
```

## Testing

Run all tests:

```bash
dotnet test
```

Some integration tests require external databases (SQL Server, PostgreSQL, MySQL, Elasticsearch, etc.). These tests use test containers, so you'll need Docker installed.

## Code Style

- Follow the existing patterns in the codebase.
- The solution uses `LangVersion=latest`.
- Use the same formatting and naming conventions found in nearby files.
- Prefer `async`/`await` for I/O-bound operations in providers.

## Pull Request Guidelines

1. Fork the repository and create a feature branch from `master`.
2. Keep changes focused: one logical change per pull request.
3. Include or update tests when applicable.
4. Make sure `dotnet build` and `dotnet test` pass before submitting.
5. Write a clear PR description explaining what the change does and why.

## Adding a New Provider

1. Create a new project under `src/Providers/<ProviderName>/`.
2. Implement `IInputProvider` and/or `IOutputProvider` from `Transformalize.Contracts`.
3. Create an Autofac module project (e.g., `Transformalize.Provider.<Name>.Autofac`) that registers your provider.
4. Add a `README.md` in the provider directory describing configuration and usage.
5. Reference the Autofac module from the CLI project (`src/CLI/CLI.csproj`) if the provider should be included in the CLI tool.

## Adding a New Transform (similar for Validator)

1. Create a class in `src/Transformalize/Transforms/` that extends `BaseTransform`.
2. Implement `IRow Operate(IRow row)` with your transformation logic.
3. Override `GetSignatures()` to return one or more `OperationSignature` instances that define the method name(s) used in the `t` attribute.
4. Register the transform in `src/Containers/Autofac/TransformBuilder.cs`.

## Reporting Issues

Use [GitHub Issues](https://github.com/dalenewman/Transformalize/issues) to report bugs or request features. Include:

- Steps to reproduce the problem
- The arrangement (XML/JSON) you are using (sanitized of secrets)
- Expected vs. actual behavior
- .NET SDK version and operating system

## License

By contributing, you agree that your contributions will be licensed under the [Apache License 2.0](LICENSE).
