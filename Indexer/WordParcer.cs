using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;


namespace Indexer
{
    public class WordParcer
    {

        public string ExtractTextPptOpenXML(string inFileName)
        {
            try
            {
                string sldText = "";
                using (PresentationDocument ppt = PresentationDocument.Open(inFileName, true))
                {
                    // Get the relationship ID of the first slide.
                    PresentationPart part = ppt.PresentationPart;
                    OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;
                    int slidesCount = part.SlideParts.Count();
                    int i;
                    for (i = 0; i <= slidesCount; i++)
                    {
                        string relId = (slideIds[i] as SlideId).RelationshipId;

                        // Get the slide part from the relationship ID.
                        SlidePart slide = (SlidePart)part.GetPartById(relId);

                        // Build a StringBuilder object.
                        StringBuilder paragraphText = new StringBuilder();

                        // Get the inner text of the slide:
                        IEnumerable<DocumentFormat.OpenXml.Presentation.Text> texts = slide.Slide.Descendants<DocumentFormat.OpenXml.Presentation.Text>();
                        foreach (DocumentFormat.OpenXml.Presentation.Text text in texts)
                        {
                            paragraphText.Append(text.Text);
                        }
                        sldText = paragraphText.ToString();
                    }
                    return sldText;
                }
            }
            catch (Exception m)
            {
                MyException mobj = new MyException("ExtractTextPptOpenXML() :" + m.Message);
                return "";
            }
        }

        public string ExtractTextWordOpenXML(string inFileName)
        {
            try
            {
                string swrText = "";
                using (WordprocessingDocument myDocument = WordprocessingDocument.Open(inFileName, true))
                {
                    Body body = myDocument.MainDocumentPart.Document.Body;
                    swrText = body.InnerText;
                }
                return swrText;
            }

            catch (Exception m)
            {
                MyException mobj = new MyException("ExtractTextWordOpenXML() :" + m.Message);
                return "";
            }
        }

        //static void ExtractTextWordUsingIntorpt(string inFileName)
        //{
        //    try
        //    {
        //        Microsoft.Office.Interop.Word.Application msWordApp = new Microsoft.Office.Interop.Word.Application();
        //        object nullobj = System.Reflection.Missing.Value;
        //        object ofalse = false;
        //        //object ofile = file;

        //        Microsoft.Office.Interop.Word.Document doc = msWordApp.Documents.Open(
        //                                                    inFileName, ref nullobj, ref nullobj,
        //                                                    ref nullobj, ref nullobj, ref nullobj,
        //                                                    ref nullobj, ref nullobj, ref nullobj,
        //                                                    ref nullobj, ref nullobj, ref nullobj,
        //                                                    ref nullobj, ref nullobj, ref nullobj,
        //                                                    ref nullobj);
        //        string result = doc.Content.Text.Trim();
        //        doc.Close(ref ofalse, ref nullobj, ref nullobj);
        //        msWordApp.Quit();

        //    }
        //    catch
        //    {

        //    }
        //}
        //public string ExtractTextPptUsingIntorpt(string inFileName)
        //{
        //    try
        //    {
        //        //Microsoft.Office.Interop.PowerPoint.Application app = new Microsoft.Office.Interop.PowerPoint.Application();
        //        Microsoft.Office.Core.MsoTriState ofalse = Microsoft.Office.Core.MsoTriState.msoFalse;
        //        Microsoft.Office.Core.MsoTriState otrue = Microsoft.Office.Core.MsoTriState.msoFalse;
        //        Microsoft.Office.Interop.PowerPoint.Application PowerPoint_App = new Microsoft.Office.Interop.PowerPoint.Application();
        //        Microsoft.Office.Interop.PowerPoint.Presentations multi_presentations = PowerPoint_App.Presentations;
        //        Microsoft.Office.Interop.PowerPoint.Presentation presentation = multi_presentations.Open(inFileName, ofalse, otrue);
        //        string presentation_text = "";
        //        for (int i = 0; i < presentation.Slides.Count; i++)
        //        {
        //            foreach (var item in presentation.Slides[i + 1].Shapes)
        //            {
        //                var shape = (Microsoft.Office.Interop.PowerPoint.Shape)item;
        //                if (shape.HasTextFrame == MsoTriState.msoTrue)
        //                {
        //                    if (shape.TextFrame.HasText == MsoTriState.msoTrue)
        //                    {
        //                        var textRange = shape.TextFrame.TextRange;
        //                        var text = textRange.Text;
        //                        presentation_text += text + " ";
        //                    }
        //                }
        //            }
        //        }
        //        PowerPoint_App.Quit();
        //        return presentation_text;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}
    }
}
