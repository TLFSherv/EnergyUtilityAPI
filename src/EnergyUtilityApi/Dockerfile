FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["EnergyUtilityApi.csproj", "./"]
RUN dotnet restore

# Copy everything from src/EnergyUtilityApi into the container
COPY . .
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the built files from the build stage
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "EnergyUtilityApi.dll"]