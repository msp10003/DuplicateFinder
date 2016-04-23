/* Class: Record
 * This class is a simple DAO for the rows pulled back from the Excel spreadsheet.
 * Note that data that will not be used for duplicate detection is ommitted
 * */

using System;

namespace DuplicateFinder
{
    class Record
    {
        private String firstName, lastName, middleName;
        private DateTime claimDate;
        private long claimNumber;

        public Record(String first, String last, String middle, DateTime date, long claimNum)
        {
            firstName = first;
            lastName = last;
            middleName = middle;
            claimDate = date;
            claimNumber = claimNum;
        }
    }
}
