using System;

namespace DanfeSharp.Modelo
{
    public class ProdutoViewModel
    {
        /// <summary>
        /// <para>Código do produto ou serviço</para>
        /// <para>Tag cProd</para>
        /// </summary>
        public String Codigo { get; set; }

        /// <summary>
        /// <para>Informações Adicionais do Produto</para>
        /// <para>Tag infAdProd</para>
        /// </summary>
        public String InformacoesAdicionais { get; set; }

        /// <summary>
        /// <para>Descrição do produto ou serviço</para>
        /// <para>Tag xProd</para>
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// <para>Código NCM com 8 dígitos ou 2 dígitos (gênero)</para>
        /// <para>Tag NCM</para>
        /// </summary>
        public String Ncm { get; set; }

        /// <summary>
        /// <para>Origem da mercadoria + Tributação do ICMS</para>
        /// <para>Tag orig e CST</para>
        /// </summary>
        public String OCst { get; set; }

        /// <summary>
        /// <para>Código Fiscal de Operações e Prestações</para>
        /// <para>Tag CFOP</para>
        /// </summary>
        public int Cfop { get; set; }

        /// <summary>
        /// <para>Unidade Comercial</para>
        /// <para>Tag uCom</para>
        /// </summary>
        public String Unidade { get; set; }

        /// <summary>
        /// <para>Quantidade Comercial</para>
        /// <para>Tag qCom</para>
        /// </summary>
        public double Quantidade { get; set; }

        /// <summary>
        /// <para>Valor Unitário de Comercialização</para>
        /// <para>Tag vUnCom</para>
        /// </summary>
        public double ValorUnitario { get; set; }

        /// <summary>
        /// <para>Valor Total Bruto dos Produtos ou Serviços</para>
        /// <para>Tag vProd</para>
        /// </summary>
        public double ValorTotal { get; set; }

        /// <summary>
        /// <para>Valor da BC do ICMS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public double BaseIcms { get; set; }

        ///<summary>
        /// Valor da BC do ICMS ST (vBCST)
        /// </summary>
        public double? BaseIcmsST { get; set; }

        /// <summary>
        /// <para>Valor do ICMS</para>
        /// <para>Tag vICMS</para>
        /// </summary>
        public double ValorIcms { get; set; }

        /// <summary>
        /// <para>Alíquota do imposto</para>
        /// <para>Tag pICMS</para>
        /// </summary>
        public double AliquotaIcms { get; set; }

        /// <summary>
        /// <para>Valor do IPI</para>
        /// <para>Tag vIPI</para>
        /// </summary>
        public double? ValorIpi { get; set; }

        /// <summary>
        /// <para>Alíquota do IPI</para>
        /// <para>Tag pIPI</para>
        /// </summary>
        public double? AliquotaIpi { get; set; }

        /// <summary>
        /// <para>Valor aproximado total de tributos federais, estaduais e municipais. [NT2013.003]</para>
        /// <para>Tag vTotTrib</para>
        /// </summary>
        public double? ValorAproximadoTributos { get; set; }

        /// <summary>
        /// <para>Chave de acesso (44 dígitos) da NF-e original referenciada por este item.</para>
        /// <para>Aplicável apenas em Notas de Crédito por Recusa Parcial
        /// (<c>finNFe=5</c> + <c>tpNFCredito=06</c>, Ajuste SINIEF 8/26).
        /// Mapeia para <c>det/DFeReferenciado/chaveAcesso</c>.</para>
        /// </summary>
        public String ChaveAcessoReferenciada { get; set; }

        /// <summary>
        /// <para>Número do item específico no documento referenciado.</para>
        /// <para>Aplicável apenas em Notas de Crédito por Recusa Parcial.
        /// Mapeia para <c>det/DFeReferenciado/nItem</c>.</para>
        /// </summary>
        public int? NumeroItemReferenciado { get; set; }

        public ProdutoViewModel()
        {
            AliquotaIpi = null;
            ValorIpi = null;
        }

        public String DescricaoCompleta
        {
            get
            {
                String descriCaoCompleta = Descricao;

                if (!String.IsNullOrWhiteSpace(InformacoesAdicionais))
                {
                    descriCaoCompleta += "\r\n" + InformacoesAdicionais;
                }

                // Para Nota de Crédito por Recusa Parcial: anexa a referência
                // ao item recusado da NF-e original na descrição (sem coluna
                // dedicada, mantendo o layout do DANFE clássico).
                if (!String.IsNullOrWhiteSpace(ChaveAcessoReferenciada))
                {
                    descriCaoCompleta += "\r\nNF-e ref.: " + ChaveAcessoReferenciada;
                    if (NumeroItemReferenciado.HasValue)
                    {
                        descriCaoCompleta += " — item " + NumeroItemReferenciado.Value;
                    }
                }

                return descriCaoCompleta;
            }
        }
    }
}