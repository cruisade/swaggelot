FROM mcr.microsoft.com/dotnet/sdk:6.0.100 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./SampleOcelot.csproj ./SampleOcelot/
COPY ../Swaggelot/Swaggelot.csproj ./Swaggelot/
WORKDIR /app/SampleOcelot
RUN dotnet restore
RUN dotnet publish -c Release -o out

# copy and publish app and libraries
WORKDIR /app/
COPY . ./SampleOcelot/
COPY ../Swaggelot/. ./Swaggelot/
WORKDIR /app/SampleOcelot
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "SampleOcelot.dll"]