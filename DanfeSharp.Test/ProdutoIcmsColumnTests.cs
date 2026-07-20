using System.Collections.Generic;
using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Tests for the ICMS column (origem + CST/CSOSN) rendering in the
    /// DANFE product table. Covers the fix for nfe/DanfeSharp#38.
    /// See openspec/changes/fix-danfe-csosn-rendering/specs/danfe-icms-column/spec.md
    /// </summary>
    [TestClass]
    public class ProdutoIcmsColumnTests
    {
        // === ProdutoViewModel.OCst — cell composition rules ===

        [TestMethod]
        public void OCst_RegimeNormal_OrigemComCst_RetornaOrigemBarraCst()
        {
            var produto = new ProdutoViewModel { Origem = "1", Cst = "20" };
            Assert.AreEqual("1/20", produto.OCst);
        }

        [TestMethod]
        public void OCst_SimplesNacional_OrigemComCsosn_RetornaOrigemBarraCsosn()
        {
            var produto = new ProdutoViewModel { Origem = "0", Csosn = "102" };
            Assert.AreEqual("0/102", produto.OCst);
        }

        [TestMethod]
        public void OCst_SemOrigem_ApenasCst_RetornaCodigoSemBarra()
        {
            var produto = new ProdutoViewModel { Cst = "40" };
            Assert.AreEqual("40", produto.OCst);
        }

        [TestMethod]
        public void OCst_SemOrigem_ApenasCsosn_RetornaCodigoSemBarra()
        {
            var produto = new ProdutoViewModel { Csosn = "500" };
            Assert.AreEqual("500", produto.OCst);
        }

        [TestMethod]
        public void OCst_ApenasOrigem_SemCstNemCsosn_RetornaOrigemSemBarra()
        {
            var produto = new ProdutoViewModel { Origem = "2" };
            Assert.AreEqual("2", produto.OCst);
        }

        [TestMethod]
        public void OCst_TodosNull_RetornaStringVazia()
        {
            var produto = new ProdutoViewModel();
            Assert.AreEqual(string.Empty, produto.OCst);
        }

        [TestMethod]
        public void OCst_OrigemEmString_TrataComoAusente()
        {
            // O ProcNFe deserializer pode entregar "" para tags ausentes;
            // o producer normaliza para null, mas o OCst computado deve
            // tratar "" como ausente também, para defesa em profundidade.
            var produto = new ProdutoViewModel { Origem = "", Cst = "20" };
            Assert.AreEqual("20", produto.OCst);
        }

        [TestMethod]
        public void OCst_CstPreferidoSobreCsosn_QuandoAmbosPresentes()
        {
            // Caso teoricamente impossível (CST e CSOSN mutuamente exclusivos
            // por item), mas se aparecer prevalece CST por convenção.
            var produto = new ProdutoViewModel { Origem = "1", Cst = "20", Csosn = "102" };
            Assert.AreEqual("1/20", produto.OCst);
        }

        // === ProdutoViewModel.CalcularCabecalhoColunaIcms — dynamic header ===

        [TestMethod]
        public void Cabecalho_TodosItensComCst_RetornaOCST()
        {
            var produtos = new List<ProdutoViewModel>
            {
                new ProdutoViewModel { Origem = "1", Cst = "20" },
                new ProdutoViewModel { Origem = "0", Cst = "00" },
                new ProdutoViewModel { Origem = "1", Cst = "40" }
            };
            Assert.AreEqual("O/CST", ProdutoViewModel.CalcularCabecalhoColunaIcms(produtos));
        }

        [TestMethod]
        public void Cabecalho_TodosItensComCsosn_RetornaOCSOSN()
        {
            var produtos = new List<ProdutoViewModel>
            {
                new ProdutoViewModel { Origem = "0", Csosn = "102" },
                new ProdutoViewModel { Origem = "0", Csosn = "500" }
            };
            Assert.AreEqual("O/CSOSN", ProdutoViewModel.CalcularCabecalhoColunaIcms(produtos));
        }

        [TestMethod]
        public void Cabecalho_NenhumItemComCodigo_RetornaOCSTFallback()
        {
            var produtos = new List<ProdutoViewModel>
            {
                new ProdutoViewModel { Origem = "1" },
                new ProdutoViewModel()
            };
            Assert.AreEqual("O/CST", ProdutoViewModel.CalcularCabecalhoColunaIcms(produtos));
        }

        [TestMethod]
        public void Cabecalho_ProdutosNull_RetornaOCSTFallback()
        {
            Assert.AreEqual("O/CST", ProdutoViewModel.CalcularCabecalhoColunaIcms(null));
        }

        [TestMethod]
        public void Cabecalho_ListaVazia_RetornaOCSTFallback()
        {
            Assert.AreEqual("O/CST", ProdutoViewModel.CalcularCabecalhoColunaIcms(new List<ProdutoViewModel>()));
        }

        [TestMethod]
        public void Cabecalho_MistoCstECsosn_PrevalenceCst()
        {
            // Cenário teoricamente impossível pelo MOC (regime é por emitente,
            // não por item), mas se aparecer no XML, "O/CST" prevalece.
            var produtos = new List<ProdutoViewModel>
            {
                new ProdutoViewModel { Cst = "20" },
                new ProdutoViewModel { Csosn = "102" }
            };
            Assert.AreEqual("O/CST", ProdutoViewModel.CalcularCabecalhoColunaIcms(produtos));
        }

        [TestMethod]
        public void Cabecalho_RegressaoBug38_RevendaMaisComCstESemEmitenteCRT()
        {
            // Cenário concreto reportado pelo cliente Revenda Mais
            // (invoice 86fed76fd38f42e1833375e33a94567c): XML do emitente
            // sem <CRT>, mas itens têm <CST>. Antes do fix #38, o cabeçalho
            // caía em "O/CSOSN" porque o código checava só Emitente.CRT.
            // Após o fix, deriva dos itens e mostra "O/CST" corretamente.
            var produtos = new List<ProdutoViewModel>
            {
                new ProdutoViewModel { Origem = "1", Cst = "20" }
            };
            Assert.AreEqual("O/CST", ProdutoViewModel.CalcularCabecalhoColunaIcms(produtos));
        }
    }
}
