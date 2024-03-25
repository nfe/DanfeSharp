<h1><img src="https://github.com/JJConsulting/DanfeNet/blob/main/DanfeNet.png" alt="DanfeNet" style="height:50px"/> DanfeNet</h1>

DanfeNet é uma biblioteca em C# que permite a geração do DANFE em formato PDF.

A biblioteca PDF Clown é utilizada para a escrita dos arquivos em PDF.

Exemplo de uso:
```csharp

using DanfeNet;
using DanfeNet.Models;

//Cria o modelo a partir do arquivo Xml da NF-e.
var danfe = DanfeFactory.FromXmlFilePath(@"c:\nfe.xml");


//O modelo também pode ser criado e preenchido de outra forma.
var danfe = new Danfe()
{
    NfNumero = 123456,
    NfSerie = 123,
    ChaveAcesso = "123456987...",
    Emitente = new EmpresaInfo()
    {
        CnpjCpf = "123456...",
        Nome = "Espresas JJ Ltda",    
	...


//Inicia o pdf com o modelo da DANFE criada
using (var pdf = new DanfePdf(danfe))
{
	pdf.Generate();
	pdf.SalveAs(@"C:\danfe.pdf");
}
```


