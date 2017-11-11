using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using HeadXMLLib;
using ExtractFileLib;
using FileGlueLib;
using System.IO;
using System.Collections;
using System.Xml;

namespace LQNCollectorLib
{
    public struct outputSheetInfo
    {
        public string SheetName;
        public int LineIndex;
        public int StartFrom;
        public bool IsColumn;

        public outputSheetInfo(string sheetName)
        {
            SheetName = sheetName;
            LineIndex = 0;
            StartFrom = 0;
            IsColumn = true;
        }

        public outputSheetInfo(string sheetName, int lineIndex, int startFrom, bool isColumn)
        {
            SheetName = sheetName;
            LineIndex = lineIndex;
            StartFrom = startFrom;
            IsColumn = isColumn;
        }
    }

    public struct matchingSheetInfo
    {
        public string SheetName;
        public string indexString;
        public int LineIndex;
        public int StartFrom;
        public int EndBy;
        public int OutputLineIndex;
        public bool IsColumn;

        public matchingSheetInfo(string sheetName,int indexI,int indexJ)
        {
            SheetName = sheetName;
            indexString = "\"" + indexI.ToString() + "," + indexJ.ToString() + "\"";
            LineIndex = 0;
            StartFrom = 0;
            EndBy = 0;
            OutputLineIndex = 1;
            IsColumn = true;
        }

        public matchingSheetInfo(string sheetName, int indexI, int indexJ,int lineIndex,int startFrom,int endBy,int outputLineIndex,bool isColumn)
        {
            SheetName = sheetName;
            indexString = "\"" + indexI.ToString() + "," + indexJ.ToString() + "\"";
            LineIndex = lineIndex;
            StartFrom = startFrom;
            EndBy = endBy;
            OutputLineIndex = outputLineIndex;
            IsColumn = isColumn;
        }
    }

    public struct fillerInfo
    {
        public Dictionary<string, string> replaceInfo;//Key = before replace , Value = after replace
        public string indexString;//For example: "1-1".

        public fillerInfo(int indexI, int indexJ, Dictionary<string, string> ReplaceInfo)
        {
            indexString = "\"" + indexI.ToString() + "," + indexJ.ToString() + "\"";
            replaceInfo = ReplaceInfo;
        }

        public fillerInfo(int indexI, int indexJ)
        {
            indexString = "";
            indexString = "\"" + indexI.ToString() + "," + indexJ.ToString() + "\"";
            replaceInfo = new Dictionary<string, string>();
        }
    }

    //Using NPOI Library http://npoi.codeplex.com/ to operate excel files.
    public class WorkbookOperator
    {      
        public string targetWorkbookPath
        {
            get { return targetworkbookPath; }
            set
            {
                if (Path.GetExtension(value).ToLower() == ".xlsx")
                {
                    targetworkbookPath = value;

                    FileStream fStream= new FileStream(value, FileMode.Open);              
                    XSSFWorkbook tempWB = new XSSFWorkbook(fStream);

                    fStream.Close();
                    targetWorkbook = tempWB;
                }
                else if (Path.GetExtension(value).ToLower() == ".xls")
                {
                    targetworkbookPath=value;

                    FileStream fStream = new FileStream(value, FileMode.Open);
                    HSSFWorkbook tempWB = new HSSFWorkbook(fStream);

                    fStream.Close();
                    targetWorkbook = tempWB;
                }                        
            }
        }

        private string targetworkbookPath;

        private IWorkbook targetWorkbook;

        //Output version.
        public void Collect_Text_IntoWorkbook(ref XmlDocument summaryXml,
                                              outputSheetInfo sheetInfo,
                                              fillerInfo filler)
        {

            if (sheetInfo.IsColumn == true)
            {
                Collect_Text_IntoWorkbookColumn(ref summaryXml, 
                                                sheetInfo.SheetName, 
                                                sheetInfo.LineIndex, 
                                                sheetInfo.StartFrom, 
                                                filler.indexString, 
                                                ref filler.replaceInfo);
            }
            else
            { 
                Collect_Text_IntoWorkbookRow(ref summaryXml, 
                                             sheetInfo.SheetName, 
                                             sheetInfo.LineIndex,
                                             sheetInfo.StartFrom, 
                                             filler.indexString, 
                                             ref filler.replaceInfo);
            }
        }

        //Output version.
        private void Collect_Text_IntoWorkbookColumn(ref XmlDocument summaryXml, 
                                                     string sheetName, 
                                                     int columnIndex,
                                                     int startFrom, 
                                                     string indexString, 
                                                     ref Dictionary<string, string> replaceInfo)
        {
            ISheet sheet = targetWorkbook.GetSheet(sheetName);

            XmlNodeList summaryNodes = summaryXml.GetElementsByTagName("Summary");

            int rowIndex = startFrom;

            foreach (XmlNode node in summaryNodes)
            {
                string innerText = node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText;

                innerText = replace(innerText, ref replaceInfo);

                if (sheet.GetRow(rowIndex) == null)
                {
                    sheet.CreateRow(rowIndex);
                }

                sheet.GetRow(rowIndex).CreateCell(columnIndex).SetCellValue(innerText);
                rowIndex++;
            }
        }

        //Output version
        private void Collect_Text_IntoWorkbookRow(ref XmlDocument summaryXml, 
                                                  string sheetName, 
                                                  int rowIndex, 
                                                  int startFrom, 
                                                  string indexString, 
                                                  ref Dictionary<string, string> replaceInfo)
        {
            ISheet sheet = targetWorkbook.GetSheet(sheetName);

            XmlNodeList summaryNodes = summaryXml.GetElementsByTagName("Summary");

            if (sheet.GetRow(rowIndex) == null)
            {
                sheet.CreateRow(rowIndex);
            }

            int columnIndex = startFrom;

            foreach (XmlNode node in summaryNodes)
            {
                string innerText = node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText;

                innerText = replace(innerText, ref replaceInfo);

                sheet.GetRow(rowIndex).CreateCell(columnIndex).SetCellValue(innerText);
                columnIndex++;
            }
        }

        //Matched output version.
        public void Collect_Text_IntoWorkbook(ref XmlDocument summaryXml,
                                              matchingSheetInfo sheetInfo,
                                              fillerInfo filler)
        {
            if (sheetInfo.IsColumn == true)
            {
                Collect_Text_IntoWorkbookColumn(ref summaryXml, 
                                                sheetInfo.SheetName, 
                                                sheetInfo.indexString,
                                                sheetInfo.LineIndex, 
                                                sheetInfo.StartFrom,
                                                sheetInfo.EndBy,
                                                sheetInfo.OutputLineIndex,
                                                filler.indexString,
                                                ref filler.replaceInfo);
            }
            else
            {
                Collect_Text_IntoWorkbookRow(ref summaryXml, 
                                             sheetInfo.SheetName, 
                                             sheetInfo.indexString,
                                             sheetInfo.LineIndex, 
                                             sheetInfo.StartFrom, 
                                             sheetInfo.EndBy, 
                                             sheetInfo.OutputLineIndex, 
                                             filler.indexString, 
                                             ref filler.replaceInfo);
            }
        }

        //Matched output version.
        private void Collect_Text_IntoWorkbookColumn(ref XmlDocument summaryXml, 
                                                     string sheetName,
                                                     string sindexString, 
                                                     int columnIndex, 
                                                     int startFrom,
                                                     int endBy,
                                                     int OutputLineIndex, 
                                                     string indexString, 
                                                     ref Dictionary<string, string> replaceInfo)
        {
            ISheet sheet = targetWorkbook.GetSheet(sheetName);

            XmlNodeList summaryNodes = summaryXml.GetElementsByTagName("Summary");

            foreach (XmlNode node in summaryNodes)
            {
                string innerText = node.SelectSingleNode(".//Item[@Index=" + sindexString + "]").InnerText;
                for (int i = startFrom; i <= endBy; i++)
                {
                    if (sheet.GetRow(i) != null)
                    {
                        sheet.GetRow(i).GetCell(columnIndex).SetCellType(CellType.String);//Turn into string "CellType".
                        if (sheet.GetRow(i).GetCell(columnIndex).StringCellValue.ToLower().Trim(' ') == innerText.ToLower().Trim(' '))
                        {
                            sheet.GetRow(i).CreateCell(OutputLineIndex).SetCellValue(replace(node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText, ref replaceInfo));
                        }
                    }
                }
            }
        }

        //Matched output version
        private void Collect_Text_IntoWorkbookRow(ref XmlDocument summaryXml,
                                                  string sheetName, 
                                                  string sindexString, 
                                                  int rowIndex, 
                                                  int startFrom, 
                                                  int endBy,
                                                  int OutputLineIndex, 
                                                  string indexString, 
                                                  ref Dictionary<string, string> replaceInfo)
        {
            ISheet sheet = targetWorkbook.GetSheet(sheetName);

            XmlNodeList summaryNodes = summaryXml.GetElementsByTagName("Summary");

            if (sheet.GetRow(OutputLineIndex)==null) { sheet.CreateRow(OutputLineIndex); }

            foreach (XmlNode node in summaryNodes)
            {
                string innerText = node.SelectSingleNode(".//Item[@Index=" + sindexString + "]").InnerText;
                for (int i = startFrom; i <= endBy; i++)
                {
                    sheet.GetRow(rowIndex).GetCell(i).SetCellType(CellType.String);
                    if (sheet.GetRow(rowIndex).GetCell(i).StringCellValue.ToLower().Trim(' ') == innerText.ToLower().Trim(' '))
                    {
                        sheet.GetRow(OutputLineIndex).CreateCell(i).SetCellValue(replace(node.SelectSingleNode(".//Item[@Index=" + indexString + "]").InnerText, ref replaceInfo));
                    }
                }
            }
        }


        
       
        private string replace(string sourceText,ref Dictionary<string,string> replaceInfo)
        {
            string value = sourceText;

            if (replaceInfo.Count > 0)
            {
                
                try
                {
                    value = replaceInfo[sourceText];
                }
                catch
                {

                }
            }

            return value;
        }

        //Get workbook sheets names.
        public ArrayList GetSheets()
        {
            ArrayList sheetsArray = new ArrayList();
            for (int i = 0; i < targetWorkbook.NumberOfSheets; i++)
            {
                sheetsArray.Add(targetWorkbook.GetSheetName(i));
            }
            return sheetsArray;
        }

        //Save after editing.
        public void SaveWorkbook(string path="")
        {
            if (path == "")
            {
                path = targetWorkbookPath;
            }

            FileStream fStream = File.Create(targetworkbookPath);
            targetWorkbook.Write(fStream);
            fStream.Close();
        }        
    }
}
