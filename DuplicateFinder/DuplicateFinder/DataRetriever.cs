/* Class: DataRetriever
 * This class serves as a simple Data Access interface between Excel and the program.
 * All data requests and manipulations should come through this class.
 * */

using System;
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
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
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

        public long getClaimNum(ExcelRow row)
        {
            if (claimNumColumn == null)
            {
                return 0;
            }
            long claimNum;
            if(long.TryParse(row.Cells[claimNumColumn].StringValue, out claimNum))
            {
                return claimNum;
            }
            return 0;
        }

        public DateTime getClaimDate(ExcelRow row)
        {
            //TODO: convert the string to a datetime
            String dateString = row.Cells[claimDateColumn].StringValue;
            DateTime claimDate;
            if (DateTime.TryParse(dateString, out claimDate))
            {
                return claimDate;
            }
            return new DateTime();
        }

        public String getName(ExcelRow row)
        {
            return row.Cells[nameColumn].StringValue;
        }

        public String getDescription(ExcelRow row)
        {
            return row.Cells[descriptionCol].StringValue;
        }

        public int getRowID(ExcelRow row)
        {
            return row.Index;
        }
    }
}
