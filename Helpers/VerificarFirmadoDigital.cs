using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiUCI.Helpers
{
    public class VerificarFirmadoDigital
    {
        public bool VerificarFirmaFormulario(string contenidoJson, byte[] firmaDigital, byte[] hashDocumento, byte[] llavePublicaBytes)
        {
            try
            {
                // Crear un objeto RSA y cargar la llave pública
                using var rsa = RSA.Create();

                rsa.ImportSubjectPublicKeyInfo(llavePublicaBytes, out _);

                var contenidoBytes = Encoding.UTF8.GetBytes(contenidoJson);
                // Generar el hash del contenido original
                var hasDocumento = SHA256.HashData(contenidoBytes);

                // Verificar la firma digital usando la llave pública
                bool firmaValida = rsa.VerifyHash(hasDocumento, firmaDigital, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return firmaValida;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine(ex.Message);
                return false;
            }
    }
    }
}