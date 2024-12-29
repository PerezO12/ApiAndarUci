using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Contracts.V1
{
    public class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = $"{Root}/{Version}";

        public static class Account
        {
            public const string RutaGenaral = Base + "/account";
            
            
            
            public const string Login = "login";
            public const string ObtenerPerfil = "obtener-perfil";
            public const string CambiarPassword = "cambiar-password";
        }
        public static class Carrera
        {
            public const string RutaGenaral = Base + "/carrera";
        }
        public static class Departamento
        {
            public const string RutaGenaral = Base + "/departamento";
        }
        public static class Encargado
        {
            public const string RutaGenaral = Base + "/encargado";
            public const string GetByUserId = "usuario/{id}";
            public const string RegistrarEncargado = "registrar-encargado";
            public const string CambiarLlaves = "cambiar-llave";
            public const string GenerarLlaves = "generar-llaves";

        }
        public static class Estudiante
        {
            public const string RutaGenaral = Base + "/estudiante";
            public const string GetByUserId = "usuario/{id}";
            public const string RegistrarEstudiante =  "registrar-estudiante";

        }
        public static class Facultad
        {
            public const string RutaGenaral = Base + "/facultad";
        }
        public static class Formulario
        {
            public const string RutaGenaral = Base + "/formulario";
            public const string GetFormularioEstudiante = "estudiantes";
            public const string GetAllFormulariosByEncargado = "encargados";
            public const string GetFormEstudianteByIdForEncargadoAsync = "encargados/{id}";
            public const string FirmarFormulario = "firmar/{id}";
        }
        public static class Rol
        {
            public const string RutaGenaral = Base + "/rol";
        }
        public static class Usuario
        {
            public const string RutaGenaral = Base + "/usuario";
            public const string RegistrarAdmin = "registrar-admin";
        }
    }
}