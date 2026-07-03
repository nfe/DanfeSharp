using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão III (NT 2026.003) — totais da operação: quantidade de itens, valor dos
    /// produtos, acréscimos/descontos, valor total, formas de pagamento, valor pago e
    /// troco (a linha TROCO é sempre impressa, ainda que 0,00).
    /// </summary>
    internal class BlocoTotaisSimplificadoT2 : ElementoBase
    {
        public BlocoTotaisSimplificadoT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);

            Y_NFC = y + 10;
            primitiveComposer.ShowText("QTD. TOTAL DE ITENS", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.CalculoImposto.QuantidadeTotal.ToString(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;
            primitiveComposer.ShowText("VALOR DOS PRODUTOS", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.CalculoImposto.ValorTotalProdutos.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            var acrescimos = viewModel.CalculoImposto.ValorFrete + viewModel.CalculoImposto.ValorSeguro + viewModel.CalculoImposto.OutrasDespesas;

            if (acrescimos > 0)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("ACRÉSCIMOS (+)", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(acrescimos.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            if (viewModel.CalculoImposto.Desconto > 0)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("DESCONTO (-)", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(viewModel.CalculoImposto.Desconto.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            Y_NFC += 10;
            primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
            primitiveComposer.ShowText("VALOR TOTAL", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.CalculoImposto.ValorTotalNota.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("FORMAS DE PAGAMENTO", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);

            foreach (var pagamento in viewModel.Pagamento)
            {
                if (pagamento.DetalhePagamento == null) continue;

                foreach (var detalhe in pagamento.DetalhePagamento)
                {
                    Y_NFC += 10;
                    primitiveComposer.ShowText(detalhe.FormaPagamento.FormaPagamentoToString(), new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                    primitiveComposer.ShowText(detalhe.Valor.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
                }
            }

            Y_NFC += 10;
            primitiveComposer.ShowText("VALOR PAGO", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.ValorPagoTotal.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;
            primitiveComposer.ShowText("TROCO", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.TrocoTotal.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;
            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
