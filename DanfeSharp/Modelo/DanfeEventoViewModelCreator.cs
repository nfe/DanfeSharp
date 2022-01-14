using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using DanfeSharp.Esquemas;
using System.Xml.Serialization;

namespace DanfeSharp.Modelo
{
    public static class DanfeEventoViewModelCreator
    {
        #region Methods

        //internal static DanfeEventoViewModel CreateFromXmlStream(Stream stream)
        //{
        //    try
        //    {
        //        var evento = NFeProcEvento.Load(stream);
        //        return CreateFromXml(evento);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (ex.InnerException is XmlException e) throw new System.Exception($"Não foi possível interpretar o Xml. Linha {e.LineNumber} Posição {e.LinePosition}.");
        //        throw new XmlException("O Xml não parece ser um Evento processado.", ex);
        //    }
        //}

        private static XmlSerializer ProcNFeSerializer = new XmlSerializer(typeof(NFeProcEvento));

        internal static DanfeEventoViewModel CreateFromXmlString(string xml)
        {
            NFeProcEvento nfe = null;
            try
            {
                //var evento = NFeProcEvento.Load(xml);

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
        public static DanfeEventoViewModel CriarDeStringXml(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(nameof(str));
            return CreateFromXmlString(str);
        }

        /// <summary>
        ///     Cria o modelo a partir de um arquivo xml.
        /// </summary>
        /// <param name="caminho"></param>
        /// <returns></returns>
        //public static DanfeEventoViewModel CriarDeArquivoXml(string caminho)
        //{
        //    using var sr = new StreamReader(caminho, true);
        //    return CreateFromXmlStream(sr.BaseStream);

        public static DanfeEventoViewModel CriarDeArquivoXml(String caminho)
        {
            using (var sr = new StreamReader(caminho, true))
            {
                return CriarDeArquivoXmlInternal(sr);
            }
        }

        public static DanfeEventoViewModel CriarDeArquivoXml(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var sr = new StreamReader(stream, true))
            {
                return CriarDeArquivoXmlInternal(sr);
            }
        }

        private static DanfeEventoViewModel CriarDeArquivoXmlInternal(TextReader reader)
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
                    throw new XmlException(String.Format("Não foi possível interpretar o Xml. Linha {0} Posição {1}.", ex.LineNumber, ex.LinePosition), e.InnerException, ex.LineNumber, ex.LinePosition);
                }

                throw new XmlException("O Xml não parece ser uma NF-e processada.", e);
            }
        }

        //    try
        //    {
        //        using (var reader = new StringReader(xml))
        //        {
        //            nfe = (ProcNFe)ProcNFeSerializer.Deserialize(reader);
        //        }

        //        return CreateFromProcNFe(nfe);
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        throw new Exception("Não foi possível interpretar o texto Xml.", e);
        //    }
        //}

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

        public static DanfeEventoViewModel CreateFromXml(NFeProcEvento procEvento)
        {
            var infEvento = procEvento.evento.infEvento;
            var retInfEvento = procEvento.retEvento.infEvento;
            var model = new DanfeEventoViewModel
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
}
