using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Test
{
    [TestClass]
    public class DanfeXmlTests
    {
        public readonly string OutputDirectory = Path.Combine("Output", "DeXml");
        public readonly string InputXmlDirectoryPrefix = Path.Combine("Xml", "NFe");

        public DanfeXmlTests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        public void TestXml(String xmlPath)
        {
            var outPdfFilePath = Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(xmlPath) + ".pdf");
            var model = DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputXmlDirectoryPrefix, xmlPath));
            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(outPdfFilePath);
            }
        }

        [TestMethod]
        public void v1() => TestXml("v1.00/v1.xml");

        [TestMethod]
        public void v2_Retrato() => TestXml("v2.00/v2_Retrato.xml");

        [TestMethod]
        public void v3_10_Retrato() => TestXml("v3.10/v3.10_Retrato.xml");

        [TestMethod]
        public void v4_ComLocalEntrega() => TestXml("v4.00/v4_ComLocalEntrega.xml");

        [TestMethod]
        public void v4_ComLocalRetirada() => TestXml("v4.00/v4_ComLocalRetirada.xml");

        /// <summary>
        /// Fixture demo construída para exibir os ajustes desta sprint:
        /// (a) bloco "Forma de Pagamento" com 3 detPag variados (PIX, Cartão
        /// de Crédito, Outros com xPag="VALE TROCO");
        /// (b) bloco "Fatura / Duplicata" com 6 duplicatas mensais — mostra
        /// o novo layout horizontal de campos + a paginação em 2 linhas de 3
        /// duplicatas cada (numeroElementosLinha=3 em retrato).
        /// </summary>
        [TestMethod]
        public void v4_RevendaMaisDemo() => TestXml("v4.00/v4_RevendaMaisDemo.xml");

        /// <summary>
        /// Mesma fixture v4_RevendaMaisDemo mas forçando Orientacao=Paisagem.
        /// O DanfeViewModelCreator hoje hardcoda Retrato (ver
        /// DanfeViewModelCreator.cs:411 — comentário sobre netcore2.0), então
        /// para visualizar paisagem precisamos sobrescrever após o load.
        /// Antes do fix em Danfe.Gerar() (pagar guard de overflow), este teste
        /// falhava com "Height is invalid" — a fixture rica em paisagem
        /// fazia os blocos topo consumirem toda a altura. Agora passa: o
        /// guard pula página sem espaço, próxima página renderiza a tabela.
        /// </summary>
        [TestMethod]
        public void v4_RevendaMaisDemo_Paisagem()
        {
            var inputPath = Path.Combine(InputXmlDirectoryPrefix, "v4.00", "v4_RevendaMaisDemo.xml");
            var outPath = Path.Combine(OutputDirectory, "v4_RevendaMaisDemo_Paisagem.pdf");
            var model = DanfeViewModelCreator.CriarDeArquivoXml(inputPath);
            model.Orientacao = Orientacao.Paisagem;
            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(outPath);
            }
        }

        /// <summary>
        /// Versão paisagem da fixture leve (1 duplicata, 1 detPag) para
        /// confirmar que o pipeline paisagem funciona com NF-e simples —
        /// isolando se o erro "Height is invalid" do demo paisagem é por
        /// overflow de conteúdo (6 duplicatas + 3 detPag) ou bug geral.
        /// </summary>
        [TestMethod]
        public void v4_ComLocalEntrega_Paisagem()
        {
            var inputPath = Path.Combine(InputXmlDirectoryPrefix, "v4.00", "v4_ComLocalEntrega.xml");
            var outPath = Path.Combine(OutputDirectory, "v4_ComLocalEntrega_Paisagem.pdf");
            var model = DanfeViewModelCreator.CriarDeArquivoXml(inputPath);
            model.Orientacao = Orientacao.Paisagem;
            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(outPath);
            }
        }

        // === Fixtures de demonstração geradas por generate-fixtures.sh ===

        /// <summary>DANFE mínimo: 1 item, sem cobr, sem pag, sem infAdProd.</summary>
        [TestMethod]
        public void v4_DanfeMinimo() => TestXml("v4.00/v4_DanfeMinimo.xml");

        /// <summary>DANFE intermediário: 5 itens (3 com infAdProd), 2 duplicatas, 2 detPag.</summary>
        [TestMethod]
        public void v4_DanfeIntermediario() => TestXml("v4.00/v4_DanfeIntermediario.xml");

        /// <summary>DANFE completo: 20 itens (todos com infAdProd), 6 duplicatas, 3 detPag.</summary>
        [TestMethod]
        public void v4_DanfeCompleto() => TestXml("v4.00/v4_DanfeCompleto.xml");

        // === Mesmas 3 fixtures, mas em PAISAGEM (override de Orientacao) ===

        private void RenderPaisagem(string xmlRelativePath, string outFileName)
        {
            var inputPath = Path.Combine(InputXmlDirectoryPrefix, xmlRelativePath);
            var outPath = Path.Combine(OutputDirectory, outFileName);
            var model = DanfeViewModelCreator.CriarDeArquivoXml(inputPath);
            model.Orientacao = Orientacao.Paisagem;
            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(outPath);
            }
        }

        [TestMethod]
        public void v4_DanfeMinimo_Paisagem() =>
            RenderPaisagem("v4.00/v4_DanfeMinimo.xml", "v4_DanfeMinimo_Paisagem.pdf");

        [TestMethod]
        public void v4_DanfeIntermediario_Paisagem() =>
            RenderPaisagem("v4.00/v4_DanfeIntermediario.xml", "v4_DanfeIntermediario_Paisagem.pdf");

        [TestMethod]
        public void v4_DanfeCompleto_Paisagem() =>
            RenderPaisagem("v4.00/v4_DanfeCompleto.xml", "v4_DanfeCompleto_Paisagem.pdf");
    }
}
