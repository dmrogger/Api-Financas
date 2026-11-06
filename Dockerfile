# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ApiFinancas/ApiFinancas.csproj ApiFinancas/
RUN dotnet restore ApiFinancas/ApiFinancas.csproj

COPY ApiFinancas/ ApiFinancas/
RUN dotnet publish ApiFinancas/ApiFinancas.csproj -c Release -o /app/publish --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER app
ENTRYPOINT ["dotnet", "ApiFinancas.dll"]
