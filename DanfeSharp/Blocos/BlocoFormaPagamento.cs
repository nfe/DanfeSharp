using System.Collections.Generic;
using System.Linq;
using DanfeSharp.Elementos;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Bloco "Forma de Pagamento" da DANFE de NF-e modelo 55.
    /// Renderiza um <see cref="PagamentoDetalhe"/> por <c>DetalheViewModel</c>
    /// usando o mesmo padrão visual de <see cref="BlocoDuplicataFatura"/>:
    /// múltiplos detalhes lado a lado em uma <c>FlexibleLine</c>, cada um com
    /// 2 colunas (FORMA PAGAMENTO + VALOR).
    /// </summary>
    /// <remarks>
    /// <para>O bloco é omitido quando <see cref="DanfeViewModel.Pagamento"/> está vazio
    /// ou nenhum <see cref="PagamentoViewModel.DetalhePagamento"/> está preenchido —
    /// preserva compat com NF-e anteriores à NT 2016.002.</para>
    /// <para>Referência: openspec/specs/danfe-payment-block/spec.md.</para>
    /// </remarks>
    internal class BlocoFormaPagamento : BlocoBase
    {
        public BlocoFormaPagamento(DanfeViewModel viewModel, Estilo estilo)
            : base(viewModel, estilo)
        {
            var detalhes = ViewModel.Pagamento?
                .SelectMany(p => p.DetalhePagamento ?? new List<DetalheViewModel>())
                .Select(d => new PagamentoDetalhe(estilo, d))
                .ToList();

            if (detalhes == null || detalhes.Count == 0) return;

            var eh = detalhes.First().Height;

            // Espelha o layout de BlocoDuplicataFatura: N por linha conforme
            // orientação. Pagamento tem labels mais longas (ex.: "Pagamento
            // Instantâneo (PIX)" = 28 chars) — limitar cards/linha para a
            // descrição caber sem sobrepor com o VALOR adjacente.
            int numeroElementosLinha = ViewModel.IsPaisagem ? 3 : 2;
            int i = 0;

            while (i < detalhes.Count)
            {
                var fl = new FlexibleLine(Width, eh);

                int i2;
                for (i2 = 0; i2 < numeroElementosLinha && i < detalhes.Count; i2++, i++)
                {
                    fl.ComElemento(detalhes[i]);
                }

                // Preenche o restante da linha com células vazias (apenas
                // borda) — mesma técnica de BlocoDuplicataFatura, para que
                // o quadro vá até a margem direita quando há menos formas
                // de pagamento do que colunas.
                for (; i2 < numeroElementosLinha; i2++)
                    fl.ComElemento(new CelulaVazia(estilo));

                fl.ComLargurasIguais();
                MainVerticalStack.Add(fl);
            }
        }

        public override string Cabecalho => "Forma de Pagamento";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
    }
}
