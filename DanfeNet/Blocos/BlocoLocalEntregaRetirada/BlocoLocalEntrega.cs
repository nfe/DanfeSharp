using DanfeNet.Elementos;
using DanfeNet.Models;

namespace DanfeNet.Blocos.BlocoLocalEntregaRetirada;

class BlocoLocalEntrega : BlocoLocalEntregaRetirada
{
    public BlocoLocalEntrega(DanfeViewModel viewModel, Estilo estilo) 
        : base(viewModel, estilo, viewModel.LocalEntrega)
    {
    }

    public override string Cabecalho => "Informações do local de entrega";
}