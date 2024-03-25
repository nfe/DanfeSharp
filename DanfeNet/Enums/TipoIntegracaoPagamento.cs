using System.Xml.Serialization;

namespace DanfeNet;

/// <summary>
///     <para>1=Pagamento integrado com o sistema de automação da empresa(Ex.: equipamento TEF, Comércio Eletrônico);</para>
///     <para>Pagamento não integrado com o sistema de automação da empresa(Ex.: equipamento POS);</para>
/// </summary>
public enum TipoIntegracaoPagamento
{
    [XmlEnum("1")]
    TipIntegradoAutomacao = 1,

    [XmlEnum("2")]
    TipNaoIntegrado = 2
}