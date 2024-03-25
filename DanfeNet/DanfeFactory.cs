using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DanfeNet.Esquemas;
using DanfeNet.Models;
using Duplicata = DanfeNet.Models.Duplicata;
using Empresa = DanfeNet.Models.Empresa;
using LocalEntregaRetirada = DanfeNet.Models.LocalEntregaRetirada;
using Produto = DanfeNet.Models.Produto;

namespace DanfeNet;

public static class DanfeFactory
{

    private static Empresa CreateEmpresaFrom(Esquemas.Empresa empresa)
    {
        Empresa model = new Empresa();

        model.RazaoSocial = empresa.xNome;
        model.CnpjCpf = !string.IsNullOrWhiteSpace(empresa.CNPJ) ? empresa.CNPJ : empresa.CPF;
        model.Ie = empresa.IE;
        model.IeSt = empresa.IEST;
        model.Email = empresa.email;

        if (string.IsNullOrWhiteSpace(empresa.IE) && string.IsNullOrWhiteSpace(empresa.IEST))
            model.Ie = "ISENTO";

        var end = empresa.Endereco;

        if (end != null)
        {
            model.EnderecoLogadrouro = end.xLgr;
            model.EnderecoNumero = end.nro;
            model.EnderecoComplemento = end.xCpl;
            model.EnderecoBairro = end.xBairro;
            model.Municipio = end.xMun;
            model.EnderecoUf = end.UF;
            model.EnderecoCep = end.CEP;
            model.Telefone = end.fone;
            model.Email = empresa.email;
        }

        if (empresa is Emitente)
        {
            var emit = empresa as Emitente;
            model.IM = emit.IM;
            model.CRT = emit.CRT;
            model.NomeFantasia = emit.xFant;
        }

        return model;
    }

    internal static Danfe CreateFromXmlString(string xml)
    {
        ProcNFe nfe;

        try
        {
            var procNFeSerializer = new XmlSerializer(typeof(ProcNFe));
            using (var reader = new StringReader(xml))
            {
                nfe = (ProcNFe)procNFeSerializer.Deserialize(reader);
            }

            return CreateFromProcNFe(nfe);
        }
        catch (InvalidOperationException e)
        {
            throw new Exception("Não foi possível interpretar o texto Xml.", e);
        }
    }

      

    /// <summary>
    /// Cria o modelo a partir de um arquivo xml.
    /// </summary>
    /// <param name="caminho"></param>
    /// <returns></returns>
    public static Danfe CriarModeloNFCeDeArquivoXml(string caminho)
    {
        using (var sr = new StreamReader(caminho, true))
        {
            return CriarModeloNFCeDeArquivoXmlInternal(sr);
        }
    }

    /// <summary>
    /// Cria o modelo a partir de um arquivo xml contido num stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>Modelo</returns>
    public static Danfe CriarModeloNFCeDeArquivoXml(Stream stream)
    {
        if (stream == null) 
            throw new ArgumentNullException(nameof(stream));

        using (var sr = new StreamReader(stream, true))
        {
            return CriarModeloNFCeDeArquivoXmlInternal(sr);
        }
    }

    /// <summary>
    /// Cria o modelo a partir de um arquivo xml.
    /// </summary>
    /// <param name="caminho"></param>
    /// <returns></returns>
    public static Danfe CriarDeArquivoXml(string caminho)
    {
        using (var sr = new StreamReader(caminho, true))
        {
            return CriarDeArquivoXmlInternal(sr);
        }
    }

    /// <summary>
    /// Cria o modelo a partir de um arquivo xml contido num stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>Modelo</returns>
    public static Danfe CriarDeArquivoXml(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        using (var sr = new StreamReader(stream, true))
        {
            return CriarDeArquivoXmlInternal(sr);
        }
    }

    /// <summary>
    /// Cria o modelo a partir de uma string xml.
    /// </summary>
    public static Danfe CriarDeStringXml(string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        using (StringReader sr = new StringReader(str))
        {
            return CriarDeArquivoXmlInternal(sr);
        }
    }

    private static Danfe CriarModeloNFCeDeArquivoXmlInternal(TextReader reader)
    {
        ProcNFe nfe = null;

        try
        {
            var procNFeSerializer = new XmlSerializer(typeof(ProcNFe));
            nfe = (ProcNFe)procNFeSerializer.Deserialize(reader);
            return CreateFromProcNFCe(nfe);
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

    private static Danfe CriarDeArquivoXmlInternal(TextReader reader)
    {
        ProcNFe nfe = null;

        try
        {
            var procNFeSerializer = new XmlSerializer(typeof(ProcNFe));
            nfe = (ProcNFe)procNFeSerializer.Deserialize(reader);
            return CreateFromProcNFe(nfe);
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

    internal static void ExtrairDatas(Danfe model, InfNFe infNfe)
    {
        var ide = infNfe.ide;

        if (infNfe.Versao.Maior >= 3)
        {
            if(ide.dhEmi.HasValue) model.DataHoraEmissao = ide.dhEmi?.DateTimeOffsetValue.DateTime;
            if(ide.dhSaiEnt.HasValue) model.DataSaidaEntrada = ide.dhSaiEnt?.DateTimeOffsetValue.DateTime;

            if (model.DataSaidaEntrada.HasValue)
            {
                model.HoraSaidaEntrada = model.DataSaidaEntrada?.TimeOfDay;
            }
        }
        else
        {
            model.DataHoraEmissao = ide.dEmi;
            model.DataSaidaEntrada = ide.dSaiEnt;

            if (!string.IsNullOrWhiteSpace(ide.hSaiEnt))
            {
                model.HoraSaidaEntrada = TimeSpan.Parse(ide.hSaiEnt);
            }
        }
    }

    internal static CalculoImposto CriarCalculoImpostoViewModel(ICMSTotal i)
    {
        return new CalculoImposto()
        {
            ValorAproximadoTributos = i.vTotTrib,//i.vICMS + i.vST + i.vII + i.vIPI + i.vPIS + i.vCOFINS,
            BaseCalculoIcms = i.vBC,
            ValorIcms = i.vICMS,
            BaseCalculoIcmsSt = i.vBCST,
            ValorIcmsSt = i.vST,
            ValorTotalProdutos = i.vProd,
            ValorFrete = i.vFrete,
            ValorSeguro = i.vSeg,
            Desconto = i.vDesc,
            ValorII = i.vII,
            ValorIpi = i.vIPI,
            ValorPis = i.vPIS,
            ValorCofins = i.vCOFINS,
            OutrasDespesas = i.vOutro,
            ValorTotalNota = i.vNF,
            vFCPUFDest = i.vFCPUFDest,
            vICMSUFDest = i.vICMSUFDest,
            vICMSUFRemet = i.vICMSUFRemet
        };
    }

    // Manual de Especificacoes Tecnicas do DANFE NFCeQRCode_Versao3.4_26_10_2015
    public static Danfe CreateFromProcNFCe(ProcNFe procNfe)
    {
        Danfe model = new Danfe();

        var nfe = procNfe.NFe;
        var infNfe = nfe.infNFe;
        var ide = infNfe.ide;

        if (ide.mod != 65)
        {
            throw new Exception("Modelo da nota difere de 65");
        }

        if (ide.tpEmis != FormaEmissao.Normal && ide.tpEmis != FormaEmissao.ContingenciaDPEC && ide.tpEmis != FormaEmissao.ContingenciaFSDA && ide.tpEmis != FormaEmissao.ContingenciaSVCAN && ide.tpEmis != FormaEmissao.ContingenciaSVCRS)
        {
            throw new Exception("Somente o tpEmis==1 está implementado.");
        }

        // Divisão 1 - Informações do Cabeçalho
        model.Emitente.RazaoSocial = !string.IsNullOrWhiteSpace(infNfe.emit.xNome) ? infNfe.emit.xNome : null;
        model.Emitente.CnpjCpf = !string.IsNullOrWhiteSpace(infNfe.emit.CNPJ) ? infNfe.emit.CNPJ : infNfe.emit.CPF;

        if (infNfe.emit.Endereco != null)
        {
            model.Emitente.EnderecoLogadrouro = infNfe?.emit?.Endereco?.xLgr;
            model.Emitente.EnderecoNumero = infNfe?.emit?.Endereco?.nro;
            model.Emitente.EnderecoBairro = infNfe?.emit?.Endereco?.xBairro;
            model.Emitente.Municipio = infNfe?.emit?.Endereco?.xMun;
            model.Emitente.EnderecoUf = infNfe?.emit?.Endereco?.UF;
        }

        // Divisão 2 - Identificação do DANFE NFCe
        // Está no bloco TabelaProdutosServicosNFC

        // Divisão 3 - Informações de detalhes de produtos/serviços
        foreach (var det in infNfe.det)
        {
            var produto = new Produto();
            produto.Codigo = det.prod.cProd;
            produto.Descricao = det.prod.xProd;
            produto.Unidade = det.prod.uCom;
            produto.Quantidade = det.prod.qCom;
            produto.ValorUnitario = det.prod.vUnCom;
            produto.ValorTotal = det.prod.vProd;

            model.Produtos.Add(produto);
        }

        // Divisão 4 - Informações de Totais do DANFE NFCe
        model.CalculoImposto = new CalculoImposto()
        {
            QuantidadeTotal = model.Produtos.Count(),
            ValorTotalProdutos = infNfe.total.ICMSTot.vProd,
            ValorFrete = infNfe.total.ICMSTot.vFrete,
            ValorSeguro = infNfe.total.ICMSTot.vSeg,
            OutrasDespesas = infNfe.total.ICMSTot.vOutro,
            Desconto = infNfe.total.ICMSTot.vDesc,
            ValorTotalNota = infNfe.total.ICMSTot.vNF,
        };

        foreach (var pag in infNfe.pag)
        {
            var pagamento = new Pagamento();
            pagamento.DetalhePagamento = new System.Collections.Generic.List<DetalheViewModel>();
            pagamento.Troco = pag.vTroco;

            foreach (var detPag in pag.detPag)
            {
                var detalhe = new DetalheViewModel();

                detalhe.FormaPagamento = (FormaPagamento)detPag.tPag;
                detalhe.Valor = detPag.vPag;

                pagamento.DetalhePagamento.Add(detalhe);
            }

            model.Pagamento.Add(pagamento);
        }

        // Divisão 5 - Área de Mensagem Fiscal (BlocoInformacaoFiscal)
        model.TipoAmbiente = (int)ide.tpAmb;

        // Divisão 6 - Informações de Identificação da NFC-e e do Protocolo de Autorização
        model.NfNumero = ide.nNF;
        model.NfSerie = ide.serie;
        model.DataHoraEmissao = ide.dhEmi.GetValueOrDefault().DateTimeOffsetValue.DateTime;

        // consultar os sites para consultar nfce --> http://nfce.encat.org/consulte-sua-nota-qr-code-versao-2-0/ 
        if (ide.tpAmb == TAmb.Producao)
            model.EndConsulta = infNfe?.emit?.Endereco?.UF.UrlNFCeProduction();
        else if (ide.tpAmb == TAmb.Homologacao)
        {
            model.EndConsulta = infNfe?.emit?.Endereco?.UF.UrlNFCeTest();
        }

        // dividir a chave de acesso em 11 blocos com espaço em cada bloco 999 999 999 999 999 999 999 999 999 999 999
        model.ChaveAcesso = procNfe.NFe.infNFe.Id.Substring(3).SpaceOnAccessKey();

        model.ProtocoloAutorizacao = string.Format(Formatador.Cultura, "{0}  {1}", procNfe.protNFe.infProt.nProt, procNfe.protNFe.infProt.dhRecbto.DateTimeOffsetValue.DateTime);

        // Divisão 7 - Informações sobre o Consumidor
        model.Destinatario.RazaoSocial = !string.IsNullOrWhiteSpace(infNfe?.dest?.xNome) ? infNfe.dest.xNome : null;

        if (infNfe.dest != null)
        {
            if (!string.IsNullOrWhiteSpace(infNfe?.dest?.CPF))
                model.Destinatario.CnpjCpf = infNfe.dest.CPF;

            if (!string.IsNullOrWhiteSpace(infNfe?.dest?.CNPJ))
                model.Destinatario.CnpjCpf = infNfe.dest.CNPJ;

            if (infNfe.dest != null && infNfe.dest.Endereco != null)
            {
                model.Destinatario.EnderecoLogadrouro = infNfe.dest.Endereco.xLgr;
                model.Destinatario.EnderecoNumero = infNfe.dest.Endereco.nro;
                model.Destinatario.EnderecoBairro = infNfe.dest.Endereco.xBairro;
                model.Destinatario.Municipio = infNfe.dest.Endereco.xMun;
                model.Destinatario.EnderecoUf = infNfe.dest.Endereco.UF;
            }
        }
        // Divisão 8 - Informações da Consulta via QR CODE
        model.QrCode = nfe.infNFeSupl.qrCode;

        // Divisão 9 - Mensagem de Interesse do Contribuinte
        model.CalculoImposto.ValorAproximadoTributos = infNfe.total.ICMSTot.vICMS + infNfe.total.ICMSTot.vST + infNfe.total.ICMSTot.vII + infNfe.total.ICMSTot.vIPI + infNfe.total.ICMSTot.vPIS + infNfe.total.ICMSTot.vCOFINS;

        model.CalculoImposto.ValorAproximadoTributosEstaduais = infNfe.total.ICMSTot.vICMS + infNfe.total.ICMSTot.vST;
        model.CalculoImposto.ValorAproximadoTributosFederais = infNfe.total.ICMSTot.vII + infNfe.total.ICMSTot.vIPI + infNfe.total.ICMSTot.vPIS + infNfe.total.ICMSTot.vCOFINS;

        return model;
    }

    public static Danfe CreateFromProcNFe(ProcNFe procNfe)
    {
        Danfe model = new Danfe();

        var nfe = procNfe.NFe;
        var infNfe = nfe.infNFe;
        var ide = infNfe.ide;
        model.TipoEmissao = ide.tpEmis;

        if (ide.mod != 55)
        {
            throw new NotSupportedException("Somente o mod==55 está implementado.");
        }


        model.Orientacao = Orientacao.Retrato;

        var infProt = procNfe.protNFe.infProt;
        model.CodigoStatusReposta = infProt.cStat;
        model.DescricaoStatusReposta = infProt.xMotivo;
        model.TipoAmbiente = (int)ide.tpAmb;
        model.NfNumero = ide.nNF;
        model.NfSerie = ide.serie;
        model.NaturezaOperacao = ide.natOp;
        model.ChaveAcesso = procNfe.NFe.infNFe.Id.Substring(3);
        model.TipoNF = (int)ide.tpNF;

        model.TipoEmissao = ide.tpEmis;
        model.ContingenciaDataHora = ide.dhCont;
        model.ContingenciaJustificativa = ide.xJust;

        model.Emitente = CreateEmpresaFrom(infNfe.emit);
        model.Destinatario = CreateEmpresaFrom(infNfe.dest);

        // Local retirada e entrega 
        if (infNfe.retirada != null)
        {
            model.LocalRetirada = CreateLocalRetiradaEntrega(infNfe.retirada);
        }

        if (infNfe.entrega != null)
        {
            model.LocalEntrega = CreateLocalRetiradaEntrega(infNfe.entrega);
        }

        model.NotasFiscaisReferenciadas = ide.NFref.Select(x => x.ToString()).ToList();

        // Informações adicionais de compra
        if (infNfe.compra != null)
        {
            model.Contrato = infNfe.compra.xCont;
            model.NotaEmpenho = infNfe.compra.xNEmp;
            model.Pedido = infNfe.compra.xPed;
        }

        foreach (var det in infNfe.det)
        {
            Produto produto = new Produto();
            produto.Codigo = det.prod.cProd;
            produto.Descricao = det.prod.xProd;
            produto.Ncm = det.prod.NCM;
            produto.Cfop = det.prod.CFOP;
            produto.Unidade = det.prod.uCom;
            produto.Quantidade = det.prod.qCom;
            produto.ValorUnitario = det.prod.vUnCom;
            produto.ValorTotal = det.prod.vProd;
            produto.InformacoesAdicionais = det?.infAdProd?.Replace("\\n", "\n");

            var imposto = det.imposto;

            if (imposto != null)
            {
                if (imposto.ICMS != null)
                {
                    var icms = imposto.ICMS.ICMS;

                    if (icms != null)
                    {
                        produto.ValorIcms = icms.vICMS;
                        produto.BaseIcms = icms.vBC;
                        produto.AliquotaIcms = icms.pICMS;
                        produto.OCst = icms.orig + icms.CST + icms.CSOSN;
                        produto.BaseIcmsST = icms.vBCST;
                    }
                }

                if (imposto.IPI != null)
                {
                    var ipi = imposto.IPI.IPITrib;

                    if (ipi != null)
                    {
                        produto.ValorIpi = ipi.vIPI;
                        produto.AliquotaIpi = ipi.pIPI;
                    }
                }
            }

            model.Produtos.Add(produto);
        }

        if (infNfe.cobr != null)
        {
            foreach (var item in infNfe.cobr.dup)
            {
                Duplicata duplicata = new Duplicata();
                duplicata.Numero = item.nDup;
                duplicata.Valor = item.vDup;
                duplicata.Vecimento = item.dVenc;

                model.Duplicatas.Add(duplicata);
            }
        }

        model.CalculoImposto = CriarCalculoImpostoViewModel(infNfe.total.ICMSTot);

        var issqnTotal = infNfe.total.ISSQNtot;

        if (issqnTotal != null)
        {
            var c = model.CalculoIssqn;
            c.InscricaoMunicipal = infNfe.emit.IM;
            c.BaseIssqn = issqnTotal.vBC;
            c.ValorTotalServicos = issqnTotal.vServ;
            c.ValorIssqn = issqnTotal.vISS;
            c.Mostrar = true;
        }

        var transp = infNfe.transp;
        var transportadora = transp.transporta;
        var transportadoraModel = model.Transportadora;

        transportadoraModel.ModalidadeFrete = (int)transp.modFrete;

        if (transp.veicTransp != null)
        {
            transportadoraModel.VeiculoUf = transp.veicTransp.UF;
            transportadoraModel.CodigoAntt = transp.veicTransp.RNTC;
            transportadoraModel.Placa = transp.veicTransp.placa;
        }

        if (transportadora != null)
        {
            transportadoraModel.RazaoSocial = transportadora.xNome;
            transportadoraModel.EnderecoUf = transportadora.UF;
            transportadoraModel.CnpjCpf = !string.IsNullOrWhiteSpace(transportadora.CNPJ) ? transportadora.CNPJ : transportadora.CPF;
            transportadoraModel.EnderecoLogadrouro = transportadora.xEnder;
            transportadoraModel.Municipio = transportadora.xMun;
            transportadoraModel.Ie = transportadora.IE;
        }


        var vol = transp.vol.FirstOrDefault();

        if (vol != null)
        {
            transportadoraModel.QuantidadeVolumes = vol.qVol;
            transportadoraModel.Especie = vol.esp;
            transportadoraModel.Marca = vol.marca;
            transportadoraModel.Numeracao = vol.nVol;
            transportadoraModel.PesoBruto = vol.pesoB;
            transportadoraModel.PesoLiquido = vol.pesoL;
        }

        var infAdic = infNfe.infAdic;
        if (infAdic != null)
        {
            model.InformacoesComplementares = procNfe.NFe.infNFe.infAdic.infCpl;
            model.InformacoesAdicionaisFisco = procNfe.NFe.infNFe.infAdic.infAdFisco;
        }

        var infoProto = procNfe.protNFe.infProt;

        model.ProtocoloAutorizacao = string.Format(Formatador.Cultura, "{0} - {1}", infoProto.nProt, infoProto.dhRecbto.DateTimeOffsetValue.DateTime);

        ExtrairDatas(model, infNfe);

        model.ContingenciaDataHora = ide.dhCont;
        model.ContingenciaJustificativa = ide.xJust;

        return model;
    }

    private static LocalEntregaRetirada CreateLocalRetiradaEntrega(Esquemas.LocalEntregaRetirada local)
    {
        var m = new LocalEntregaRetirada()
        {
            NomeRazaoSocial = local.xNome,
            CnpjCpf = !string.IsNullOrWhiteSpace(local.CNPJ) ? local.CNPJ : local.CPF,
            InscricaoEstadual = local.IE,
            Bairro = local.xBairro,
            Municipio = local.xMun,
            Uf = local.UF,
            Cep = local.CEP,
            Telefone = local.fone
        };

        StringBuilder sb = new StringBuilder();
        sb.Append(local.xLgr);

        if (!string.IsNullOrWhiteSpace(local.nro))
        {
            sb.Append(", ").Append(local.nro);
        }

        if (!string.IsNullOrWhiteSpace(local.xCpl))
        {
            sb.Append(" - ").Append(local.xCpl);
        }

        m.Endereco = sb.ToString();

        return m;
    }
}