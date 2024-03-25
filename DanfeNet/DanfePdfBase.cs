using System;
using org.pdfclown.files;
using org.pdfclown.documents;

namespace DanfeNet;

public abstract class DanfePdfBase : IDisposable
{
    internal Document PdfDocument { get; set; }
    
    public File File { get; internal set; }
        
    public string CreditsInfo { get;  set; }
    public string MetadataInfo { get; set; }
        
        
    public void Salvar(System.IO.Stream stream)
    {
        if (stream == null) 
            throw new ArgumentNullException(nameof(stream));

        File.Save(new org.pdfclown.bytes.Stream(stream), SerializationModeEnum.Incremental);
    }
        
    public void Salvar(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) 
            throw new ArgumentException(nameof(path));

        File.Save(path, SerializationModeEnum.Incremental);
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