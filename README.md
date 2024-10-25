# Sistema de Gestión de Facultad

## Descripción
Este es un proyecto ASP.NET Core con Entity Framework que gestiona una base de datos de facultades, permitiendo crear, leer, actualizar y eliminar facultades, además de gestionar usuarios que se dividen en estudiantes y profesores.
Sera ampliado poco a poco

## Tecnologías utilizadas
- ASP.NET Core
- Entity Framework Core
- Postgresql

##Restaura los paquetes
- dotnet restore
- Configura la base de datos en el archivo appsettings.json:
 {
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FacultadDB;User Id=tu_usuario;Password=tu_contraseña;"
  }
}
- dotnet ef database update
- dotnet run

