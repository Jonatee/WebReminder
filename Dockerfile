# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["WebReminder.csproj", "."]
RUN dotnet restore "WebReminder.csproj"
COPY . .
RUN dotnet publish "WebReminder.csproj" -c Release -o /app/publish --no-restore

# Use the official .NET 9 ASP.NET runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "WebReminder.dll"]
