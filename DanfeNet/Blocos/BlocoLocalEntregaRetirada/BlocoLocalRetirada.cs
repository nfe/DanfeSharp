using DanfeNet.Elementos;
using DanfeNet.Models;

namespace DanfeNet.Blocos.BlocoLocalEntregaRetirada;

class BlocoLocalRetirada : BlocoLocalEntregaRetirada
{
    public BlocoLocalRetirada(Danfe viewModel, Estilo estilo) 
        : base(viewModel, estilo, viewModel.LocalRetiradaInfo)
    {
    }

    public override string Cabecalho => "Informações do local de retirada";
}