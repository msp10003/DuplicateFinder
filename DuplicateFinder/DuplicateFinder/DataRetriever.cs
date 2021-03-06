﻿/* Class: DataRetriever
 * This class serves as a simple Data Access interface between Excel and the program.
 * All data requests and manipulations should come through this class.
 * */

using System;
using SpreadsheetLight;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Drawing;

namespace DuplicateFinder
{
    class DataRetriever
    {
        private String spreadSheetPath, nameColumn, claimNumColumn, claimDateColumn, descriptionCol;
        private SLDocument excelFile;
        public int numCols, numRows;
        private const int NUM_ROWS_OFFSET = 2;
        private List<SLCellPointRange> rows;

        public DataRetriever(String pathName, String nameCol, String claimDateCol, String descCol, int numberOfColumns)
        {
            try
            {
                spreadSheetPath = pathName;
                excelFile = new SLDocument(pathName);
            }
            catch
            {
                throw (new Exception("Could not find spreadsheet file"));
            }
            finally
            {
                nameColumn = nameCol;
                claimDateColumn = claimDateCol;
                descriptionCol = descCol;
                numCols = numberOfColumns;
                //TODO probably don't want to hardcode this in
                numRows = NUM_ROWS_OFFSET;
            }
        }

        public SLDocument setSpreadsheet(String pathName)
        {
            spreadSheetPath = pathName;
            excelFile = new SLDocument(spreadSheetPath);
            return excelFile;
        }

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

        public void writeDuplicates(List<Cluster> clusters, String pathName)
        {
            SLDocument targetFile = new SLDocument(pathName);
            SLStyle style = targetFile.CreateStyle();

            foreach (Cluster c in clusters)
            {
                List<Record> records = c.getRecords();
                if (records.Count < 2)
                {
                    continue;
                }
                //mark the first record in orange
                style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Orange, System.Drawing.Color.Blue);

                targetFile.SetCellStyle(records[0].getID(), 1, records[0].getID(), numCols + 1, style);
                targetFile.SetCellValue(records[0].getID(), numCols + 1, "This row has potential duplicates");
                targetFile.SetColumnWidth(numCols + 1, 80);

                style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Red, System.Drawing.Color.Blue);

                for(int i=1; i<records.Count; i++)
                {                     
                    //highlight the possible duplicates in red
                    targetFile.SetCellStyle(records[i].getID(), 1, records[i].getID(), numCols+1, style); 
                    targetFile.SetCellValue(records[i].getID(), numCols+1, "Possible duplicate of Row #" + records[0].getID() + " , Claimant " + records[0].getFullName());
                }
            }
            targetFile.Save();

        }

        public void copySpreadsheetToFile(String outputPath)
        {
            try
            {
                excelFile.SaveAs(outputPath);
                SLDocument duplicateExcelFile = new SLDocument(outputPath);
                duplicateExcelFile.Save();
            }
            catch
            {
                throw (new Exception("The output file path does not exist"));
            }
            
        }

        public void writeNames(List<Record> records, String pathName)
        {
            try
            {
                SLDocument targetFile = new SLDocument(pathName);
                SLStyle style = targetFile.CreateStyle();
                
                foreach (Record r in records)
                {
                    targetFile.SetCellValue(r.getID(), numCols + 2, r.getLastName() +" ," + r.getFirstName()+" " +r.getMiddleName());
                }

                targetFile.Save();
            }
            catch
            {
                throw (new Exception("There was a problem creating the output spreadsheet"));
            }
        }
    }
}
