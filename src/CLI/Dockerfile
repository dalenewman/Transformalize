#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/CLI/CLI.csproj", "src/CLI/"]
COPY ["Containers/Autofac/Transformalize.Container.Autofac.Standard.20/Transformalize.Container.Autofac.Standard.20.csproj", "Containers/Autofac/Transformalize.Container.Autofac.Standard.20/"]
COPY ["Compatibility/Transformalize.Standard.20/Transformalize.Standard.20.csproj", "Compatibility/Transformalize.Standard.20/"]
COPY ["Providers/Console/Console.Autofac.Standard.20/Console.Autofac.Standard.20.csproj", "Providers/Console/Console.Autofac.Standard.20/"]
COPY ["Providers/Console/Console.Standard.20/Console.Standard.20.csproj", "Providers/Console/Console.Standard.20/"]
RUN dotnet restore "src/CLI/CLI.csproj"
COPY . .
WORKDIR "/src/src/CLI"
RUN dotnet build "CLI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CLI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tfl.dll"]