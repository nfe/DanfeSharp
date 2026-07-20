using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão IV (NT 2026.003) — consulta pela chave de acesso: URL da SEFAZ e chave
    /// de 44 dígitos em 11 blocos de 4 dígitos.
    /// </summary>
    internal class BlocoConsultaChaveT2 : ElementoBase
    {
        public BlocoConsultaChaveT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("Consulta pela chave de acesso em", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.EndConsulta, new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("CHAVE DE ACESSO", new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            // Chave em 11 blocos de 4 dígitos (54 caracteres com espaços) — fonte 7
            // para caber na largura útil do cupom.
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, 7);
            primitiveComposer.ShowText(viewModel.ChaveAcesso.SpaceOnAccessKey(), new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 50;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
