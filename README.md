
# Sistema de Gestión de Facultad

## Descripción
Este es un proyecto desarrollado en **ASP.NET Core** que utiliza **Entity Framework** y **PostgreSQL** para gestionar una base de datos de facultades. Permite crear, leer, actualizar y eliminar registros de facultades, además de gestionar usuarios, quienes se dividen en estudiantes y profesores. Este proyecto será ampliado de forma progresiva para añadir más funcionalidades.

## Tecnologías utilizadas
- **ASP.NET Core** - Framework para el desarrollo de aplicaciones web
- **Entity Framework Core** - ORM para el acceso y manipulación de datos en la base de datos
- **PostgreSQL** - Base de datos relacional para el almacenamiento de datos
- **Identity.Core** - Biblioteca para la gestión de usuarios y roles
- **JWT Bearer** - Protocolo para la autenticación mediante tokens
- **OpenAPI** - Herramienta para la documentación de la API

## Instalación y configuración

1. **Restaura los paquetes** del proyecto para instalar las dependencias necesarias:
    ```bash
    dotnet restore
    ```

2. **Configura la conexión a la base de datos** en el archivo `appsettings.json`:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=FacultadDB;User Id=tu_usuario;Password=tu_contraseña;"
      }
    }
    ```

3. **Aplica las migraciones** para crear la base de datos con las tablas correspondientes:
    ```bash
    dotnet ef database update
    ```

4. **Ejecuta la aplicación**:
    ```bash
    dotnet run
    ```


- Para explorar los endpoints de la API, puedes utilizar la documentación generada con **OpenAPI** accediendo a `http://localhost:puerto/swagger`.

## Autenticación y Autorización

Este proyecto utiliza **JWT Bearer** para manejar la autenticación y **ASP.NET Core Identity** para la gestión de roles y permisos de usuario. Solo ciertos endpoints están disponibles para usuarios autenticados y roles específicos, como administradores, encargados y estudiantes.


