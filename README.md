
# Sistema de Gestión de Facultad  

## Descripción  
Este proyecto, desarrollado en **C#** con **ASP.NET Core**, implementa un sistema de gestión para la administración de facultades universitarias. Utiliza **Entity Framework Core** como ORM y **PostgreSQL** como base de datos relacional.  

El sistema permite gestionar usuarios (estudiantes, profesores y administradores), realizar operaciones CRUD sobre las facultades y gestionar la baja de estudiantes recién graduados mediante formularios que deben ser aprobados digitalmente por los encargados de los departamentos correspondientes.  

### Contexto del Proyecto  
El objetivo del proyecto es implementar un sistema que facilite la baja de estudiantes recién graduados. El problema se plantea de la siguiente manera:  

1. **Modelo de Datos**:  
   - Cada estudiante está registrado con su nombre, correo institucional, número de carné, facultad, carrera y estado en el sistema.  
   - Para que un estudiante sea dado de baja, debe enviar un formulario firmado digitalmente por el encargado de su facultad.  
2. **Estructura Organizativa**:  
   - Las facultades incluyen: **Zorros**, **Escorpiones**, **Dragones** y **Gladiadores**.  
   - Cada facultad tiene cuatro departamentos: **Docencia**, **TI**, **Economía** y **Residencia**.  
   - Los encargados de los departamentos están registrados con un usuario, correo, número de identidad y un hash SHA256 que representa su firma digital.  
3. **Flujo del Proceso**:  
   - Los formularios enviados por los estudiantes son únicos para cada departamento.  
   - Un formulario es válido solo si está firmado digitalmente por el encargado correspondiente.  

## Tecnologías Utilizadas  
El proyecto integra varias tecnologías modernas para garantizar escalabilidad, seguridad y buen rendimiento:  

- **ASP.NET Core**: Framework para el desarrollo de aplicaciones web.  
- **Entity Framework Core**: ORM para la manipulación de datos.  
- **PostgreSQL**: Base de datos relacional.  
- **ASP.NET Core Identity**: Gestión de usuarios y roles.  
- **JWT Bearer**: Autenticación basada en tokens.  
- **FluentValidation**: Validación de DTOs.  
- **Redis**: Cache distribuido para mejorar el rendimiento.  
- **OpenAPI**: Generación automática de documentación para la API.  

## Instalación y Configuración  

### Requisitos Previos  
- .NET SDK instalado.  
- PostgreSQL configurado en tu entorno.  
- Herramientas como Postman o Swagger para probar la API.  

### Pasos de Instalación  

1. **Clonar el Repositorio**:  
   Clona el repositorio en tu máquina local:  
   ```bash  
   git clone https://github.com/tu-usuario/sistema-gestion-facultad.git  
   cd sistema-gestion-facultad  
   ```  

2. **Restaurar Dependencias**:  
   Instala los paquetes necesarios:  
   ```bash  
   dotnet restore  
   ```  

3. **Configurar la Base de Datos**:  
   Edita el archivo `appsettings.json` para configurar la conexión:  
   ```json  
   {  
     "ConnectionStrings": {  
       "DefaultConnection": "Server=localhost;Database=FacultadDB;User Id=tu_usuario;Password=tu_contraseña;"  
     }  
   }  
   ```  

4. **Aplicar Migraciones**:  
   Genera las tablas en la base de datos ejecutando:  
   ```bash  
   dotnet ef database update  
   ```  

5. **Ejecutar la Aplicación**:  
   Inicia el servidor de desarrollo:  
   ```bash  
   dotnet run  
   ```  

### Acceso a la API  
Una vez ejecutada, la API estará disponible en:  
`http://localhost:<puerto>/swagger`  

Utiliza **Swagger** para explorar y probar los endpoints de manera interactiva.  

## Autenticación y Autorización  

El proyecto implementa autenticación basada en **JWT Bearer**, lo que asegura que solo usuarios autenticados puedan acceder a los recursos protegidos.  

### Roles de Usuario  
- **Administrador**: Control total sobre la aplicación.  
- **Encargado**: Gestión de firmas digitales y validación de formularios.  
- **Estudiante**: Envío de formularios para su proceso de baja.  

Cada rol tiene acceso restringido a ciertos endpoints, asegurando un manejo seguro y eficiente de los permisos.  

## Documentación y Colaboración  
La documentación completa del proyecto está disponible en el portal de **Swagger**, donde se detallan los endpoints y los modelos utilizados.  

