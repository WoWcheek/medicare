#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RNDVU.API/RNDVU.API.csproj", "RNDVU.API/"]
RUN dotnet restore "RNDVU.API/RNDVU.API.csproj"
COPY . .
WORKDIR "/src/RNDVU.API"
RUN dotnet build "RNDVU.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RNDVU.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RNDVU.API.dll"]