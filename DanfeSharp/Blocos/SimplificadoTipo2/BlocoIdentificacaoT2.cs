using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão VII (NT 2026.003) — identificação da NF-e: número, série, data/hora de
    /// emissão em horário local, rótulo da via e protocolo de autorização (quando
    /// autorizada).
    /// </summary>
    internal class BlocoIdentificacaoT2 : ElementoBase
    {
        public BlocoIdentificacaoT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y, string rotuloVia) : base(estilo)
        {
            primitiveComposer.BeginLocalState();

            primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
            primitiveComposer.ShowText($"NF-e Nº: {viewModel.NfNumero}  Série: {viewModel.NfSerie}", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);
            primitiveComposer.ShowText($"{viewModel.DataHoraEmissao.FormatarDataHoraWithoutGMT()} - {rotuloVia}", new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 20;

            if (!string.IsNullOrWhiteSpace(viewModel.ProtocoloAutorizacao))
            {
                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
                primitiveComposer.ShowText("PROTOCOLO DE AUTORIZAÇÃO", new PointF(140, Y_NFC + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(viewModel.ProtocoloAutorizacao, new PointF(140, Y_NFC + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                Y_NFC += 20;
            }

            Y_NFC += 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
