namespace DanfeSharp
{
    /// <summary>
    /// Célula vazia (apenas borda, sem conteúdo) usada para preencher
    /// linhas incompletas em blocos tabulares como
    /// <see cref="Blocos.BlocoDuplicataFatura"/> e
    /// <see cref="Blocos.BlocoFormaPagamento"/>: quando o número de itens
    /// não fecha a linha (1 duplicata num bloco que cabe 3, p. ex.), as
    /// posições restantes ficavam visualmente vazias e o quadro parecia
    /// truncado. <c>CelulaVazia</c> mantém a continuidade do retângulo até
    /// a margem direita herdando o desenho da borda de
    /// <see cref="ElementoBase.Draw"/>.
    /// </summary>
    internal class CelulaVazia : ElementoBase
    {
        public CelulaVazia(Estilo estilo) : base(estilo) { }
    }
}
