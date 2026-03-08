#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'EOF'
Usage: ./scripts/nuget-push.sh [options]

Pushes all .nupkg files in a directory to NuGet or MyGet using dotnet CLI.

Options:
  --source <nuget|myget|url>  Package source (default: nuget)
  --package-dir <path>        Directory containing .nupkg files (default: artifacts/nuget)
  --version <version>         Only push packages matching this version (e.g. 1.2.3)
  --api-key <value>           API key/token value
  --api-key-env <env-name>    Read API key/token from environment variable
  --dry-run                   Print what would be pushed and exit
  --help                      Show this help
EOF
}

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SOURCE="${SOURCE:-nuget}"
PACKAGE_DIR="${PACKAGE_DIR:-$ROOT_DIR/artifacts/nuget}"
API_KEY="${API_KEY:-}"
API_KEY_ENV=""
VERSION=""
DRY_RUN=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    --source)
      SOURCE="$2"
      shift 2
      ;;
    --package-dir)
      PACKAGE_DIR="$2"
      shift 2
      ;;
    --version)
      VERSION="$2"
      shift 2
      ;;
    --api-key)
      API_KEY="$2"
      shift 2
      ;;
    --api-key-env)
      API_KEY_ENV="$2"
      shift 2
      ;;
    --dry-run)
      DRY_RUN=1
      shift
      ;;
    --help|-h)
      usage
      exit 0
      ;;
    *)
      echo "Unknown option: $1" >&2
      usage >&2
      exit 1
      ;;
  esac
done

case "$SOURCE" in
  nuget)
    SOURCE_URL="https://api.nuget.org/v3/index.json"
    ;;
  myget)
    SOURCE_URL="https://www.myget.org/F/transformalize/api/v3/index.json"
    ;;
  http://*|https://*)
    SOURCE_URL="$SOURCE"
    ;;
  *)
    echo "Invalid source '$SOURCE'. Use nuget, myget, or a full source URL." >&2
    exit 1
    ;;
esac

if [[ -n "$API_KEY_ENV" ]]; then
  API_KEY="${!API_KEY_ENV:-}"
fi

if [[ ! -d "$PACKAGE_DIR" ]]; then
  echo "Package directory not found: $PACKAGE_DIR" >&2
  exit 1
fi

PACKAGES=()
while IFS= read -r package; do
  PACKAGES+=("$package")
done < <(find "$PACKAGE_DIR" -maxdepth 1 -type f -name '*.nupkg' ! -name '*.snupkg' ! -name '*.symbols.nupkg' | sort)

if [[ -n "$VERSION" ]]; then
  FILTERED=()
  for pkg in "${PACKAGES[@]}"; do
    if [[ "$(basename "$pkg")" == *".${VERSION}.nupkg" ]]; then
      FILTERED+=("$pkg")
    fi
  done
  PACKAGES=("${FILTERED[@]}")
  if [[ ${#PACKAGES[@]} -eq 0 ]]; then
    echo "No .nupkg files matching version '$VERSION' found in: $PACKAGE_DIR" >&2
    exit 1
  fi
fi

if [[ ${#PACKAGES[@]} -eq 0 ]]; then
  echo "No .nupkg files found in: $PACKAGE_DIR" >&2
  exit 1
fi

if [[ "$DRY_RUN" -eq 1 ]]; then
  echo "Dry run enabled. Would push ${#PACKAGES[@]} package(s) to $SOURCE_URL:"
  printf '  - %s\n' "${PACKAGES[@]}"
  exit 0
fi

if [[ -z "$API_KEY" ]]; then
  echo "API key/token is required. Use --api-key or --api-key-env." >&2
  exit 1
fi

for package in "${PACKAGES[@]}"; do
  echo "Pushing: $(basename "$package")"
  dotnet nuget push "$package" \
    --source "$SOURCE_URL" \
    --api-key "$API_KEY" \
    --skip-duplicate \
    --no-symbols
done
