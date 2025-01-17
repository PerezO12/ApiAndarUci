# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia solo los archivos necesarios para restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código y compila
COPY . ./
RUN dotnet publish -c Release -o /out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia los binarios publicados desde la etapa de build
COPY --from=build /out .

# Expón el puerto usado por la aplicación
EXPOSE 5000

# Establece la URL donde ASP.NET escuchará
ENV ASPNETCORE_URLS=http://+:5000

# Define el punto de entrada
ENTRYPOINT ["dotnet", "ApiUci.dll"]
