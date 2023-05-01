FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FileSorter.App/FileSorter.App.csproj", "FileSorter.App/"]
RUN dotnet restore "FileSorter.App/FileSorter.App.csproj"
COPY . .
WORKDIR /src/FileSorter.App
RUN dotnet build "FileSorter.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileSorter.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileSorter.App.dll"]
