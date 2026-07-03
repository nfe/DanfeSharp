using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisão VI (NT 2026.003) — identificação do consumidor: "CONSUMIDOR CNPJ:" ou
    /// "CONSUMIDOR CPF:" com nome e endereço quando presentes no XML. A obrigatoriedade
    /// em operação não presencial é validada na emissão (domínio), não aqui.
    /// </summary>
    internal class BlocoConsumidorT2 : ElementoBase
    {
        public BlocoConsumidorT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();
            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);

            var dest = viewModel.Destinatario;

            if (dest == null || string.IsNullOrWhiteSpace(dest.CnpjCpf))
            {
                Y_NFC = y + 10;
                primitiveComposer.ShowText("CONSUMIDOR NÃO IDENTIFICADO", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            }
            else
            {
                var rotuloDocumento = dest.CnpjCpf.Trim().Length == 14 ? "CNPJ" : "CPF";

                Y_NFC = y + 10;
                primitiveComposer.ShowText($"CONSUMIDOR {rotuloDocumento}: {Formatador.FormatarCpfCnpj(dest.CnpjCpf)}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

                if (!string.IsNullOrWhiteSpace(dest.RazaoSocial))
                {
                    Y_NFC += 10;
                    var nome = dest.RazaoSocial.Length > 60 ? dest.RazaoSocial.Substring(0, 60) : dest.RazaoSocial;
                    primitiveComposer.ShowText(nome, new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                }

                if (!string.IsNullOrWhiteSpace(dest.EnderecoLogadrouro) &&
                    !string.IsNullOrWhiteSpace(dest.Municipio))
                {
                    Y_NFC += 10;
                    primitiveComposer.ShowText($"{dest.EnderecoLogadrouro}, {dest.EnderecoNumero} - {dest.EnderecoBairro}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                    Y_NFC += 10;
                    primitiveComposer.ShowText($"{dest.Municipio} - {dest.EnderecoUf}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
                }
            }

            Y_NFC += 10;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}
