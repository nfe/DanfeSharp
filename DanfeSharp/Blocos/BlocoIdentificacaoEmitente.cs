using System.Drawing;
using org.pdfclown.documents.contents.xObjects;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    internal class BlocoIdentificacaoEmitente : BlocoBase
    {
        public const float LarguraCampoChaveNFe = 93F;
        public const float AlturaLinha1 = 30;

        NumeroNfSerie2 ifdNfe;
        IdentificacaoEmitente idEmitente;

        public BlocoIdentificacaoEmitente(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {

            var textoConsulta = new TextoSimples(Estilo, Strings.TextoConsulta)
            {
                Height = 8,
                AlinhamentoHorizontal = AlinhamentoHorizontal.Centro,
                AlinhamentoVertical = AlinhamentoVertical.Centro,
                TamanhoFonte = 9
            };

            var campoChaveAcesso = new Campo("Chave de Acesso", Formatador.FormatarChaveAcesso(ViewModel.ChaveAcesso), estilo, AlinhamentoHorizontal.Centro) { Height = Constantes.CampoAltura };
            var codigoBarras = new Barcode128C(viewModel.ChaveAcesso, Estilo) { Height = AlturaLinha1 - textoConsulta.Height - campoChaveAcesso.Height };

            var coluna3 = new VerticalStack();
            coluna3.Add(codigoBarras, campoChaveAcesso, textoConsulta);

            ifdNfe = new NumeroNfSerie2(estilo, ViewModel);
            idEmitente = new IdentificacaoEmitente(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = coluna3.Height }
            .ComElemento(idEmitente)
            .ComElemento(ifdNfe)
            .ComElemento(coluna3)
            .ComLarguras(0, 15, 46.5F);

            MainVerticalStack.Add(fl);

            AdicionarLinhaCampos()
                .ComCampo("Natureza da operação", ViewModel.NaturezaOperacao)
                .ComCampo("Protocolo de autorização", ViewModel.ProtocoloAutorizacao, AlinhamentoHorizontal.Centro)
                .ComLarguras(0, 46.5F);

            var linhaInscricoes = AdicionarLinhaCampos()
                .ComCampo("Inscrição Estadual", ViewModel.Emitente.Ie, AlinhamentoHorizontal.Centro);

            // NT 2026.010 (item 4.2): Código do Regime Tributário no quadro do emitente. O "Tipo de Regime de
            // Apuração do IBS e da CBS" ainda não tem tag publicada: campo reservado, não impresso (item 4.4).
            if (ViewModel.ExibirReformaTributaria)
                linhaInscricoes.ComCampo("Código do Regime Tributário", Formatador.FormatarCrt(ViewModel.Emitente.CRT), AlinhamentoHorizontal.Centro);

            linhaInscricoes
                .ComCampo("Inscrição Estadual do Subst. Tributário", ViewModel.Emitente.IeSt, AlinhamentoHorizontal.Centro)
                .ComCampo("Cnpj", Formatador.FormatarCnpj(ViewModel.Emitente.CnpjCpf), AlinhamentoHorizontal.Centro);

            // Com o CRT são 4 campos: larguras pelo tamanho do rótulo/conteúdo, para o rótulo da IE ST não invadir
            // o do CNPJ e o "2 - SIMPLES NACIONAL - EXC. SUBLIMITE" caber sem ficar abaixo da fonte mínima.
            if (ViewModel.ExibirReformaTributaria)
                linhaInscricoes.ComLarguras(18F, 30F, 28F, 0);
            else
                linhaInscricoes.ComLargurasIguais();

        }

        public XObject Logo
        {
            get => idEmitente.Logo;
            set => idEmitente.Logo = value;
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override bool VisivelSomentePrimeiraPagina => false;

        public RectangleF RetanguloNumeroFolhas => ifdNfe.RetanguloNumeroFolhas;
    }
}
