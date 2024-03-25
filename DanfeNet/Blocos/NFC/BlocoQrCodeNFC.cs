using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DanfeNet.Elementos;
using DanfeNet.Models;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC;

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
            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, 7);
                
            primitiveComposer.ShowText($"CONFORME LEI 12.741/2012 o valor aproximado dos tributos é {viewModel.CalculoImposto.ValorAproximadoTributos.Formatar()}", new PointF(140, y + 180), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText($"O valor aproximado dos tributos Federais é {viewModel.CalculoImposto.ValorAproximadoTributosFederais.Formatar()}", new PointF(140, y + 190), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText($"O valor aproximado dos tributos Estaduais é {viewModel.CalculoImposto.ValorAproximadoTributosEstaduais.Formatar()}", new PointF(140, y + 200), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
        }
    }
}