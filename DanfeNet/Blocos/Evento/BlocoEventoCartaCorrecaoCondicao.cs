using DanfeNet.Elementos;
using DanfeNet.Modelo;

namespace DanfeNet.Blocos.Evento
{
    internal class BlocoEventoCartaCorrecaoCondicao : BlocoEventoBase
    {
        #region Constructors

        public BlocoEventoCartaCorrecaoCondicao(DanfeEventoViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var condicao = new CampoMultilinha("", viewModel.CondicaoUso, estilo) {Height = AlturaCondicao};
            var linha = new FlexibleLine {Height = AlturaCondicao};
            linha.ComElemento(condicao).ComLargurasIguais();

            MainVerticalStack.Add(linha);
        }

        #endregion

        #region Properties

        public const float AlturaCondicao = 25;

        public override string Cabecalho => "Condição de Uso";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;

        #endregion
    }
}