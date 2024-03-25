namespace DanfeNet.Tests;

[TestClass]
public class DanfeXmlTests
{
    public readonly string OutputDirectory = Path.Combine("Output", "DeXml");
    public readonly string InputXmlDirectoryPrefix = Path.Combine("Xml", "NFe");

    public DanfeXmlTests()
    {
        if (!Directory.Exists(OutputDirectory))
            Directory.CreateDirectory(OutputDirectory);
    }

    public void TestXml(string xmlPath)
    {
        var outPdfFilePath = Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(xmlPath) + ".pdf");
        var model = DanfeFactory.FromXmlFilePath(Path.Combine(InputXmlDirectoryPrefix, xmlPath));
        using (var danfePdf = new DanfePdf(model))
        {
            danfePdf.Generate();
            danfePdf.SaveAs(outPdfFilePath);
        }
    }

    [TestMethod]
    public void v1() => TestXml("v1.00/v1.xml");

    [TestMethod]
    public void v2_Retrato() => TestXml("v2.00/v2_Retrato.xml");

    [TestMethod]
    public void v3_10_Retrato() => TestXml("v3.10/v3.10_Retrato.xml");

    [TestMethod]
    public void v4_ComLocalEntrega() => TestXml("v4.00/v4_ComLocalEntrega.xml");

    [TestMethod]
    public void v4_ComLocalRetirada() => TestXml("v4.00/v4_ComLocalRetirada.xml");

    [TestMethod]
    public void v4_Default() => TestXml("v4.00/v4_Default.xml");

    [TestMethod]
    public void v4_SKUSize() => TestXml("v4.00/v4_SKUSize.xml");
    
    [TestMethod]
    public void v4_Cancelada() => TestXml("v4.00/v4_Cancelada.xml");

}