﻿/* Class: Record
 * This class is a simple DAO for the rows pulled back from the Excel spreadsheet.
 * Note that data that will not be used for duplicate detection is ommitted
 * */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder
{
    public class Record
    {
        //TODO: use get and set syntax
        private String firstName, lastName, middleName, description, fullName, reverseFullName, key, reverseKey;
        private DateTime claimDate;
        private Int64 claimNumber;
        private int recordID;
        public List<String> nGrams;
        private Cluster cluster;

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
            reverseFullName = reverseString(fullName);
            key = fullName + " " + recordID;
            reverseKey = reverseFullName + " " + recordID;
            nGrams = new List<String>();
        }

        //TODO find a better place for this
        private String reverseString(String input)
        {
            return new String(input.ToCharArray().Reverse().ToArray());
        }

        public String getFullName()
        {
            return fullName;
        }

        public int getID()
        {
            return recordID;
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

        public String getReverseKey()
        {
            return reverseKey;
        }
        public Cluster getCluster()
        {
            return cluster;
        }

        public void setCluster(Cluster c)
        {
            cluster = c;
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
