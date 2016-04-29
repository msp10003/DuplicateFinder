/*Class: RowMapper
 * Maps a row from the Excel sheet to a Record object
 * */

using System.Collections.Generic;
using GemBox.Spreadsheet;
using System;

namespace DuplicateFinder
{
    class RowMapper
    {
        NameParser parser;

        public RowMapper()
        {
            parser = new NameParser();
        }

        public ICollection<Record> mapRows(DataRetriever dataRetriever)
        {
            List<Record> records = new List<Record>();

            foreach(ExcelRow row in dataRetriever.getRows())
            {
                Record r = rowToRecord(row, dataRetriever);
                records.Add(r);
            }

            return records;
        }

        private Record rowToRecord(ExcelRow row, DataRetriever dataRetriever)
        {
            long claimNum = dataRetriever.getClaimNum(row);
            String claimDesc = dataRetriever.getDescription(row);
            DateTime claimDate = dataRetriever.getClaimDate(row);
            String name = dataRetriever.getName(row);
            String[] nameTokens = parser.parseName(name);
            int recordID = dataRetriever.getRowID(row);
            return new Record(recordID, nameTokens[0], nameTokens[1], nameTokens[2], claimDate, claimNum, claimDesc);
        }
    }
}
