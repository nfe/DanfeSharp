using System;
using System.Drawing;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp
{
    /// <summary>
    /// Elemento que desenha uma duplicata do quadro "FATURA / DUPLICATA" da DANFE.
    /// Layout: 3 colunas horizontais lado a lado (Número | Vencimento | Valor),
    /// cada coluna com label em cima e valor abaixo — segue o mesmo padrão visual
    /// do <c>BlocoCalculoImposto</c> e demais blocos com campos. Antes desta change
    /// os 3 campos ficavam empilhados verticalmente dentro de uma única coluna —
    /// reportado pelo cliente Revenda Mais durante revisão do PR #42.
    /// </summary>
    [AlturaFixa]
    internal class Duplicata : ElementoBase
    {
        public Fonte FonteA { get; private set; }
        public Fonte FonteB { get; private set; }
        public DuplicataViewModel ViewModel { get; private set; }

        private static readonly String[] Chaves = { "Número", "Vencimento:", "Valor:" };

        public Duplicata(Estilo estilo, DuplicataViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
            FonteA = estilo.CriarFonteRegular(7.5F);
            FonteB = estilo.CriarFonteNegrito(7.5F);
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var r = BoundingBox.InflatedRetangle(Estilo.PaddingSuperior, Estilo.PaddingInferior, Estilo.PaddingHorizontal);

            String[] valores = { ViewModel.Numero, ViewModel.Vecimento.Formatar(), ViewModel.Valor.FormatarMoeda() };

            float colWidth = r.Width / Chaves.Length;

            for (int i = 0; i < Chaves.Length; i++)
            {
                var colR = new RectangleF(r.X + i * colWidth, r.Y, colWidth, r.Height);
                gfx.DrawString(Chaves[i], colR, FonteA, AlinhamentoHorizontal.Esquerda);
                var valR = colR.CutTop(FonteA.AlturaLinha);
                gfx.DrawString(valores[i], valR, FonteB, AlinhamentoHorizontal.Esquerda);
            }
        }

        public override float Height
        {
            get => 2 * FonteB.AlturaLinha + Estilo.PaddingSuperior + Estilo.PaddingInferior;
            set => throw new NotSupportedException();
        }
    }
}
