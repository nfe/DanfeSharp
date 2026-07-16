namespace DanfeSharp.Modelo
{
    /// <summary>
    /// <para>Totais dos tributos da Reforma Tributária (grupo IBSCBSTot) para a
    /// Divisão III-A do DANFE Simplificado Tipo 2 (NT 2026.003).</para>
    /// <para>Instância nula no <see cref="DanfeViewModel.TributosIbsCbs"/> significa
    /// que o XML não tem o grupo — a divisão é omitida (fidelidade ao XML).</para>
    /// </summary>
    public class TributosIbsCbsViewModel
    {
        /// <summary>
        /// Base de cálculo comum do IBS e da CBS (vBCIBSCBS)
        /// </summary>
        public decimal? BaseCalculo { get; set; }

        /// <summary>
        /// Valor total do IBS (vIBS)
        /// </summary>
        public decimal? ValorIbs { get; set; }

        /// <summary>
        /// Parcela estadual do IBS (vIBSUF)
        /// </summary>
        public decimal? ValorIbsUF { get; set; }

        /// <summary>
        /// Parcela municipal do IBS (vIBSMun)
        /// </summary>
        public decimal? ValorIbsMun { get; set; }

        /// <summary>
        /// Valor total da CBS (vCBS)
        /// </summary>
        public decimal? ValorCbs { get; set; }

        /// <summary>
        /// Valor total do Imposto Seletivo (vIS), quando houver
        /// </summary>
        public decimal? ValorIS { get; set; }
    }
}
