using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DanfeNet.Esquemas;

namespace DanfeNet.Models;

public static class DanfeEventoViewModelCreator
{
    #region Methods

    private static XmlSerializer ProcNFeSerializer = new XmlSerializer(typeof(NFeProcEvento));

    internal static DanfeEvento CreateFromXmlString(string xml)
    {
        NFeProcEvento nfe = null;
        try
        {
            using (var reader = new StringReader(xml))
            {
                nfe = (NFeProcEvento)ProcNFeSerializer.Deserialize(reader);
            }
            return CreateFromXml(nfe);
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("Não foi possível interpretar o texto Xml.", ex);
        }
    }

    /// <summary>
    ///     Cria o modelo a partir de uma string xml.
    /// </summary>
    public static DanfeEvento CriarDeStringXml(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(nameof(str));
        return CreateFromXmlString(str);
    }

    /// <summary>
    ///     Cria o modelo a partir de um arquivo xml.
    /// </summary>
    /// <param name="caminho"></param>
    /// <returns></returns>
    public static DanfeEvento CriarDeArquivoXml(string caminho)
    {
        using (var sr = new StreamReader(caminho, true))
        {
            return CriarDeArquivoXmlInternal(sr);
        }
    }

    public static DanfeEvento CriarDeArquivoXml(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        using (var sr = new StreamReader(stream, true))
        {
            return CriarDeArquivoXmlInternal(sr);
        }
    }

    private static DanfeEvento CriarDeArquivoXmlInternal(TextReader reader)
    {
        NFeProcEvento nfe = null;

        try
        {
            nfe = (NFeProcEvento)ProcNFeSerializer.Deserialize(reader);
            return CreateFromXml(nfe);
        }
        catch (InvalidOperationException e)
        {
            if (e.InnerException is XmlException ex)
            {
                throw new XmlException(string.Format("Não foi possível interpretar o Xml. Linha {0} Posição {1}.", ex.LineNumber, ex.LinePosition), e.InnerException, ex.LineNumber, ex.LinePosition);
            }

            throw new XmlException("O Xml não parece ser uma NF-e processada.", e);
        }
    }

    /// <summary>
    ///     Cria o modelo a partir de um arquivo xml contido num stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>Modelo</returns>
    //public static DanfeEventoViewModel CriarDeArquivoXml(Stream stream)
    //{
    //    if (stream == null) throw new ArgumentNullException(nameof(stream));
    //    return CreateFromXmlStream(stream);
    //}

    public static DanfeEvento CreateFromXml(NFeProcEvento procEvento)
    {
        var infEvento = procEvento.evento.infEvento;
        var retInfEvento = procEvento.retEvento.infEvento;
        var model = new DanfeEvento
        {
            ChaveAcesso = infEvento.chNFe,
            Orgao = (int)infEvento.cOrgao,
            TipoAmbiente = (int)infEvento.tpAmb,
            DataHoraEvento = infEvento.dhEvento,
            TipoEvento = infEvento.tpEvento,
            DescricaoEvento = infEvento.detEvento.descEvento,
            SequenciaEvento = infEvento.nSeqEvento,
            CodigoStatus = retInfEvento.cStat.ToString(),
            Motivo = retInfEvento.xMotivo,
            Protocolo = retInfEvento.nProt,
            Justificativa = infEvento.detEvento.xJust,
            Correcao = infEvento.detEvento.xCorrecao,
            CondicaoUso = infEvento.detEvento.xCondUso
        };

        return model;
    }

    #endregion
}