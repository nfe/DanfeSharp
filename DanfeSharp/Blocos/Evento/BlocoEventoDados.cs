

using DanfeSharp.Atributos;
using DanfeSharp.Modelo;
using System.Linq;

namespace DanfeSharp.Blocos
{
    internal class BlocoEventoDados : BlocoEventoBase
    {
        #region Constructors

        public BlocoEventoDados(DanfeEventoViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var ambiente = viewModel.TipoAmbiente == 1 ? "PRODUÇÃO" : "HOMOLOGAÇÃO - SEM VALOR FISCAL";

            AdicionarLinhaCampos()
                .ComCampo("ORGÃO", viewModel.Orgao.ToString())
                .ComCampo("AMBIENTE", ambiente, AlinhamentoHorizontal.Centro)
                .ComCampo("DATA / HORA DO EVENTO", ViewModel.DataHoraEvento.FormatarDataHora(), AlinhamentoHorizontal.Centro)
                .ComLarguras(30F * Proporcao, 0, 45F * Proporcao);

            AdicionarLinhaCampos()
                .ComCampo("EVENTO", GetDFeValue(viewModel.TipoEvento))
                .ComCampo("DESCRIÇÃO DO EVENTO", viewModel.DescricaoEvento)
                .ComCampo("SEQUÊNCIA DO EVENTO", viewModel.SequenciaEvento.ToString(), AlinhamentoHorizontal.Centro)
                .ComLarguras(30F * Proporcao, 0, 45F * Proporcao);

            AdicionarLinhaCampos()
                .ComCampo("STATUS", $"{viewModel.CodigoStatus} - {viewModel.Motivo}")
                .ComCampo("PROTOCOLO", viewModel.Protocolo)
                .ComLarguras(0, 45F * Proporcao);
        }

        #endregion

        public string GetDFeValue<T>(T value) where T : System.Enum
        {
            var member = typeof(T).GetMember(value.ToString()).FirstOrDefault();
            var enumAttribute = member?.GetCustomAttributes(false).OfType<DFeEnumAttribute>().FirstOrDefault();
            var enumValue = enumAttribute?.Value;
            return enumValue ?? value.ToString();
        }

        #region Properties

        public override string Cabecalho => ViewModel.TituloEvento;
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;

        #endregion
    }
}