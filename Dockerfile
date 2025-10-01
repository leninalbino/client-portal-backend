# Use the official .NET 8.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the project files to the container
COPY ["src/ClientPortal.WebAPI/ClientPortal.WebAPI.csproj", "src/ClientPortal.WebAPI/"]
COPY ["src/ClientPortal.Application/ClientPortal.Application.csproj", "src/ClientPortal.Application/"]
COPY ["src/ClientPortal.Infrastructure/ClientPortal.Infrastructure.csproj", "src/ClientPortal.Infrastructure/"]
COPY ["src/ClientPortal.Domain/ClientPortal.Domain.csproj", "src/ClientPortal.Domain/"]

# Copy the solution file
COPY ["ClientPortal.sln", "./"]

# Restore dependencies
RUN dotnet restore "ClientPortal.sln"

# Copy the entire source code
COPY . .

# Build the application in Release mode
WORKDIR "/src/src/ClientPortal.WebAPI"
RUN dotnet build "ClientPortal.WebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ClientPortal.WebAPI.csproj" -c Release -o /app/publish

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=publish /app/publish .

# Create directory for file uploads
RUN mkdir -p /app/uploads/cv /app/uploads/photo

# Expose the port that the application will run on
EXPOSE 8080

# Set the environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Define the entry point for the container
ENTRYPOINT ["dotnet", "ClientPortal.WebAPI.dll"]