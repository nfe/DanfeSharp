using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.xObjects;

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

        /// <summary>QR Code como XObject do PDF (JPEG de qualidade máxima, o formato que o pdfclown carrega).</summary>
        internal static XObject GerarXObject(string text, Document document)
        {
            using (var stream = new MemoryStream())
            {
                using (var bitmap = GerarQRCode(text))
                using (var encoderParams = new EncoderParameters(1))
                {
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                    bitmap.Save(stream, ObterJpegEncoder(), encoderParams);
                }

                stream.Position = 0;
                return org.pdfclown.documents.contents.entities.Image.Get(stream).ToXObject(document);
            }
        }

        private static ImageCodecInfo ObterJpegEncoder()
        {
            foreach (var codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                    return codec;
            }

            return null;
        }
    }
}