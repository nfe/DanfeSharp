using System;
using System.Drawing;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp
{
    /// <summary>
    /// Elemento que desenha um <c>DetalheViewModel</c> do quadro
    /// "FORMA DE PAGAMENTO" da DANFE.
    /// Layout: 2 colunas horizontais lado a lado (FORMA PAGAMENTO | VALOR),
    /// cada coluna com label em cima e conteúdo abaixo — mesmo padrão visual
    /// de <see cref="Duplicata"/> (quadro Fatura/Duplicata).
    /// </summary>
    [AlturaFixa]
    internal class PagamentoDetalhe : ElementoBase
    {
        public Fonte FonteA { get; private set; }
        public Fonte FonteB { get; private set; }
        public DetalheViewModel ViewModel { get; private set; }

        // Title Case espelhando o padrão de Duplicata.cs (Chaves = {"Número",
        // "Vencimento:", "Valor:"}) — primeira label sem ':', segunda com.
        private static readonly String[] Chaves = { "Forma de Pagamento", "Valor:" };

        public PagamentoDetalhe(Estilo estilo, DetalheViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
            FonteA = estilo.CriarFonteRegular(7.5F);
            FonteB = estilo.CriarFonteNegrito(7.5F);
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var r = BoundingBox.InflatedRetangle(Estilo.PaddingSuperior, Estilo.PaddingInferior, Estilo.PaddingHorizontal);

            // Descrição: prevalece xPag (DetalheViewModel.Descricao) sobre o
            // [Description] do enum FormaPagamento — regra da spec
            // danfe-payment-block.
            String descricao = !String.IsNullOrEmpty(ViewModel.Descricao)
                ? ViewModel.Descricao
                : ViewModel.FormaPagamento.GetDescricao();

            String[] valores = { descricao, ((double?)(double)ViewModel.Valor).FormatarMoeda() };

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
