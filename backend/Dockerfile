# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY ProyectoAmbos_Alanski.csproj ./
RUN dotnet restore ProyectoAmbos_Alanski.csproj

# Copiar todo y compilar
COPY . ./
RUN dotnet publish ProyectoAmbos_Alanski.csproj -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# El puerto lo asigna Render dinámicamente con la variable $PORT
ENV ASPNETCORE_URLS=http://+:$PORT

# Comando de inicio
ENTRYPOINT ["dotnet", "ProyectoAmbos_Alanski.dll"]
