using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DanfeSharp.Test
{
    [TestClass]
    public class DanfeTest
    {

        [TestMethod]
        public void GerarDanfeNFCXml()
        {
            string pasta = @"D:\teste\d\";
            string caminhoOut = @"D:\teste\s\";

            if (!Directory.Exists(caminhoOut)) Directory.CreateDirectory(caminhoOut);

            foreach (var pathXml in Directory.EnumerateFiles(pasta, "*.xml"))
            {
                var model = DanfeViewModelCreator.CriarModeloNFCeDeArquivoXml(pathXml);
                using (var danfe = new DanfeNFC(viewModel: model))
                {
                    danfe.Gerar();
                    danfe.Salvar($"{caminhoOut}/{model.ChaveAcesso}.pdf");
                }
            }
        }

        [TestMethod]
        public void GerarDanfeXml()
        {
            string pasta = @"C:\works\Companies\nfe\João\pdf";
            string caminhoOut = @"C:\works\Companies\nfe\João\pdf";

            if (!Directory.Exists(caminhoOut)) Directory.CreateDirectory(caminhoOut);

            foreach (var pathXml in Directory.EnumerateFiles(pasta, "*.xml"))
            {
                var model = DanfeViewModelCreator.CriarDeArquivoXml(pathXml);
                using (Danfe danfe = new Danfe(model))
                {
                    danfe.Gerar();
                    danfe.Salvar($"{caminhoOut}/{model.ChaveAcesso}.pdf");
                }
            }
        }

        [TestMethod]
        public void GerarDanfeEventoXml()
        {
            string pasta = @"C:\Users\BrunoAlencar\Documents\pdf";
            string caminhoOut = @"C:\Users\BrunoAlencar\Documents\pdf";

            if (!Directory.Exists(caminhoOut)) Directory.CreateDirectory(caminhoOut);

            foreach (var pathXml in Directory.EnumerateFiles(pasta, "*.xml"))
            {
                var model = DanfeEventoViewModelCreator.CriarDeArquivoXml(pathXml);
                using (DanfeCCC danfe = new DanfeCCC(model))
                {
                    danfe.Gerar();
                    danfe.Salvar($"{caminhoOut}/{model.ChaveAcesso}.pdf");
                }
            }
        }

        [TestMethod]
        public void RetratoSemIcmsInterestadual()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.ExibirIcmsInterestadual = false;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void PaisagemSemIcmsInterestadual()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.ExibirIcmsInterestadual = false;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }       

        [TestMethod]
        public void Paisagem_2Canhotos()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.QuantidadeCanhotos = 2;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato_2Canhotos()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.QuantidadeCanhotos = 2;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Paisagem_SemCanhoto()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.QuantidadeCanhotos = 0;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato_SemCanhoto()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.QuantidadeCanhotos = 0;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Contingencia_SVC_AN()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoEmissao = Esquemas.NFe.FormaEmissao.ContingenciaSVCAN;
            model.ContingenciaDataHora = DateTimeOffset.Now;
            model.ContingenciaJustificativa = "Aqui vai o motivo da contingência";
            model.Orientacao = Orientacao.Retrato;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Contingencia_SVC_RS()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoEmissao = Esquemas.NFe.FormaEmissao.ContingenciaSVCRS;
            model.ContingenciaDataHora = DateTimeOffset.Now;
            model.ContingenciaJustificativa = "Aqui vai o motivo da contingência";
            model.Orientacao = Orientacao.Retrato;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);       
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void OpcaoPreferirEmitenteNomeFantasia_False()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.PreferirEmitenteNomeFantasia = false;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Paisagem()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void RetratoHomologacao()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoAmbiente = 2;
            model.Orientacao = Orientacao.Retrato;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void PaisagemHomologacao()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoAmbiente = 2;
            model.Orientacao = Orientacao.Paisagem;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void ComBlocoLocalEntrega()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.LocalEntrega = FabricaFake.LocalEntregaRetiradaFake();
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void ComBlocoLocalRetirada()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.DataHoraEmissao = DateTimeOffset.UtcNow.DateTime;
            model.DataSaidaEntrada = DateTimeOffset.UtcNow.DateTime;
            model.LocalRetirada = FabricaFake.LocalEntregaRetiradaFake();
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

    }
}
