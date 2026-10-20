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
                .ComCampoNumerico("Valor do IBS UF", m.ValorIbsUf)
                .ComCampoNumerico("Valor do IBS Município", m.ValorIbsMun)
                .ComCampoNumerico("Valor do Imposto Seletivo", m.ValorIs)
                .ComLargurasIguais();

            if (m.PossuiMonofasia)
            {
                AdicionarLinhaCampos()
                    .ComCampoNumerico("Valor do IBS Monofásico", m.ValorIbsMono)
                    .ComCampoNumerico("Valor da CBS Monofásica", m.ValorCbsMono)
                    .ComCampoNumerico("Valor do IBS Monofásico por Retenção", m.ValorIbsMonoReten)
                    .ComCampoNumerico("Valor da CBS Monofásica por Retenção", m.ValorCbsMonoReten)
                    .ComLargurasIguais();
            }
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Total do IBS / CBS / IS";
    }
}
