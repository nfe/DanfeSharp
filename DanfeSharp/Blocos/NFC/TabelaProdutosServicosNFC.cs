using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.NFC
{
    internal class TabelaProdutosServicosNFC : ElementoBase
    {
        public TabelaProdutosServicosNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            if (y == 40)
                Y_NFC = 100;
            if(y == 50)
                Y_NFC = 100 + 10;

            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
            primitiveComposer.ShowText("DOCUMENTO AUXILIAR DA NOTA FISCAL", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("DE CONSUMIDOR ELETRÔNICA", new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.DrawLine(new PointF(15, y + 30), new PointF(265, y + 30));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);
            primitiveComposer.ShowText("DETALHE DA VENDA", new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.DrawLine(new PointF(15, y + 50), new PointF(265, y + 50));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("CÓDIGO", new PointF(40, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("DESC.", new PointF(80, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("QTD.", new PointF(120, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("UNID.", new PointF(160, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("PREÇO", new PointF(200, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("TOTAL", new PointF(240, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            foreach (var produto in viewModel.Produtos)
            {
                Y_NFC = Y_NFC + 10;
                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
                primitiveComposer.ShowText(produto.Codigo, new PointF(40, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.Descricao.Substring(0, 10), new PointF(80, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.Quantidade.Formatar(), new PointF(120, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.Unidade, new PointF(160, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.ValorUnitario.Formatar(), new PointF(200, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.ValorTotal.Formatar(), new PointF(240, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            }

            Y_NFC = Y_NFC + 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}