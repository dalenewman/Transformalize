#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG BASE_IMAGE=mcr.microsoft.com/dotnet/runtime:8.0
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${BASE_IMAGE} AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM ${BUILD_IMAGE} AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/CLI/CLI.csproj", "src/CLI/"]
COPY ["src/Providers/Console/Console.Autofac.Standard.20/Console.Autofac.Standard.20.csproj", "src/Providers/Console/Console.Autofac.Standard.20/"]
COPY ["src/Providers/Console/Console.Standard.20/Console.Standard.20.csproj", "src/Providers/Console/Console.Standard.20/"]
COPY ["src/Transformalize/Transformalize.csproj", "src/Transformalize/"]
COPY ["src/Containers/Autofac/Autofac.Standard.20/Autofac.Standard.20.csproj", "src/Containers/Autofac/Autofac.Standard.20/"]
RUN dotnet restore "src/CLI/CLI.csproj"
COPY . .
WORKDIR "/src/src/CLI"
RUN dotnet build "CLI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CLI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "/app/tfl.dll"]