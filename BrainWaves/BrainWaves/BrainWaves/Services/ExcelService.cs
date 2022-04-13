using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrainWaves.Helpers;
using BrainWaves.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BrainWaves.Services
{
    public class ExcelService
    {
        public List<string> ExcellSheetsNames = new List<string>
        {
            Constants.ExcellSheetName1,
            Constants.ExcellSheetName2,
            Constants.ExcellSheetName3,
            Constants.ExcellSheetName4,
            Constants.ExcellSheetName5,
            Constants.ExcellSheetName6,
            Constants.ExcellSheetName7,
            Constants.ExcellSheetName8,
            Constants.ExcellSheetName9,
            Constants.ExcellSheetName10
        };

        private string AppFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public string PathToExcellFile(string fileName)
        {
            return Path.Combine(AppFolder, fileName);
        }

        private Cell ConstructCell(string value, CellValues dataTypes) =>
              new Cell()
              {
                  CellValue = new CellValue(value),
                  DataType = new EnumValue<CellValues>(dataTypes)
              };

        public void GenerateExcel(string filePath)
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            // Creating the SpreadsheetDocument in the indicated FilePath
            var document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            var wbPart = document.AddWorkbookPart();
            wbPart.Workbook = new Workbook();

            var part = wbPart.AddNewPart<WorksheetPart>();
            part.Worksheet = new Worksheet(new SheetData());

            //  Here are created the sheets, you can add all the child sheets that you need.
            var sheets = wbPart.Workbook.AppendChild
                (
                   new Sheets(
                            new Sheet()
                            {
                                Id = wbPart.GetIdOfPart(part),
                                SheetId = 1,
                                Name = Constants.ExcellSheetName1
                            }
                        )
                );

            // Just save and close you Excel file
            wbPart.Workbook.Save();
            document.Close();
            // Dont't forget return the filePath
        }

        public void InsertDataIntoSheet(string fileName, string sheetName, ExcelStructure data)
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            using (var document = SpreadsheetDocument.Open(fileName, true))
            {
                var wbPart = document.WorkbookPart;
                var sheets = wbPart.Workbook.GetFirstChild<Sheets>().
                             Elements<Sheet>().FirstOrDefault().
                             Name = sheetName;

                var part = wbPart.WorksheetParts.First();
                var sheetData = part.Worksheet.Elements<SheetData>().First();

                var row = sheetData.AppendChild(new Row());

                foreach (var header in data.Headers)
                {
                    row.Append(ConstructCell(header, CellValues.String));
                }

                foreach (var value in data.Values)
                {
                    var dataRow = sheetData.AppendChild(new Row());

                    foreach (var dataElement in value)
                    {
                        dataRow.Append(ConstructCell(dataElement, CellValues.String));
                    }
                }
                wbPart.Workbook.Save();
            }
        }

        public void CreateAndInsertDataToManySheets(string filePath, ExcelStructure data)
        {
            Environment.SetEnvironmentVariable("MONO_URI_DOTNETRELATIVEORABSOLUTE", "true");

            SpreadsheetDocument ssDoc = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookPart = ssDoc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = ssDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            int numOfSheets = (data.Values.Count / Constants.MaxEntriesForSheet) + 1;

            for (int i = 0; i < numOfSheets; i++)
            {
                // Begin: Code block for every Excel sheet
                WorksheetPart worksheetPart1 = workbookPart.AddNewPart<WorksheetPart>();
                Worksheet worksheet1 = new Worksheet();
                SheetData sheetData1 = new SheetData();

                //=================================
                //var sheetData = worksheetPart1.Worksheet.Elements<SheetData>().First();
                var row = sheetData1.AppendChild(new Row());
                if(i == 0)
                {
                    foreach (var header in data.Headers)
                    {
                        row.Append(ConstructCell(header, CellValues.String));
                    }

                    for (int j = 0; j < Constants.MaxEntriesForSheet; j++)
                    {
                        var dataRow = sheetData1.AppendChild(new Row());

                        foreach (var dataElement in data.Values[j])
                        {
                            dataRow.Append(ConstructCell(dataElement, CellValues.String));
                        }
                    }
                }
                else
                {
                    int maxValue = data.Values.Count > Constants.MaxEntriesForSheet * (i + 1) ? Constants.MaxEntriesForSheet * (i + 1) : data.Values.Count;
                    for (int j = Constants.MaxEntriesForSheet * i; j < maxValue; j++)
                    {
                        var dataRow = sheetData1.AppendChild(new Row());

                        foreach (var dataElement in data.Values[j])
                        {
                            dataRow.Append(ConstructCell(dataElement, CellValues.String));
                        }
                    }
                }
                worksheet1.AppendChild(sheetData1);
                worksheetPart1.Worksheet = worksheet1;

                Sheet sheet1 = new Sheet()
                {
                    Id = ssDoc.WorkbookPart.GetIdOfPart(worksheetPart1),
                    SheetId = (uint)i,
                    Name = ExcellSheetsNames[i]
                };
                sheets.Append(sheet1);
            }

            workbookPart.Workbook.Save();
            ssDoc.Close();
        }
    }
}
