using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Models;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC;

internal class BlocoProdutoServicoTotalNFC : ElementoBase
{
    public BlocoProdutoServicoTotalNFC(Danfe viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
    {
        primitiveComposer.BeginLocalState();

        primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
        primitiveComposer.ShowText("QNT. TOTAL DE ITENS", new PointF(25, y + 10), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.QuantidadeTotal.ToString(), new PointF(250, y + 10), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

        primitiveComposer.ShowText("VALOR DOS PRODUTOS", new PointF(25, y + 20), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.ValorTotalProdutos.Formatar(), new PointF(250, y + 20), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

        //if (viewModel.CalculoImposto.ValorFrete > 0)
        //{
        primitiveComposer.ShowText("Valor do Frete", new PointF(25, y + 30), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.ValorFrete.Formatar(), new PointF(250, y + 30), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
        //}

        //if (viewModel.CalculoImposto.ValorSeguro > 0)
        //{
        primitiveComposer.ShowText("Valor do Seguro", new PointF(25, y + 40), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.ValorSeguro.Formatar(), new PointF(250, y + 40), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
        //}

        //if (viewModel.CalculoImposto.OutrasDespesas > 0)
        //{
        primitiveComposer.ShowText("OUTRAS DESPESAS", new PointF(25, y + 50), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.OutrasDespesas.Formatar(), new PointF(250, y + 50), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
        //}

        //if (viewModel.CalculoImposto.Desconto > 0)
        //{
        primitiveComposer.ShowText("DESCONTO (-)", new PointF(25, y + 60), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.Desconto.Formatar(), new PointF(250, y + 60), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
        //}

        primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
        primitiveComposer.ShowText("VALOR TOTAL", new PointF(25, y + 70), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
        primitiveComposer.ShowText(viewModel.CalculoImposto.ValorTotalNota.Formatar(), new PointF(250, y + 70), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

        Y_NFC = y + 80;

        primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
        primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

        primitiveComposer.Stroke();
        primitiveComposer.End();
    }
}