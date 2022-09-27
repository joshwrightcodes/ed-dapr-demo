FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
RUN apk update && apk upgrade
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
RUN apk update && apk upgrade
WORKDIR /src
COPY ["Services/UserGroups.Api/UserGroups.Api.csproj", "Services/UserGroups.Api/"]
COPY ["Directory.Build.props", ""]
COPY ["Directory.Packages.props", ""]
RUN dotnet restore "Services/UserGroups.Api/UserGroups.Api.csproj"
COPY . .
WORKDIR "/src/Services/UserGroups.Api"
RUN dotnet build "UserGroups.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserGroups.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserGroups.Api.dll"]
