using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Aspose.BarCode.Generation;

public class QRCodeGenerator
{
    // Función asíncrona para generar un código QR
    public static async Task GenerateQRCodeAsync(string url, string outputFilePath, int pixels = 4, BarCodeImageFormat format = BarCodeImageFormat.Png)
    {
        try
        {
            BarcodeGenerator generator = new BarcodeGenerator(EncodeTypes.QR, url);
            
            generator.Parameters.Barcode.XDimension.Pixels = pixels;
            
            await Task.Run(() => generator.Save(outputFilePath, format));

            Console.WriteLine($"Código QR generado exitosamente: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generando el código QR: {ex.Message}");
        }
    }
}