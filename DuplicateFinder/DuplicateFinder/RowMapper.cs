/*Class: RowMapper
 * Maps a row from the Excel sheet to a Record object
 * */

using System.Collections.Generic;
using System;
using SpreadsheetLight;

namespace DuplicateFinder
{
    class RowMapper
    {
        private NameParser parser;

        public RowMapper()
        {
            parser = new NameParser();
        }

        public ICollection<Record> mapRows(DataRetriever dataRetriever)
        {
            List<Record> records = new List<Record>();

            foreach(SLCellPointRange cpr in dataRetriever.getRows())
            {
                Record r = rowToRecord(cpr, dataRetriever);
                records.Add(r);
            }

            return records;
        }

        private Record rowToRecord(SLCellPointRange cpr, DataRetriever dataRetriever)
        {
            //Int64 claimNum = dataRetriever.getClaimNum(cpr.StartRowIndex);
            String claimDesc = dataRetriever.getDescription(cpr.StartRowIndex);
            DateTime claimDate = dataRetriever.getClaimDate(cpr.StartRowIndex);
            String name = dataRetriever.getName(cpr.StartRowIndex);
            String[] nameTokens = parser.parseName(name);
            int recordID = dataRetriever.getRowID(cpr);
            Record r = new Record(recordID, nameTokens[0], nameTokens[1], nameTokens[2], claimDate, claimDesc);
            //r.nGrams = parser.parseNGrams(r.getFullName(), 3);
            //r.nGrams = parser.parseRecordNGrams(r, 3);
            return r;
        }
    }
}
