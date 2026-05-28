using System.Collections.Generic;
using System.Linq;
using DanfeSharp.Elementos;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Bloco "Forma de Pagamento" da DANFE de NF-e modelo 55.
    /// Renderiza uma linha por <c>DetalheViewModel</c> com colunas FORMA PAGAMENTO + VALOR.
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
                .ToList();

            if (detalhes == null || detalhes.Count == 0) return;

            foreach (var d in detalhes)
            {
                var descricao = !string.IsNullOrEmpty(d.Descricao)
                    ? d.Descricao
                    : d.FormaPagamento.GetDescricao();

                AdicionarLinhaCampos()
                    .ComCampo("FORMA PAGAMENTO", descricao, AlinhamentoHorizontal.Esquerda)
                    .ComCampoNumerico("VALOR", (double)d.Valor, 2)
                    .ComLargurasIguais();
            }
        }

        public override string Cabecalho => "Forma de Pagamento";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
    }
}
