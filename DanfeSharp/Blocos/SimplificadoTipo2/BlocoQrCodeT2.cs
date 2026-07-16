using DanfeSharp.Modelo;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão V (NT 2026.003) — QR Code de consulta, mínimo 25×25 mm (≈ 70,9 pt) com
    /// quiet zone de 3 mm. Renderizado apenas quando o consumer populou
    /// <see cref="DanfeViewModel.QrCode"/>; a geração do conteúdo (QR v3 para mod. 55)
    /// é responsabilidade de quem monta o ViewModel.
    /// </summary>
    internal class BlocoQrCodeT2 : ElementoBase
    {
        // 120 pt ≈ 42 mm, acima do mínimo de 25 mm da NT; quiet zone garantida pela
        // centralização na largura de 280 pt.
        private const float TamanhoQrCode = 120;

        public BlocoQrCodeT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y, Document context) : base(estilo)
        {
            if (string.IsNullOrWhiteSpace(viewModel.QrCode))
            {
                Y_NFC = y;
                return;
            }

            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);

            Y_NFC = y + 10;
            primitiveComposer.ShowText("CONSULTA VIA LEITOR DE QR CODE", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            Y_NFC += 10;

            using (var result = new MemoryStream())
            {
                var bitmap = GerarQrCode.GerarQRCode(viewModel.QrCode);
                bitmap.Save(result, ImageFormat.Jpeg);
                result.Position = 0;

                var image = org.pdfclown.documents.contents.entities.Image.Get(result);
                var imageXObject = image.ToXObject(context);
                primitiveComposer.ShowXObject(imageXObject, new PointF(140, Y_NFC), new SizeF(TamanhoQrCode, TamanhoQrCode), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            }

            Y_NFC += TamanhoQrCode + 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
