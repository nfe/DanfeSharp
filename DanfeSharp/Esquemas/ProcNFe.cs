using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DanfeSharp.Esquemas.NFe
{
    /// <summary>
    /// NF-e processada
    /// </summary>
    [XmlType(Namespace = Namespaces.NFe)]
    [XmlRoot("nfeProc", Namespace = Namespaces.NFe, IsNullable = false)]
    public class ProcNFe
    {
        public NFe NFe { get; set; }

        public ProtNFe protNFe { get; set; }

        [XmlAttribute]
        public string versao { get; set; }
    }

    /// <summary>
    /// Identificação do Ambiente
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.NFe)]
    public enum TAmb
    {
        [XmlEnum("1")]
        Producao = 1,

        [XmlEnum("2")]
        Homologacao = 2,
    }

    /// <summary>
    /// Dados do protocolo de status
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class InfProt
    {
        /// <summary>
        /// Identificação do Ambiente
        /// </summary>
        public TAmb tpAmb { get; set; }

        public string verAplic { get; set; }
        public string chNFe { get; set; }

        public DateTimeOffsetIso8601 dhRecbto { get; set; }

        public string nProt { get; set; }
        public int cStat { get; set; }
        public string xMotivo { get; set; }

        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }
    }

    /// <summary>
    /// Tipo Protocolo de status resultado do processamento da NF-e<
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.NFe)]
    public partial class ProtNFe
    {
        public InfProt infProt { get; set; }

        [XmlAttribute]
        public string versao { get; set; }
    }

    [Serializable]
    //[XmlRoot(Namespace = Namespaces.NFe, IsNullable = false)]
    //public partial class NFe
    [XmlType(Namespace = Namespaces.NFe)]
    public class NFe
    {
        public InfNFe infNFe { get; set; }

        /// <summary>
        /// ZX01 - Informações suplementares da Nota Fiscal
        /// </summary>
        public infNFeSupl infNFeSupl;
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.NFe)]
    public class Endereco
    {
        /// <summary>
        /// Logradouro
        /// </summary>
        public string xLgr { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public string nro { get; set; }

        /// <summary>
        /// Complemento
        /// </summary>
        public string xCpl { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        public string xBairro { get; set; }

        /// <summary>
        /// Código do município 
        /// </summary>
        public string cMun { get; set; }

        /// <summary>
        /// Nome do município
        /// </summary>
        public string xMun { get; set; }

        /// <summary>
        /// Sigla da UF
        /// </summary>
        public string UF { get; set; }

        /// <summary>
        /// Código do CEP
        /// </summary>
        public string CEP { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string fone { get; set; }
    }

    /// <summary>
    /// Nota Técnica 2018.005
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.NFe)]
    public class LocalEntregaRetirada : Endereco
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }

        /// <summary>
        /// Razão Social ou Nome do Expedidor/Recebedor
        /// </summary>
        public string xNome { get; set; }

        /// <summary>
        /// Inscrição Estadual do Estabelecimento Expedidor/Recebedor
        /// </summary>
        public string IE { get; set; }
    }


    public class Empresa
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string xNome { get; set; }
        public string IE { get; set; }
        public string IEST { get; set; }
        public string email { get; set; }

        [XmlIgnore]
        public virtual Endereco Endereco { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Destinatario : Empresa
    {
        public string ISUF { get; set; }
        //public string email;

        [XmlElement("enderDest")]
        public override Endereco Endereco { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Emitente : Empresa
    {
        public string xFant { get; set; }
        public string IM { get; set; }
        public string CNAE { get; set; }

        [XmlElement("enderEmit")]
        public override Endereco Endereco { get; set; }

        /// <summary>
        /// Código de Regime Tributário
        /// </summary>
        public string CRT { get; set; }
    }

    /// <summary>
    /// Dados dos produtos e serviços da NF-e
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Produto
    {
        public string cProd { get; set; }
        public string cEAN { get; set; }
        public string xProd { get; set; }
        public string NCM { get; set; }
        public string EXTIPI { get; set; }
        public int CFOP { get; set; }
        public string uCom { get; set; }
        public double qCom { get; set; }
        public double vUnCom { get; set; }
        public double vProd { get; set; }
        public string cEANTrib { get; set; }
        public string uTrib { get; set; }
        public string qTrib { get; set; }
        public string vUnTrib { get; set; }
        public string vFrete { get; set; }
        public string vSeg { get; set; }
        public string vDesc { get; set; }
        public string vOutro { get; set; }
        public string xPed { get; set; }
        public string nItemPed { get; set; }
        public string nFCI { get; set; }
    }

    [Serializable]
    [XmlTypeAttribute(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class ImpostoICMS
    {
        public string orig { get; set; }
        public string CST { get; set; }
        public string CSOSN { get; set; }
        public double vBC { get; set; }
        public double pICMS { get; set; }
        public double vICMS { get; set; }
        public double? vBCST { get; set; }
    }

    public class ImpostoICMS00 : ImpostoICMS { }
    public class ImpostoICMS02 : ImpostoICMS { }
    public class ImpostoICMS10 : ImpostoICMS { }
    public class ImpostoICMS15 : ImpostoICMS { }
    public class ImpostoICMS20 : ImpostoICMS { }
    public class ImpostoICMS30 : ImpostoICMS { }
    public class ImpostoICMS40 : ImpostoICMS { }
    public class ImpostoICMS51 : ImpostoICMS { }
    public class ImpostoICMS53 : ImpostoICMS { }
    public class ImpostoICMS60 : ImpostoICMS { }
    public class ImpostoICMS61 : ImpostoICMS { }
    public class ImpostoICMS70 : ImpostoICMS { }
    public class ImpostoICMS90 : ImpostoICMS { }
    public class ImpostoICMSPart : ImpostoICMS { }
    public class ImpostoICMSSN101 : ImpostoICMS { }
    public class ImpostoICMSSN102 : ImpostoICMS { }
    public class ImpostoICMSSN201 : ImpostoICMS { }
    public class ImpostoICMSSN202 : ImpostoICMS { }
    public class ImpostoICMSSN500 : ImpostoICMS { }
    public class ImpostoICMSSN900 : ImpostoICMS { }
    public class ImpostoICMSST : ImpostoICMS { }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ProdutoICMS
    {
        [XmlElement("ICMS00", typeof(ImpostoICMS00))]
        [XmlElement("ICMS02", typeof(ImpostoICMS02))]
        [XmlElement("ICMS10", typeof(ImpostoICMS10))]
        [XmlElement("ICMS15", typeof(ImpostoICMS15))]
        [XmlElement("ICMS20", typeof(ImpostoICMS20))]
        [XmlElement("ICMS30", typeof(ImpostoICMS30))]
        [XmlElement("ICMS40", typeof(ImpostoICMS40))]
        [XmlElement("ICMS51", typeof(ImpostoICMS51))]
        [XmlElement("ICMS53", typeof(ImpostoICMS53))]
        [XmlElement("ICMS60", typeof(ImpostoICMS60))]
        [XmlElement("ICMS61", typeof(ImpostoICMS61))]
        [XmlElement("ICMS70", typeof(ImpostoICMS70))]
        [XmlElement("ICMS90", typeof(ImpostoICMS90))]
        [XmlElement("ICMSPart", typeof(ImpostoICMSPart))]
        [XmlElement("ICMSSN101", typeof(ImpostoICMSSN101))]
        [XmlElement("ICMSSN102", typeof(ImpostoICMSSN102))]
        [XmlElement("ICMSSN201", typeof(ImpostoICMSSN201))]
        [XmlElement("ICMSSN202", typeof(ImpostoICMSSN202))]
        [XmlElement("ICMSSN500", typeof(ImpostoICMSSN500))]
        [XmlElement("ICMSSN900", typeof(ImpostoICMSSN900))]
        [XmlElement("ICMSST", typeof(ImpostoICMSST))]
        public ImpostoICMS ICMS;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ProdutoIPI
    {
        public string clEnq { get; set; }
        public string CNPJProd { get; set; }
        public string cSelo { get; set; }
        public string qSelo { get; set; }
        public string cEnq { get; set; }
        public IPITrib IPITrib { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class IPITrib
    {
        public string CST { get; set; }
        public double? pIPI { get; set; }
        public double? qUnid { get; set; }
        public double? vBC { get; set; }
        public double? vUnid { get; set; }
        public double? vIPI { get; set; }
    }

    /// <summary>
    /// Tributos incidentes nos produtos ou serviços da NF-e
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ProdutoImposto
    {
        public string vTotTrib { get; set; }
        public ProdutoICMS ICMS { get; set; }
        public ProdutoIPI IPI { get; set; }
    }

    /// <summary>
    /// Dados dos detalhes da NF-e
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class Detalhe
    {
        public Produto prod { get; set; }
        public ProdutoImposto imposto { get; set; }
        public string infAdProd { get; set; }

        [XmlAttribute]
        public string nItem { get; set; }
    }

    /// <summary>
    /// Grupo de Formas de Pagamento
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class pag
    {
        /// <summary>
        /// YA01a - Grupo Detalhamento da Forma de Pagamento
        /// VERSÃO 4.00
        /// </summary>
        [XmlElement("detPag")]
        public List<detPag> detPag { get; set; }

        /// <summary>
        /// Valor do troco (vTroco)
        /// VERSÃO 4.00
        /// </summary>
        public decimal? vTroco { get; set; }
    }

    public class detPag
    {
        public IndicadorPagamentoDetalhePagamento? indPag { get; set; }
        /// <summary>
        /// YA02 - Forma de pagamento
        /// </summary>
        public FormaPagamento tPag { get; set; }

        public decimal vPag { get; set; }

        public card card { get; set; }
    }

    public class card
    {
        /// <summary>
        ///     YA04a - Tipo de Integração para pagamento
        /// </summary>
        public TipoIntegracaoPagamento? tpIntegra { get; set; }

        /// <summary>
        ///     YA05 - CNPJ da Credenciadora de cartão de crédito e/ou débito
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        ///     YA06 - Bandeira da operadora de cartão de crédito e/ou débito
        /// </summary>
        public BandeiraCartao? tBand { get; set; }

        /// <summary>
        ///     YA07 - Número de autorização da operação cartão de crédito e/ou débito
        /// </summary>
        public string cAut { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Duplicata
    {
        public string nDup { get; set; }
        public DateTime? dVenc { get; set; }
        public double? vDup { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Fatura
    {
        public string nFat { get; set; }
        public string vOrig { get; set; }
        public string vDesc { get; set; }
        public string vLiq { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Cobranca
    {
        public Fatura fat { get; set; }

        [XmlElement("dup")]
        public List<Duplicata> dup { get; set; }

        public Cobranca()
        {
            dup = new List<Duplicata>();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ObsCont
    {
        public string xTexto { get; set; }

        [XmlAttribute]
        public string xCampo { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ObsFisco
    {
        public string xTexto { get; set; }

        [XmlAttribute]
        public string xCampo { get; set; }
    }

    /// <summary>
    /// Informações adicionais da NF-e
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class InfAdic
    {
        public string infAdFisco { get; set; }
        public string infCpl { get; set; }

        [XmlElement("obsCont")]
        public List<ObsCont> obsCont { get; set; }

        [XmlElement("obsFisco")]
        public List<ObsFisco> obsFisco { get; set; }

        public InfAdic()
        {
            obsCont = new List<ObsCont>();
            obsFisco = new List<ObsFisco>();
        }
    }

    /// <summary>
    /// Grupo de Valores Totais referentes ao ICMS
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ICMSTotal
    {
        /// <summary>
        /// Base de Cálculo do ICMS 
        /// </summary>
        public double vBC { get; set; }

        /// <summary>
        /// Valor Total do ICMS 
        /// </summary>
        public double vICMS { get; set; }

        /// <summary>
        /// Valor total do ICMS Interestadual para a UF de destino
        /// </summary>
        public double? vICMSUFDest { get; set; }

        /// <summary>
        /// Valor total do ICMS Interestadual para a UF do remetente
        /// </summary>
        public double? vICMSUFRemet { get; set; }

        /// <summary>
        /// Valor total do ICMS relativo Fundo de Combate à Pobreza(FCP) da UF de destino
        /// </summary>
        public double? vFCPUFDest { get; set; }

        /// <summary>
        /// Base de Cálculo do ICMS ST 
        /// </summary>
        public double vBCST { get; set; }

        /// <summary>
        /// Valor Total do ICMS ST 
        /// </summary>
        public double vST { get; set; }

        /// <summary>
        /// Valor Total dos produtos e serviços
        /// </summary>
        public double vProd { get; set; }

        /// <summary>
        /// Valor Total do Frete 
        /// </summary>
        public double vFrete { get; set; }

        /// <summary>
        /// Valor Total do Seguro
        /// </summary>
        public double vSeg { get; set; }

        /// <summary>
        /// Valor Total do Desconto
        /// </summary>
        public double vDesc { get; set; }

        /// <summary>
        /// Valor Total do II 
        /// </summary>
        public double vII { get; set; }

        /// <summary>
        /// Valor Total do IPI 
        /// </summary>
        public double vIPI { get; set; }

        /// <summary>
        /// Valor do PIS 
        /// </summary>
        public double vPIS { get; set; }

        /// <summary>
        /// Valor do COFINS 
        /// </summary>
        public double vCOFINS { get; set; }

        /// <summary>
        /// Outras Despesas acessórias 
        /// </summary>
        public double vOutro { get; set; }

        /// <summary>
        /// Valor Total da NF-e 
        /// </summary>
        public double vNF { get; set; }

        /// <summary>
        /// Valor aproximado total de tributos federais, estaduais e municipais
        /// </summary>

        public double? vTotTrib { get; set; }
    }

    /// <summary>
    /// Totais referentes ao ISSQN
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ISSQNTotal
    {
        public double? vServ { get; set; }
        public double? vBC { get; set; }
        public double? vISS { get; set; }
        public double? vPIS { get; set; }
        public double? vCOFINS { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Total
    {
        public ICMSTotal ICMSTot { get; set; }
        public ISSQNTotal ISSQNtot { get; set; }
    }

    /// <summary>
    /// Modalidade do frete
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public enum ModalidadeFrete
    {
        [XmlEnum("0")]
        PorContaRemetente = 0,
        [XmlEnum("1")]
        PorContaDestinatario = 1,
        [XmlEnum("2")]
        PorContaTerceiros = 2,
        [XmlEnum("3")]
        ProprioContaRemetente = 3,
        [XmlEnum("4")]
        ProprioContaDestinatario = 4,
        [XmlEnum("9")]
        SemTransporte = 9,
    }

    /// <summary>
    /// Dados do transportador
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Transportador
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string xNome { get; set; }
        public string IE { get; set; }
        public string xEnder { get; set; }
        public string xMun { get; set; }
        public string UF { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.NFe)]
    public partial class Veiculo
    {
        public string placa { get; set; }
        public string UF { get; set; }
        public string RNTC { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Volume
    {
        public double? qVol { get; set; }
        public string esp { get; set; }
        public string marca { get; set; }
        public string nVol { get; set; }
        public double? pesoL { get; set; }
        public double? pesoB { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Transporte
    {
        public ModalidadeFrete modFrete { get; set; }
        public Transportador transporta { get; set; }

        public string balsa { get; set; }
        public string vagao { get; set; }

        public Veiculo reboque { get; set; }
        public Veiculo veicTransp { get; set; }

        [XmlElement("vol")]
        public List<Volume> vol { get; set; }

        public Transporte()
        {
            vol = new List<Volume>();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class infNFeSupl
    {
        /// <summary>
        /// ZX02 - Texto com o QR-Code impresso no DANFE NFC-e
        /// O atributo qrCode deve ser serializado como CDATA, conforme NT2015.002, V141, regra ZX02-22
        /// </summary>
        [XmlElement("qrCode")]
        public string qrCode { get; set; }

        /// <summary>
        /// ZX03 - Texto com a URL de consulta por chave de acesso a ser imrpessa no DANFE NFC-e
        /// VERSÃO 4.00
        /// </summary>
        [XmlElement("urlChave")]
        public string urlChave { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(typeof(infNFeSupl).Name);

            reader.ReadStartElement("qrCode");
            qrCode = reader.ReadString();
            reader.ReadEndElement();

            if (reader.IsStartElement("urlChave"))
            {
                reader.ReadStartElement("urlChave");
                urlChave = reader.ReadString();
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("qrCode");
            writer.WriteCData(qrCode);
            writer.WriteEndElement();

            if (urlChave == null) return;

            writer.WriteStartElement("urlChave");
            writer.WriteString(urlChave);
            writer.WriteEndElement();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class InfNFe
    {
        public Identificacao ide { get; set; }
        public Emitente emit { get; set; }
        public Destinatario dest { get; set; }

        /// <summary>
        /// Identificação do Local de retirada 
        /// </summary>
        public LocalEntregaRetirada retirada { get; set; }

        /// <summary>
        /// Identificação do Local de entrega 
        /// </summary>
        public LocalEntregaRetirada entrega { get; set; }


        [XmlElement("det")]
        public List<Detalhe> det { get; set; }

        public Total total { get; set; }
        public Transporte transp { get; set; }
        public Cobranca cobr { get; set; }
        public InfAdic infAdic { get; set; }

        /// <summary>
        ///     YA01 - Grupo de Formas de Pagamento
        ///     <para>Ocorrência: 0-100</para>
        /// </summary>
        [XmlElement("pag")]
        public List<pag> pag { get; set; }

        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        /// Informação adicional de compra
        /// </summary>
        public InfCompra compra { get; set; }

        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }

        public InfNFe()
        {
            det = new List<Detalhe>();
        }

        [XmlIgnore]
        [SoapIgnore]
        internal Versao Versao
        {
            get
            {
                return Versao.Parse(versao);
            }
        }
    }

    /// <summary>
    /// Informações de Compras
    /// </summary>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class InfCompra
    {
        /// <summary>
        /// Nota de Empenho
        /// </summary>
        public string xNEmp { get; set; }

        /// <summary>
        /// Pedido
        /// </summary>
        public string xPed { get; set; }

        /// <summary>
        /// Contrato
        /// </summary>
        public string xCont { get; set; }
    }

    /// <summary>
    /// Forma de emissão da NF-e
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public enum FormaEmissao
    {
        [XmlEnum("1")]
        Normal = 1,
        [XmlEnum("2")]
        ContingenciaFS = 2,
        [XmlEnum("3")]
        ContingenciaSCAN = 3,
        [XmlEnum("4")]
        ContingenciaDPEC = 4,
        [XmlEnum("5")]
        ContingenciaFSDA = 5,
        [XmlEnum("6")]
        ContingenciaSVCAN = 6,
        [XmlEnum("7")]
        ContingenciaSVCRS = 7,
        [XmlEnum("9")]
        ContingenciaOffLineNFCe = 9,
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class Identificacao
    {
        public string natOp { get; set; }

        /// <summary>
        /// Código do modelo do Documento Fiscal. 55 = NF-e; 65 = NFC-e.
        /// </summary>
        public int mod { get; set; }

        public short serie { get; set; }
        public int nNF { get; set; }
        public DateTime? dEmi { get; set; }


        #region DataHora Emissão e Saída v2-

        /// <summary>
        /// Data de Saída/Entrada, NFe2
        /// </summary>
        public DateTime? dSaiEnt { get; set; }

        /// <summary>
        /// Hora de Saída/Entrada, NFe2
        /// </summary>
        public string hSaiEnt { get; set; }

        #endregion

        #region DataHora Emissão e Saída v3+

        /// <summary>
        /// Data e Hora de Emissão, NFe v3
        /// </summary>
        public DateTimeOffsetIso8601? dhEmi { get; set; }

        /// <summary>
        /// Data e Hora de Saída/Entrada, NFe v3
        /// </summary>
        public DateTimeOffsetIso8601? dhSaiEnt { get; set; }

        #endregion

        public Tipo tpNF { get; set; }

        /// <summary>
        /// Tipo de Impressao
        /// </summary>
        public int tpImp { get; set; }

        /// <summary>
        /// Forma de emissão da NF-e
        /// </summary>
        public FormaEmissao tpEmis { get; set; }

        public TAmb tpAmb { get; set; }

        /// <summary>
        /// Data e Hora da entrada em contingência
        /// </summary>
        public DateTimeOffsetIso8601? dhCont { get; set; }

        /// <summary>
        /// Justificativa da entrada em contingência 
        /// </summary>
        public string xJust { get; set; }

        /// <summary>
        /// Grupo de informação das NF/NF-e referenciadas
        /// </summary>
        [XmlElementAttribute("NFref")]
        public List<NFReferenciada> NFref { get; set; }

        public Identificacao()
        {
            NFref = new List<NFReferenciada>();
        }
    }

    /// <summary>
    /// Tipo do Documento Fiscal
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public enum Tipo
    {

        [XmlEnum("0")]
        Entrada = 0,
        [XmlEnum("1")]
        Saida = 1,
    }

    /// <summary>
    ///     Meio de pagamento
    ///     <para>01 - Dinheiro</para>
    ///     <para>02 - Cheque</para>
    ///     <para>03 - Cartão de Crédito</para>
    ///     <para>04 - Cartão de Débito</para>
    ///     <para>05 - Crédito Loja</para>
    ///     <para>10 - Vale Alimentação</para>
    ///     <para>11 - Vale Refeição</para>
    ///     <para>12 - Vale Presente</para>
    ///     <para>13 - Vale Combustível</para>
    ///     <para>14 - Duplicata Mercantil (versão 4.00)</para>
    ///     <para>15 - Boleto Bancário (versão 4.00)</para>
    ///     <para>16 - Depósito Bancário (versão 4.00)</para>
    ///     <para>17 - Pagamento Instantâneo (PIX) (versão 4.00)</para>
    ///     <para>18 - Transferência bancária, Carteira Digital (versão 4.00)</para>
    ///     <para>19 - Programa de fidelidade, Cashback, Crédito Virtual (versão 4.00)</para>
    ///     <para>90 - Sem pagamento (versão 4.00)</para>
    ///     <para>99 - Outros</para>
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
        [XmlEnum("16")] fpDepositoBancario = 16,

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

    /// <summary>
    ///     Bandeira da operadora de cartão de crédito e/ou débito
    ///     <para>01 - Visa</para>
    ///     <para>02 - Mastercard</para>
    ///     <para>03 - American Express</para>
    ///     <para>04 - Sorocred</para>
    ///     <para>05 - Diners Club (versão 4.00)</para>
    ///     <para>06 - Elo (versão 4.00)</para>
    ///     <para>07 - Hipercard (versão 4.00)</para>
    ///     <para>08 - Aura (versão 4.00)</para>
    ///     <para>09 - Cabal (versão 4.00)</para>
    ///     <para>10 - Alelo</para>
    ///     <para>11 - Banes Card</para>
    ///     <para>12 - CalCard</para>
    ///     <para>13 - Credz</para>
    ///     <para>14 - Discover</para>
    ///     <para>15 - GoodCard</para>
    ///     <para>16 - GreenCard</para>
    ///     <para>17 - Hiper</para>
    ///     <para>18 - JCB</para>
    ///     <para>19 - Mais</para>
    ///     <para>20 - MaxVan</para>
    ///     <para>21 - Policard</para>
    ///     <para>22 - RedeCompras</para>
    ///     <para>23 - Sodexo</para>
    ///     <para>24 - ValeCard</para>
    ///     <para>25 - Verocheque</para>
    ///     <para>26 - VR</para>
    ///     <para>27 - Ticket</para>
    ///     <para>99 - Outros</para>
    /// </summary>
    public enum BandeiraCartao
    {
        [Description("Visa")][XmlEnum("01")] bcVisa = 01,

        [Description("Mastercard")][XmlEnum("02")] bcMasterCard = 02,

        [Description("American Express")][XmlEnum("03")] bcAmericanExpress = 03,

        [Description("Sorocred")][XmlEnum("04")] bcSorocred = 04,

        [Description("Diners Club")][XmlEnum("05")] bcDinersClub = 05,

        [Description("Elo")][XmlEnum("06")] Elo = 06,

        [Description("Hipercard")][XmlEnum("07")] Hipercard = 07,

        [Description("Aura")][XmlEnum("08")] Aura = 08,

        [Description("Cabal")][XmlEnum("09")] Cabal = 09,

        [Description("Alelo")][XmlEnum("10")] Alelo = 10,

        [Description("BanesCard")][XmlEnum("11")] BanesCard = 11,

        [Description("CalCard")][XmlEnum("12")] CalCard = 12,

        [Description("Credz")][XmlEnum("13")] Credz = 13,

        [Description("Discover")][XmlEnum("14")] Discover = 14,

        [Description("GoodCard")][XmlEnum("15")] GoodCard = 15,

        [Description("GreenCard")][XmlEnum("16")] GreenCard = 16,

        [Description("Hiper")][XmlEnum("17")] Hiper = 17,

        [Description("JCB")][XmlEnum("18")] JCB = 18,

        [Description("Mais")][XmlEnum("19")] Mais = 19,

        [Description("MaxVan")][XmlEnum("20")] MaxVan = 20,

        [Description("Policard")][XmlEnum("21")] Policard = 21,

        [Description("RedeCompras")][XmlEnum("22")] RedeCompras = 22,

        [Description("Sodexo")][XmlEnum("23")] Sodexo = 23,

        [Description("ValeCard")][XmlEnum("24")] ValeCard = 24,

        [Description("Verocheque")][XmlEnum("25")] Verocheque = 25,

        [Description("VR")][XmlEnum("26")] VR = 26,

        [Description("Ticket")][XmlEnum("27")] Ticket = 27,

        [Description("Outros")][XmlEnum("99")] bcOutros = 99,
    }

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

    public enum IndicadorPagamentoDetalhePagamento
    {
        [XmlEnum("0")] ipDetPgVista = 0,
        [XmlEnum("1")] ipDetPgPrazo = 1,
    }
}