# NuGet Packaging and Publishing (Cross-Platform)

This repository now supports cross-platform NuGet packaging and publishing with `dotnet` CLI scripts.

## Local packaging

```bash
./scripts/nuget-pack.sh
```

Output packages are written to `artifacts/nuget`.

Optional flags:

```bash
./scripts/nuget-pack.sh --package-version 0.12.1-beta
./scripts/nuget-pack.sh --dry-run
./scripts/nuget-pack.sh --fail-on-missing
```

`nuget-pack.sh` resolves package targets by discovering SDK-style `src/**/*.csproj` projects where `IsPackable=true`, and filtering by package id prefix (`Transformalize` by default).

## Local publishing

Publish to NuGet.org:

```bash
export NUGET_API_KEY="<token>"
./scripts/nuget-push.sh --source nuget --api-key-env NUGET_API_KEY
```

Publish to MyGet:

```bash
export MYGET_API_KEY="<token>"
./scripts/nuget-push.sh --source myget --api-key-env MYGET_API_KEY
```

Dry run:

```bash
./scripts/nuget-push.sh --source nuget --dry-run
```

## GitHub Actions (manual)

Workflow: `.github/workflows/nuget-publish.yml`

Required repository secrets:

- `NUGET_API_KEY` for NuGet.org publishing
- `MYGET_API_KEY` for MyGet publishing

Run the **NuGet Publish** workflow manually and select:

- package version
- whether to publish to NuGet.org
- whether to publish to MyGet

The workflow always uploads generated `.nupkg` files as artifacts.

## 1.5.0 streaming release

All 63 SDK package projects use version `1.5.0`, including their Autofac wrappers. See the [streaming migration guide](async-streaming-migration.md) before opting applications into the new execution path.

To keep this release separate from older local packages:

```bash
./scripts/nuget-pack.sh --output-dir artifacts/nuget/1.5.0
./scripts/nuget-push.sh --package-dir artifacts/nuget/1.5.0 --source nuget --api-key-env NUGET_API_KEY --dry-run
```

Remove `--dry-run` when ready to publish. The legacy non-SDK Oracle project is excluded from the packaging script.
