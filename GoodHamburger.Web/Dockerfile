FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GoodHamburger.Web/GoodHamburger.Web.csproj", "GoodHamburger.Web/"]
COPY ["GoodHamburger.Core/GoodHamburger.Core.csproj", "GoodHamburger.Core/"]
RUN dotnet restore "GoodHamburger.Web/GoodHamburger.Web.csproj"

COPY . .
RUN dotnet publish "GoodHamburger.Web/GoodHamburger.Web.csproj" -c Release -o /app/publish --no-restore

FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY GoodHamburger.Web/nginx.conf /etc/nginx/conf.d/default.conf
