#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# find out way to migrate db
# ENV PATH $PATH:/root/.dotnet/tools
# RUN dotnet tool install -g dotnet-ef 
# RUN dotnet-ef migrations script --project "Infrastucture" -s "Api"

WORKDIR "/src"

COPY ["Api/", "Api/"]
COPY ["Szpek.Data/", "Szpek.Data/"]
COPY ["SzpekCore/", "SzpekCore/"]
COPY ["Infrastucture/", "Infrastucture/"]
COPY ["Dto/", "Dto/"]

WORKDIR "/src/Api"
RUN dotnet restore "Szpek.Api.csproj"
RUN dotnet build "Szpek.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Szpek.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Szpek.Api.dll"]