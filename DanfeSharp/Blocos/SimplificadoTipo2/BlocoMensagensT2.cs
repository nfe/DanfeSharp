using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System;
using System.Drawing;

namespace DanfeSharp.Blocos.SimplificadoTipo2
{
    /// <summary>
    /// Divisões VIII/IX (NT 2026.003) — mensagem fiscal (infAdFisco) e mensagem do
    /// contribuinte (infCpl + tributos aproximados da Lei 12.741/2012).
    /// </summary>
    internal class BlocoMensagensT2 : ElementoBase
    {
        private const int MaxLength = 80;

        public BlocoMensagensT2(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            Y_NFC = y;

            primitiveComposer.BeginLocalState();

            if (!string.IsNullOrWhiteSpace(viewModel.InformacoesAdicionaisFisco))
            {
                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, 7);
                MostrarTextoQuebrado(primitiveComposer, viewModel.InformacoesAdicionaisFisco);
            }

            if (viewModel.CalculoImposto.ValorAproximadoTributos > 0)
            {
                primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, 7);

                Y_NFC += 10;
                primitiveComposer.ShowText($"CONFORME LEI 12.741/2012 o valor aproximado dos tributos é {viewModel.CalculoImposto.ValorAproximadoTributos.Formatar()}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

                Y_NFC += 10;
                primitiveComposer.ShowText($"O valor aproximado dos tributos Federais é {viewModel.CalculoImposto.ValorAproximadoTributosFederais.Formatar()}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

                Y_NFC += 10;
                primitiveComposer.ShowText($"O valor aproximado dos tributos Estaduais é {viewModel.CalculoImposto.ValorAproximadoTributosEstaduais.Formatar()}", new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.InformacoesComplementares))
            {
                primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, 7);
                MostrarTextoQuebrado(primitiveComposer, viewModel.InformacoesComplementares);
            }

            primitiveComposer.End();
        }

        // Quebra o texto em linhas de até MaxLength caracteres (máx. 2, como no cupom
        // NFC-e), centralizadas.
        private void MostrarTextoQuebrado(PrimitiveComposer primitiveComposer, string texto)
        {
            Y_NFC += 10;

            if (texto.Length > MaxLength)
            {
                primitiveComposer.ShowText(texto.Substring(0, MaxLength), new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

                Y_NFC += 10;
                primitiveComposer.ShowText(texto.Substring(MaxLength, Math.Min(texto.Length - MaxLength, MaxLength)), new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            }
            else
            {
                primitiveComposer.ShowText(texto, new PointF(140, Y_NFC), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);
            }
        }
    }
}
