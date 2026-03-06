#!/usr/bin/env sh
set -eu

REPO="${TFL_REPO:-dalenewman/Transformalize}"
INSTALL_DIR="${TFL_INSTALL_DIR:-$HOME/.local/bin}"
VERSION="${TFL_VERSION:-latest}"

os="$(uname -s)"
arch="$(uname -m)"

case "$os" in
  Darwin)
    case "$arch" in
      arm64) rid="osx-arm64" ;;
      x86_64) rid="osx-x64" ;;
      *)
        echo "Unsupported macOS architecture: $arch" >&2
        exit 1
        ;;
    esac
    ;;
  Linux)
    case "$arch" in
      x86_64) rid="linux-x64" ;;
      aarch64|arm64) rid="linux-arm64" ;;
      *)
        echo "Unsupported Linux architecture: $arch" >&2
        exit 1
        ;;
    esac
    ;;
  *)
    echo "Unsupported operating system: $os" >&2
    exit 1
    ;;
esac

if [ "$VERSION" = "latest" ]; then
  archive_url="https://github.com/$REPO/releases/latest/download/tfl-$rid.tar.gz"
else
  archive_url="https://github.com/$REPO/releases/download/$VERSION/tfl-$rid.tar.gz"
fi

tmp_dir="$(mktemp -d)"
trap 'rm -rf "$tmp_dir"' EXIT INT TERM

echo "Downloading $archive_url"
curl -fsSL "$archive_url" -o "$tmp_dir/tfl.tar.gz"
tar -xzf "$tmp_dir/tfl.tar.gz" -C "$tmp_dir"

binary_path="$(find "$tmp_dir" -type f -name tfl -perm -u+x | head -n 1)"
if [ -z "${binary_path:-}" ]; then
  echo "Failed to find extracted tfl binary in archive." >&2
  exit 1
fi

mkdir -p "$INSTALL_DIR"
install -m 755 "$binary_path" "$INSTALL_DIR/tfl"

echo "Installed tfl to $INSTALL_DIR/tfl"
if ! echo ":$PATH:" | grep -q ":$INSTALL_DIR:"; then
  echo "Add $INSTALL_DIR to PATH to run 'tfl' directly."
fi
