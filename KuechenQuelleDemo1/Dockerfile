FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY . KuechenQuelleDemo1/
RUN dotnet restore KuechenQuelleDemo1/KuechenQuelleDemo1.csproj
WORKDIR /src/KuechenQuelleDemo1
RUN dotnet build KuechenQuelleDemo1.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish KuechenQuelleDemo1.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

EXPOSE 80
ENTRYPOINT ["dotnet", "KuechenQuelleDemo1.dll"]
