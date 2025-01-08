using System.Security.Cryptography;
using System.Text;

namespace ApiUci.Helpers
{
    public class FirmaDigital
    {
        public bool VerificarFirmaFormulario(string contenidoJson, byte[] firmaDigital, byte[] hashDocumento, byte[] llavePublicaBytes)
        {
            try
            {
                // Crear un objeto RSA y cargar la clave pública
                using var rsa = RSA.Create();

                rsa.ImportSubjectPublicKeyInfo(llavePublicaBytes, out _);

                var contenidoBytes = Encoding.UTF8.GetBytes(contenidoJson);
                // Generar el hash del contenido original
                var hasDocumento = SHA256.HashData(contenidoBytes);

                // Verificar la firma digital usando la clave pública
                bool firmaValida = rsa.VerifyHash(hasDocumento, firmaDigital, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return firmaValida;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }
    }
}