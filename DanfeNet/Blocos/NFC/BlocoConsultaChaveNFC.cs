using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Modelo;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC
{
    internal class BlocoConsultaChaveNFC : ElementoBase
    {
        public BlocoConsultaChaveNFC(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText("Consulta pela chave de acesso em", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.EndConsulta, new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText("CHAVE DE ACESSO", new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            primitiveComposer.ShowText(viewModel.ChaveAcesso, new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 50;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}