using System.Diagnostics;
using DanfeNet.Elementos;
using DanfeNet.Graphics;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using File = org.pdfclown.files.File;


namespace DanfeNet.Tests;

internal class SimplePdfPageTester : IDisposable
{
    public File File { get; set; }
    public Document Document { get; set; }
    public PrimitiveComposer PrimitiveComposer { get; set; }
    public Gfx Gfx { get; set; }
    public Page Page { get; set; }

    public SimplePdfPageTester()
    {            
        File = new File();
        Document = File.Document;

        Page = new Page(Document);
        Document.Pages.Add(Page);

        PrimitiveComposer = new PrimitiveComposer(Page);
        Gfx = new Gfx(PrimitiveComposer);
    }

    public void Save(string path)
    {
        File.Save(path, SerializationModeEnum.Standard);
    }

    public Estilo CriarEstilo()
    {
        return new Estilo(new StandardType1Font(Document, StandardType1Font.FamilyEnum.Times, false, false),
            new StandardType1Font(Document, StandardType1Font.FamilyEnum.Times, true, false),
            new StandardType1Font(Document, StandardType1Font.FamilyEnum.Times, false, true));
    }

    public void Save()
    {
        Save(new StackTrace().GetFrame(1).GetMethod().Name + ".pdf");
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (File != null) 
                    File.Dispose();
            }

            disposedValue = true;
        }
    }


    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }
    #endregion
}