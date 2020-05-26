FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

# copy csproj and restore as distinct layers
COPY SampleOcelot/*.csproj ./SampleOcelot/
COPY Swaggelot/*.csproj ./Swaggelot/
WORKDIR /app/SampleOcelot
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY SampleOcelot/. ./SampleOcelot/
COPY Swaggelot/. ./Swaggelot/
WORKDIR /app/SampleOcelot
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/SampleOcelot/out ./
ENTRYPOINT ["dotnet", "SampleOcelot.dll"]

