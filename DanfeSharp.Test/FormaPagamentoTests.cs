using System.IO;
using System.Xml.Serialization;
using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// Aliases para desambiguar os dois enums FormaPagamento que coexistem
// no projeto (um em DanfeSharp/Enums.cs, outro em DanfeSharp/Esquemas/ProcNFe.cs).
using FormaPagamentoVm = DanfeSharp.FormaPagamento;
using FormaPagamentoSchema = DanfeSharp.Esquemas.NFe.FormaPagamento;
using detPagSchema = DanfeSharp.Esquemas.NFe.detPag;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Tests for the FORMA DE PAGAMENTO block rendering in DANFE NF-e modelo 55.
    /// Covers nfe/DanfeSharp#39.
    /// See openspec/changes/add-danfe-payment-block/specs/danfe-payment-block/spec.md
    /// </summary>
    [TestClass]
    public class FormaPagamentoTests
    {
        // === Helper FormaPagamentoExtensions.GetDescricao ===

        [TestMethod]
        public void GetDescricao_Dinheiro_RetornaDinheiro()
        {
            Assert.AreEqual("Dinheiro", FormaPagamentoVm.fpDinheiro.GetDescricao());
        }

        [TestMethod]
        public void GetDescricao_CartaoCredito_RetornaCartaoCredito()
        {
            Assert.AreEqual("Cartão de Crédito", FormaPagamentoVm.fpCartaoCredito.GetDescricao());
        }

        [TestMethod]
        public void GetDescricao_CartaoDebito_RetornaCartaoDebito()
        {
            Assert.AreEqual("Cartão de Débito", FormaPagamentoVm.fpCartaoDebito.GetDescricao());
        }

        [TestMethod]
        public void GetDescricao_Outros_RetornaOutros()
        {
            Assert.AreEqual("Outros", FormaPagamentoVm.fpOutro.GetDescricao());
        }

        [TestMethod]
        public void GetDescricao_Pix_RetornaDescricaoNaoVazia()
        {
            // tPag=17 (Pagamento Instantâneo - PIX, NT 2020.001)
            var resultado = FormaPagamentoVm.fpPagamentoInstantaneoPIX.GetDescricao();
            Assert.IsFalse(string.IsNullOrEmpty(resultado), "PIX deve ter descrição não-vazia");
        }

        [TestMethod]
        public void GetDescricao_ValorInvalido_RetornaStringVazia()
        {
            // Valor fora do enum (defesa em profundidade contra XML com tPag novo)
            var resultado = ((FormaPagamentoVm)999).GetDescricao();
            Assert.AreEqual(string.Empty, resultado);
        }

        // === Schema parsing — detPag.xPag deserialization ===

        private detPagSchema DeserializarDetPag(string xml)
        {
            var serializer = new XmlSerializer(typeof(detPagSchema));
            using (var reader = new StringReader(xml))
            {
                return (detPagSchema)serializer.Deserialize(reader);
            }
        }

        [TestMethod]
        public void Schema_XmlComXPag_DeserializaCampo()
        {
            var xml = @"<?xml version='1.0'?>
                <detPag>
                    <tPag>99</tPag>
                    <xPag>RECURSOS PROPRIOS</xPag>
                    <vPag>48000.00</vPag>
                </detPag>";

            var detPag = DeserializarDetPag(xml);

            Assert.AreEqual("RECURSOS PROPRIOS", detPag.xPag);
            Assert.AreEqual(FormaPagamentoSchema.fpOutro, detPag.tPag);
            Assert.AreEqual(48000.00m, detPag.vPag);
        }

        [TestMethod]
        public void Schema_XmlSemXPag_RetornaNull()
        {
            var xml = @"<?xml version='1.0'?>
                <detPag>
                    <tPag>01</tPag>
                    <vPag>100.00</vPag>
                </detPag>";

            var detPag = DeserializarDetPag(xml);

            Assert.IsNull(detPag.xPag);
            Assert.AreEqual(FormaPagamentoSchema.fpDinheiro, detPag.tPag);
        }

        [TestMethod]
        public void Schema_XmlComXPagVazio_RetornaStringVazia()
        {
            // Edge case: <xPag></xPag> presente mas sem conteúdo
            var xml = @"<?xml version='1.0'?>
                <detPag>
                    <tPag>01</tPag>
                    <xPag></xPag>
                    <vPag>100.00</vPag>
                </detPag>";

            var detPag = DeserializarDetPag(xml);

            // XmlSerializer entrega string.Empty para elemento vazio
            // (o producer normaliza para null antes de chegar ao ViewModel)
            Assert.IsTrue(string.IsNullOrEmpty(detPag.xPag));
        }

        // === DetalheViewModel.Descricao ===

        [TestMethod]
        public void ViewModel_DescricaoSettable()
        {
            var detalhe = new DetalheViewModel
            {
                FormaPagamento = FormaPagamentoVm.fpOutro,
                Descricao = "BOLETO BB",
                Valor = 500m
            };

            Assert.AreEqual("BOLETO BB", detalhe.Descricao);
            Assert.AreEqual(FormaPagamentoVm.fpOutro, detalhe.FormaPagamento);
        }

        [TestMethod]
        public void ViewModel_DescricaoNullPorDefault()
        {
            var detalhe = new DetalheViewModel
            {
                FormaPagamento = FormaPagamentoVm.fpDinheiro,
                Valor = 100m
            };

            Assert.IsNull(detalhe.Descricao);
        }

        // === Integration: helper + viewmodel + selection rule ===

        [TestMethod]
        public void DescricaoPrevalecesoSobreEnum_QuandoAmbosPresentes()
        {
            // Regra da spec: quando Descricao preenchido, prevalece sobre [Description] do enum
            var detalhe = new DetalheViewModel
            {
                FormaPagamento = FormaPagamentoVm.fpCartaoCredito,  // descricao default: "Cartão de Crédito"
                Descricao = "VISA 4× 2,5%",
                Valor = 100m
            };

            var resultado = !string.IsNullOrEmpty(detalhe.Descricao)
                ? detalhe.Descricao
                : detalhe.FormaPagamento.GetDescricao();

            Assert.AreEqual("VISA 4× 2,5%", resultado);
        }

        [TestMethod]
        public void EnumDescricaoUsadoComoFallback_QuandoDescricaoEhNull()
        {
            var detalhe = new DetalheViewModel
            {
                FormaPagamento = FormaPagamentoVm.fpDinheiro,
                Descricao = null,
                Valor = 100m
            };

            var resultado = !string.IsNullOrEmpty(detalhe.Descricao)
                ? detalhe.Descricao
                : detalhe.FormaPagamento.GetDescricao();

            Assert.AreEqual("Dinheiro", resultado);
        }

        [TestMethod]
        public void TPag99SemDescricao_RetornaOutrosComoFallback()
        {
            // Edge case: XML mal-formado com tPag=99 mas sem xPag
            var detalhe = new DetalheViewModel
            {
                FormaPagamento = FormaPagamentoVm.fpOutro,
                Descricao = null,
                Valor = 100m
            };

            var resultado = !string.IsNullOrEmpty(detalhe.Descricao)
                ? detalhe.Descricao
                : detalhe.FormaPagamento.GetDescricao();

            Assert.AreEqual("Outros", resultado);
        }
    }
}

namespace DanfeSharp.Test
{
    using System.IO;
    using DanfeSharp.Modelo;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Integration tests garantindo que o producer (CreateFromProcNFe) popula
    /// model.Pagamento a partir do XML — bug #39 raiz: a população estava só
    /// em CreateFromProcNFCe (modelo 65), faltava em CreateFromProcNFe
    /// (modelo 55).
    /// </summary>
    [TestClass]
    public class IntegracaoPagamentoNFe
    {
        [TestMethod]
        public void V4ComLocalEntrega_PopulaPagamentoDoGrupoPag()
        {
            var path = Path.Combine("Xml", "NFe", "v4.00", "v4_ComLocalEntrega.xml");
            var model = DanfeViewModelCreator.CriarDeArquivoXml(path);

            Assert.IsNotNull(model.Pagamento, "ViewModel.Pagamento não deve ser null");
            Assert.IsTrue(model.Pagamento.Count > 0,
                "model.Pagamento.Count==0 indica que CreateFromProcNFe não populou — checar regressão do fix #39");

            var detalhe = model.Pagamento[0].DetalhePagamento[0];
            Assert.AreEqual(3977.00m, detalhe.Valor);
            Assert.AreEqual(DanfeSharp.FormaPagamento.fpOutro, detalhe.FormaPagamento);
        }

        [TestMethod]
        public void V3_10Retrato_SemPag_NaoQuebraEPagamentoFicaVazio()
        {
            // Regressão: NF-e antiga sem <pag> no XML continua válida
            var path = Path.Combine("Xml", "NFe", "v3.10", "v3.10_Retrato.xml");
            var model = DanfeViewModelCreator.CriarDeArquivoXml(path);

            Assert.IsNotNull(model.Pagamento);
            Assert.AreEqual(0, model.Pagamento.Count);
        }
    }
}
