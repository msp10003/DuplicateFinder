/* Class: Record
 * This class is a simple DAO for the rows pulled back from the Excel spreadsheet.
 * Note that data that will not be used for duplicate detection is ommitted
 * */

using System;

namespace DuplicateFinder
{
    class Record
    {
        //TODO: use get and set syntax
        private String firstName, lastName, middleName, description, fullName;
        private DateTime claimDate;
        private Int64 claimNumber;
        private int recordID;

        public Record(int ID, String last, String first, String middle, DateTime date, Int64 claimNum, string desc)
        {
            recordID = ID;
            firstName = first;
            lastName = last;
            middleName = middle;
            claimDate = date;
            claimNumber = claimNum;
            description = desc;
            fullName = last + " " + first + " " + middle;
        }

        public String getFullName()
        {
            return fullName;
        }

        public override string ToString()
        {
            string s = "ID: " + recordID + " " + fullName + " " + claimNumber + " " + description;
            return s;
        }
    }
}
