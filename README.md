# DanfeSharp

DanfeSharp é uma biblioteca em C# que permite a geração do DANFE em formato PDF.

A biblioteca PDF Clown é utilizada para a escrita dos arquivos em PDF.

Exemplo de uso:
```csharp

using DanfeSharp;
using DanfeSharp.Models;

//Cria o modelo a partir do arquivo Xml da NF-e.
var modelo = DanfeFactory.CriarDeArquivoXml("nfe.xml");


//O modelo também pode ser criado e preenchido de outra forma.
var danfe = new Danfe()
{
    NfNumero = 123456,
    NfSerie = 123,
    ChaveAcesso = "123456987...",
    Emitente = new EmpresaViewModel()
    {
        CnpjCpf = "123456...",
        Nome = "DanfeSharp Ltda",    
	...


//Inicia o Danfe com o modelo criado
using (var pdf = new DanfePdf(modelo))
{
	danfe.Gerar();
	danfe.Salvar("danfe.pdf");
}
```


