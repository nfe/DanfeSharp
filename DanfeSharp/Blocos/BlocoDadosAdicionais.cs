using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using DanfeSharp;
using System;

namespace DanfeSharp.Blocos
{
    internal class BlocoDadosAdicionais : BlocoBase
    {
        public const float AlturaMinima = 25;
        private CampoMultilinha _cInfComplementares;
        private FlexibleLine _Linha;
        private Campo _cReservadoFisco;
        public const float InfComplementaresLarguraPorcentagem = 75;

        /// <summary>Altura mínima com o QR Code, para ele sair com pelo menos 25 mm de lado.</summary>
        public const float AlturaMinimaComQrCode = 30;

        private readonly QuadroQrCode _cQrCode;

        public BlocoDadosAdicionais(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            _cInfComplementares = new CampoMultilinha("Informações Complementares", ViewModel.TextoAdicional(), estilo);
            _cReservadoFisco = new CampoMultilinha("Reservado ao fisco", ViewModel.TextoReservadoFisco(), estilo);

            _Linha = new FlexibleLine() { Height = _cInfComplementares.Height }
            .ComElemento(_cInfComplementares)
            .ComElemento(_cReservadoFisco);

            // NT 2026.010 (itens 4.4 e 4.5): quadro do QR Code ao lado dos dados adicionais, só no leiaute da
            // Reforma Tributária e com o qrCode no XML.
            if (ViewModel.UsarLeiauteReformaTributaria && !String.IsNullOrWhiteSpace(ViewModel.QrCode))
            {
                _cQrCode = new QuadroQrCode(estilo);
                _Linha.ComElemento(_cQrCode).ComLarguras(60, 0, 15);
            }
            else
            {
                _Linha.ComLarguras(InfComplementaresLarguraPorcentagem, 0);
            }
                        
            MainVerticalStack.Add(_Linha);           
        }

        public override float Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                // Força o ajuste da altura do InfComplementares
                if (_cInfComplementares != null && _Linha != null)
                {
                    _Linha.Width = value;
                    _Linha.Posicionar();
                    _cInfComplementares.Height = _cQrCode == null ? AlturaMinima : AlturaMinimaComQrCode;
                    _Linha.Height = _cInfComplementares.Height;
                }
            }
        }

        /// <summary>QR Code já convertido para o PDF; atribuído pelo <see cref="Danfe" /> antes de gerar.</summary>
        public org.pdfclown.documents.contents.xObjects.XObject QrCode
        {
            set
            {
                if (_cQrCode != null)
                    _cQrCode.QrCode = value;
            }
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Base;
        public override string Cabecalho => "Dados adicionais";
    }
}
