# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# فقط csproj کپی کن
COPY UI_OpenAPI.csproj ./
RUN dotnet restore

# فقط source کپی کن (نه bin/obj)
COPY . ./
# publish بدون کپی bin
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "UI_OpenAPI.dll"]