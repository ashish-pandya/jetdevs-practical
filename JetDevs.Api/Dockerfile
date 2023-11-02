FROM microsoft/aspnetcore:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:3.1 AS build
WORKDIR /src
COPY JetDevs.sln ./
COPY JetDevs.Api/JetDevs.Api.csproj JetDevs.Api/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/JetDevs.Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JetDevs.Api.dll"]
