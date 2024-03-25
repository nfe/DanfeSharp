using System;
using System.Collections.Generic;
using System.Drawing;
using DanfeNet.Blocos;
using DanfeNet.Blocos.NFC;
using DanfeNet.Elementos;
using DanfeNet.Models;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;

namespace DanfeNet;

public class DanfeNfcPdf : DanfePdfBase
{
    private readonly StandardType1Font _FonteRegular;
    private readonly StandardType1Font _FonteNegrito;
    private readonly StandardType1Font _FonteItalico;
    private readonly StandardType1Font.FamilyEnum _FonteFamilia;
    private bool _FoiGerado;
    private readonly SizeF _size;
    private Page _page;
    private PrimitiveComposer _primitiveComposer;
    public Danfe ViewModel { get; private set; }
    
    internal List<BlocoBase> _Blocos;
    internal Estilo EstiloPadrao { get; private set; }


    public DanfeNfcPdf(Danfe viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        File = new File();
        PdfDocument = File.Document;

        if (viewModel.Produtos.Count <= 20)
            _size = new SizeF(280, viewModel.Produtos.Count * 30 + 600);
        else if (viewModel.Produtos.Count <= 40)
            _size = new SizeF(280, viewModel.Produtos.Count * 14 + 600);
        else if (viewModel.Produtos.Count <= 75)
            _size = new SizeF(280, viewModel.Produtos.Count * 12 + 600);
        else if (viewModel.Produtos.Count <= 150)
            _size = new SizeF(280, viewModel.Produtos.Count * 11 + 600);
        else if (viewModel.Produtos.Count <= 250)
            _size = new SizeF(280, viewModel.Produtos.Count * 10.5F + 600);
        else if (viewModel.Produtos.Count <= 400)
            _size = new SizeF(280, viewModel.Produtos.Count * 10.2F + 600);
        else if (viewModel.Produtos.Count <= 480)
            _size = new SizeF(280, viewModel.Produtos.Count * 10.1F + 600);
        else if (viewModel.Produtos.Count <= 700)
            _size = new SizeF(280, viewModel.Produtos.Count * 9.8F + 600);

        // 1. Add the page to the document!
        _page = new Page(PdfDocument, _size); // Instantiates the page inside the document context.
        PdfDocument.Pages.Add(_page); // Puts the page in the pages collection.

        // 2. Create a content composer for the page!
        _primitiveComposer = new PrimitiveComposer(_page);

        // De acordo com o item 7.7, a fonte deve ser Times New Roman ou Courier New.
        _FonteFamilia = StandardType1Font.FamilyEnum.Helvetica;
        _FonteRegular = new StandardType1Font(PdfDocument, _FonteFamilia, false, false);
        _FonteNegrito = new StandardType1Font(PdfDocument, _FonteFamilia, true, false);
        _FonteItalico = new StandardType1Font(PdfDocument, _FonteFamilia, false, true);

        EstiloPadrao = CriarEstilo();

        AdicionarMetadata();

        _FoiGerado = false;
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

    private Estilo CriarEstilo(float tFonteCampoCabecalho = 9, float tFonteCampoConteudo = 8)
    {
        return new Estilo(_FonteRegular, _FonteNegrito, _FonteItalico, tFonteCampoCabecalho, tFonteCampoConteudo);
    }

    public void Gerar()
    {
        if (_FoiGerado) throw new InvalidOperationException("O Danfe já foi gerado.");

        var identificacaoEmitente = new BlocoIdentificacaoEmitenteNFC(ViewModel, EstiloPadrao, _primitiveComposer);

        var tableProdutos = new TabelaProdutosServicosNFC(ViewModel, EstiloPadrao, _primitiveComposer, identificacaoEmitente.Y_NFC);

        var produtoServicoTotal = new BlocoProdutoServicoTotalNFC(ViewModel, EstiloPadrao, _primitiveComposer, tableProdutos.Y_NFC);

        var formaPagamento = new BlocoFormaPagamentoNFC(ViewModel, EstiloPadrao, _primitiveComposer, produtoServicoTotal.Y_NFC);

        var informacaoFiscal = new BlocoInformacaoFiscal(ViewModel, EstiloPadrao, _primitiveComposer, formaPagamento.Y_NFC);

        var consultaChave = new BlocoConsultaChaveNFC(ViewModel, EstiloPadrao, _primitiveComposer, informacaoFiscal.Y_NFC);

        var destinatarioRemetente = new BlocoDestinatarioRemetenteNFC(ViewModel, EstiloPadrao, _primitiveComposer, consultaChave.Y_NFC);

        var identificacaoNFC = new BlocoIdentificacaoNFC(ViewModel, EstiloPadrao, _primitiveComposer, destinatarioRemetente.Y_NFC);

        var qrCodeNFC = new BlocoQrCodeNFC(ViewModel, EstiloPadrao, _primitiveComposer, identificacaoNFC.Y_NFC, PdfDocument);

        // 4. Flush the contents into the page!
        _primitiveComposer.Flush();

        _FoiGerado = true;
    }

}