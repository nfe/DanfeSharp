using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão III-A (NT 2026.003) — discriminação dos tributos da Reforma Tributária
    /// (IBS, CBS e IS quando houver). Omitido por inteiro quando o XML não tem o grupo
    /// IBSCBSTot (fidelidade ao XML).
    /// </summary>
    internal class BlocoTributosIbsCbsT2 : ElementoBase
    {
        public BlocoTributosIbsCbsT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            var tributos = viewModel.TributosIbsCbs;

            if (tributos == null)
            {
                Y_NFC = y;
                return;
            }

            primitiveComposer.BeginLocalState();

            Y_NFC = y + 10;
            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);
            primitiveComposer.ShowText("TRIBUTOS IBS/CBS", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);

            if (tributos.BaseCalculo.HasValue)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("BASE DE CÁLCULO IBS/CBS", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(tributos.BaseCalculo.Value.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            if (tributos.ValorIbs.HasValue)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("IBS", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(tributos.ValorIbs.Value.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            if (tributos.ValorIbsUF.HasValue)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("  IBS ESTADUAL (UF)", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(tributos.ValorIbsUF.Value.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            if (tributos.ValorIbsMun.HasValue)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("  IBS MUNICIPAL", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(tributos.ValorIbsMun.Value.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            if (tributos.ValorCbs.HasValue)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("CBS", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(tributos.ValorCbs.Value.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            if (tributos.ValorIS.HasValue)
            {
                Y_NFC += 10;
                primitiveComposer.ShowText("IMPOSTO SELETIVO (IS)", new PointF(25, Y_NFC), XAlignmentEnum.Left, YAlignmentEnum.Middle, 0);
                primitiveComposer.ShowText(tributos.ValorIS.Value.Formatar(), new PointF(250, Y_NFC), XAlignmentEnum.Right, YAlignmentEnum.Middle, 0);
            }

            Y_NFC += 10;
            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
