using System;
using DanfeSharp.Esquemas.NFe;

namespace DanfeSharp.Modelo
{
    /// <summary>
    /// Leitura dos grupos da Reforma Tributária do XML (IBSCBS/IS do item, IBSCBSTot/ISTot do total) para o view
    /// model do DANFE (NT 2026.010). Chamado pelo <see cref="DanfeViewModelCreator" />.
    /// </summary>
    internal static class ReformaTributariaViewModelMapper
    {
        /// <summary>
        /// IBS/CBS e IS do item para o DANFE (NT 2026.010, item 4.3). A alíquota impressa é a efetiva
        /// (gRed/pAliqEfet) quando o grupo de redução existe — redução de alíquota ou compra governamental —
        /// e a vigente (pIBSUF/pIBSMun/pCBS) caso contrário.
        /// </summary>
        internal static void Mapear(ProdutoViewModel produto, ProdutoImposto imposto)
        {
            var ibsCbs = imposto?.IBSCBS;
            if (ibsCbs != null)
            {
                produto.ClassificacaoTributaria = String.IsNullOrWhiteSpace(ibsCbs.cClassTrib) ? null : ibsCbs.cClassTrib;

                var grupoIbsCbs = ibsCbs.gIBSCBS;
                if (grupoIbsCbs != null)
                {
                    produto.BaseIbsCbs = grupoIbsCbs.vBC;
                    produto.AliquotaIbsUF = grupoIbsCbs.gIBSUF?.gRed?.pAliqEfet ?? grupoIbsCbs.gIBSUF?.pIBSUF;
                    produto.ValorIbsUF = grupoIbsCbs.gIBSUF?.vIBSUF;
                    produto.AliquotaIbsMun = grupoIbsCbs.gIBSMun?.gRed?.pAliqEfet ?? grupoIbsCbs.gIBSMun?.pIBSMun;
                    produto.ValorIbsMun = grupoIbsCbs.gIBSMun?.vIBSMun;
                    produto.AliquotaCbs = grupoIbsCbs.gCBS?.gRed?.pAliqEfet ?? grupoIbsCbs.gCBS?.pCBS;
                    produto.ValorCbs = grupoIbsCbs.gCBS?.vCBS;
                }
            }

            var impostoSeletivo = imposto?.IS;
            if (impostoSeletivo != null)
            {
                produto.BaseIS = impostoSeletivo.vBCIS;
                produto.AliquotaIS = impostoSeletivo.pIS;
                produto.ValorIS = impostoSeletivo.vIS;
            }
        }

        /// <summary>Totais do bloco "Total do IBS/CBS/IS" (NT 2026.010, item 4.1).</summary>
        internal static void MapearTotais(CalculoImpostoViewModel calculo, Total total)
        {
            var ibsCbs = total?.IBSCBSTot;
            if (ibsCbs != null)
            {
                calculo.ValorCbs = ibsCbs.gCBS?.vCBS;
                calculo.ValorIbsUF = ibsCbs.gIBS?.gIBSUF?.vIBSUF;
                calculo.ValorIbsMun = ibsCbs.gIBS?.gIBSMun?.vIBSMun;

                if (ibsCbs.gMono != null)
                {
                    calculo.ValorIbsMono = ibsCbs.gMono.vIBSMono;
                    calculo.ValorCbsMono = ibsCbs.gMono.vCBSMono;
                    calculo.ValorIbsMonoReten = ibsCbs.gMono.vIBSMonoReten;
                    calculo.ValorCbsMonoReten = ibsCbs.gMono.vCBSMonoReten;
                }
            }

            calculo.ValorIS = total?.ISTot?.vIS;
        }
    }
}
