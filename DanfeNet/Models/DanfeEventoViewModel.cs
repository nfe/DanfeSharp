using System;
using DanfeNet.Esquemas;

namespace DanfeNet.Models;

public class DanfeEventoViewModel
{
    #region Fields

    private float _Margem;

    #endregion

    #region Constructors

    public DanfeEventoViewModel()
    {
        Margem = 4;
        Orientacao = Orientacao.Retrato;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Magens horizontais e verticais do DANFE.
    /// </summary>
    public float Margem
    {
        get => _Margem;
        set
        {
            if (value >= 2 && value <= 5)
                _Margem = value;
            else
                throw new ArgumentOutOfRangeException("A margem deve ser entre 2 e 5.");
        }
    }

    public Orientacao Orientacao { get; set; }

    /// <summary>
    ///     <para>Chave de Acesso da NF-e vinculada ao evento</para>
    ///     <para>tag chNFe</para>
    /// </summary>
    public string ChaveAcesso { get; set; }

    /// <summary>
    ///     <para>Orgão Autorizador</para>
    ///     <para>tag cOrgao</para>
    /// </summary>
    public int Orgao { get; set; }

    /// <summary>
    ///     <para>Identificação do Ambiente</para>
    ///     <para>tag tpAmb</para>
    /// </summary>
    public int TipoAmbiente { get; set; }

    /// <summary>
    ///     <para>Data e Hora de emissão do Evento</para>
    ///     <para>tag dhRegEvento </para>
    /// </summary>
    public DateTime? DataHoraEvento { get; set; }

    /// <summary>
    ///     <para>Código do tipo do Evento</para>
    ///     <para>tag tpEvento</para>
    /// </summary>
    public NFeTipoEvento TipoEvento { get; set; }

    /// <summary>
    ///     <para>Descrição do Evento</para>
    ///     <para>tag xEvento</para>
    /// </summary>
    public string DescricaoEvento { get; set; }

    /// <summary>
    ///     <para>Sequencial do evento, conforme informado na mensagem de entrada</para>
    ///     <para>tag nSeqEvento</para>
    /// </summary>
    public int SequenciaEvento { get; set; }

    /// <summary>
    ///     Status do Evento
    ///     <para>tag cStat</para>
    /// </summary>
    public string CodigoStatus { get; set; }

    /// <summary>
    ///     Descrição do Status da resposta
    ///     <para>tag xMotivo</para>
    /// </summary>
    public string Motivo { get; set; }

    /// <summary>
    ///     Numero do protocolo do cancelamento
    ///     <para>tag nProt</para>
    /// </summary>
    public string Protocolo { get; set; }

    /// <summary>
    ///     Justificativa, utilizada em Cancelamento
    ///     <para>tag xJust</para>
    /// </summary>
    public string Justificativa { get; set; }

    /// <summary>
    ///     Descrição da Correção, utilizada na Carta Correção
    ///     <para>tag xCorrecao</para>
    /// </summary>
    public string Correcao { get; set; }

    /// <summary>
    ///     Condição de uso, utilizada na Carta Correção
    ///     <para>tag xCondUso</para>
    /// </summary>
    public string CondicaoUso { get; set; }

    /// <summary>
    ///     Titulo do Evento utilizado na impressão
    /// </summary>
    public string TituloEvento
    {
        get
        {
            switch (TipoEvento)
            {
                case NFeTipoEvento.CartaCorrecao:
                    return "CARTA DE CORREÇÃO ELETRÔNICA";
                case NFeTipoEvento.Cancelamento:
                case NFeTipoEvento.CancelamentoST:
                    return "CANCELAMENTO DE NF-E";
                case NFeTipoEvento.EPEC:
                    return "EVENTO EPEC DE NF-E";
                default:
                    return "EVENTO DE NF-E";
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Substitui o ponto e vírgula (;) por uma quebra de linha.
    /// </summary>
    private string BreakLines(string str) => str == null ? string.Empty : str.Replace(';', '\n');

    public void DefinirTextoCreditos(string textoCreditos)
    {
        //DanfeConstantes.TextoCreditos = textoCreditos;
    }

    #endregion
}