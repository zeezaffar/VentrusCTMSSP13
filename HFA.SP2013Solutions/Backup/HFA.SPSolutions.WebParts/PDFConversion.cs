using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpertPdf.HtmlToPdf;
using System.Drawing;
using ExpertPdf.HtmlToPdf.PdfDocument;
using Microsoft.SharePoint;

namespace HFA.SPSolutions.WebParts
{
    static class PDFConversion
    {
        public static PdfConverter GetPdfConverter()
        {
            PdfConverter pdfConverter = new PdfConverter();

            pdfConverter.LicenseKey = "cFtBUEhQQUBASVBIXkBQQ0FeQUJeSUlJSQ==";

            // set the HTML page width in pixels
            // the default value is 1024 pixels

            pdfConverter.PageWidth = 0; // autodetect the HTML page width

            // set if the generated PDF contains selectable text or an embedded image - default value is true
            pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;

            //set the PDF page size 
            pdfConverter.PdfDocumentOptions.PdfPageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4");
            // set the PDF compression level
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = (PdfCompressionLevel)Enum.Parse(typeof(PdfCompressionLevel), "Normal");
            // set the PDF page orientation (portrait or landscape)
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = (PDFPageOrientation)Enum.Parse(typeof(PDFPageOrientation), "Portrait");
            //set the PDF standard used to generate the PDF document
            pdfConverter.PdfStandardSubset = GetPdfStandard("PDF");
            // show or hide header and footer
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = false;
            //set the PDF document margins
            pdfConverter.PdfDocumentOptions.LeftMargin = 10;// int.Parse(textBoxLeftMargin.Text.Trim());
            pdfConverter.PdfDocumentOptions.RightMargin = 10; //int.Parse(textBoxRightMargin.Text.Trim());
            pdfConverter.PdfDocumentOptions.TopMargin = 10;//int.Parse(textBoxTopMargin.Text.Trim());
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;// int.Parse(textBoxBottomMargin.Text.Trim());
            // set if the HTTP links are enabled in the generated PDF
            pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;// cbLiveLinksEnabled.Checked;
            // set if the HTML content is resized if necessary to fit the PDF page width - default is true
            pdfConverter.PdfDocumentOptions.FitWidth = true;// cbFitWidth.Checked;
            // set if the PDF page should be automatically resized to the size of the HTML content when FitWidth is false
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
            // embed the true type fonts in the generated PDF document
            pdfConverter.PdfDocumentOptions.EmbedFonts = false;// cbEmbedFonts.Checked;
            // compress the images in PDF with JPEG to reduce the PDF document size - default is true
            pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;// cbJpegCompression.Checked;
            // set if the JavaScript is enabled during conversion 
            pdfConverter.ScriptsEnabled = pdfConverter.ScriptsEnabledInImage = false;// cbScriptsEnabled.Checked;

            // set if the converter should try to avoid breaking the images between PDF pages
            pdfConverter.AvoidImageBreak = false;// cbAvoidImageBreak.Checked;

            pdfConverter.PdfHeaderOptions.HeaderText = "Header Text";// textBoxHeaderText.Text;
            pdfConverter.PdfHeaderOptions.HeaderTextColor = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), "Black"));
            pdfConverter.PdfHeaderOptions.HeaderSubtitleText = "Subjct Title";// textBoxHeaderSubtitle.Text;
            pdfConverter.PdfHeaderOptions.DrawHeaderLine = true;// cbDrawHeaderLine.Checked;
            pdfConverter.PdfHeaderOptions.HeaderHeight = 50;

            pdfConverter.PdfFooterOptions.FooterText = "Footer Text";// textBoxFooterText.Text;
            pdfConverter.PdfFooterOptions.FooterTextColor = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), "Black"));
            pdfConverter.PdfFooterOptions.DrawFooterLine = true;// cbDrawFooterLine.Checked;
            pdfConverter.PdfFooterOptions.PageNumberText = "Page Number Text";// textBoxPageNmberText.Text;
            pdfConverter.PdfFooterOptions.ShowPageNumber = true;// cbShowPageNumber.Checked;
            pdfConverter.PdfFooterOptions.FooterHeight = 50;

            //pdfConverter.PdfBookmarkOptions.TagNames = cbBookmarks.Checked ? new string[] { "h1", "h2" } : null;

            return pdfConverter;
        }

        public static PdfStandardSubset GetPdfStandard(string standardName)
        {
            switch (standardName)
            {
                case "PDF":
                    return PdfStandardSubset.Full;
                case "PDF/A":
                    return PdfStandardSubset.Pdf_A_1b;
                case "PDF/X":
                    return PdfStandardSubset.Pdf_X_1a;
                case "PDF/SiqQA":
                    return PdfStandardSubset.Pdf_SiqQ_a;
                case "PDF/SiqQB":
                    return PdfStandardSubset.Pdf_SiqQ_b;
                default:
                    return PdfStandardSubset.Full;

            }
        }

        public static bool CreateFinalPDF(string mainFile, string signatureFile, string convertedFile)
        {
           
            try
            {
             Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
          {
              using (SPSite site = new SPSite(siteID))
              {
                  using (SPWeb web = site.AllWebs[webID])
                  {
                      //Generate PDF from converted file
                      PdfConverter pdfConverter = PDFConversion.GetPdfConverter();
                      Document pdfDocument = pdfConverter.GetPdfDocumentObjectFromUrl(mainFile);
                      PdfPage newPage = pdfDocument.Pages.AddNewPage();
                      HtmlToPdfElement htmlToPDFUrl = new HtmlToPdfElement(signatureFile);
                      newPage.AddElement(htmlToPDFUrl);
                      pdfDocument.Save(convertedFile);
                      pdfDocument.Close();
                  }
              }
          });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
