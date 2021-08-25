// Archivo: CompatiblePDFReader.cs
// Clase 'CompatiblePDFReader' que contiene las funciones básicas de conversión de archivos PDF entre PDF Sharp y iTextSharp para .NET Core.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using PdfSharpCore.Pdf.IO;
using iTextSharp.text.pdf;

namespace Utilities
{
  /// <summary>
  /// Clase 'CompatiblePDFReader' que contiene las funciones básicas de conversión de archivos PDF entre PDF Sharp y iTextSharp para .NET Core.
  /// </summary>
  public sealed class CompatiblePDFReader
  {
    /// <summary>
    /// Uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open.
    /// </summary>
    public static async Task<PdfSharpCore.Pdf.PdfDocument> OpenAsync(string PdfPath, PdfDocumentOpenMode openmode)
    {
      using (FileStream fileStream = new FileStream(PdfPath, FileMode.Open, FileAccess.Read))
      {
        int len = (int)fileStream.Length;
        byte[] fileArray = new byte[len];
        fileStream.Read(fileArray, 0, len);
        fileStream.Close();

        return await OpenAsync(fileArray, openmode);
      }
    }

    /// <summary>
    /// Uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open.
    /// </summary>
    public static async Task<PdfSharpCore.Pdf.PdfDocument> OpenAsync(byte[] fileArray, PdfDocumentOpenMode openmode) => await OpenAsync(new MemoryStream(fileArray), openmode);

    /// <summary>
    /// Uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open.
    /// </summary>
    public static async Task<PdfSharpCore.Pdf.PdfDocument> OpenAsync(MemoryStream sourceStream, PdfDocumentOpenMode openmode)
    {
      PdfSharpCore.Pdf.PdfDocument outDoc = null;
      sourceStream.Position = 0;

      try
      {
        outDoc = PdfSharpCore.Pdf.IO.PdfReader.Open(sourceStream, openmode);
      }
      catch (PdfReaderException)
      {
        //workaround if PdfSharpCore doesn't support this pdf
        sourceStream.Position = 0;
        MemoryStream outputStream = new MemoryStream();
        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
        PdfStamper pdfStamper = new PdfStamper(reader, outputStream) { FormFlattening = true };
        pdfStamper.Writer.SetPdfVersion(PdfWriter.PdfVersion14);
        pdfStamper.Writer.SetFullCompression();
        pdfStamper.Writer.CloseStream = false;
        pdfStamper.Close();

        outDoc = PdfSharpCore.Pdf.IO.PdfReader.Open(outputStream, openmode);
      }

      await Task.Delay(1000); return outDoc;
    }
  }
}
