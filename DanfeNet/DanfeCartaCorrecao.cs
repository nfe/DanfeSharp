using System;
using System.Collections.Generic;
using System.IO;
using DanfeNet.Blocos.Evento;
using DanfeNet.Elementos;
using DanfeNet.Esquemas;
using DanfeNet.Models;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using org.pdfclown.objects;
using File = org.pdfclown.files.File;

namespace DanfeNet;

public class DanfeCartaCorrecao : IDisposable
{
    #region Fields

    public File File { get; private set; }
    private bool _FoiGerado;

    private readonly StandardType1Font _fonteRegular;
    private readonly StandardType1Font _fonteNegrito;
    private readonly StandardType1Font _fonteItalico;

    private readonly string _creditos;
    private readonly string _metadataCriador;
    #endregion

    #region Constructors

    public DanfeCartaCorrecao(DanfeEvento viewModel, string creditos = null, string metadataCriador = null)
    {

        _creditos = creditos ?? "Impresso com DanfeNet";
        _metadataCriador = metadataCriador ?? string.Format("{0} {1} - {2}", "DanfeNet", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, "https://github.com/SilverCard/DanfeNet");


        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        File = new File();
        Blocos = new List<BlocoEventoBase>();
        PdfDocument = File.Document;

        // De acordo com o item 7.7, a fonte deve ser Times New Roman ou Courier New.
        var fonteFamilia = StandardType1Font.FamilyEnum.Times;
        _fonteRegular = new StandardType1Font(PdfDocument, fonteFamilia, false, false);
        _fonteNegrito = new StandardType1Font(PdfDocument, fonteFamilia, true, false);
        _fonteItalico = new StandardType1Font(PdfDocument, fonteFamilia, false, true);

        EstiloPadrao = CriarEstilo();
        EstiloPadrao.FonteBlocoCabecalho.Tamanho = 10;

        var estiloInformacao = CriarEstilo(tFonteCampoConteudo: 12f);
        estiloInformacao.FonteBlocoCabecalho.Tamanho = 10;

        Paginas = new List<DanfeEventoPagina>();

        AdicionarBloco<BlocoEventoCabecalho>();
        AdicionarBloco<BlocoEventoIdentificacao>();
        AdicionarBloco<BlocoEventoDados>();

        switch (viewModel.TipoEvento)
        {
            case NFeTipoEvento.CartaCorrecao:
                AdicionarBloco<BlocoEventoCartaCorrecaoCondicao>();
                AdicionarBloco<BlocoEventoCartaCorrecao>(estiloInformacao);
                break;
            case NFeTipoEvento.Cancelamento:
            case NFeTipoEvento.CancelamentoST:
                AdicionarBloco<BlocoEventoCancelamento>(estiloInformacao);
                break;
        }

        AdicionarMetadata();
        _FoiGerado = false;
    }

    #endregion

    #region Properties

    internal org.pdfclown.documents.Document PdfDocument { get; private set; }
    internal List<BlocoEventoBase> Blocos { get; private set; }
    internal Estilo EstiloPadrao { get; private set; }
    internal List<DanfeEventoPagina> Paginas { get; private set; }

    public DanfeEvento ViewModel { get; private set; }

    #endregion

    #region Methods

    public void Gerar()
    {
        if (_FoiGerado) throw new InvalidOperationException("O Danfe já foi gerado.");

        var page = new DanfeEventoPagina(this);
        Paginas.Add(page);
        page.DesenharBlocos();
        page.DesenharCreditos(_creditos);

        if (ViewModel.TipoAmbiente == 2)
            page.DesenharAvisoHomologacao();

        page.Gfx.Stroke();
        page.Gfx.Flush();

        _FoiGerado = true;
    }

    public void Salvar(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));
        File.Save(path, SerializationModeEnum.Incremental);
    }

    public void Salvar(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        File.Save(new org.pdfclown.bytes.Stream(stream), SerializationModeEnum.Incremental);
    }

    public byte[] ObterPdfBytes(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var pdfStrean = new org.pdfclown.bytes.Stream(stream);
        File.Save(pdfStrean, SerializationModeEnum.Incremental);
        return pdfStrean.ToByteArray();
    }

    #endregion

    #region Private Methods

    private void AdicionarMetadata()
    {
        var info = PdfDocument.Information;
        info[new PdfName("ChaveAcesso")] = ViewModel.ChaveAcesso;
        info[new PdfName("TipoDocumento")] = "DANFE Evento";
        info.CreationDate = DateTime.Now;
        info.Title = "DANFE Evento (Documento auxiliar do evento da NFe)";
        info.Creator = _metadataCriador;
    }

    private Estilo CriarEstilo(float tFonteCampoCabecalho = 6, float tFonteCampoConteudo = 10) => new Estilo(_fonteRegular, _fonteNegrito, _fonteItalico, tFonteCampoCabecalho, tFonteCampoConteudo);

    #endregion

    #region Internal Methods

    internal T CriarBloco<T>() where T : BlocoEventoBase => (T)Activator.CreateInstance(typeof(T), ViewModel, EstiloPadrao);

    internal T CriarBloco<T>(Estilo estilo) where T : BlocoEventoBase => (T)Activator.CreateInstance(typeof(T), ViewModel, estilo);

    internal T AdicionarBloco<T>() where T : BlocoEventoBase
    {
        var bloco = CriarBloco<T>();
        Blocos.Add(bloco);
        return bloco;
    }

    internal T AdicionarBloco<T>(Estilo estilo) where T : BlocoEventoBase
    {
        var bloco = CriarBloco<T>(estilo);
        Blocos.Add(bloco);
        return bloco;
    }

    internal void AdicionarBloco(BlocoEventoBase bloco)
    {
        Blocos.Add(bloco);
    }

    #endregion

    #region IDisposable Support

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing) File.Dispose();
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}