using System;
using System.Linq;
using System.Text;
using DanfeNet.Esquemas;
using DanfeNet.Models;

namespace DanfeNet.Mappers;

internal static class DanfeMapper
{
    public static Danfe CreateNFe(ProcNFe procNfe)
    {
        var model = new Danfe();
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
            model.LocalRetiradaInfo = CreateLocalRetiradaEntrega(infNfe.retirada);
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
            ProdutoInfo produtoInfo = new ProdutoInfo();
            produtoInfo.Codigo = det.prod.cProd;
            produtoInfo.Descricao = det.prod.xProd;
            produtoInfo.Ncm = det.prod.NCM;
            produtoInfo.Cfop = det.prod.CFOP;
            produtoInfo.Unidade = det.prod.uCom;
            produtoInfo.Quantidade = det.prod.qCom;
            produtoInfo.ValorUnitario = det.prod.vUnCom;
            produtoInfo.ValorTotal = det.prod.vProd;
            produtoInfo.InformacoesAdicionais = det?.infAdProd?.Replace("\\n", "\n");

            var imposto = det.imposto;

            if (imposto != null)
            {
                if (imposto.ICMS != null)
                {
                    var icms = imposto.ICMS.ICMS;

                    if (icms != null)
                    {
                        produtoInfo.ValorIcms = icms.vICMS;
                        produtoInfo.BaseIcms = icms.vBC;
                        produtoInfo.AliquotaIcms = icms.pICMS;
                        produtoInfo.OCst = icms.orig + icms.CST + icms.CSOSN;
                        produtoInfo.BaseIcmsST = icms.vBCST;
                    }
                }

                if (imposto.IPI != null)
                {
                    var ipi = imposto.IPI.IPITrib;

                    if (ipi != null)
                    {
                        produtoInfo.ValorIpi = ipi.vIPI;
                        produtoInfo.AliquotaIpi = ipi.pIPI;
                    }
                }
            }

            model.Produtos.Add(produtoInfo);
        }

        if (infNfe.cobr != null)
        {
            foreach (var item in infNfe.cobr.dup)
            {
                DuplicataInfo duplicataInfo = new DuplicataInfo();
                duplicataInfo.Numero = item.nDup;
                duplicataInfo.Valor = item.vDup;
                duplicataInfo.Vecimento = item.dVenc;

                model.Duplicatas.Add(duplicataInfo);
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
    
    private static EmpresaInfo CreateEmpresaFrom(Empresa empresa)
    {
        EmpresaInfo model = new EmpresaInfo();

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
    
    public static LocalEntregaRetiradaInfo CreateLocalRetiradaEntrega(Esquemas.LocalEntregaRetirada local)
    {
        var m = new LocalEntregaRetiradaInfo()
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

        var sb = new StringBuilder();
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
    
}