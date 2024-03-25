using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Modelo;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC
{
    internal class BlocoFormaPagamentoNFC : ElementoBase
    {
        public BlocoFormaPagamentoNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("FORMAS DE PAGAMENTO", new PointF(25, y + 10), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("VALOR PAGO", new PointF(250, y + 10), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            //primitiveComposer.ShowText("Troco", new PointF(160, y + 10), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);

            y = y + 10;

            foreach (var item in viewModel.Pagamento)
            {
                foreach (var detalhe in item.DetalhePagamento)
                {
                    y = y + 10;

                    primitiveComposer.ShowText(detalhe.FormaPagamento.FormaPagamentoToString(), new PointF(25, y), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                    primitiveComposer.ShowText(detalhe.Valor.Formatar(), new PointF(250, y), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
                }
            }

            Y_NFC = y + 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}