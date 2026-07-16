using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão I (NT 2026.003) — identificação do emitente + título destacado
    /// "DANFE Simplificado Tipo 2".
    /// </summary>
    internal class BlocoCabecalhoSimplificadoT2 : ElementoBase
    {
        public BlocoCabecalhoSimplificadoT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);

            primitiveComposer.ShowText("DANFE Simplificado Tipo 2", new PointF(140, 12), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            float y = 22;

            var emitente = viewModel.Emitente;
            var razaoSocial = emitente.RazaoSocial ?? string.Empty;

            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);

            if (razaoSocial.Length > 39)
            {
                primitiveComposer.ShowText(razaoSocial.Substring(0, 39), new PointF(140, y), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
                y += 10;
                primitiveComposer.ShowText(razaoSocial.Substring(39), new PointF(140, y), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            }
            else
            {
                primitiveComposer.ShowText(razaoSocial, new PointF(140, y), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            }

            y += 10;
            primitiveComposer.ShowText($"CNPJ: {Formatador.FormatarCnpj(emitente.CnpjCpf)}", new PointF(140, y), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);

            if (!string.IsNullOrWhiteSpace(emitente.EnderecoLogadrouro) &&
                !string.IsNullOrWhiteSpace(emitente.Municipio))
            {
                y += 10;
                primitiveComposer.ShowText($"{emitente.EnderecoLogadrouro}, {emitente.EnderecoNumero}", new PointF(140, y), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
                y += 10;
                primitiveComposer.ShowText($"{emitente.EnderecoBairro} - {emitente.Municipio} - {emitente.EnderecoUf}", new PointF(140, y), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            }

            Y_NFC = y + 15;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
