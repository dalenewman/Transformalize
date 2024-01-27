#!/bin/bash

# docker buildx create --driver=docker-container --name=container

version="0.10.17"
build="mcr.microsoft.com/dotnet/sdk:8.0"
base="mcr.microsoft.com/dotnet/runtime:8.0"
name="transformalize.cli"

# docker build -f "./src/CLI/Dockerfile" \
#   --force-rm \
#   -t dalenewman/$name:$version \
#   -t dalenewman/$name:latest \
#   --build-arg BASE_IMAGE=$base \
#   --build-arg BUILD_IMAGE=$build \
#   "."

# docker push dalenewman/$name:$version
# docker push dalenewman/$name:latest

docker buildx build --builder=container \
  --platform=linux/amd64,linux/arm64 \
  -f "./src/CLI/Dockerfile" \
  --force-rm \
  -t dalenewman/$name:$version \
  -t dalenewman/$name:latest \
  --build-arg BASE_IMAGE=$base \
  --build-arg BUILD_IMAGE=$build \
  --push .

build="mcr.microsoft.com/dotnet/sdk:8.0-alpine"
base="mcr.microsoft.com/dotnet/runtime:8.0-alpine"

# docker build -f "./src/CLI/Dockerfile" \
#   --force-rm \
#   -t dalenewman/$name:$version-alpine \
#   -t dalenewman/$name:$latest-alpine
#   --build-arg BASE_IMAGE=$base \
#   --build-arg BUILD_IMAGE=$build \
#   "."

# docker push dalenewman/$name:$version-alpine
# docker push dalenewman/$name:latest-alpine

docker buildx build --builder=container \
  --platform=linux/amd64,linux/arm64 \
  -f "./src/CLI/Dockerfile" \
  --force-rm \
  -t dalenewman/$name:$version-alpine \
  -t dalenewman/$name:latest-alpine \
  --build-arg BASE_IMAGE=$base \
  --build-arg BUILD_IMAGE=$build \
  --push .
