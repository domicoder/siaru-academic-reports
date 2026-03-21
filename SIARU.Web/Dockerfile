FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SIARU.Web/SIARU.Web.csproj", "SIARU.Web/"]
COPY ["SIARU.Application/SIARU.Application.csproj", "SIARU.Application/"]
COPY ["SIARU.Domain/SIARU.Domain.csproj", "SIARU.Domain/"]
COPY ["SIARU.Infrastructure/SIARU.Infrastructure.csproj", "SIARU.Infrastructure/"]

RUN dotnet restore "SIARU.Web/SIARU.Web.csproj"

COPY . .
WORKDIR "/src/SIARU.Web"
RUN dotnet publish "SIARU.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SIARU.Web.dll"]