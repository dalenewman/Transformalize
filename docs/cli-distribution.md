# CLI Distribution

`tfl` can be shipped as a self-contained single-file executable, so recipients do not need to install .NET.

## Local publish commands

Build one target locally:

```bash
dotnet publish src/CLI/CLI.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=false
```

Repeat with other runtime identifiers as needed:

- `osx-arm64`
- `osx-x64`
- `linux-x64`
- `linux-arm64`
- `win-x64`

## GitHub Actions release workflow

This repository includes [`.github/workflows/cli-release.yml`](../.github/workflows/cli-release.yml) which:

1. Builds self-contained single-file binaries for macOS, Linux, and Windows.
2. Packages them as release assets:
   - `tfl-osx-arm64.tar.gz`
   - `tfl-osx-x64.tar.gz`
   - `tfl-linux-x64.tar.gz`
   - `tfl-linux-arm64.tar.gz`
   - `tfl-win-x64.zip`
3. Publishes checksum files (`*.sha256`) for each archive.

### Triggering releases

- Automatic: push a tag like `v0.12.0`.
- Manual: run workflow dispatch and provide `release_tag`.

## Installer script (macOS/Linux)

Use [scripts/install-tfl.sh](../scripts/install-tfl.sh):

```bash
curl -fsSL https://raw.githubusercontent.com/dalenewman/Transformalize/master/scripts/install-tfl.sh | sh
```

Optional environment variables:

- `TFL_REPO` (default: `dalenewman/Transformalize`)
- `TFL_VERSION` (default: `latest`, or pass a tag like `v0.12.0`)
- `TFL_INSTALL_DIR` (default: `$HOME/.local/bin`)

## Do you need separate machines per OS?

Not strictly. .NET can cross-publish many RIDs from one machine.

For production distribution, building each target on its native GitHub-hosted runner (as configured in the workflow) is the safer default because it avoids edge cases in runtime packaging and native dependencies.
