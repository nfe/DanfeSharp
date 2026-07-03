using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão II (NT 2026.003) — produtos/serviços: código, descrição, quantidade,
    /// unidade, valor unitário e valor total por item.
    /// </summary>
    internal class TabelaProdutosSimplificadoT2 : ElementoBase
    {
        public TabelaProdutosSimplificadoT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();

            Y_NFC = y + 10;

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("CÓDIGO", new PointF(65, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("DESCRIÇÃO", new PointF(70, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;

            primitiveComposer.ShowText("QTD.", new PointF(70, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("UNID.", new PointF(100, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("PREÇO", new PointF(200, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("TOTAL", new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);

            Y_NFC += 10;

            // Stroke imediato: texto emitido com path aberto é inválido pela spec do
            // PDF e some na extração/renderização estrita.
            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();

            foreach (var produto in viewModel.Produtos)
            {
                Y_NFC += 10;

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
