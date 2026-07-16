using DanfeSharp.Blocos.NFC;
using DanfeSharp.Blocos.SimplificadoTipo2;
using DanfeSharp.Modelo;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using System;
using System.Drawing;

namespace DanfeSharp
{
    /// <summary>
    /// <para>DANFE Simplificado Tipo 2 (NT 2026.003) — documento auxiliar em formato
    /// cupom para NF-e modelo 55 emitida com tpImp=6, nas 9 divisões da NT.</para>
    /// <para>Mesmo padrão do <see cref="DanfeNFC"/>: página estreita (280 pt ≈ 98,8 mm,
    /// acima do mínimo de 56 mm), altura dinâmica e blocos empilhados. Em contingência
    /// pendente de autorização gera duas vias ("Via do Consumidor" e "Via do
    /// Estabelecimento"), conforme a Divisão VIII.</para>
    /// </summary>
    public class DanfeSimplificadoTipo2 : IDisposable
    {
        private const float Width = 280;

        private readonly StandardType1Font _FonteRegular;
        private readonly StandardType1Font _FonteNegrito;
        private readonly StandardType1Font _FonteItalico;
        private readonly StandardType1Font.FamilyEnum _FonteFamilia;
        private Boolean _FoiGerado;
        private readonly string _creditos;
        private readonly string _metadataCriador;
        private readonly SizeF _size;

        public DanfeViewModel ViewModel { get; private set; }
        public File File { get; private set; }
        internal Document PdfDocument { get; private set; }
        internal Estilo EstiloPadrao { get; private set; }

        public DanfeSimplificadoTipo2(DanfeViewModel viewModel, string creditos = null, string metadataCriador = null)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            // Divisão IV — sem URL informada pelo consumer, usa a consulta do portal
            // nacional da NF-e, válida para qualquer UF.
            if (string.IsNullOrWhiteSpace(viewModel.EndConsulta))
            {
                viewModel.EndConsulta = (viewModel.Emitente?.EnderecoUf).UrlNFeConsulta(viewModel.TipoAmbiente);
            }

            File = new File();
            PdfDocument = File.Document;

            _size = new SizeF(Width, EstimarAltura(viewModel));

            _creditos = creditos ?? "Impresso com DanfeSharp";
            _metadataCriador = metadataCriador ?? String.Format("{0} {1} - {2}", "DanfeSharp", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, "https://github.com/SilverCard/DanfeSharp");

            _FonteFamilia = StandardType1Font.FamilyEnum.Helvetica;
            _FonteRegular = new StandardType1Font(PdfDocument, _FonteFamilia, false, false);
            _FonteNegrito = new StandardType1Font(PdfDocument, _FonteFamilia, true, false);
            _FonteItalico = new StandardType1Font(PdfDocument, _FonteFamilia, false, true);

            EstiloPadrao = new Estilo(_FonteRegular, _FonteNegrito, _FonteItalico, 9, 8);

            AdicionarMetadata();

            _FoiGerado = false;
        }

        // Estima a altura da página acompanhando os incrementos reais de cada bloco,
        // com folga — o PDFClown 0.2.0-beta não permite redimensionar a página depois
        // da composição.
        private static float EstimarAltura(DanfeViewModel viewModel)
        {
            float altura = 100;                              // Divisão I — cabeçalho + folga

            if (viewModel.PendenteAutorizacao)
                altura += 60;                                // Divisão VIII — contingência, 2 locais

            if (viewModel.TipoAmbiente == 2)
                altura += 20;                                // Divisão VIII — homologação

            altura += 40 + viewModel.Produtos.Count * 20;    // Divisão II — cabeçalho + 2 linhas/item

            altura += 90;                                    // Divisão III — totais fixos + valor pago + troco
            foreach (var pagamento in viewModel.Pagamento)
                altura += (pagamento.DetalhePagamento?.Count ?? 0) * 10;

            if (viewModel.TributosIbsCbs != null)
                altura += 90;                                // Divisão III-A

            altura += 60;                                    // Divisão IV — consulta chave

            if (!string.IsNullOrWhiteSpace(viewModel.QrCode))
                altura += 160;                               // Divisão V — QR Code

            altura += 60;                                    // Divisão VI — consumidor

            altura += 60;                                    // Divisão VII — identificação

            if (!string.IsNullOrWhiteSpace(viewModel.InformacoesAdicionaisFisco))
                altura += 30;                                // Divisão VIII — infAdFisco

            if (viewModel.CalculoImposto.ValorAproximadoTributos > 0)
                altura += 40;                                // Divisão IX — Lei 12.741

            if (!string.IsNullOrWhiteSpace(viewModel.InformacoesComplementares))
                altura += 30;                                // Divisão IX — infCpl

            return altura + 30;                              // folga final
        }

        private void AdicionarMetadata()
        {
            var info = PdfDocument.Information;
            info[new org.pdfclown.objects.PdfName("ChaveAcesso")] = ViewModel.ChaveAcesso;
            info[new org.pdfclown.objects.PdfName("TipoDocumento")] = "DANFE";
            info.CreationDate = DateTime.Now;
            info.Title = "DANFE Simplificado Tipo 2 (Documento auxiliar da NFe)";
            info.Creator = _metadataCriador;
        }

        public void Gerar()
        {
            if (_FoiGerado) throw new InvalidOperationException("O Danfe já foi gerado.");

            ComporVia("Via do Consumidor");

            // Divisão VIII — em contingência pendente de autorização a NT exige a
            // impressão de segunda via identificada, guardada até a autorização.
            if (ViewModel.PendenteAutorizacao)
            {
                ComporVia("Via do Estabelecimento");
            }

            _FoiGerado = true;
        }

        private void ComporVia(string rotuloVia)
        {
            var page = new Page(PdfDocument, _size);
            PdfDocument.Pages.Add(page);

            var composer = new PrimitiveComposer(page);

            // Divisão I
            var cabecalho = new BlocoCabecalhoSimplificadoT2(ViewModel, EstiloPadrao, composer);

            // Divisão VIII — contingência (1ª ocorrência, abaixo do cabeçalho)
            var contingenciaTopo = new BlocoMensagemContingenciaT2(ViewModel, EstiloPadrao, composer, cabecalho.Y_NFC);

            // Divisão VIII — homologação (bloco NFC reusado: condiciona por TipoAmbiente == 2)
            var informacaoFiscal = new BlocoInformacaoFiscal(ViewModel, EstiloPadrao, composer, contingenciaTopo.Y_NFC);

            // Divisão II
            var tabelaProdutos = new TabelaProdutosSimplificadoT2(ViewModel, EstiloPadrao, composer, informacaoFiscal.Y_NFC);

            // Divisão III
            var totais = new BlocoTotaisSimplificadoT2(ViewModel, EstiloPadrao, composer, tabelaProdutos.Y_NFC);

            // Divisão III-A
            var tributosIbsCbs = new BlocoTributosIbsCbsT2(ViewModel, EstiloPadrao, composer, totais.Y_NFC);

            // Divisão IV
            var consultaChave = new BlocoConsultaChaveT2(ViewModel, EstiloPadrao, composer, tributosIbsCbs.Y_NFC);

            // Divisão V
            var qrCode = new BlocoQrCodeT2(ViewModel, EstiloPadrao, composer, consultaChave.Y_NFC, PdfDocument);

            // Divisão VI
            var consumidor = new BlocoConsumidorT2(ViewModel, EstiloPadrao, composer, qrCode.Y_NFC);

            // Divisão VII
            var identificacao = new BlocoIdentificacaoT2(ViewModel, EstiloPadrao, composer, consumidor.Y_NFC, rotuloVia);

            // Divisão VIII — contingência (2ª ocorrência, abaixo da identificação)
            var contingenciaBase = new BlocoMensagemContingenciaT2(ViewModel, EstiloPadrao, composer, identificacao.Y_NFC);

            // Divisões VIII/IX — mensagens fiscal e do contribuinte
            var mensagens = new BlocoMensagensT2(ViewModel, EstiloPadrao, composer, contingenciaBase.Y_NFC);

            composer.Flush();
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
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    File.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
