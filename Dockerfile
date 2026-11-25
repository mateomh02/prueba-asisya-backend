FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

COPY Backend/src/Asisya.Domain/Asisya.Domain.csproj Backend/src/Asisya.Domain/
COPY Backend/src/Asisya.Application/Asisya.Application.csproj Backend/src/Asisya.Application/
COPY Backend/src/Asisya.Infrastructure/Asisya.Infrastructure.csproj Backend/src/Asisya.Infrastructure/
COPY Backend/src/Asisya.Api/Asisya.Api.csproj Backend/src/Asisya.Api/
RUN dotnet restore Backend/src/Asisya.Api/Asisya.Api.csproj

COPY Backend/src/ Backend/src/
RUN dotnet publish Backend/src/Asisya.Api/Asisya.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["dotnet", "Asisya.Api.dll"]