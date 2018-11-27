using QRCoder;
using System.Drawing;

namespace DanfeSharp
{
    public class GerarQrCode
    {
        public static Bitmap GerarQRCode(string text)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText: text, eccLevel: QRCodeGenerator.ECCLevel.M, eciMode: QRCodeGenerator.EciMode.Iso8859_1);
                QRCode qrCode = new QRCode(qrCodeData);
                return qrCode.GetGraphic(20);
            }
            catch
            {
                throw;
            }
        }
    }
}