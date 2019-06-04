using DanfeSharp.Blocos;
using DanfeSharp.Blocos.NFC;
using DanfeSharp.Modelo;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DanfeSharp
{
    public class DanfeNFC : IDisposable
    {
        private readonly StandardType1Font _FonteRegular;
        private readonly StandardType1Font _FonteNegrito;
        private readonly StandardType1Font _FonteItalico;
        private readonly StandardType1Font.FamilyEnum _FonteFamilia;
        private Boolean _FoiGerado;
        private readonly string _creditos;
        private readonly string _metadataCriador;
        private readonly SizeF _size;
        private Page _page;
        private PrimitiveComposer _primitiveComposer;
        public DanfeViewModel ViewModel { get; private set; }
        public File File { get; private set; }
        internal Document PdfDocument { get; private set; }
        internal BlocoIdentificacaoEmitenteNFC IdentificacaoEmitente { get; private set; }
        internal List<BlocoBase> _Blocos;
        internal Estilo EstiloPadrao { get; private set; }

        private org.pdfclown.documents.contents.xObjects.XObject _LogoObject = null;

        public DanfeNFC(DanfeViewModel viewModel, string creditos = null, string metadataCriador = null)
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

            _creditos = creditos ?? "Impresso com DanfeSharp";
            _metadataCriador = metadataCriador ?? String.Format("{0} {1} - {2}", "DanfeSharp", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, "https://github.com/SilverCard/DanfeSharp");

            // De acordo com o item 7.7, a fonte deve ser Times New Roman ou Courier New.
            _FonteFamilia = StandardType1Font.FamilyEnum.Times;
            _FonteRegular = new StandardType1Font(PdfDocument, _FonteFamilia, false, false);
            _FonteNegrito = new StandardType1Font(PdfDocument, _FonteFamilia, true, false);
            _FonteItalico = new StandardType1Font(PdfDocument, _FonteFamilia, false, true);

            EstiloPadrao = CriarEstilo();

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

        public void AdicionarLogoImagem(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                AdicionarLogoImagem(fs);
            }
        }

        public void AdicionarLogoPdf(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

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
            info.Creator = _metadataCriador;
        }

        private Estilo CriarEstilo(float tFonteCampoCabecalho = 10, float tFonteCampoConteudo = 9)
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

        public void Salvar(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            File.Save(path, SerializationModeEnum.Incremental);
        }

        public void Salvar(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            File.Save(new org.pdfclown.bytes.Stream(stream), SerializationModeEnum.Incremental);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    File.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Danfe() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}