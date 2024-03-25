using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DanfeNet.Esquemas;
using DanfeNet.Mappers;
using DanfeNet.Models;

namespace DanfeNet;

public static class DanfeNfcFactory
{

    /// <summary>
    /// Cria o modelo a partir de um arquivo xml.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Danfe FromXmlFilePath(string path)
    {
        using (var sr = new StreamReader(path, true))
        {
            return FromStream(sr);
        }
    }
   
    private static Danfe FromStream(TextReader reader)
    {
        ProcNFe nfe;
        try
        {
            var procNFeSerializer = new XmlSerializer(typeof(ProcNFe));
            nfe = (ProcNFe)procNFeSerializer.Deserialize(reader);
            return FromNFCe(nfe);
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


    // Manual de Especificacoes Tecnicas do DANFE NFCeQRCode_Versao3.4_26_10_2015
    public static Danfe FromNFCe(ProcNFe procNfe)
    {
        return DanfeNfcMapper.CreateFromProcNFCe(procNfe);
    }



    
}