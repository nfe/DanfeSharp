using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Bloco "Total do ICMS/IPI" do leiaute da Reforma Tributária (NT 2026.010, modelo de referência). As linhas de
    /// FCP/DIFAL e de ICMS monofásico só aparecem com valor no XML (item 4.4). O imposto de importação, fora do
    /// modelo, entra só quando a nota o tem, para não sumir do DANFE de importação.
    /// </summary>
    class BlocoTotalIcmsIpi : BlocoBase
    {
        public BlocoTotalIcmsIpi(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var m = ViewModel.CalculoImposto;

            var linha = AdicionarLinhaCampos()
                .ComCampoNumerico("Base de Cálculo do ICMS", m.BaseCalculoIcms)
                .ComCampoNumerico("Valor do ICMS", m.ValorIcms)
                .ComCampoNumerico("Base de Cálculo do ICMS ST", m.BaseCalculoIcmsSt)
                .ComCampoNumerico("Valor do ICMS ST", m.ValorIcmsSt)
                .ComCampoNumerico("Valor do IPI", m.ValorIpi);

            if (m.ValorII > 0)
                linha.ComCampoNumerico("Valor do Imp. Importação", m.ValorII);

            linha.ComLargurasIguais();

            if (m.PossuiFcpOuDifal)
            {
                AdicionarLinhaCampos()
                    .ComCampoNumerico("Valor do FCP", m.ValorFcp)
                    .ComCampoNumerico("Valor do FCP Retido por ST", m.ValorFcpSt)
                    .ComCampoNumerico("Valor do DIFAL na UF de Destino", m.vICMSUFDest)
                    .ComCampoNumerico("Valor do FCP na UF de Destino", (double?)m.vFCPUFDest)
                    .ComLargurasIguais();
            }

            if (m.PossuiIcmsMonofasico)
            {
                AdicionarLinhaCampos()
                    .ComCampoNumerico("BC do ICMS Monofásico", m.QuantidadeBcIcmsMono, 4)
                    .ComCampoNumerico("Valor do ICMS Monofásico", m.ValorIcmsMono)
                    .ComCampoNumerico("BC do ICMS Monofásico por Retenção", m.QuantidadeBcIcmsMonoReten, 4)
                    .ComCampoNumerico("Valor do ICMS Monofásico por Retenção", m.ValorIcmsMonoReten)
                    .ComLargurasIguais();
            }
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Total do ICMS / IPI";
    }
}
