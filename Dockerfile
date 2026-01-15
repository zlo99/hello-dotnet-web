# -------------------------------
# Build stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source code
COPY . ./

# Build and publish the app
RUN dotnet publish -c Release -o /app/publish

# -------------------------------
# Runtime stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish ./

# Expose the port the app listens on
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "HelloWorld.dll"]
