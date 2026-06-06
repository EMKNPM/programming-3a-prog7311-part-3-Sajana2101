# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GLMS2.API2/GLMS2.API2.csproj", "GLMS2.API2/"]

RUN dotnet restore "GLMS2.API2/GLMS2.API2.csproj"

COPY . .

WORKDIR "/src/GLMS2.API2"

RUN dotnet publish "GLMS2.API2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GLMS2.API2.dll"]