FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Workforce360.Api/Workforce360.Api.csproj src/Workforce360.Api/
RUN dotnet restore src/Workforce360.Api/Workforce360.Api.csproj
COPY . .
RUN dotnet publish src/Workforce360.Api/Workforce360.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet","Workforce360.Api.dll"]
