# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProyectoAmbos_Alanski/ProyectoAmbos_Alanski.csproj", "ProyectoAmbos_Alanski/"]
RUN dotnet restore "ProyectoAmbos_Alanski/ProyectoAmbos_Alanski.csproj"
COPY . .
WORKDIR "/src/ProyectoAmbos_Alanski"
RUN dotnet build "ProyectoAmbos_Alanski.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "ProyectoAmbos_Alanski.csproj" -c Release -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProyectoAmbos_Alanski.dll"]
