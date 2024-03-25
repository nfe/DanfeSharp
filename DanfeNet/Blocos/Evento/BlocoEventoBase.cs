using System;
using DanfeNet.Elementos;
using DanfeNet.Graphics;
using DanfeNet.Modelo;

namespace DanfeNet.Blocos.Evento
{
    /// <summary>
    ///     Define um bloco básico do DANFE Evento.
    /// </summary>
    internal abstract class BlocoEventoBase : ElementoBase
    {
        #region Constructors

        protected BlocoEventoBase(DanfeEventoViewModel viewModel, Estilo estilo) : base(estilo)
        {
            MainVerticalStack = new VerticalStack();
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            if (!string.IsNullOrWhiteSpace(Cabecalho)) MainVerticalStack.Add(new CabecalhoBloco(estilo, Cabecalho));
        }

        #endregion

        #region Properties

        public DanfeEventoViewModel ViewModel { get; private set; }

        public abstract PosicaoBloco Posicao { get; }

        /// <summary>
        ///     Constante de proporção dos campos para o formato retrato A4, porcentagem dividida pela largura desenhável.
        /// </summary>
        public const float Proporcao = 100F / 200F;

        /// <summary>
        ///     Pilha principal.
        /// </summary>
        public VerticalStack MainVerticalStack { get; private set; }

        public virtual string Cabecalho => null;

        public override float Height
        {
            get => MainVerticalStack.Height;
            set => throw new NotSupportedException();
        }
        
        public override bool PossuiContono => false;

        #endregion

        #region Methods

        public LinhaCampos AdicionarLinhaCampos()
        {
            var l = new LinhaCampos(Estilo, Width)
            {
                Width = Width,
                Height = Constantes.CampoAltura
            };
            MainVerticalStack.Add(l);
            return l;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);
            MainVerticalStack.SetPosition(X, Y);
            MainVerticalStack.Width = Width;
            MainVerticalStack.Draw(gfx);
        }

        #endregion
    }
}