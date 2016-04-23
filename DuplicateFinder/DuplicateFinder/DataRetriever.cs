/* Class: DataRetriever
 * This class serves as a simple Data Access interface between Excel and the program.
 * All data requests and manipulations should come through this class.
 * */

using System;
using System.Collections.Generic;
using GemBox.Spreadsheet;

namespace DuplicateFinder
{
    class DataRetriever
    {
        private String spreadSheetPath;
        private ExcelFile excelFile;
        private ExcelWorksheet worksheet;

        public DataRetriever(String pathName)
        {
            spreadSheetPath = pathName;
            excelFile = ExcelFile.Load(spreadSheetPath);
            worksheet = excelFile.Worksheets.ActiveWorksheet;
        }

        public ExcelFile setSpreadsheet(String pathName)
        {
            spreadSheetPath = pathName;
            excelFile = ExcelFile.Load(spreadSheetPath);
            worksheet = excelFile.Worksheets.ActiveWorksheet;
            return excelFile;
        }

        public ExcelRow getRow(int rowNum)
        {
            ExcelRow row = worksheet.Rows[rowNum];
            return row;
        }

    }
}
