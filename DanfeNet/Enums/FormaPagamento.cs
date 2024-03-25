using System.ComponentModel;
using System.Xml.Serialization;

namespace DanfeNet;

/// <summary>
/// Meio de pagamento
/// </summary>
public enum FormaPagamento
{
    /// <summary>
    /// 01 - Dinheiro
    /// </summary>
    [Description("Dinheiro")]
    [XmlEnum("01")]
    fpDinheiro = 01,

    /// <summary>
    /// 02 - Cheque
    /// </summary>
    [Description("Cheque")]
    [XmlEnum("02")]
    fpCheque = 02,

    /// <summary>
    /// 03 - Cartão de Crédito
    /// </summary>
    [Description("Cartão de Crédito")]
    [XmlEnum("03")]
    fpCartaoCredito = 03,

    /// <summary>
    /// 04 - Cartão de Débito
    /// </summary>
    [Description("Cartão de Débito")]
    [XmlEnum("04")]
    fpCartaoDebito = 04,

    /// <summary>
    /// 05 - Crédito Loja
    /// </summary>
    [Description("Crédito Loja")]
    [XmlEnum("05")]
    fpCreditoLoja = 05,

    /// <summary>
    /// 10 - Vale Alimentação
    /// </summary>
    [Description("Vale Alimentação")]
    [XmlEnum("10")]
    fpValeAlimentacao = 10,

    /// <summary>
    /// 11 - Vale Refeição
    /// </summary>
    [Description("Vale Refeição")]
    [XmlEnum("11")]
    fpValeRefeicao = 11,

    /// <summary>
    /// 12  -Vale Presente
    /// </summary>
    [Description("Vale Presente")]
    [XmlEnum("12")]
    fpValePresente = 12,

    /// <summary>
    /// 13 - Vale Combustível
    /// </summary>
    [Description("Vale Combustível")]
    [XmlEnum("13")]
    fpValeCombustivel = 13,

    /// <summary>
    /// 14 - Duplicata Mercantil      
    /// <para>Na NT2016.002 (v1.50), foi excluída esta forma de pagamento na emissão de NFC-e (modelo 65), 
    /// porém para NFe (modelo 55) a SEFAZ, até o momento, ainda permite o seu uso.</para>
    /// <see langword="https://github.com/ZeusAutomacao/DFe.NET/issues/790"></see>
    /// </summary>
    [Description("Duplicata Mercantil")]
    [XmlEnum("14")]
    fpDuplicataMercantil = 14,

    /// <summary>
    /// 15 - Boleto Bancário
    /// </summary>
    [Description("Boleto Bancário")]
    [XmlEnum("15")] fpBoletoBancario = 15,
        
    /// <summary>
    /// 16 - Depósito Bancário
    /// </summary>
    [Description("=Depósito Bancário")]
    [XmlEnum("16")] fpDepositoBancario= 16,
        
    /// <summary>
    /// 17 - Pagamento Instantâneo (PIX)
    /// </summary>
    [Description("Pagamento Instantâneo (PIX)")]
    [XmlEnum("17")] fpPagamentoInstantaneoPIX = 17,
        
    /// <summary>
    /// 18 - Transferência bancária, Carteira Digital
    /// </summary>
    [Description("Transferência bancária, Carteira Digital")]
    [XmlEnum("18")] fpTransferenciabancaria = 18,
        
    /// <summary>
    /// 19 - Programa de fidelidade, Cashback, Crédito Virtual
    /// </summary>
    [Description("Programa de fidelidade, Cashback, Crédito Virtual")]
    [XmlEnum("19")] fpProgramadefidelidade = 19,
        

    /// <summary>
    /// 90 - Sem pagamento
    /// </summary>
    [Description("Sem pagamento")]
    [XmlEnum("90")]
    fpSemPagamento = 90,

    /// <summary>
    /// 99 - Outros
    /// </summary>
    [Description("Outros")]
    [XmlEnum("99")]
    fpOutro = 99
}