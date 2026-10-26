using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Bloco "Total dos Produtos e Total da Nota" do leiaute da Reforma Tributária (NT 2026.010, modelo de
    /// referência). Com o <see cref="BlocoTotalIcmsIpi" />, substitui o "Cálculo do Imposto" do leiaute atual.
    /// PIS/COFINS e o valor aproximado dos tributos seguem as mesmas opções do leiaute atual.
    /// </summary>
    class BlocoTotaisNota : BlocoBase
    {
        public BlocoTotaisNota(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var m = ViewModel.CalculoImposto;

            AdicionarLinhaCampos()
                .ComCampoNumerico("Valor Total dos Produtos", m.ValorTotalProdutos)
                .ComCampoNumerico("Valor do Frete", m.ValorFrete)
                .ComCampoNumerico("Valor do Seguro", m.ValorSeguro)
                .ComCampoNumerico("Desconto", m.Desconto)
                .ComCampoNumerico("Outras Despesas", m.OutrasDespesas)
                .ComLargurasIguais();

            var linha = AdicionarLinhaCampos();

            if (ViewModel.ExibirPisConfins)
            {
                linha.ComCampoNumerico("Valor do PIS", m.ValorPis)
                    .ComCampoNumerico("Valor da COFINS", m.ValorCofins);
            }

            if (ViewModel.ExibirIcmsInterestadual)
                linha.ComCampoNumerico("V. Tot. Trib.", m.ValorAproximadoTributos);

            linha.ComCampoNumerico("Valor Total da Nota", m.ValorTotalNota)
                .ComLargurasIguais();
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Total dos Produtos e Total da Nota";
    }
}
