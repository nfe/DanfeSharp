using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Models;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC;

internal class BlocoInformacaoFiscal : ElementoBase
{
    public BlocoInformacaoFiscal(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
    {
        if (viewModel.TipoAmbiente == 2)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteTamanhoMinimo + 1);
            primitiveComposer.ShowText("EMITIDA EM AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL", new PointF(132.5F, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 20;
            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
        else
        {
            Y_NFC = y;
        }
    }
}