/* Class: DataRetriever
 * This class serves as a simple Data Access interface between Excel and the program.
 * All data requests and manipulations should come through this class.
 * */

using System;
using SpreadsheetLight;
using System.Collections.Generic;

namespace DuplicateFinder
{
    class DataRetriever
    {
        private String spreadSheetPath, nameColumn, claimNumColumn, claimDateColumn, descriptionCol;
        private SLDocument excelFile;
        public int numCols, numRows;
        private const int NUM_ROWS_OFFSET = 2;
        private List<SLCellPointRange> rows;

        public DataRetriever(String pathName, String nameCol, String claimNumCol, String claimDateCol, String descCol)
        {
            spreadSheetPath = pathName;
            excelFile = new SLDocument(pathName);
            nameColumn = nameCol;
            claimNumColumn = claimNumCol;
            claimDateColumn = claimDateCol;
            descriptionCol = descCol;
            numCols = 1;
            //TODO probably don't want to hardcode this in
            numRows = NUM_ROWS_OFFSET;
        }

        public SLDocument setSpreadsheet(String pathName)
        {
            spreadSheetPath = pathName;
            excelFile = new SLDocument(spreadSheetPath);
            return excelFile;
        }

        /*public ExcelRow getRow(int rowNum)
        {
            SLCellPointRange cellRange = new SLCellPointRange()
            return row;
        }*/

        public SLDocument getExcelFile()
        {
            return excelFile;
        }

        public ICollection<SLCellPointRange> getRows()
        {
            if(rows != null)
            {
                return rows;
            }

            rows = new List<SLCellPointRange>();

            while (true)
            {
                String s = excelFile.GetCellValueAsString(1, numCols);
                if (String.IsNullOrEmpty(excelFile.GetCellValueAsString(1, numCols)))
                {
                    break;
                }
                numCols++;
            }

            while (true)
            {
                if(String.IsNullOrEmpty(excelFile.GetCellValueAsString(numRows,1)))
                {
                    break;
                }
                rows.Add(new SLCellPointRange(numRows, 1, numRows, numCols));
                numRows++;

            }
            return rows;
        }

        public Int64 getClaimNum(int rowNum)
        {
            if (claimNumColumn == null)
            {
                return 0;
            }

            String cellIndex = claimNumColumn + rowNum;
            return excelFile.GetCellValueAsInt64(cellIndex);
        }

        public DateTime getClaimDate(int rowNum)
        {
            String cellIndex = claimDateColumn + rowNum;
            return excelFile.GetCellValueAsDateTime(cellIndex);
        }

        public String getName(int rowNum)
        {
            String cellIndex = nameColumn + rowNum;
            return excelFile.GetCellValueAsString(cellIndex);
        }

        public String getDescription(int rowNum)
        {
            String cellIndex = descriptionCol + rowNum;
            return excelFile.GetCellValueAsString(cellIndex);
        }

        public int getRowID(SLCellPointRange cpr)
        {
            return cpr.StartRowIndex;
        }

        public int getNumRowsOffset()
        {
            return NUM_ROWS_OFFSET;
        }

        public void writeToFile(List<String> records, SLDocument targetFile)
        {

        }

        public void copySpreadsheetToFile(String outputPath)
        {
            excelFile.SaveAs(outputPath);
            SLDocument duplicateExcelFile = new SLDocument(outputPath);
            duplicateExcelFile.SetCellValue(2, 30, "TEST");
            duplicateExcelFile.Save();
            
        }
    }
}
