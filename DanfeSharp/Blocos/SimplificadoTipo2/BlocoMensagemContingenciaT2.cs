using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão VIII (NT 2026.003) — mensagem de contingência "EMITIDA EM CONTINGÊNCIA /
    /// PENDENTE DE AUTORIZAÇÃO", centralizada, caixa alta, 2 linhas. Instanciado em dois
    /// locais: abaixo do cabeçalho e abaixo da identificação da NF-e. Não renderiza nada
    /// quando a nota não está pendente de autorização.
    /// </summary>
    internal class BlocoMensagemContingenciaT2 : ElementoBase
    {
        public BlocoMensagemContingenciaT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            if (!viewModel.PendenteAutorizacao)
            {
                Y_NFC = y;
                return;
            }

            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);
            primitiveComposer.ShowText("EMITIDA EM CONTINGÊNCIA", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("PENDENTE DE AUTORIZAÇÃO", new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 30;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
