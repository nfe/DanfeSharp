namespace DanfeSharp.Modelo
{
    public class CalculoImpostoViewModel
    {
        /// <summary>
        /// <para>Base de Cálculo do ICMS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public double BaseCalculoIcms { get; set; }

        /// <summary>
        /// <para>Valor Total do ICMS</para>
        /// <para>Tag vICMS</para>
        /// </summary>
        public double ValorIcms { get; set; }

        /// <summary>
        /// <para>Tag vICMSUFDest</para>
        /// </summary>
        public double? vICMSUFDest { get; set; }

        /// <summary>
        ///     Valor do ICMS relativo ao Fundo de Combate à Pobreza (FCP) da UF de destino (vFCPUFDest
        /// </summary>
        public decimal? vFCPUFDest { get; set; }

        /// <summary>
        /// <para>Tag vICMSUFRemet</para>
        /// </summary>
        public double? vICMSUFRemet { get; set; }

        /// <summary>
        /// <para>Tag vFCP</para>
        /// </summary>
        public double? ValorFcp { get; set; }

        /// <summary>
        /// <para>Base de Cálculo do ICMS ST</para>
        /// <para>Tag vBCST</para>
        /// </summary>
        public double BaseCalculoIcmsSt { get; set; }

        /// <summary>
        /// <para>Valor Total do ICMS ST</para>
        /// <para>Tag vST</para>
        /// </summary>
        public double ValorIcmsSt { get; set; }

        /// <summary>
        /// <para>Valor Total dos produtos e serviços</para>
        /// <para>Tag vProd</para>
        /// </summary>
        public double? ValorTotalProdutos { get; set; }

        /// <summary>
        /// <para>Valor Total do Frete</para>
        /// <para>Tag vFrete</para>
        /// </summary>
        public double ValorFrete { get; set; }

        /// <summary>
        /// <para>Valor Total do Seguro</para>
        /// <para>Tag vSeg</para>
        /// </summary>
        public double ValorSeguro { get; set; }

        /// <summary>
        /// <para>Valor Total do Desconto </para>
        /// <para>Tag vDesc</para>
        /// </summary>
        public double Desconto { get; set; }

        /// <summary>
        /// <para>Outras Despesas acessórias</para>
        /// <para>Tag vOutro</para>
        /// </summary>
        public double OutrasDespesas { get; set; }

        /// <summary>
        /// Valor do imposto de importação.
        /// </summary>
        public double ValorII { get; set; }

        /// <summary>
        /// <para>Valor Total do IPI</para>
        /// <para>Tag vIPI</para>
        /// </summary>
        public double ValorIpi { get; set; }

        /// <summary>
        /// Valor do PIS
        /// </summary>
        public double ValorPis { get; set; }

        /// <summary>
        /// Valor do COFINS
        /// </summary>
        public double ValorCofins { get; set; }

        /// <summary>
        /// <para>Valor Total da NF-e </para>
        /// <para>Tag vNF</para>
        /// </summary>
        public double ValorTotalNota { get; set; }

        /// <summary>
        /// <para>Valor aproximado total de tributos federais, estaduais e municipais (NT 2013.003)</para>
        /// <para>Tag vTotTrib</para>
        /// </summary>
        public double? ValorAproximadoTributos { get; set; }

        public double? ValorAproximadoTributosFederais { get; set; }
        public double? ValorAproximadoTributosEstaduais{ get; set; }

        /// <summary>
        /// Linhas impressas no bloco da Lei 12.741/2012: o total e, quando informadas, as parcelas
        /// federal e estadual. Usado para reservar a altura do bloco no cupom.
        /// </summary>
        public int LinhasTributosAproximados =>
            1 + (ValorAproximadoTributosFederais.HasValue ? 1 : 0) + (ValorAproximadoTributosEstaduais.HasValue ? 1 : 0);


        /// <summary>
        /// <para>Quantidade total de produtos distintos</para>
        /// </summary>
        public decimal QuantidadeTotal { get; set; }

        // ---- Bloco "Total do IBS/CBS/IS" (NT 2026.010, item 4.1) ----

        /// <summary>Valor da CBS (vCBS, W56).</summary>
        public double? ValorCbs { get; set; }

        /// <summary>Valor do IBS UF (vIBSUF, W41).</summary>
        public double? ValorIbsUf { get; set; }

        /// <summary>Valor do IBS Município (vIBSMun, W46).</summary>
        public double? ValorIbsMun { get; set; }

        /// <summary>Valor do Imposto Seletivo (vIS, W33).</summary>
        public double? ValorIs { get; set; }

        /// <summary>Valor do IBS monofásico (vIBSMono, W58). Só com o grupo gMono.</summary>
        public double? ValorIbsMono { get; set; }

        /// <summary>Valor da CBS monofásica (vCBSMono, W59). Só com o grupo gMono.</summary>
        public double? ValorCbsMono { get; set; }

        /// <summary>Valor do IBS monofásico por retenção (vIBSMonoReten, W59a). Só com o grupo gMono.</summary>
        public double? ValorIbsMonoReten { get; set; }

        /// <summary>Valor da CBS monofásica por retenção (vCBSMonoReten, W59b). Só com o grupo gMono.</summary>
        public double? ValorCbsMonoReten { get; set; }

        /// <summary>A nota tem totais da Reforma Tributária (IBSCBSTot ou ISTot).</summary>
        public bool PossuiTotaisReformaTributaria =>
            ValorCbs.HasValue || ValorIbsUf.HasValue || ValorIbsMun.HasValue || ValorIs.HasValue || PossuiMonofasia;

        /// <summary>A nota tem o grupo de monofasia nos totais (gMono, W57).</summary>
        public bool PossuiMonofasia =>
            ValorIbsMono.HasValue || ValorCbsMono.HasValue || ValorIbsMonoReten.HasValue || ValorCbsMonoReten.HasValue;
    }
}