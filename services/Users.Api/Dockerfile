FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
RUN apk update && apk upgrade
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
RUN apk update && apk upgrade
WORKDIR /src
COPY ["services/Users.Api/Users.Api.csproj", "services/Users.Api/"]
COPY ["shared/BasePathFilter/BasePathFilter.csproj", "shared/BasePathFilter/"]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
RUN dotnet restore "services/Users.Api/Users.Api.csproj"
COPY . .
WORKDIR "/src/services/Users.Api"
RUN dotnet build "Users.Api.csproj" \
    -c Release \
    -o /app/build

FROM build AS publish
RUN dotnet publish "Users.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Api.dll"]
