using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    internal class BlocoEventoCabecalho : BlocoEventoBase
    {
        #region Constructors

        public BlocoEventoCabecalho(DanfeEventoViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var ts = new TextStack(BoundingBox) {Height = 30, AlinhamentoVertical = AlinhamentoVertical.Centro, AlinhamentoHorizontal = AlinhamentoHorizontal.Centro, LineHeightScale = 2F}
                .AddLine(viewModel.TituloEvento, Estilo.CriarFonteNegrito(15))
                .AddLine("Não possui valor fiscal. Simples representação do evento indicado abaixo.", Estilo.FonteCampoConteudo)
                .AddLine("CONSULTE A AUTENTICIDADE NO SITE DA SEFAZ AUTORIZADORA.", Estilo.FonteCampoConteudo);
            MainVerticalStack.Add(ts);
        }

        #endregion

        #region Properties

        public override bool PossuiContono => true;
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;

        #endregion
    }
}