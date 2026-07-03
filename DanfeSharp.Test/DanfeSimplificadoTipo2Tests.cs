using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DanfeSharp.Esquemas.NFe;
using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.pdfclown.tools;
using PdfFile = org.pdfclown.files.File;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Testes do DANFE Simplificado Tipo 2 (NT 2026.003, tpImp=6) — 9 divisões
    /// renderizadas pela classe DanfeSimplificadoTipo2.
    /// See openspec/specs/danfe-simplificado-tipo-2/spec.md
    /// </summary>
    [TestClass]
    public class DanfeSimplificadoTipo2Tests
    {
        public readonly string OutputDirectory = Path.Combine("Output", "SimplificadoTipo2");

        public DanfeSimplificadoTipo2Tests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        private static DanfeViewModel CriarViewModelTipo2()
        {
            var m = new DanfeViewModel
            {
                NfNumero = 123456,
                NfSerie = 1,
                ChaveAcesso = "35260607952851000109550010001234561000123459",
                TipoAmbiente = 1,
                TipoEmissao = FormaEmissao.Normal,
                ProtocoloAutorizacao = "135260000012345 01/07/2026 10:30:00",
                DataHoraEmissao = new DateTime(2026, 7, 1, 10, 15, 0),
                EndConsulta = "SP".UrlNFeConsulta(1),
                Emitente = new EmpresaViewModel
                {
                    CnpjCpf = "07952851000109",
                    RazaoSocial = "Mercado Tupã Ltda",
                    EnderecoLogadrouro = "Avenida Brasil",
                    EnderecoNumero = "1000",
                    EnderecoBairro = "Centro",
                    EnderecoUf = "SP",
                    Municipio = "São Paulo"
                },
                Destinatario = new EmpresaViewModel
                {
                    CnpjCpf = "12345678909",
                    RazaoSocial = "João da Silva",
                    EnderecoLogadrouro = "Rua das Flores",
                    EnderecoNumero = "42",
                    EnderecoBairro = "Jardim",
                    EnderecoUf = "SP",
                    Municipio = "São Paulo"
                },
                Produtos = new List<ProdutoViewModel>
                {
                    new ProdutoViewModel
                    {
                        Codigo = "001",
                        Descricao = "Arroz Tipo 1 5kg",
                        Quantidade = 2,
                        Unidade = "UN",
                        ValorUnitario = 25.50,
                        ValorTotal = 51.00
                    }
                },
                Pagamento = new List<PagamentoViewModel>
                {
                    new PagamentoViewModel
                    {
                        Troco = 9.00m,
                        DetalhePagamento = new List<DetalheViewModel>
                        {
                            new DetalheViewModel { FormaPagamento = FormaPagamento.fpDinheiro, Valor = 60.00m }
                        }
                    }
                }
            };

            m.CalculoImposto.QuantidadeTotal = 2;
            m.CalculoImposto.ValorTotalProdutos = 51.00;
            m.CalculoImposto.ValorTotalNota = 51.00;

            return m;
        }

        private string GerarPdfETexto(DanfeViewModel model, string outFileName)
        {
            var pdfPath = Path.Combine(OutputDirectory, outFileName);
            using (var danfe = new DanfeSimplificadoTipo2(model))
            {
                danfe.Gerar();
                danfe.Salvar(pdfPath);
            }
            return pdfPath;
        }

        private static string ExtrairTextoPdf(string pdfPath)
        {
            var sb = new StringBuilder();
            using (var file = new PdfFile(pdfPath))
            {
                var extractor = new TextExtractor();
                foreach (var page in file.Document.Pages)
                {
                    var areaTextStrings = extractor.Extract(page);
                    foreach (var pair in areaTextStrings)
                    {
                        foreach (var textString in pair.Value)
                        {
                            sb.AppendLine(textString.Text);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        private static int ContarPaginas(string pdfPath)
        {
            using (var file = new PdfFile(pdfPath))
            {
                return file.Document.Pages.Count;
            }
        }

        // === Divisão I — Cabeçalho ===

        [TestMethod]
        public void DivisaoI_CabecalhoContemTituloEmitenteECnpj()
        {
            var texto = ExtrairTextoPdf(GerarPdfETexto(CriarViewModelTipo2(), "DivisaoI_Cabecalho.pdf"));

            StringAssert.Contains(texto, "DANFE Simplificado Tipo 2");
            StringAssert.Contains(texto, "Mercado Tupã Ltda");
            StringAssert.Contains(texto, "07.952.851/0001-09");
        }

        // === Divisões II e III — Produtos e Totais ===

        [TestMethod]
        public void DivisaoII_ProdutoRenderizado()
        {
            var texto = ExtrairTextoPdf(GerarPdfETexto(CriarViewModelTipo2(), "DivisaoII_Produtos.pdf"));

            StringAssert.Contains(texto, "Arroz Tipo 1 5kg");
            StringAssert.Contains(texto, "QTD. TOTAL DE ITENS");
        }

        [TestMethod]
        public void DivisaoIII_TrocoSemprePresente_MesmoSemValor()
        {
            var model = CriarViewModelTipo2();
            model.Pagamento[0].Troco = null;

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoIII_TrocoZerado.pdf"));

            StringAssert.Contains(texto, "TROCO");
            StringAssert.Contains(texto, "VALOR PAGO");
        }

        [TestMethod]
        public void DivisaoIII_FormaPagamentoEValores()
        {
            var texto = ExtrairTextoPdf(GerarPdfETexto(CriarViewModelTipo2(), "DivisaoIII_Pagamento.pdf"));

            StringAssert.Contains(texto, "Dinheiro");
            StringAssert.Contains(texto, "60,00");
            StringAssert.Contains(texto, "9,00");
        }

        // === Divisão III-A — IBS/CBS/IS ===

        [TestMethod]
        public void DivisaoIIIA_ComGrupoIbsCbs_RenderizaRotulosEValores()
        {
            var model = CriarViewModelTipo2();
            model.TributosIbsCbs = new TributosIbsCbsViewModel
            {
                BaseCalculo = 51.00m,
                ValorIbs = 5.10m,
                ValorIbsUF = 3.06m,
                ValorIbsMun = 2.04m,
                ValorCbs = 4.59m
            };

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoIIIA_ComIbsCbs.pdf"));

            StringAssert.Contains(texto, "TRIBUTOS IBS/CBS");
            StringAssert.Contains(texto, "IBS");
            StringAssert.Contains(texto, "CBS");
            StringAssert.Contains(texto, "5,10");
            StringAssert.Contains(texto, "4,59");
        }

        [TestMethod]
        public void DivisaoIIIA_SemGrupoIbsCbs_DivisaoOmitida()
        {
            var model = CriarViewModelTipo2();
            model.TributosIbsCbs = null;

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoIIIA_SemIbsCbs.pdf"));

            Assert.IsFalse(texto.Contains("TRIBUTOS IBS/CBS"),
                "Divisão III-A deve ser omitida quando o XML não tem o grupo IBSCBSTot (fidelidade ao XML).");
        }

        [TestMethod]
        public void DivisaoIIIA_ImpostoSeletivo_QuandoPresente()
        {
            var model = CriarViewModelTipo2();
            model.TributosIbsCbs = new TributosIbsCbsViewModel { ValorIS = 1.23m };

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoIIIA_ComIS.pdf"));

            StringAssert.Contains(texto, "IMPOSTO SELETIVO (IS)");
        }

        // === Divisão IV — Consulta via chave ===

        [TestMethod]
        public void DivisaoIV_ChaveEmOnzeBlocosDeQuatroDigitos()
        {
            var model = CriarViewModelTipo2();
            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoIV_Chave.pdf"));

            StringAssert.Contains(texto, model.ChaveAcesso.SpaceOnAccessKey(),
                "Chave deve aparecer formatada em 11 blocos de 4 dígitos separados por espaço.");
            StringAssert.Contains(texto, "CHAVE DE ACESSO");
            StringAssert.Contains(texto, model.EndConsulta);
        }

        // === Divisão V — QR Code ===

        [TestMethod]
        public void DivisaoV_ComQrCode_RenderizaSemExcecao()
        {
            var model = CriarViewModelTipo2();
            model.QrCode = "https://www.nfe.fazenda.gov.br/portal/consultaRecaptcha.aspx?p=35260607952851000109550010001234561000123459|3|1|1|ABCD";

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoV_ComQr.pdf"));

            StringAssert.Contains(texto, "CONSULTA VIA LEITOR DE QR CODE");
        }

        [TestMethod]
        public void DivisaoV_SemQrCode_OmiteDivisaoSemExcecao()
        {
            var model = CriarViewModelTipo2();
            model.QrCode = null;

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoV_SemQr.pdf"));

            Assert.IsFalse(texto.Contains("CONSULTA VIA LEITOR DE QR CODE"),
                "Sem conteúdo de QR o bloco deve ser omitido, sem erro.");
        }

        // === Divisão VI — Consumidor ===

        [TestMethod]
        public void DivisaoVI_ConsumidorComCpf()
        {
            var texto = ExtrairTextoPdf(GerarPdfETexto(CriarViewModelTipo2(), "DivisaoVI_ConsumidorCpf.pdf"));

            StringAssert.Contains(texto, "CONSUMIDOR CPF:");
            StringAssert.Contains(texto, "João da Silva");
        }

        [TestMethod]
        public void DivisaoVI_ConsumidorComCnpj()
        {
            var model = CriarViewModelTipo2();
            model.Destinatario.CnpjCpf = "07952851000109";

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoVI_ConsumidorCnpj.pdf"));

            StringAssert.Contains(texto, "CONSUMIDOR CNPJ:");
        }

        [TestMethod]
        public void DivisaoVI_ConsumidorNaoIdentificado()
        {
            var model = CriarViewModelTipo2();
            model.Destinatario = null;

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoVI_NaoIdentificado.pdf"));

            StringAssert.Contains(texto, "CONSUMIDOR NÃO IDENTIFICADO");
        }

        // === Divisão VII — Identificação da NF-e ===

        [TestMethod]
        public void DivisaoVII_NumeroSerieProtocoloEVia()
        {
            var texto = ExtrairTextoPdf(GerarPdfETexto(CriarViewModelTipo2(), "DivisaoVII_Identificacao.pdf"));

            StringAssert.Contains(texto, "123456");
            StringAssert.Contains(texto, "Série: 1");
            StringAssert.Contains(texto, "PROTOCOLO DE AUTORIZAÇÃO");
            StringAssert.Contains(texto, "135260000012345");
            StringAssert.Contains(texto, "Via do Consumidor");
        }

        // === Divisão VIII — Homologação e Contingência ===

        [TestMethod]
        public void DivisaoVIII_Homologacao_ExibeMensagemSemValorFiscal()
        {
            var model = CriarViewModelTipo2();
            model.TipoAmbiente = 2;

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoVIII_Homologacao.pdf"));

            StringAssert.Contains(texto, "EMITIDA EM AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL");
        }

        [TestMethod]
        public void DivisaoVIII_ContingenciaPendente_MensagemEmDoisLocaisESegundaVia()
        {
            var model = CriarViewModelTipo2();
            model.TipoEmissao = FormaEmissao.ContingenciaFSDA;
            model.ProtocoloAutorizacao = null;

            var path = GerarPdfETexto(model, "DivisaoVIII_Contingencia.pdf");
            var texto = ExtrairTextoPdf(path);

            Assert.AreEqual(2, ContarPaginas(path), "Contingência pendente deve gerar 2 vias (2 páginas).");

            var ocorrencias = Regex.Matches(texto, "EMITIDA EM CONTINGÊNCIA").Count;
            Assert.AreEqual(4, ocorrencias, "Mensagem deve aparecer em 2 locais por via × 2 vias = 4 ocorrências.");

            StringAssert.Contains(texto, "PENDENTE DE AUTORIZAÇÃO");
            StringAssert.Contains(texto, "Via do Consumidor");
            StringAssert.Contains(texto, "Via do Estabelecimento");
        }

        [TestMethod]
        public void DivisaoVIII_EmissaoNormal_SemContingenciaEUmaViaSo()
        {
            var path = GerarPdfETexto(CriarViewModelTipo2(), "DivisaoVIII_Normal.pdf");
            var texto = ExtrairTextoPdf(path);

            Assert.AreEqual(1, ContarPaginas(path), "Emissão normal gera via única.");
            Assert.IsFalse(texto.Contains("EMITIDA EM CONTINGÊNCIA"));
        }

        [TestMethod]
        public void DivisaoVIII_ContingenciaJaAutorizada_NaoExibeMensagemPendente()
        {
            // Autorizada após contingência: com protocolo presente não há pendência —
            // mensagem e segunda via não se aplicam.
            var model = CriarViewModelTipo2();
            model.TipoEmissao = FormaEmissao.ContingenciaFSDA;

            var path = GerarPdfETexto(model, "DivisaoVIII_ContingenciaAutorizada.pdf");
            var texto = ExtrairTextoPdf(path);

            Assert.AreEqual(1, ContarPaginas(path));
            Assert.IsFalse(texto.Contains("EMITIDA EM CONTINGÊNCIA"));
        }

        // === Divisão IX — Mensagem do contribuinte ===

        [TestMethod]
        public void DivisaoIX_Lei12741EInformacoesComplementares()
        {
            var model = CriarViewModelTipo2();
            model.CalculoImposto.ValorAproximadoTributos = 9.18;
            model.InformacoesComplementares = "Obrigado pela preferência!";
            model.InformacoesAdicionaisFisco = "Documento emitido conforme NT 2026.003.";

            var texto = ExtrairTextoPdf(GerarPdfETexto(model, "DivisaoIX_Mensagens.pdf"));

            StringAssert.Contains(texto, "LEI 12.741/2012");
            StringAssert.Contains(texto, "Obrigado pela preferência!");
            StringAssert.Contains(texto, "Documento emitido conforme NT 2026.003.");
        }

        // === Smoke de altura dinâmica ===

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(30)]
        [DataRow(200)]
        public void Smoke_AlturaDinamica_GeraSemExcecao(int quantidadeProdutos)
        {
            var model = CriarViewModelTipo2();
            model.Produtos = new List<ProdutoViewModel>();

            for (int i = 1; i <= quantidadeProdutos; i++)
            {
                model.Produtos.Add(new ProdutoViewModel
                {
                    Codigo = i.ToString(),
                    Descricao = $"Produto da linha {i}",
                    Quantidade = 1,
                    Unidade = "UN",
                    ValorUnitario = 1.00,
                    ValorTotal = 1.00
                });
            }

            var path = GerarPdfETexto(model, $"Smoke_{quantidadeProdutos}itens.pdf");

            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(new FileInfo(path).Length > 0);

            var texto = ExtrairTextoPdf(path);
            StringAssert.Contains(texto, $"Produto da linha {quantidadeProdutos}",
                "Último produto deve estar dentro da página (altura estimada suficiente).");
        }
    }
}
