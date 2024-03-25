using DanfeNet.Elementos;
using DanfeNet.Models;

namespace DanfeNet.Blocos.Evento;

internal class BlocoEventoCancelamento : BlocoEventoBase
{
    #region Constructors

    public BlocoEventoCancelamento(DanfeEventoViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
    {
        var campoJustificativa = new CampoMultilinha("", viewModel.Justificativa, estilo, AlinhamentoHorizontal.Esquerda, true) { Height = AlturaCampo };
        var linha = new FlexibleLine { Height = AlturaCampo }
            .ComElemento(campoJustificativa)
            .ComLargurasIguais();

        MainVerticalStack.Add(linha);
    }

    #endregion

    #region Properties

    public const float AlturaCampo = Constantes.A4Altura - 99;
    public override string Cabecalho => "JUSTIFICATIVA";
    public override PosicaoBloco Posicao => PosicaoBloco.Topo;

    #endregion
}