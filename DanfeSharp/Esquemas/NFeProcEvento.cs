﻿using DanfeSharp.Atributos;
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

        #region Constructors

        //public NFeProcEvento()
        //{
        //    Evento = new NFeEvento();
        //    RetEvento = new NFeRetEvento();
        //}

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

        //[DFeIgnore] public bool Processado => RetEvento.InfEvento.CStat.IsIn(Constantes.EventoProcessado);

        #endregion

        #region Methods

        //public void Gravar(NFeConfig configuracoes)
        //{
        //    if (!configuracoes.Arquivos.Salvar) return;

        //    var nomeArquivo = $"{RetEvento.InfEvento.Chave}_{RetEvento.InfEvento.TpEvento.GetValueOrDefault().GetDFeValue()}_{RetEvento.InfEvento.NSeqEvento:00}-procEventoNFe.xml";
        //    var caminho = configuracoes.Arquivos.ObterCaminhoAutorizado(RetEvento.InfEvento.DhRegEvento.DateTime);
        //    Save(Path.Combine(caminho, nomeArquivo));

        //    #region Backup

        //    if (configuracoes.Arquivos.DiretorioAutorizadasBackup.IsNotNullOrEmpty())
        //    {
        //        var caminhoBackup = configuracoes.Arquivos.ObterCaminhoAutorizado(RetEvento.InfEvento.DhRegEvento.DateTime, configuracoes.Arquivos.DiretorioAutorizadasBackup);
        //        Save(Path.Combine(caminhoBackup, nomeArquivo));
        //    }

        //    #endregion
        //}

        #endregion
    }

    [DFeRoot("retEvento", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public sealed class NFeRetEvento :/* DFeSignDocument<NFeRetEvento>, */INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        //public NFeRetEvento()
        //{
        //    InfEvento = new NFeRetInfEvento();
        //    //Signature = new DFeSignature();
        //}

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
        [DFeElement(TipoCampo.Str, "xMotivo", Id = "HR17", Min = 1, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string xMotivo { get; set; }

        /// <summary>
        ///     HR18 - Chave de Acesso da NF-e vinculada ao evento.
        /// </summary>
        [DFeElement(TipoCampo.StrNumber, "chNFe", Id = "HR18", Min = 44, Max = 44, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string chave { get; set; }

        /// <summary>
        ///     HR19 - Código do Tipo do Evento.
        /// </summary>
        [DFeElement(TipoCampo.Enum, "tpEvento", Id = "HR19", Min = 6, Max = 6, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public NFeTipoEvento? tpEvento { get; set; }

        /// <summary>
        ///     HR20 - Descrição do Evento – “Cancelamento homologado”
        /// </summary>
        [DFeElement(TipoCampo.Str, "xEvento", Id = "HR20", Min = 5, Max = 60, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string xEvento { get; set; }

        /// <summary>
        ///     HR21 - Sequencial do evento para o mesmo tipo de evento.
        /// </summary>
        [DFeElement(TipoCampo.Int, "nSeqEvento", Id = "HR21", Min = 1, Max = 2, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public int? nSeqEvento { get; set; }

        /// <summary>
        ///     R22 - (EPEC) Idem a mensagem de entrada.
        /// </summary>
        [DFeElement(TipoCampo.Str, "cOrgaoAutor", Id = "R22", Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string COrgaoAutor { get; set; }

        /// <summary>
        ///     HR22 - CNPJ do destinatário da NFe
        /// </summary>
        [DFeElement(TipoCampo.StrNumberFill, "CNPJDest", Id = "HR22", Min = 14, Max = 14, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string CNPJDest { get; set; }

        /// <summary>
        ///     HR23 - CPF do destinatário da NFe
        /// </summary>
        [DFeElement(TipoCampo.StrNumberFill, "CPFDest", Id = "HR23", Min = 11, Max = 11, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string CPFDest { get; set; }

        /// <summary>
        ///     HR24 - e-mail do destinatário informado na NF-e.
        /// </summary>
        [DFeElement(TipoCampo.Str, "emailDest", Id = "HR24", Min = 1, Max = 60, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string emailDest { get; set; }

        /// <summary>
        ///     HR25 - Data e hora de registro do evento. Se o evento for rejeitado informar a data e hora de recebimento do
        ///     evento.
        /// </summary>
        [DFeElement(TipoCampo.DatHorTz, "dhRegEvento", Id = "HR25", Min = 19, Max = 19, Ocorrencia = Ocorrencia.Obrigatoria)]
        public DateTimeOffset dhRegEvento { get; set; }

        /// <summary>
        ///     HR26 - Número do Protocolo da NF-e
        /// </summary>
        [DFeElement(TipoCampo.StrNumber, "nProt", Id = "HR26", Min = 15, Max = 15, Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string nProt { get; set; }

        /// <summary>
        ///     R25 - (EPEC) Relação de Chaves de Acesso de EPEC pendentes de conciliação, existentes no AN.
        /// </summary>
        [DFeElement(TipoCampo.Str, "chNFePend", Id = "R25", Ocorrencia = Ocorrencia.NaoObrigatoria)]
        public string chNFePend { get; set; }

        #endregion

        #region Methods

        //private bool ShouldSerializeTpEvento()
        //{
        //    return TpEvento.HasValue;
        //}

        //private bool ShouldSerializeNSeqEvento()
        //{
        //    return NSeqEvento.HasValue;
        //}

        //private bool ShouldSerializeCOrgaoAutor()
        //{
        //    return COrgaoAutor.IsNotNullOrEmpty();
        //}

        //private bool ShouldSerializeCNPJDest()
        //{
        //    return CNPJDest.IsNotNullOrEmpty();
        //}

        //private bool ShouldSerializeCPFDest()
        //{
        //    return CPFDest.IsNotNullOrEmpty();
        //}

        #endregion
    }

    //[DFeSignInfoElement("infEvento")]
    [DFeRoot("evento", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    [Serializable]
    [XmlType("evento", AnonymousType = true, Namespace = Namespaces.NFe)]
    public sealed class NFeEvento :/* DFeSignDocument<NFeEvento>,*/ INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        //public NFeEvento()
        //{
        //    InfEvento = new NFeInfEvento();
        //    //Signature = new DFeSignature();
        //}

        #endregion

        #region Properties

        /// <summary>
        ///     HP05 - Versão do leiaute do evento
        /// </summary>
        [DFeAttribute(TipoCampo.Enum, "versao", Id = "HP05", Min = 1, Max = 4, Ocorrencia = Ocorrencia.Obrigatoria)]
        public NFeVersao versao { get; set; }

        /// <summary>
        ///     HP06 - Grupo de informações do registro do Evento
        /// </summary>
        [DFeElement("infEvento", Id = "HP06", Ocorrencia = Ocorrencia.Obrigatoria)]
        public NFeInfEvento infEvento { get; set; }

        #endregion

        #region Method

        //public void Assinar(X509Certificate2 certificado, SaveOptions saveOptions)
        //{
        //    Guard.Against<ArgumentNullException>(certificado == null, "Certificado não pode ser nulo.");

        //    if (InfEvento.Id.IsNullOrEmpty() || InfEvento.Id.Length < 54)
        //    {
        //        var idEvento = $"ID{InfEvento.TpEvento.GetDFeValue()}{InfEvento.Chave}{InfEvento.NSeqEvento:D2}";
        //        InfEvento.Id = idEvento;
        //    }

        //    AssinarDocumento(certificado, saveOptions, false, SignDigest.SHA1);
        //}

        //public string ObterNomeXml()
        //{
        //    return $"{InfEvento.Chave}-eve-{InfEvento.TpEvento.GetDescription().ToLower()}.xml";
        //}

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

        #region Constructors

        //public NFeInfEvento()
        //{
        //    DetEvento = new NFeDetEvento();
        //}

        #endregion

        #region Properties

        /// <summary>
        ///     HP07 - Grupo de informações do registro do Evento
        /// </summary>
        [DFeAttribute(TipoCampo.Str, "Id", Id = "HP07", Min = 54, Max = 54, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string id { get; set; }

        /// <summary>
        ///     HP08 - Código do órgão de recepção do Evento.
        /// </summary>
        [DFeElement(TipoCampo.Enum, "cOrgao", Id = "HP08", Min = 2, Max = 2, Ocorrencia = Ocorrencia.Obrigatoria)]
        public CodigoUF cOrgao { get; set; }

        /// <summary>
        ///     HP09 - Identificação do Ambiente: 1=Produção /2=Homologação
        /// </summary>
        [DFeElement(TipoCampo.Enum, "tpAmb", Id = "HP09", Min = 1, Max = 1, Ocorrencia = Ocorrencia.Obrigatoria)]
        public TAmb tpAmb { get; set; }

        /// <summary>
        ///     HP10 - CNPJ do autor do Evento
        /// </summary>
        [DFeElement(TipoCampo.StrNumberFill, "CNPJ", Id = "HP10", Min = 14, Max = 14, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string CNPJ { get; set; }

        /// <summary>
        ///     HP11 - CPF do autor do Evento
        /// </summary>
        [DFeElement(TipoCampo.StrNumberFill, "CPF", Id = "HP11", Min = 11, Max = 11, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string CPF { get; set; }

        /// <summary>
        ///     HP12 - Chave de Acesso da NF-e vinculada ao Evento
        /// </summary>
        [DFeElement(TipoCampo.StrNumber, "chNFe", Id = "HP12", Min = 44, Max = 44, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string chNFe { get; set; }

        /// <summary>
        ///     HP13 - Data e hora do evento
        /// </summary>
        [DFeElement(TipoCampo.DatHorTz, "dhEvento", Id = "HP13", Min = 19, Max = 19, Ocorrencia = Ocorrencia.Obrigatoria)]
        public DateTime dhEvento { get; set; }

        /// <summary>
        ///     HP14 - Código do evento
        /// </summary>
        [DFeElement(TipoCampo.Enum, "tpEvento", Id = "HP14", Min = 6, Max = 6, Ocorrencia = Ocorrencia.Obrigatoria)]
        public NFeTipoEvento tpEvento { get; set; }

        /// <summary>
        ///     HP15 - Sequencial do evento para o mesmo tipo de evento.
        /// </summary>
        [DFeElement(TipoCampo.Int, "nSeqEvento", Id = "HP15", Min = 1, Max = 2, Ocorrencia = Ocorrencia.Obrigatoria)]
        public int nSeqEvento { get; set; }

        /// <summary>
        ///     HP16 - Versão do detalhe do evento
        /// </summary>
        [DFeElement(TipoCampo.Enum, "verEvento", Id = "HP16", Min = 3, Max = 5, Ocorrencia = Ocorrencia.Obrigatoria)]
        public NFeVersao verEvento { get; set; }

        /// <summary>
        ///     HP17 - Informações do Evento (Cancelamento, Carta de Correcao, EPEC, Manifestação)
        /// </summary>
        [DFeElement("detEvento", Id = "HP17", Ocorrencia = Ocorrencia.Obrigatoria)]
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


        //private bool ShouldSerializeCNPJ()
        //{
        //    return CNPJ.IsNotNullOrEmpty();
        //}

        //private bool ShouldSerializeCPF()
        //{
        //    return CPF.IsNotNullOrEmpty();
        //}

        //#endregion
    }

    public enum NFeTipoEvento
    {
        /// <summary>
        ///     110110 - Carta de Correção
        /// </summary>
        [DFeEnum("110110")]
        [Description("CCe")]
        [XmlEnum("110110")]
        CartaCorrecao = 110110,

        /// <summary>
        ///     110111 - Cancelamento
        /// </summary>
        [DFeEnum("110111")]
        [Description("Cancelamento")]
        [XmlEnum("110111")]
        Cancelamento = 110111,

        /// <summary>
        ///     110112 - Cancelamento Substituição
        /// </summary>
        [DFeEnum("110112")]
        [Description("Cancelamento ST")]
        [XmlEnum("110112")]
        CancelamentoST = 110112,

        /// <summary>
        ///     110113 - EPEC
        /// </summary>
        [DFeEnum("110113")]
        [Description("EPEC")]
        [XmlEnum("110113")]
        EPEC = 110113,
    }

    public enum NFeVersao
    {
        [DFeEnum("1.00")]
        [Description("1.00")]
        [XmlEnum("1.00")]
        v100,
        [DFeEnum("3.10")]
        [Description("3.10")]
        [XmlEnum("3.10")]
        v310,
        [DFeEnum("4.00")]
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
        [DFeEnum("12")] [Description("AC")] AC = 12,

        /// <summary>
        ///     27 - Alagoas
        /// </summary>
        [XmlEnum("27")]
        [DFeEnum("27")] [Description("AL")] AL = 27,

        /// <summary>
        ///     25 - Paraíba
        /// </summary>
        [XmlEnum("25")]
        [DFeEnum("25")] [Description("PB")] PB = 25,

        /// <summary>
        ///     26 - Pernambuco
        /// </summary>
        [XmlEnum("26")]
        [DFeEnum("26")] [Description("PE")] PE = 26,

        /// <summary>
        ///     22 - Piauí
        /// </summary>
        [XmlEnum("22")]
        [DFeEnum("22")] [Description("PI")] PI = 22,

        /// <summary>
        ///     41 - Paraná
        /// </summary>
        [XmlEnum("41")]
        [DFeEnum("41")] [Description("PR")] PR = 41,

        /// <summary>
        ///     33 - Rio de Janeiro
        /// </summary>
        [XmlEnum("33")]
        [DFeEnum("33")] [Description("RJ")] RJ = 33,

        /// <summary>
        ///     24 - Rio Grande do Norte
        /// </summary>
        [XmlEnum("24")]
        [DFeEnum("24")] [Description("RN")] RN = 24,

        /// <summary>
        ///     11 - Rondônia
        /// </summary>
        [XmlEnum("11")]
        [DFeEnum("11")] [Description("RO")] RO = 11,

        /// <summary>
        ///     14 - Roraima
        /// </summary>
        [XmlEnum("14")]
        [DFeEnum("14")] [Description("RR")] RR = 14,

        /// <summary>
        ///     43 - Rio Grande do Sul
        /// </summary>
        [XmlEnum("43")]
        [DFeEnum("43")] [Description("RS")] RS = 43,

        /// <summary>
        ///     42 - Santa Catarina
        /// </summary>
        [XmlEnum("42")]
        [DFeEnum("42")] [Description("SC")] SC = 42,

        /// <summary>
        ///     28 - Sergipe
        /// </summary>
        [XmlEnum("28")]
        [DFeEnum("28")] [Description("SE")] SE = 28,

        /// <summary>
        ///     35 - São Paulo
        /// </summary>
        [XmlEnum("35")]
        [DFeEnum("35")] [Description("SP")] SP = 35,

        /// <summary>
        ///     17 - Tocantins
        /// </summary>
        [XmlEnum("17")]
        [DFeEnum("17")] [Description("TO")] TO = 17,

        /// <summary>
        ///     91 - Ambiente Nacional
        /// </summary>
        [XmlEnum("91")]
        [DFeEnum("91")] [Description("AN")] AN = 91,

        /// <summary>
        ///     00 - Exterior
        /// </summary>
        [XmlEnum("00")]
        [DFeEnum("00")] [Description("EX")] EX = 0,

        /// <summary>
        ///     99 - Suframa
        /// </summary>
        [XmlEnum("99")]
        [DFeEnum("99")] [Description("SU")] SU = 99
    }


    [Serializable]
    [XmlType("detEvento", AnonymousType = true, Namespace = Namespaces.NFe)]
    public sealed class NFeDetEvento : GenericClone<NFeDetEvento>, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     HP18 - Versão do Pedido de Cancelamento, da carta de correção ou EPEC, deve ser informado com a mesma informação da
        ///     tag verEvento (HP16)
        /// </summary>
        [DFeAttribute(TipoCampo.Enum, "versao", Id = "HP18", Min = 4, Max = 4, Ocorrencia = Ocorrencia.Obrigatoria)]
        public NFeVersao versao { get; set; }

        /// <summary>
        ///     HP19 - "Cancelamento", "Carta de Correção", "Carta de Correcao" ou "EPEC"
        /// </summary>
        [DFeElement(TipoCampo.Str, "descEvento", Id = "HP19", Min = 5, Max = 60, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string descEvento { get; set; }

        #endregion

        #region Carta de Correção

        /// <summary>
        ///     HP20 - Correção a ser considerada, texto livre. A correção mais recente substitui as anteriores.
        /// </summary>
        [DFeElement(TipoCampo.Str, "xCorrecao", Id = "HP20", Min = 15, Max = 1000, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string xCorrecao { get; set; }

        /// <summary>
        ///     HP20a - Condições de uso da Carta de Correção
        /// </summary>

        [DFeElement(TipoCampo.Custom, "xCondUso", Id = "HP20a", Ocorrencia = Ocorrencia.Obrigatoria)]
        public string xCondUso { get; set; }

        #endregion

        #region Cancelamento

        /// <summary>
        ///     HP20 - Informar o número do Protocolo de Autorização da NF-e a ser Cancelada.
        /// </summary>
        [DFeElement(TipoCampo.StrNumber, "nProt", Id = "HP20", Min = 15, Max = 15, Ocorrencia = Ocorrencia.Obrigatoria)]
        public string nProt { get; set; }

        /// <summary>
        ///     HP21 - Informar a justificativa do cancelamento
        /// </summary>
        [DFeElement(TipoCampo.Str, "xJust", Id = "HP21", Min = 15, Max = 255, Ocorrencia = Ocorrencia.Obrigatoria)]
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