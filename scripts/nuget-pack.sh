#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'EOF'
Usage: ./scripts/nuget-pack.sh [options]

Packages NuGet modules by discovering SDK-style projects with IsPackable=true.

Options:
  --output-dir <path>         Output directory for .nupkg files (default: artifacts/nuget)
  --configuration <config>    Build configuration (default: Release)
  --package-version <version> Override package version for all packages
  --id-prefix <prefix>        Package id must start with this prefix (default: Transformalize)
  --dry-run                   Print package mapping and exit
  --help                      Show this help
EOF
}

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUTPUT_DIR="${OUTPUT_DIR:-$ROOT_DIR/artifacts/nuget}"
CONFIGURATION="${CONFIGURATION:-Release}"
PACKAGE_VERSION="${PACKAGE_VERSION:-}"
ID_PREFIX="${ID_PREFIX:-Transformalize}"
DRY_RUN=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir)
      OUTPUT_DIR="$2"
      shift 2
      ;;
    --configuration)
      CONFIGURATION="$2"
      shift 2
      ;;
    --package-version)
      PACKAGE_VERSION="$2"
      shift 2
      ;;
    --id-prefix)
      ID_PREFIX="$2"
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

PROJECT_FILES=()
while IFS= read -r project; do
  PROJECT_FILES+=("$project")
done < <(find "$ROOT_DIR/src" -type f -name '*.csproj' | sort)

get_msbuild_property() {
  local project="$1"
  local property="$2"
  dotnet msbuild "$project" -nologo "-getProperty:$property" | tr -d '\r'
}

PACKAGE_PROJECTS=()
SKIPPED_NOT_SDK=()
SKIPPED_NOT_PACKABLE=()
SKIPPED_PREFIX=()

for project in "${PROJECT_FILES[@]}"; do
  if ! grep -q '<Project Sdk=' "$project"; then
    SKIPPED_NOT_SDK+=("${project#$ROOT_DIR/}")
    continue
  fi

  is_packable="$(get_msbuild_property "$project" "IsPackable" | tr '[:upper:]' '[:lower:]')"
  if [[ "$is_packable" != "true" ]]; then
    SKIPPED_NOT_PACKABLE+=("${project#$ROOT_DIR/}")
    continue
  fi

  package_id="$(get_msbuild_property "$project" "PackageId")"
  if [[ -z "${package_id:-}" ]]; then
    package_id="$(get_msbuild_property "$project" "AssemblyName")"
  fi
  if [[ -z "${package_id:-}" ]]; then
    package_id="$(basename "$project" .csproj)"
  fi

  if [[ "$package_id" != "$ID_PREFIX"* ]]; then
    SKIPPED_PREFIX+=("${package_id}|${project#$ROOT_DIR/}")
    continue
  fi

  PACKAGE_PROJECTS+=("$project")
done

echo "Discovered ${#PACKAGE_PROJECTS[@]} packable project(s) with package id prefix '$ID_PREFIX'."

if [[ ${#SKIPPED_NOT_SDK[@]} -gt 0 ]]; then
  echo "Skipped non-SDK project(s):"
  printf '  - %s\n' "${SKIPPED_NOT_SDK[@]}"
fi

if [[ ${#SKIPPED_NOT_PACKABLE[@]} -gt 0 ]]; then
  echo "Skipped IsPackable!=true project(s):"
  printf '  - %s\n' "${SKIPPED_NOT_PACKABLE[@]}"
fi

if [[ ${#SKIPPED_PREFIX[@]} -gt 0 ]]; then
  echo "Skipped package id outside prefix '$ID_PREFIX':"
  printf '  - %s\n' "${SKIPPED_PREFIX[@]}"
fi

if [[ "$DRY_RUN" -eq 1 ]]; then
  echo "Dry run enabled. Projects that would be packed:"
  printf '  - %s\n' "${PACKAGE_PROJECTS[@]}"
  exit 0
fi

if [[ -d "$OUTPUT_DIR" ]]; then
  echo "Cleaning output directory: $OUTPUT_DIR"
  rm -f "$OUTPUT_DIR"/*.nupkg "$OUTPUT_DIR"/*.snupkg
fi
mkdir -p "$OUTPUT_DIR"

for project in "${PACKAGE_PROJECTS[@]}"; do
  echo "Packing: ${project#$ROOT_DIR/}"
  cmd=(
    dotnet pack "$project"
    --configuration "$CONFIGURATION"
    --output "$OUTPUT_DIR"
    --nologo
    -p:ContinuousIntegrationBuild=true
    -p:DebugType=None
    -p:DebugSymbols=false
    -p:IncludeSymbols=false
    -p:IncludeSource=false
  )
  if [[ -n "$PACKAGE_VERSION" ]]; then
    cmd+=(-p:PackageVersion="$PACKAGE_VERSION" -p:Version="$PACKAGE_VERSION")
  fi
  "${cmd[@]}"
done

echo "NuGet package output: $OUTPUT_DIR"
