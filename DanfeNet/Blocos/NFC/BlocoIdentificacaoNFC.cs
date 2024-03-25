using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Modelo;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC
{
    internal class BlocoIdentificacaoNFC : ElementoBase
    {
        public BlocoIdentificacaoNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            
            primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
            primitiveComposer.ShowText($"Nº: {viewModel?.NfNumero}  Série: {viewModel?.NfSerie}", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);
            primitiveComposer.ShowText($"{viewModel?.DataHoraEmissao} - Via Consumidor", new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("PROTOCOLO DE AUTORIZAÇÃO", new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText($"{viewModel.ProtocoloAutorizacao}", new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 50;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}