using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Bloco "Total do IBS/CBS/IS" (NT 2026.010, item 4.1), logo após o "Cálculo do Imposto". A linha da
    /// monofasia só aparece com o grupo gMono no XML (item 4.4: exibição condicionada à informação).
    /// </summary>
    class BlocoCalculoIbsCbsIs : BlocoBase
    {
        public BlocoCalculoIbsCbsIs(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var m = ViewModel.CalculoImposto;

            AdicionarLinhaCampos()
                .ComCampoNumerico("Valor da CBS", m.ValorCbs)
                .ComCampoNumerico("Valor do IBS UF", m.ValorIbsUF)
                .ComCampoNumerico("Valor do IBS Município", m.ValorIbsMun)
                .ComCampoNumerico("Valor do Imposto Seletivo", m.ValorIS)
                .ComLargurasIguais();

            if (m.PossuiMonofasia)
            {
                var linha = AdicionarLinhaCampos()
                    .ComCampoNumerico("Valor do IBS Monofásico", m.ValorIbsMono)
                    .ComCampoNumerico("Valor da CBS Monofásica", m.ValorCbsMono);

                // NT 2026.010 (4.4): os valores por retenção só quando houver tributação monofásica sujeita a retenção.
                if (m.PossuiMonofasiaRetencao)
                {
                    linha.ComCampoNumerico("Valor do IBS Monofásico por Retenção", m.ValorIbsMonoReten)
                        .ComCampoNumerico("Valor da CBS Monofásica por Retenção", m.ValorCbsMonoReten);
                }

                linha.ComLargurasIguais();
            }
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Total do IBS / CBS / IS";
    }
}
