#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY [".", "."]
RUN dotnet restore "Pyrite.Content.Interface.Http.Sample/Pyrite.Content.Interface.Http.Sample.csproj" --configfile "nuget.config"
COPY . .
RUN dotnet build "Pyrite.Content.Interface.Http.Sample/Pyrite.Content.Interface.Http.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pyrite.Content.Interface.Http.Sample/Pyrite.Content.Interface.Http.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pyrite.Content.Interface.Http.Sample.dll"]