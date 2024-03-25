using System;
using DanfeNet.Elementos;
using DanfeNet.Graphics;
using DanfeNet.Models;

namespace DanfeNet.Blocos;

/// <summary>
/// Define um bloco básico do DANFE.
/// </summary>
internal abstract class BlocoBase : ElementoBase
{
    /// <summary>
    /// Constante de proporção dos campos para o formato retrato A4, porcentagem dividida pela largura desenhável.
    /// </summary>
    public const float Proporcao = 100F / 200F;

    public Danfe ViewModel { get; private set; }

    public abstract PosicaoBloco Posicao { get; }

    /// <summary>
    /// Pilha principal.
    /// </summary>
    public VerticalStack MainVerticalStack { get; private set; }

    /// <summary>
    /// Quando verdadeiro, o bloco é mostrado apenas na primeira página, caso contário é mostrado em todas elas.
    /// </summary>
    public virtual bool VisivelSomentePrimeiraPagina => true;

    public virtual string Cabecalho => null;

    public BlocoBase(Danfe viewModel, Estilo estilo) : base(estilo)
    {
        MainVerticalStack = new VerticalStack();
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        if (!string.IsNullOrWhiteSpace(Cabecalho))
        {
            MainVerticalStack.Add(new CabecalhoBloco(estilo, Cabecalho));
        }
    }

    public LinhaCampos AdicionarLinhaCampos()
    {
        var l = new LinhaCampos(Estilo, Width);
        l.Width = Width;
        l.Height = Constantes.CampoAltura;
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

    public override float Height { get => MainVerticalStack.Height; set => throw new NotSupportedException(); }
    public override bool PossuiContono => false;            
}