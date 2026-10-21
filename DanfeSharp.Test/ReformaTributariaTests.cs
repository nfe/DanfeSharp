using System.IO;
using System.Linq;
using System.Text;
using DanfeSharp.Blocos;
using DanfeSharp.Esquemas.NFe;
using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Leiaute da Reforma Tributária no DANFE da NF-e (NT 2026.010): formatação do CRT (4.2), células do quadro de
    /// produtos (4.3, dois tributos por linha, nada inferido — 4.4), monofasia nos totais (4.1/4.4) e leitura dos
    /// grupos IBSCBS/IS/IBSCBSTot/ISTot do XML.
    /// </summary>
    [TestClass]
    public class ReformaTributariaTests
    {
        [DataTestMethod]
        [DataRow("1", "1 - SIMPLES NACIONAL")]
        [DataRow("2", "2 - SIMPLES NACIONAL - EXC. SUBLIMITE")]
        [DataRow("3", "3 - REGIME NORMAL")]
        [DataRow("4", "4 - SIMPLES NACIONAL - MEI")]
        [DataRow(" 3 ", "3 - REGIME NORMAL")]
        [DataRow(" 5 ", "5")]
        public void FormatarCrt_CodigoComDescricao_OuOValorAparadoQuandoDesconhecido(string crt, string esperado)
        {
            Assert.AreEqual(esperado, Formatador.FormatarCrt(crt));
        }

        [TestMethod]
        public void FormatarCrt_Nulo_DevolveNulo()
        {
            Assert.IsNull(Formatador.FormatarCrt(null));
        }

        [TestMethod]
        public void Linhas_ItemCompleto_ParesNaOrdemDoModeloDaNt()
        {
            var p = new ProdutoViewModel
            {
                Cst = "00", BaseIcms = 1000, AliquotaIcms = 18, ValorIcms = 180, BaseIcmsST = 50,
                BaseIbsCbs = 1000, AliquotaCbs = 0.9, ValorCbs = 9, AliquotaIbsUF = 0.1, ValorIbsUF = 1,
                AliquotaIbsMun = 0, ValorIbsMun = 0, BaseIS = 1000, AliquotaIS = 1, ValorIS = 10,
                BaseIpi = 1000, AliquotaIpi = 5, ValorIpi = 50
            };

            CollectionAssert.AreEqual(
                new[] { "ICMS 1.000,00 / IBS/CBS 1.000,00", "IS 1.000,00 / IPI 1.000,00" },
                LinhasTributosReformaTributaria.Bases(p).ToArray(),
                "o modelo da NT não tem ICMS ST por item");
            CollectionAssert.AreEqual(
                new[] { "ICMS 18,00% / CBS 0,90%", "IBS UF 0,10% / IPI 5,00%", "IBS MUN 0,00% / IS 1,00%" },
                LinhasTributosReformaTributaria.Aliquotas(p).ToArray());
            CollectionAssert.AreEqual(
                new[] { "ICMS 180,00 / CBS 9,00", "IBS UF 1,00 / IPI 50,00", "IBS MUN 0,00 / IS 10,00" },
                LinhasTributosReformaTributaria.Valores(p).ToArray());
        }

        [TestMethod]
        public void Linhas_ItemSemIcms_NaoInventaLinhaDeIcms()
        {
            var p = new ProdutoViewModel { BaseIbsCbs = 100, AliquotaCbs = 0.9, ValorCbs = 0.9, AliquotaIbsUF = 0.1, ValorIbsUF = 0.1 };

            Assert.IsFalse(p.PossuiIcms);
            CollectionAssert.AreEqual(new[] { "IBS/CBS 100,00" }, LinhasTributosReformaTributaria.Bases(p).ToArray());
            CollectionAssert.AreEqual(new[] { "CBS 0,90% / IBS UF 0,10%" }, LinhasTributosReformaTributaria.Aliquotas(p).ToArray());
        }

        [TestMethod]
        public void PossuiIcms_SoComCsosn_EhVerdadeiro()
        {
            Assert.IsTrue(new ProdutoViewModel { Csosn = "102" }.PossuiIcms);
        }

        [TestMethod]
        public void Descricao_SemNcmNemCClassTrib_SoADescricao()
        {
            var p = new ProdutoViewModel { Descricao = "PRODUTO" };

            Assert.AreEqual("PRODUTO", LinhasTributosReformaTributaria.Descricao(p));
        }

        [TestMethod]
        public void Descricao_ComNcmECClassTrib_MarcadoresNaSegundaLinha()
        {
            var p = new ProdutoViewModel { Descricao = "PRODUTO", Ncm = "21069090", ClassificacaoTributaria = "000001" };

            Assert.AreEqual("PRODUTO\r\n[NCM 21069090] [cClassTrib 000001]", LinhasTributosReformaTributaria.Descricao(p));
        }

        [TestMethod]
        public void PossuiMonofasia_GMonoZerado_EhFalso()
        {
            var c = new CalculoImpostoViewModel { ValorIbsMono = 0, ValorCbsMono = 0, ValorIbsMonoReten = 0, ValorCbsMonoReten = 0 };

            Assert.IsFalse(c.PossuiMonofasia);
            Assert.IsFalse(c.PossuiMonofasiaRetencao);
        }

        [TestMethod]
        public void PossuiMonofasia_SoComRetencao_EhVerdadeiroComRetencao()
        {
            var c = new CalculoImpostoViewModel { ValorIbsMono = 0, ValorCbsMono = 0, ValorIbsMonoReten = 4.3, ValorCbsMonoReten = 0 };

            Assert.IsTrue(c.PossuiMonofasia);
            Assert.IsTrue(c.PossuiMonofasiaRetencao);
        }

        [TestMethod]
        public void PossuiMonofasia_SemRetencao_EhVerdadeiroSemRetencao()
        {
            var c = new CalculoImpostoViewModel { ValorIbsMono = 2.1, ValorCbsMono = 0, ValorIbsMonoReten = 0, ValorCbsMonoReten = 0 };

            Assert.IsTrue(c.PossuiMonofasia);
            Assert.IsFalse(c.PossuiMonofasiaRetencao);
        }

        [TestMethod]
        public void Mapear_GRedEmCadaGrupo_UsaAAliquotaEfetivaDoProprioGrupo()
        {
            var imposto = new ProdutoImposto
            {
                IBSCBS = new ProdutoIBSCBS
                {
                    cClassTrib = "200001",
                    gIBSCBS = new GrupoIBSCBS
                    {
                        vBC = 100,
                        gIBSUF = new GrupoIBSUF { pIBSUF = 0.1, gRed = new GrupoReducao { pAliqEfet = 0.04 }, vIBSUF = 0.04 },
                        gIBSMun = new GrupoIBSMun { pIBSMun = 0.05, gRed = new GrupoReducao { pAliqEfet = 0.02 }, vIBSMun = 0.02 },
                        gCBS = new GrupoCBS { pCBS = 0.9, gRed = new GrupoReducao { pAliqEfet = 0.36 }, vCBS = 0.36 }
                    }
                }
            };
            var produto = new ProdutoViewModel();

            ReformaTributariaViewModelMapper.Mapear(produto, imposto);

            Assert.AreEqual(0.04, produto.AliquotaIbsUF);
            Assert.AreEqual(0.02, produto.AliquotaIbsMun);
            Assert.AreEqual(0.36, produto.AliquotaCbs);
            Assert.AreEqual("200001", produto.ClassificacaoTributaria);
        }

        [TestMethod]
        public void Mapear_ItemSoComImpostoSeletivo_TemReformaTributaria()
        {
            var produto = new ProdutoViewModel();

            ReformaTributariaViewModelMapper.Mapear(produto, new ProdutoImposto { IS = new ProdutoIS { vBCIS = 100, pIS = 2, vIS = 2 } });

            Assert.IsTrue(produto.PossuiReformaTributaria);
            Assert.AreEqual(100, produto.BaseIS);
            Assert.AreEqual(2, produto.ValorIS);
        }

        [TestMethod]
        public void CriarDeArquivoXml_IsEIsTot_LidosDoXml()
        {
            var xml = Xml(itemImposto: "<IS><CSTIS>000</CSTIS><cClassTribIS>000001</cClassTribIS><vBCIS>1000.00</vBCIS><pIS>2.0000</pIS><vIS>20.00</vIS></IS>",
                total: "<ISTot><vIS>20.00</vIS></ISTot>");

            var model = DanfeViewModelCreator.CriarDeArquivoXml(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            var item = model.Produtos.Single();
            Assert.AreEqual(1000, item.BaseIS);
            Assert.AreEqual(2, item.AliquotaIS);
            Assert.AreEqual(20, item.ValorIS);
            Assert.AreEqual(20, model.CalculoImposto.ValorIS);
            Assert.IsTrue(item.PossuiReformaTributaria);
            Assert.IsTrue(model.CalculoImposto.PossuiTotaisReformaTributaria);
        }

        [TestMethod]
        public void CriarDeArquivoXml_IpiTributado_BaseLidaDoXml()
        {
            var xml = Xml(itemImposto: "<IPI><cEnq>999</cEnq><IPITrib><CST>50</CST><vBC>1000.00</vBC><pIPI>5.00</pIPI><vIPI>50.00</vIPI></IPITrib></IPI>");

            var item = DanfeViewModelCreator.CriarDeArquivoXml(new MemoryStream(Encoding.UTF8.GetBytes(xml))).Produtos.Single();

            Assert.AreEqual(1000, item.BaseIpi);
            Assert.AreEqual(5, item.AliquotaIpi);
            Assert.AreEqual(50, item.ValorIpi);
        }

        private static string Xml(string itemImposto = "", string total = "") => $@"<?xml version=""1.0"" encoding=""utf-8""?>
<nfeProc versao=""4.00"" xmlns=""http://www.portalfiscal.inf.br/nfe"">
  <NFe>
    <infNFe Id=""NFe35261207952851000109550010000000101000000105"" versao=""4.00"">
      <ide><cUF>35</cUF><cNF>00000010</cNF><natOp>VENDA</natOp><mod>55</mod><serie>1</serie><nNF>10</nNF>
        <dhEmi>2026-12-01T10:00:00-03:00</dhEmi><tpNF>1</tpNF><idDest>1</idDest><cMunFG>3550308</cMunFG>
        <tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>5</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>0</indFinal>
        <indPres>1</indPres><procEmi>0</procEmi><verProc>NFe.io</verProc></ide>
      <emit><CNPJ>07952851000109</CNPJ><xNome>EMITENTE TESTE</xNome>
        <enderEmit><xLgr>RUA TESTE</xLgr><nro>1</nro><xBairro>CENTRO</xBairro><cMun>3550308</cMun><xMun>SAO PAULO</xMun><UF>SP</UF><CEP>01000000</CEP></enderEmit>
        <IE>111111111111</IE><CRT>3</CRT></emit>
      <dest><CNPJ>11222333000181</CNPJ><xNome>DESTINATARIO TESTE</xNome>
        <enderDest><xLgr>RUA DEST</xLgr><nro>2</nro><xBairro>CENTRO</xBairro><cMun>3550308</cMun><xMun>SAO PAULO</xMun><UF>SP</UF><CEP>01000000</CEP></enderDest>
        <indIEDest>9</indIEDest></dest>
      <det nItem=""1"">
        <prod><cProd>001</cProd><cEAN>SEM GTIN</cEAN><xProd>PRODUTO</xProd><NCM>21069090</NCM><CFOP>5102</CFOP>
          <uCom>UN</uCom><qCom>10.0000</qCom><vUnCom>100.00</vUnCom><vProd>1000.00</vProd><cEANTrib>SEM GTIN</cEANTrib>
          <uTrib>UN</uTrib><qTrib>10.0000</qTrib><vUnTrib>100.00</vUnTrib><indTot>1</indTot></prod>
        <imposto><ICMS><ICMS00><orig>0</orig><CST>00</CST><modBC>3</modBC><vBC>1000.00</vBC><pICMS>18.00</pICMS><vICMS>180.00</vICMS></ICMS00></ICMS>{itemImposto}</imposto>
      </det>
      <total>
        <ICMSTot><vBC>1000.00</vBC><vICMS>180.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST>
          <vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><vProd>1000.00</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg>
          <vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS>
          <vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>1000.00</vNF></ICMSTot>{total}
      </total>
      <transp><modFrete>9</modFrete></transp>
      <pag><detPag><tPag>01</tPag><vPag>1000.00</vPag></detPag></pag>
    </infNFe>
  </NFe>
  <protNFe versao=""4.00""><infProt><tpAmb>2</tpAmb><verAplic>SP_NFE</verAplic><chNFe>35261207952851000109550010000000101000000105</chNFe>
    <dhRecbto>2026-12-01T10:00:02-03:00</dhRecbto><nProt>135260000000010</nProt><cStat>100</cStat><xMotivo>Autorizado o uso da NF-e</xMotivo></infProt></protNFe>
</nfeProc>";
    }
}
