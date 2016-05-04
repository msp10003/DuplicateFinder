/* Class: Record
 * This class is a simple DAO for the rows pulled back from the Excel spreadsheet.
 * Note that data that will not be used for duplicate detection is ommitted
 * */

using System;
using System.Collections.Generic;

namespace DuplicateFinder
{
    class Record
    {
        //TODO: use get and set syntax
        private String firstName, lastName, middleName, description, fullName, key;
        private DateTime claimDate;
        private Int64 claimNumber;
        private int recordID;
        public List<String> nGrams;

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
            key = fullName + " " + recordID;
            nGrams = new List<String>();
        }

        public String getFullName()
        {
            return fullName;
        }

        public String getFirstName()
        {
            return firstName;
        }

        public String getLastName()
        {
            return lastName;
        }

        public String getMiddleName()
        {
            return middleName;
        }

        public String getDescription()
        {
            return description;
        }

        public String getKey()
        {
            return key;
        }

        public override string ToString()
        {
            string s = "ID: " + recordID + " " + fullName + " " + claimNumber + " " + description;
            return s;
        }

        public string ToKeyString()
        {
            return key;
        }

        public string nGramsString()
        {
            String s = "";
            foreach(String ng in nGrams)
            {
                s = s +"|"+ ng;
            }
            return s;
        }
    }
}
