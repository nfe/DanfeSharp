using System;
using DanfeNet.Esquemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeNet.Test
{
    [TestClass]
    public class DanfeTest
    {

     
        [TestMethod]
        public void RetratoSemIcmsInterestadual()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.ExibirIcmsInterestadual = false;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void PaisagemSemIcmsInterestadual()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.ExibirIcmsInterestadual = false;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }       

        [TestMethod]
        public void Paisagem_2Canhotos()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.QuantidadeCanhotos = 2;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato_2Canhotos()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.QuantidadeCanhotos = 2;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Paisagem_SemCanhoto()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.QuantidadeCanhotos = 0;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato_SemCanhoto()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.QuantidadeCanhotos = 0;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Contingencia_SVC_AN()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoEmissao = FormaEmissao.ContingenciaSVCAN;
            model.ContingenciaDataHora = DateTimeOffset.Now;
            model.ContingenciaJustificativa = "Aqui vai o motivo da contingência";
            model.Orientacao = Orientacao.Retrato;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Contingencia_SVC_RS()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoEmissao = FormaEmissao.ContingenciaSVCRS;
            model.ContingenciaDataHora = DateTimeOffset.Now;
            model.ContingenciaJustificativa = "Aqui vai o motivo da contingência";
            model.Orientacao = Orientacao.Retrato;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            Danfe d = new Danfe(model);       
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void OpcaoPreferirEmitenteNomeFantasia_False()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.PreferirEmitenteNomeFantasia = false;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Paisagem()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void RetratoHomologacao()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoAmbiente = 2;
            model.Orientacao = Orientacao.Retrato;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void PaisagemHomologacao()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoAmbiente = 2;
            model.Orientacao = Orientacao.Paisagem;
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void ComBlocoLocalEntrega()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.LocalEntrega = FabricaFake.LocalEntregaRetiradaFake();
            Danfe d = new Danfe(model);
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
            Danfe d = new Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

    }
}
