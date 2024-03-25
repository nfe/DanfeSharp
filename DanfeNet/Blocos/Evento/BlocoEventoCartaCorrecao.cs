using DanfeNet.Elementos;
using DanfeNet.Modelo;

namespace DanfeNet.Blocos.Evento
{
    internal class BlocoEventoCartaCorrecao : BlocoEventoBase
    {
        #region Constructors

        public BlocoEventoCartaCorrecao(DanfeEventoViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var correcao = new CampoMultilinha("", viewModel.Correcao, estilo, AlinhamentoHorizontal.Esquerda, true) {Height = AlturaCorrecao};
            var linha = new FlexibleLine {Height = AlturaCorrecao};
            linha.ComElemento(correcao).ComLargurasIguais();

            MainVerticalStack.Add(linha);
        }

        #endregion

        #region Properties

        public const float AlturaCorrecao = Constantes.A4Altura - 132;

        public override string Cabecalho => "CORREÇÃO A SER CONSIDERADA";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;

        #endregion
    }
}