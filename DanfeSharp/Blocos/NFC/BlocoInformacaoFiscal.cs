using DanfeSharp.Esquemas.NFe;
using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.NFC
{
    internal class BlocoInformacaoFiscal : ElementoBase
    {
        public BlocoInformacaoFiscal(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            var currentY = y;

            if (viewModel.TipoAmbiente == 2)
            {
                primitiveComposer.BeginLocalState();
                primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteTamanhoMinimo + 1);
                primitiveComposer.ShowText("EMITIDA EM AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL", new PointF(132.5F, currentY + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

                currentY += 20;
                primitiveComposer.DrawLine(new PointF(15, currentY), new PointF(265, currentY));
                primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
                primitiveComposer.Stroke();
                primitiveComposer.End();
            }

            // Contingência offline da NFC-e (tpEmis=9): a DANFE NFC-e DEVE indicar visivelmente que foi
            // emitida em contingência e está pendente de autorização, exibindo a data/hora de entrada em
            // contingência (dhCont) e a justificativa (xJust). Manual de Especificações Técnicas do DANFE
            // NFC-e. Os dados já vêm no view model (ContingenciaDataHora/ContingenciaJustificativa).
            if (viewModel.TipoEmissao == FormaEmissao.ContingenciaOffLineNFCe)
            {
                primitiveComposer.BeginLocalState();

                primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteTamanhoMinimo + 1);
                primitiveComposer.ShowText("EMITIDA EM CONTINGÊNCIA", new PointF(132.5F, currentY + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                currentY += 18;

                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteTamanhoMinimo);
                primitiveComposer.ShowText("Pendente de autorização da SEFAZ", new PointF(132.5F, currentY + 8), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                currentY += 12;

                if (viewModel.ContingenciaDataHora.HasValue)
                {
                    primitiveComposer.ShowText($"Entrada em contingência: {viewModel.ContingenciaDataHora.Value:dd/MM/yyyy HH:mm:ss}", new PointF(132.5F, currentY + 8), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                    currentY += 12;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.ContingenciaJustificativa))
                {
                    primitiveComposer.ShowText($"Justificativa: {viewModel.ContingenciaJustificativa}", new PointF(132.5F, currentY + 8), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                    currentY += 12;
                }

                currentY += 8;
                primitiveComposer.DrawLine(new PointF(15, currentY), new PointF(265, currentY));
                primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
                primitiveComposer.Stroke();
                primitiveComposer.End();
            }

            Y_NFC = currentY;
        }
    }
}
