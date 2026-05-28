using System.IO;
using System.Text;
using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.pdfclown.tools;
using PdfFile = org.pdfclown.files.File;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Tests for the cancelled-invoice watermark feature. Covers nfe/DanfeSharp#40.
    /// See openspec/changes/add-cancelled-watermark/specs/danfe-cancelled-watermark/spec.md
    /// </summary>
    [TestClass]
    public class DanfeCanceladaTests
    {
        private const string MarcaDaguaCancelada = "DOCUMENTO CANCELADO";

        public readonly string OutputDirectory = Path.Combine("Output", "DeCancelada");
        public readonly string InputXmlDirectoryPrefix = Path.Combine("Xml", "NFe");

        public DanfeCanceladaTests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        // === Unit-level: ViewModel default ===

        [TestMethod]
        public void DanfeViewModel_DefaultIsCancelledFalse()
        {
            var vm = new DanfeViewModel();
            Assert.IsFalse(vm.IsCancelled, "IsCancelled deve ser false por padrão (preserva comportamento anterior).");
        }

        [TestMethod]
        public void DanfeViewModel_IsCancelledSettable()
        {
            var vm = new DanfeViewModel { IsCancelled = true };
            Assert.IsTrue(vm.IsCancelled);
        }

        // === Integration: render PDF + verify watermark presence/absence ===

        private DanfeViewModel CarregarFixture(string fixtureRelative)
        {
            var path = Path.Combine(InputXmlDirectoryPrefix, fixtureRelative);
            return DanfeViewModelCreator.CriarDeArquivoXml(path);
        }

        private string GerarPdfETexto(DanfeViewModel model, string outFileName)
        {
            var pdfPath = Path.Combine(OutputDirectory, outFileName);
            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(pdfPath);
            }
            return pdfPath;
        }

        // Extrai todo o texto renderizado em todas as páginas do PDF usando o
        // TextExtractor do PDFClown (já dependência do projeto). Permite que os
        // testes verifiquem presença/ausência da marca d'água sem depender de
        // ferramenta externa como pdftotext (que não roda em CI puro).
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

        [TestMethod]
        public void Render_IsCancelledTrue_DesenhaMarcaCancelada()
        {
            var model = CarregarFixture("v4.00/v4_DanfeIntermediario.xml");
            model.IsCancelled = true;
            var path = GerarPdfETexto(model, "Intermediario_Cancelada.pdf");

            Assert.IsTrue(File.Exists(path), "PDF deve ser gerado.");
            Assert.IsTrue(new FileInfo(path).Length > 0, "PDF não pode estar vazio.");

            var texto = ExtrairTextoPdf(path);
            StringAssert.Contains(texto, MarcaDaguaCancelada,
                "Conteúdo do PDF deve conter a marca d'água 'DOCUMENTO CANCELADO' quando IsCancelled=true.");
        }

        [TestMethod]
        public void Render_cStat101_DesenhaMarcaCancelada()
        {
            // Fallback de detecção: quando IsCancelled é false mas o XML traz
            // cStat=101, a marca também deve aparecer (compat com PR #8/2023).
            var model = CarregarFixture("v4.00/v4_DanfeIntermediario.xml");
            model.CodigoStatusReposta = 101;
            var path = GerarPdfETexto(model, "Intermediario_cStat101.pdf");

            var texto = ExtrairTextoPdf(path);
            StringAssert.Contains(texto, MarcaDaguaCancelada,
                "Marca d'água deve aparecer também quando o trigger é cStat=101 (fallback).");
        }

        [TestMethod]
        public void Render_AmbosTrigger_NaoLancaExcecao()
        {
            // IsCancelled=true E cStat=101 simultaneamente — deve renderizar
            // sem duplicar marca (Danfe.cs faz OR, então marca aparece uma vez).
            var model = CarregarFixture("v4.00/v4_DanfeIntermediario.xml");
            model.IsCancelled = true;
            model.CodigoStatusReposta = 101;
            var path = GerarPdfETexto(model, "Intermediario_AmbosTrigger.pdf");
            Assert.IsTrue(File.Exists(path));
        }

        [TestMethod]
        public void Render_NaoCancelada_NaoDesenhaMarcaCancelada()
        {
            // Regressão: render normal sem nenhum trigger não deve incluir
            // a marca d'água — protege contra mudanças que sempre desenhem.
            var model = CarregarFixture("v4.00/v4_DanfeIntermediario.xml");
            var path = GerarPdfETexto(model, "Intermediario_NaoCancelada.pdf");

            Assert.IsFalse(model.IsCancelled);

            var texto = ExtrairTextoPdf(path);
            Assert.IsFalse(texto.Contains(MarcaDaguaCancelada),
                "Marca d'água NÃO pode aparecer em DANFE não-cancelada (regressão).");
        }

        // === Demo render: 3 fixtures × cancelada (Mínimo + Intermediário + Completo) ===

        [TestMethod]
        public void Demo_DanfeMinimo_Cancelada()
        {
            var model = CarregarFixture("v4.00/v4_DanfeMinimo.xml");
            model.IsCancelled = true;
            GerarPdfETexto(model, "v4_DanfeMinimo_Cancelada.pdf");
        }

        [TestMethod]
        public void Demo_DanfeIntermediario_Cancelada()
        {
            var model = CarregarFixture("v4.00/v4_DanfeIntermediario.xml");
            model.IsCancelled = true;
            GerarPdfETexto(model, "v4_DanfeIntermediario_Cancelada.pdf");
        }

        [TestMethod]
        public void Demo_DanfeCompleto_Cancelada()
        {
            var model = CarregarFixture("v4.00/v4_DanfeCompleto.xml");
            model.IsCancelled = true;
            GerarPdfETexto(model, "v4_DanfeCompleto_Cancelada.pdf");
        }

        // === Paisagem cancelada (cobertura visual em ambas as orientações) ===

        [TestMethod]
        public void Demo_DanfeIntermediario_Cancelada_Paisagem()
        {
            var model = CarregarFixture("v4.00/v4_DanfeIntermediario.xml");
            model.IsCancelled = true;
            model.Orientacao = Orientacao.Paisagem;
            GerarPdfETexto(model, "v4_DanfeIntermediario_Cancelada_Paisagem.pdf");
        }

        [TestMethod]
        public void Demo_DanfeCompleto_Cancelada_Paisagem()
        {
            var model = CarregarFixture("v4.00/v4_DanfeCompleto.xml");
            model.IsCancelled = true;
            model.Orientacao = Orientacao.Paisagem;
            GerarPdfETexto(model, "v4_DanfeCompleto_Cancelada_Paisagem.pdf");
        }
    }
}
