using System;
using System.Linq;
using DanfeNet.Esquemas;
using DanfeNet.Models;

namespace DanfeNet.Mappers;

public static class DanfeNfcMapper
{
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
            var produto = new ProdutoInfo();
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
}