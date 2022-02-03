using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using DanfeSharp.Esquemas.NFe;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace DanfeSharp.Esquemas
{
    [XmlType(Namespace = Namespaces.NFe)]
    [XmlRoot("procEventoNFe", Namespace = Namespaces.NFe, IsNullable = false)]
    public sealed class NFeProcEvento :/* DFeDocument<NFeProcEvento>, */INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     ZR02 - Versão do documento
        /// </summary>
        public NFeVersao versao { get; set; }

        /// <summary>
        ///     ZR03 - Evento
        /// </summary>

        public NFeEvento evento { get; set; }

        /// <summary>
        ///     ZR05 - Evento
        /// </summary>
        public NFeRetEvento retEvento { get; set; }


        #endregion
    }

    public sealed class NFeRetEvento :/* DFeSignDocument<NFeRetEvento>, */INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public NFeVersao Versao { get; set; }

        public NFeRetInfEvento infEvento { get; set; }

        #endregion
    }
    [Serializable]
    [XmlType("infEvento", AnonymousType = true, Namespace = Namespaces.NFe)]
    public sealed class NFeRetInfEvento : GenericClone<NFeRetInfEvento>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     HR12 - Identificador da TAG a ser assinada
        /// </summary>
        public string id { get; set; }

        /// <summary>
        ///     HR13 - Identificação do Ambiente: 1=Produção /2=Homologação
        /// </summary>
        public TAmb tpAmb { get; set; }

        /// <summary>
        ///     HR14 - Versão da aplicação que registrou o Evento
        /// </summary>
        public string verAplic { get; set; }

        /// <summary>
        ///     HR15 - Código da UF que registrou o Evento. Utilizar 91 para o Ambiente Nacional.
        /// </summary>
        public CodigoUF cOrgao { get; set; }

        /// <summary>
        ///     HR16 - Código do status da resposta.
        /// </summary>
        public int cStat { get; set; }

        /// <summary>
        ///     HR17 - Descrição do status da resposta.
        /// </summary>
        public string xMotivo { get; set; }

        /// <summary>
        ///     HR18 - Chave de Acesso da NF-e vinculada ao evento.
        /// </summary>
        public string chave { get; set; }

        /// <summary>
        ///     HR19 - Código do Tipo do Evento.
        /// </summary>
        public NFeTipoEvento? tpEvento { get; set; }

        /// <summary>
        ///     HR20 - Descrição do Evento – “Cancelamento homologado”
        /// </summary>
        public string xEvento { get; set; }

        /// <summary>
        ///     HR21 - Sequencial do evento para o mesmo tipo de evento.
        /// </summary>
        public int? nSeqEvento { get; set; }

        /// <summary>
        ///     R22 - (EPEC) Idem a mensagem de entrada.
        /// </summary>
        public string COrgaoAutor { get; set; }

        /// <summary>
        ///     HR22 - CNPJ do destinatário da NFe
        /// </summary>
        public string CNPJDest { get; set; }

        /// <summary>
        ///     HR23 - CPF do destinatário da NFe
        /// </summary>
        public string CPFDest { get; set; }

        /// <summary>
        ///     HR24 - e-mail do destinatário informado na NF-e.
        /// </summary>
        public string emailDest { get; set; }

        /// <summary>
        ///     HR25 - Data e hora de registro do evento. Se o evento for rejeitado informar a data e hora de recebimento do
        ///     evento.
        /// </summary>
        public DateTimeOffset dhRegEvento { get; set; }

        /// <summary>
        ///     HR26 - Número do Protocolo da NF-e
        /// </summary>
        public string nProt { get; set; }

        /// <summary>
        ///     R25 - (EPEC) Relação de Chaves de Acesso de EPEC pendentes de conciliação, existentes no AN.
        /// </summary>
        public string chNFePend { get; set; }

        #endregion

        #region Methods

        private bool ShouldSerializeTpEvento()
        {
            return tpEvento.HasValue;
        }

        private bool ShouldSerializeNSeqEvento()
        {
            return nSeqEvento.HasValue;
        }

        private bool ShouldSerializeCOrgaoAutor()
        {
            return string.IsNullOrWhiteSpace(COrgaoAutor);
        }

        private bool ShouldSerializeCNPJDest()
        {
            return string.IsNullOrWhiteSpace(CNPJDest);
        }

        private bool ShouldSerializeCPFDest()
        {
            return string.IsNullOrWhiteSpace(CPFDest);
        }

        #endregion
    }

    [Serializable]
    [XmlType("evento", AnonymousType = true, Namespace = Namespaces.NFe)]
    public sealed class NFeEvento : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     HP05 - Versão do leiaute do evento
        /// </summary>
        public NFeVersao versao { get; set; }

        /// <summary>
        ///     HP06 - Grupo de informações do registro do Evento
        /// </summary>
        public NFeInfEvento infEvento { get; set; }

        #endregion
    }

    public class GenericClone<T> : ICloneable where T : class
    {
        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>T.</returns>
        public T Clone()
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var ret = Activator.CreateInstance(typeof(T)) as T;

            foreach (var prop in props.Where(prop => null != prop.GetSetMethod()))
            {
                var value = prop.GetValue(this, null);
                prop.SetValue(ret, value is ICloneable cloneable ? cloneable.Clone() : value, null);
            }

            return ret;
        }

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    /// <summary>
    ///     HP06 - Grupo de informações do registro do Evento
    /// </summary>
    [Serializable]
    [XmlType("infEvento", Namespace = Namespaces.NFe)]
    public sealed class NFeInfEvento : GenericClone<NFeInfEvento>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     HP07 - Grupo de informações do registro do Evento
        /// </summary>
        public string id { get; set; }

        /// <summary>
        ///     HP08 - Código do órgão de recepção do Evento.
        /// </summary>
        public CodigoUF cOrgao { get; set; }

        /// <summary>
        ///     HP09 - Identificação do Ambiente: 1=Produção /2=Homologação
        /// </summary>
        public TAmb tpAmb { get; set; }

        /// <summary>
        ///     HP10 - CNPJ do autor do Evento
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        ///     HP11 - CPF do autor do Evento
        /// </summary>
        public string CPF { get; set; }

        /// <summary>
        ///     HP12 - Chave de Acesso da NF-e vinculada ao Evento
        /// </summary>
        public string chNFe { get; set; }

        /// <summary>
        ///     HP13 - Data e hora do evento
        /// </summary>
        public DateTime dhEvento { get; set; }

        /// <summary>
        ///     HP14 - Código do evento
        /// </summary>
        public NFeTipoEvento tpEvento { get; set; }

        /// <summary>
        ///     HP15 - Sequencial do evento para o mesmo tipo de evento.
        /// </summary>
        public int nSeqEvento { get; set; }

        /// <summary>
        ///     HP16 - Versão do detalhe do evento
        /// </summary>
        public NFeVersao verEvento { get; set; }

        /// <summary>
        ///     HP17 - Informações do Evento (Cancelamento, Carta de Correcao, EPEC, Manifestação)
        /// </summary>
        public NFeDetEvento detEvento { get; set; }

        //[DFeIgnore]
        //public string Documento
        //{
        //    get => CNPJ.IsNotNullOrEmpty() ? CNPJ : CPF;
        //    set
        //    {
        //        if (value.Length == 11)
        //            CPF = value;
        //        else
        //            CNPJ = value;
        //    }
        //}

        //#endregion


        private bool ShouldSerializeCNPJ()
        {
            return string.IsNullOrWhiteSpace(CNPJ);
        }

        private bool ShouldSerializeCPF()
        {
            return string.IsNullOrWhiteSpace(CPF);
        }

        //#endregion
    }

    public enum NFeTipoEvento
    {
        /// <summary>
        ///     110110 - Carta de Correção
        /// </summary>
        [Description("CCe")]
        [XmlEnum("110110")]
        CartaCorrecao = 110110,

        /// <summary>
        ///     110111 - Cancelamento
        /// </summary>
        [Description("Cancelamento")]
        [XmlEnum("110111")]
        Cancelamento = 110111,

        /// <summary>
        ///     110112 - Cancelamento Substituição
        /// </summary>
        [Description("Cancelamento ST")]
        [XmlEnum("110112")]
        CancelamentoST = 110112,

        /// <summary>
        ///     110113 - EPEC
        /// </summary>
        [Description("EPEC")]
        [XmlEnum("110113")]
        EPEC = 110113,
    }

    public enum NFeVersao
    {
        [Description("1.00")]
        [XmlEnum("1.00")]
        v100,
        [Description("3.10")]
        [XmlEnum("3.10")]
        v310,
        [Description("4.00")]
        [XmlEnum("4.00")]
        v400
    }

    public enum CodigoUF
    {
        /// <summary>
        ///     12 - Acre
        /// </summary>
        [XmlEnum("12")]
        [Description("AC")] AC = 12,

        /// <summary>
        ///     27 - Alagoas
        /// </summary>
        [XmlEnum("27")]
        [Description("AL")] AL = 27,

        /// <summary>
        ///     25 - Paraíba
        /// </summary>
        [XmlEnum("25")]
        [Description("PB")] PB = 25,

        /// <summary>
        ///     26 - Pernambuco
        /// </summary>
        [XmlEnum("26")]
        [Description("PE")] PE = 26,

        /// <summary>
        ///     22 - Piauí
        /// </summary>
        [XmlEnum("22")]
        [Description("PI")] PI = 22,

        /// <summary>
        ///     41 - Paraná
        /// </summary>
        [XmlEnum("41")]
        [Description("PR")] PR = 41,

        /// <summary>
        ///     33 - Rio de Janeiro
        /// </summary>
        [XmlEnum("33")]
        [Description("RJ")] RJ = 33,

        /// <summary>
        ///     24 - Rio Grande do Norte
        /// </summary>
        [XmlEnum("24")]
        [Description("RN")] RN = 24,

        /// <summary>
        ///     11 - Rondônia
        /// </summary>
        [XmlEnum("11")]
        [Description("RO")] RO = 11,

        /// <summary>
        ///     14 - Roraima
        /// </summary>
        [XmlEnum("14")]
        [Description("RR")] RR = 14,

        /// <summary>
        ///     43 - Rio Grande do Sul
        /// </summary>
        [XmlEnum("43")]
        [Description("RS")] RS = 43,

        /// <summary>
        ///     42 - Santa Catarina
        /// </summary>
        [XmlEnum("42")]
        [Description("SC")] SC = 42,

        /// <summary>
        ///     28 - Sergipe
        /// </summary>
        [XmlEnum("28")]
        [Description("SE")] SE = 28,

        /// <summary>
        ///     35 - São Paulo
        /// </summary>
        [XmlEnum("35")]
        [Description("SP")] SP = 35,

        /// <summary>
        ///     17 - Tocantins
        /// </summary>
        [XmlEnum("17")]
        [Description("TO")] TO = 17,

        /// <summary>
        ///     91 - Ambiente Nacional
        /// </summary>
        [XmlEnum("91")]
        [Description("AN")] AN = 91,

        /// <summary>
        ///     00 - Exterior
        /// </summary>
        [XmlEnum("00")]
        [Description("EX")] EX = 0,

        /// <summary>
        ///     99 - Suframa
        /// </summary>
        [XmlEnum("99")]
        [Description("SU")] SU = 99
    }


    [Serializable]
    [XmlType("detEvento", AnonymousType = true, Namespace = Namespaces.NFe)]
    public sealed class NFeDetEvento : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     HP18 - Versão do Pedido de Cancelamento, da carta de correção ou EPEC, deve ser informado com a mesma informação da
        ///     tag verEvento (HP16)
        /// </summary>
        public NFeVersao versao { get; set; }

        /// <summary>
        ///     HP19 - "Cancelamento", "Carta de Correção", "Carta de Correcao" ou "EPEC"
        /// </summary>
        public string descEvento { get; set; }

        #endregion

        #region Carta de Correção

        /// <summary>
        ///     HP20 - Correção a ser considerada, texto livre. A correção mais recente substitui as anteriores.
        /// </summary>
        public string xCorrecao { get; set; }

        /// <summary>
        ///     HP20a - Condições de uso da Carta de Correção
        /// </summary>

        public string xCondUso { get; set; }

        #endregion

        #region Cancelamento

        /// <summary>
        ///     HP20 - Informar o número do Protocolo de Autorização da NF-e a ser Cancelada.
        /// </summary>
        public string nProt { get; set; }

        /// <summary>
        ///     HP21 - Informar a justificativa do cancelamento
        /// </summary>
        public string xJust { get; set; }

        #endregion

        #region EPEC

        //private Estado? _cOrgaoAutor;

        ///// <summary>
        /////     P20 - Código do Órgão do Autor do Evento.
        /////     Nota: Informar o código da UF do Emitente para este evento.
        ///// </summary>
        //public Estado? cOrgaoAutor
        //{
        //    get => _cOrgaoAutor;
        //    set
        //    {
        //        if (value == null) return;
        //        descEvento = "EPEC";
        //        LimpaDadosCancelamento();
        //        LimpaDadosCartaCorrecao();
        //        _cOrgaoAutor = value;
        //    }
        //}

        ///// <summary>
        /////     P21 - Informar "1=Empresa Emitente" para este evento.
        /////     Nota: 1=Empresa Emitente; 2=Empresa Destinatária;
        /////     3=Empresa; 5=Fisco; 6=RFB; 9=Outros Órgãos.
        ///// </summary>
        //public TipoAutor? tpAutor { get; set; }

        ///// <summary>
        /////     P22 - Versão do aplicativo do Autor do Evento.
        ///// </summary>
        //public string verAplic { get; set; }

        ///// <summary>
        /////     P23 - Data e hora
        ///// </summary>
        //[XmlIgnore]
        //public DateTimeOffset? dhEmi { get; set; }

        ///// <summary>
        /////     Proxy para dhEmi no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        ///// </summary>
        //[XmlElement(ElementName = "dhEmi")]
        //public string ProxyDhEmi
        //{
        //    get => dhEmi.ParaDataHoraStringUtc();
        //    set => dhEmi = DateTimeOffset.Parse(value);
        //}

        ///// <summary>
        /////     P24 - 0=Entrada; 1=Saída;
        ///// </summary>
        //public TipoNFe? tpNF { get; set; }

        ///// <summary>
        /////     P25 - IE do Emitente
        ///// </summary>
        //public string IE { get; set; }

        ///// <summary>
        /////     P26
        ///// </summary>
        //public dest dest { get; set; }

        #endregion

        #region Methods

        //private bool ShouldSerializeXCorrecao()
        //{
        //    return XCorrecao.IsNotNullOrEmpty();
        //}

        //private bool ShouldSerializeXCondUso()
        //{
        //    return XCondUso.IsNotNullOrEmpty();
        //}

        //private string SerializeXCondUso()
        //{
        //    return DFeConstantes.CondicaoUso;
        //}

        //private object DeserializeXCondUso(string value)
        //{
        //    return value;
        //}

        //private bool ShouldSerializeNProt()
        //{
        //    return NProt.IsNotNullOrEmpty();
        //}

        //private bool ShouldSerializeXJust()
        //{
        //    return XJust.IsNotNullOrEmpty();
        //}

        #endregion
        #endregion
    }
}
