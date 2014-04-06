using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    class XSLSParser
    {
        internal string ExtractText(string inFileName)
        {
            string text = "";
            try
            {
                // Open the document for editing.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(inFileName, false))
                {
                    // Code removed here.

                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    foreach (WorksheetPart lWorksheetPart in workbookPart.WorksheetParts)
                    {
                        WorksheetPart worksheetPart = lWorksheetPart;
                        foreach (SheetData sheetData in worksheetPart.Worksheet.Elements<SheetData>())
                        {
                            // sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                            foreach (Row r in sheetData.Elements<Row>())
                            {
                                text = text + r.InnerText;

                            }
                        }
                    }
                }

            }
            catch (Exception m)
            {
                MyException mobj = new MyException("ExtractText() :" + m.Message);
            }
            return text;
        }

    }
}
