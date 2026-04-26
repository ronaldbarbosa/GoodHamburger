FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GoodHamburger.Api/GoodHamburger.Api.csproj", "GoodHamburger.Api/"]
COPY ["GoodHamburger.Core/GoodHamburger.Core.csproj", "GoodHamburger.Core/"]
COPY ["GoodHamburger.Data/GoodHamburger.Data.csproj", "GoodHamburger.Data/"]
RUN dotnet restore "GoodHamburger.Api/GoodHamburger.Api.csproj"

COPY . .
RUN dotnet publish "GoodHamburger.Api/GoodHamburger.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GoodHamburger.Api.dll"]
