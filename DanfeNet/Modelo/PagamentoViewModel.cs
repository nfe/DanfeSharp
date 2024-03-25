using System.Collections.Generic;

namespace DanfeNet.Modelo
{
    public class PagamentoViewModel
    {
        /// <summary>
        /// YA01a - Grupo Detalhamento da Forma de Pagamento (detPag)
        /// VERSÃO 4.00
        /// </summary>
        public List<DetalheViewModel> DetalhePagamento { get; set; }

        /// <summary>
        /// Valor do troco (vTroco)
        /// VERSÃO 4.00
        /// </summary>
        public decimal? Troco { get; set; }
    }

    public class DetalheViewModel
    {
        public TipoIntegracaoPagamento? indPag { get; set; }

        /// <summary>
        /// Forma de pagamento (tPag)
        /// </summary>
        public FormaPagamento FormaPagamento { get; set; }

        /// <summary>
        /// Valor do Pagamento (vPag)
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Grupo de Cartões (card)
        /// </summary>
        public CartaoViewModel Cartão { get; set; }
    }

    public class CartaoViewModel
    {
        /// <summary>
        /// CNPJ da Credenciadora de cartão de crédito e/ou débito (CNPJ)
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// Bandeira da operadora de cartão de crédito e/ou débito (tBand)
        /// </summary>
        public BandeiraCartao Flag { get; set; }

        /// <summary>
        ///Número de autorização da operação cartão de crédito e/ou débito (cAut)
        /// </summary>
        public string Authorization { get; set; }

        /// <summary>
        ///     YA04a - Tipo de Integração para pagamento (tpIntegra)
        /// </summary>
        public TipoIntegracaoPagamento TpIntegracao { get; set; }
    }
}