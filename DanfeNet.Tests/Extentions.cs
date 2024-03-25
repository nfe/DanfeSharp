using System.Diagnostics;

namespace DanfeNet.Tests;

public static class Extentions
{
    public const string OutputDirectoryName = "Output";

    public static void SalvarTestPdf(this DanfePdf d, string outputName = null)
    {
        if (!Directory.Exists(OutputDirectoryName)) 
            Directory.CreateDirectory(OutputDirectoryName);

        if (outputName == null) 
            outputName = new StackTrace().GetFrame(1).GetMethod().Name;

        d.SaveAs(Path.Combine(OutputDirectoryName, outputName + ".pdf"));
    }
}