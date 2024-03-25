using System;
using System.Collections.Generic;
using DanfeNet.Blocos;
using DanfeNet.Blocos.BlocoLocalEntregaRetirada;
using DanfeNet.Blocos.Evento;
using DanfeNet.Elementos;
using DanfeNet.Models;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;

namespace DanfeNet;

public class DanfePdf : DanfePdfBase
{
    private readonly StandardType1Font _FonteRegular;
    private readonly StandardType1Font _FonteNegrito;
    private readonly StandardType1Font _FonteItalico;
    private readonly StandardType1Font.FamilyEnum _FonteFamilia;

    private bool _FoiGerado;
        
    private org.pdfclown.documents.contents.xObjects.XObject _LogoObject;

    internal BlocoCanhoto Canhoto { get; private set; }
    internal BlocoIdentificacaoEmitente IdentificacaoEmitente { get;  }
    internal List<BlocoEventoBase> Blocos { get; private set; }

    internal List<BlocoBase> _Blocos;
    internal Estilo EstiloPadrao { get; private set; }

    internal List<DanfePagina> Paginas { get; private set; }

        
    public Danfe ViewModel { get; }
        
        
    public DanfePdf(Danfe viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        _Blocos = new List<BlocoBase>();
        File = new File();
        PdfDocument = File.Document;

        // De acordo com o item 7.7, a fonte deve ser Times New Roman ou Courier New.
        _FonteFamilia = StandardType1Font.FamilyEnum.Times;
        _FonteRegular = new StandardType1Font(PdfDocument, _FonteFamilia, false, false);
        _FonteNegrito = new StandardType1Font(PdfDocument, _FonteFamilia, true, false);
        _FonteItalico = new StandardType1Font(PdfDocument, _FonteFamilia, false, true);

        EstiloPadrao = CriarEstilo();

        Paginas = new List<DanfePagina>();
        Canhoto = CriarBloco<BlocoCanhoto>();
        IdentificacaoEmitente = AdicionarBloco<BlocoIdentificacaoEmitente>();
        AdicionarBloco<BlocoDestinatarioRemetente>();

        if (ViewModel.LocalRetiradaInfo != null && ViewModel.ExibirBlocoLocalRetirada)
            AdicionarBloco<BlocoLocalRetirada>();

        if (ViewModel.LocalEntrega != null && ViewModel.ExibirBlocoLocalEntrega)
            AdicionarBloco<BlocoLocalEntrega>();

        if (ViewModel.Duplicatas.Count > 0)
            AdicionarBloco<BlocoDuplicataFatura>();

        AdicionarBloco<BlocoCalculoImposto>(ViewModel.Orientacao == Orientacao.Paisagem ? EstiloPadrao : CriarEstilo(4.75F));
        AdicionarBloco<BlocoTransportador>();
        AdicionarBloco<BlocoDadosAdicionais>(CriarEstilo(tFonteCampoConteudo: 8));

        if (ViewModel.CalculoIssqn.Mostrar)
            AdicionarBloco<BlocoCalculoIssqn>();

        AdicionarMetadata();

        _FoiGerado = false;
    }

    public void AdicionarLogoImagem(System.IO.Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var img = org.pdfclown.documents.contents.entities.Image.Get(stream);
        if (img == null) throw new InvalidOperationException("O logotipo não pode ser carregado, certifique-se que a imagem esteja no formato JPEG não progressivo.");
        _LogoObject = img.ToXObject(PdfDocument);
    }

    public void AdicionarLogoPdf(System.IO.Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        using (var pdfFile = new org.pdfclown.files.File(new org.pdfclown.bytes.Stream(stream)))
        {
            _LogoObject = pdfFile.Document.Pages[0].ToXObject(PdfDocument);
        }
    }

    public void AdicionarLogoImagem(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

        using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            AdicionarLogoImagem(fs);
        }
    }

    public void AdicionarLogoPdf(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

        using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            AdicionarLogoPdf(fs);
        }
    }

    private void AdicionarMetadata()
    {
        var info = PdfDocument.Information;
        info[new org.pdfclown.objects.PdfName("ChaveAcesso")] = ViewModel.ChaveAcesso;
        info[new org.pdfclown.objects.PdfName("TipoDocumento")] = "DANFE";
        info.CreationDate = DateTime.Now;
        info.Title = "DANFE (Documento auxiliar da NFe)";
        info.Creator = MetadataInfo;
    }

    private Estilo CriarEstilo(float tFonteCampoCabecalho = 6, float tFonteCampoConteudo = 10)
    {
        return new Estilo(_FonteRegular, _FonteNegrito, _FonteItalico, tFonteCampoCabecalho, tFonteCampoConteudo);
    }

    
    public override void Generate()
    {
        if (_FoiGerado) 
            throw new InvalidOperationException("O Danfe já foi gerado.");

        IdentificacaoEmitente.Logo = _LogoObject;
        var tabela = new TabelaProdutosServicos(ViewModel, EstiloPadrao);

        while (true)
        {
            DanfePagina p = CriarPagina();

            tabela.SetPosition(p.RetanguloCorpo.Location);
            tabela.SetSize(p.RetanguloCorpo.Size);
            tabela.Draw(p.Gfx);

            p.Gfx.Stroke();
            p.Gfx.Flush();

            if (tabela.CompletamenteDesenhada)
                break;
        }

        PreencherNumeroFolhas();
        _FoiGerado = true;
    }

    private DanfePagina CriarPagina()
    {
        DanfePagina p = new DanfePagina(this);

        Paginas.Add(p);

        p.DesenharBlocos(Paginas.Count == 1);

        if (string.IsNullOrWhiteSpace(CreditsInfo) == false)
        {
            p.DesenharCreditos(CreditsInfo);
        }

        // Ambiente de homologação
        // 7. O DANFE emitido para representar NF-e cujo uso foi autorizado em ambiente de
        // homologação sempre deverá conter a frase “SEM VALOR FISCAL” no quadro “Informações
        // Complementares” ou em marca d’água destacada.
        if (ViewModel.TipoAmbiente == 2)
        {
            p.DesenharAvisoHomologacao();
        }

        // NF-e cancelada
        if (ViewModel.CodigoStatusReposta == 101)
            p.DesenharAvisoCancelamento();
        
        return p;
    }

    internal T CriarBloco<T>() where T : BlocoBase
    {
        return (T)Activator.CreateInstance(typeof(T), ViewModel, EstiloPadrao);
    }

    internal T CriarBloco<T>(Estilo estilo) where T : BlocoBase
    {
        return (T)Activator.CreateInstance(typeof(T), ViewModel, estilo);
    }

    internal T AdicionarBloco<T>() where T : BlocoBase
    {
        var bloco = CriarBloco<T>();
        _Blocos.Add(bloco);
        return bloco;
    }

    internal T AdicionarBloco<T>(Estilo estilo) where T : BlocoBase
    {
        var bloco = CriarBloco<T>(estilo);
        _Blocos.Add(bloco);
        return bloco;
    }

    internal void AdicionarBloco(BlocoBase bloco)
    {
        _Blocos.Add(bloco);
    }

    internal void PreencherNumeroFolhas()
    {
        int nFolhas = Paginas.Count;
        for (int i = 0; i < Paginas.Count; i++)
        {
            Paginas[i].DesenhaNumeroPaginas(i + 1, nFolhas);
        }
    }



}