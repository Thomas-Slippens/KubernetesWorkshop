# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/WorkshopApp/WorkshopApp.csproj ./WorkshopApp/
RUN dotnet restore ./WorkshopApp/WorkshopApp.csproj

COPY src/WorkshopApp/ ./WorkshopApp/
RUN dotnet publish ./WorkshopApp/WorkshopApp.csproj -c Release -o /app/publish

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WorkshopApp.dll"]
