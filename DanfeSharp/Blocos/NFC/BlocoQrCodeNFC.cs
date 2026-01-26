using DanfeSharp.Modelo;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DanfeSharp.Blocos.NFC
{
    internal class BlocoQrCodeNFC : ElementoBase
    {
        public BlocoQrCodeNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y, Document context) : base(estilo)
        {
            Y_NFC = y + 10;

            var result = new MemoryStream();
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("CONSULTA VIA LEITOR DE QR CODE", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            Y_NFC += 10;

            var bitmap = GerarQrCode.GerarQRCode(viewModel.QrCode);
            bitmap.Save(result, ImageFormat.Jpeg);
            result.Position = 0;
            org.pdfclown.documents.contents.entities.Image image = org.pdfclown.documents.contents.entities.Image.Get(result);
            org.pdfclown.documents.contents.xObjects.XObject imageXObject = image.ToXObject(context);
            primitiveComposer.ShowXObject(imageXObject, new PointF(140, Y_NFC), new SizeF(150, 150), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            Y_NFC += 160;

            if (viewModel.CalculoImposto.ValorAproximadoTributos > 0)
            {
                primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, 7);
                
                primitiveComposer.ShowText($"CONFORME LEI 12.741/2012 o valor aproximado dos tributos é {viewModel.CalculoImposto.ValorAproximadoTributos.Formatar()}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                Y_NFC += 10;

                primitiveComposer.ShowText($"O valor aproximado dos tributos Federais é {viewModel.CalculoImposto.ValorAproximadoTributosFederais.Formatar()}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                Y_NFC += 10;

                primitiveComposer.ShowText($"O valor aproximado dos tributos Estaduais é {viewModel.CalculoImposto.ValorAproximadoTributosEstaduais.Formatar()}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                Y_NFC += 10;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.InformacoesComplementares))
            {
                const int MaxLength = 80;

                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, 7);

                if (viewModel.InformacoesComplementares.Length > MaxLength)
                {
                    // Split content in two lines

                    primitiveComposer.ShowText(viewModel.InformacoesComplementares.Substring(0, MaxLength), new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                    Y_NFC += 10;

                    primitiveComposer.ShowText(viewModel.InformacoesComplementares.Substring(MaxLength, Math.Min(viewModel.InformacoesComplementares.Length - MaxLength, MaxLength)), new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                    Y_NFC += 10;
                }
                else
                {
                    primitiveComposer.ShowText(viewModel.InformacoesComplementares, new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                    Y_NFC += 10;
                }
            }
        }
    }
}