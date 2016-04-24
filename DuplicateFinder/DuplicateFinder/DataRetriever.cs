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
        private String spreadSheetPath, nameColumn, claimNumColumn, claimDateColumn, descriptionCol;
        private ExcelFile excelFile;
        private ExcelWorksheet worksheet;

        public DataRetriever(String pathName, String nameCol, String claimNumCol, String claimDateCol, String descCol)
        {
            spreadSheetPath = pathName;
            excelFile = ExcelFile.Load(spreadSheetPath);
            worksheet = excelFile.Worksheets.ActiveWorksheet;
            nameColumn = nameCol;
            claimNumColumn = claimNumCol;
            claimDateColumn = claimDateCol;
            descriptionCol = descCol;
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

        public ExcelFile getExcelFile()
        {
            return excelFile;
        }

        public ExcelRowCollection getRows()
        {
            return worksheet.Rows;
        }

        public String getClaimNum(ExcelRow row)
        {
            return row.Cells[claimNumColumn].StringValue;
        }

        public String getClaimDate(ExcelRow row)
        {
            //TODO: convert the string to a datetime
            return row.Cells[claimDateColumn].StringValue;
        }

        public String getName(ExcelRow row)
        {
            return row.Cells[nameColumn].StringValue;
        }
    }
}
