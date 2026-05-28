using System.Linq;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos 
{
    internal class BlocoDuplicataFatura : BlocoBase 
    {
        public BlocoDuplicataFatura(DanfeSharp.Modelo.DanfeViewModel viewModel, Estilo estilo) 
            : base (viewModel, estilo) 
        {
            var de = viewModel.Duplicatas.Select(x => new Duplicata (estilo, x)).ToList();
            var eh = de.First().Height;

            // Reduzido de 6→3 (retrato) e 7→4 (paisagem) para acomodar o
            // layout horizontal dos 3 campos (Número/Vencimento/Valor) dentro
            // de cada Duplicata — sem isso os valores ficariam cortados em
            // colunas estreitas. Ver Duplicata.cs:Draw.
            int numeroElementosLinha = ViewModel.IsPaisagem ? 4 : 3;
            int i = 0;

            while (i < de.Count) 
            {
                FlexibleLine fl = new FlexibleLine (Width, eh);

                int i2;
                for (i2 = 0; i2 < numeroElementosLinha && i < de.Count; i2++, i++) 
                {
                    fl.ComElemento (de[i]);
                }

                // Preenche o resto da linha com celulas vazias (apenas borda)
                // para que o quadro va ate a margem direita mesmo quando o
                // numero de duplicatas nao e multiplo de numeroElementosLinha
                // — antes ficava espaco em branco sem borda, truncando o
                // quadro visualmente.
                for (; i2 < numeroElementosLinha; i2++)
                    fl.ComElemento (new CelulaVazia (estilo));

                fl.ComLargurasIguais ();
                MainVerticalStack.Add (fl);
            }
        }

        public override string Cabecalho => "Fatura / Duplicata";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
    }
}