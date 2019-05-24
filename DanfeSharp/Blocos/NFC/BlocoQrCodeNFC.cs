using DanfeSharp.Modelo;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DanfeSharp.Blocos.NFC
{
    internal class BlocoQrCodeNFC : ElementoBase
    {
        public BlocoQrCodeNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y, Document context) : base(estilo)
        {
            var result = new MemoryStream();
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("CONSULTA VIA LEITOR DE QR CODE", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);

            var bitmap = GerarQrCode.GerarQRCode(viewModel.QrCode);
            bitmap.Save(result, ImageFormat.Jpeg);
            result.Position = 0;
            org.pdfclown.documents.contents.entities.Image image = org.pdfclown.documents.contents.entities.Image.Get(result);
            org.pdfclown.documents.contents.xObjects.XObject imageXObject = image.ToXObject(context);

            primitiveComposer.ShowXObject(imageXObject, new PointF(140, y + 20), new SizeF(150, 150), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);

            if (viewModel.CalculoImposto.ValorAproximadoTributos > 0)
            // valor aproximado dos tributos 
            {
                primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
                primitiveComposer.ShowText($"Valor Aproximado dos tributos Lei 12.741/2012: {viewModel.CalculoImposto.ValorAproximadoTributos.Formatar()}", new PointF(140, y + 180), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText($"Valor Aproximado dos tributos Federais: {viewModel.CalculoImposto.ValorAproximadoTributosFederais.Formatar()}", new PointF(140, y + 190), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText($"Valor Aproximado dos tributos Estaduais: {viewModel.CalculoImposto.ValorAproximadoTributosEstaduais.Formatar()}", new PointF(140, y + 200), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            }
        }
    }
}