using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace ApiUCI.Extensions
{
    public class ErrorBuilder
    {
        // Método para construir un error con un solo mensaje
        public static Dictionary<string, string[]> Build(string key, string message)
        {
            return new Dictionary<string, string[]>()
            {
                { key, new[] { message } } // Usamos un arreglo de cadenas directamente
            };
        }

        // Método para construir múltiples errores a partir de un diccionario
        public static Dictionary<string, string[]> BuildMultiple(Dictionary<string, string> errors)
        {
            return errors.ToDictionary(
                error => error.Key,
                error => new[] { error.Value } // Convertimos el valor a un arreglo de cadenas
            );
        }

        // Método para analizar errores de Identity y devolverlos en el formato correcto
        public static Dictionary<string, string[]> ParseIdentityErrors(IEnumerable<IdentityError> identityErrors)
        {
            var errors = new Dictionary<string, List<string>>(); // Usamos una lista temporal para acumular los errores

            foreach (var error in identityErrors)
            {
                if (!errors.ContainsKey(error.Code))
                {
                    errors[error.Code] = new List<string>(); // Inicializamos la lista si no existe
                }

                errors[error.Code].Add(error.Description); // Agregamos la descripción del error
            }

            // Convertimos las listas a arreglos antes de devolver el resultado
            return errors.ToDictionary(
                error => error.Key,
                error => error.Value.ToArray() // Convertimos las listas a arreglos
            );
        }
    }
}
