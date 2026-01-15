using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.NFC
{
    internal class TabelaProdutosServicosNFC : ElementoBase
    {
        public TabelaProdutosServicosNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            Y_NFC = y switch
            {
                40 => 100,
                50 => 100 + 10,
                _ => Y_NFC
            };

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
            primitiveComposer.ShowText("CÓDIGO", new PointF(65, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("DESCRIÇÃO", new PointF(70, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;

            primitiveComposer.ShowText("QTD.", new PointF(70, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("UNID.", new PointF(100, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("PREÇO", new PointF(200, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("TOTAL", new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            foreach (var produto in viewModel.Produtos)
            {
                Y_NFC += 10;

                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
                primitiveComposer.ShowText(produto.Codigo.Length > 15 ? produto.Codigo.Substring(0, 10) : produto.Codigo,
                    new PointF(65, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.Descricao.Length > 40 ? produto.Descricao.Substring(0, 40) : produto.Descricao,
                    new PointF(70, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);

                Y_NFC += 10;

                primitiveComposer.ShowText(produto.Quantidade.Formatar(), new PointF(90, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.Unidade, new PointF(100, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.ValorUnitario.Formatar(), new PointF(200, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(produto.ValorTotal.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            Y_NFC += 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}