/*Class: NameParser
 * Class for performing various parsing actions on a string
 * */

using System;
using System.Collections.Generic;

namespace DuplicateFinder
{
    class NameParser
    {
        /// <summary>
        /// Parses a name from a string
        /// </summary>
        /// <param name="rawName"></param>
        /// <returns>Returns an array with the contents (in order) Last Name, First Name, Middle Name</returns>
        public String[] parseName(String rawName)
        {
            int n;
            String strCopy = rawName;
            List<String> tokens = new List<string>();
            String currentToken = "";
            bool includesCommas = false;
            String[] nameArray;

            //handle numbers separately
            bool isNumber = int.TryParse(rawName,out n);
            if (isNumber)
            {
                tokens.Add(strCopy);
                nameArray = interpretName(tokens, isNumber, includesCommas);
                return nameArray;
            }

            for(int i=1; i < strCopy.Length; i++)
            {
                if(i == 1)
                {
                    if (Char.IsLetter(strCopy[i-1])) currentToken = currentToken + strCopy[i-1];
                }

                Char lastSeen = (Char)strCopy[i - 1];
                if (Char.IsWhiteSpace(strCopy[i]) || strCopy[i].Equals(',') || strCopy[i].Equals('.'))
                {
                    //possibly time to delimit, check what the last item we saw was
                    if (Char.IsLetter(lastSeen))
                    {   //we have seen the end of a token, add it to our list of tokens
                        tokens.Add(currentToken.ToUpper());
                        currentToken = "";
                    }
                    if (strCopy[i].Equals(',')) includesCommas = true;
                }
                //handle parentheses
                //and corner case where we have leading paren
                else if (strCopy[i].Equals('(') || ((i == 1) && lastSeen.Equals('(')))
                {
                    if(currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToUpper());
                        currentToken = "";
                    }
                    while (!strCopy[i].Equals(')')){
                        i++;
                        continue;
                    }
                }
                else if(Char.IsLetter(strCopy[i]))
                {
                    currentToken = currentToken + strCopy[i];
                }

                if ((i+1) >= strCopy.Length)    //if we reach the end of the list, force tokenization
                {
                    if (currentToken.Length > 0) tokens.Add(currentToken.ToUpper());
                }
            }
            nameArray = interpretName(tokens, includesCommas, isNumber);
            return nameArray;
        }

        /// <summary>
        /// Takes a list of tokens and returns an array ordered as [Last Name, First Name, Middle Name]
        /// </summary>
        /// <returns></returns>
        private String[] interpretName(List<String> tokens, bool includesCommas, bool isNum)
        {
            String[] nameArray = new String[3];

            if (isNum)
            {   //if it's a num, just put the number in the last name
                nameArray[0] = tokens[0];
            }
            if (includesCommas && (tokens.Count >= 3)) //if the name includes commas, we will assume that we have the Last Name, First Name format
            {
                for(int i=0; i<3; i++)  //we only care about the 3 tokens, if there are more we ignore them
                {
                    nameArray[i] = tokens[i];
                }
            }
            else
            {
                if(tokens.Count == 1)   //assume we have just the last Name
                {
                    nameArray[0] = tokens[0];
                }
                else if (tokens.Count == 2) {   //assume we have First Name <space> Last Name format
                    for (int i = 0; i < 2; i++)
                    {
                        nameArray[i] = tokens[i];
                    }
                }
                else if (tokens.Count > 2)      //assume we have First Name <space> Middle Name <space> Last Name format
                {
                    nameArray[0] = tokens[0];
                    nameArray[1] = tokens[2];
                    nameArray[2] = tokens[1];
                }
            }
            return nameArray;
        }

        /// <summary>
        /// Parses each part of a record's name into n-grams
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public List<String> parseRecordNGrams(Record r, int n)
        {
            List<String> nGrams = new List<String>();
            if (r.getFirstName() != null) nGrams.AddRange(parseNGrams(r.getFirstName(), n));
            if (r.getLastName() != null) nGrams.AddRange(parseNGrams(r.getLastName(), n));
            if (r.getMiddleName() != null) nGrams.AddRange(parseNGrams(r.getMiddleName(), n));
            return nGrams;
        }

        /// <summary>
        /// Parses a generic string into n-grams
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="n">Size of n-gram substrings</param>
        /// <returns></returns>
        public List<String> parseNGrams(String s, int n)
        {
            List<String> nGrams = new List<String>();
            String nGramSub = "";

            for(int i=0; i<s.Length; i++)
            {
                int remainingChars = s.Length - i;
                if(remainingChars < n)
                {
                    for(int j=0; j<remainingChars; j++)
                    {
                        nGramSub = nGramSub + s[i + j];
                    }
                }
                else
                {
                    for (int j = 0; j < n; j++)
                    {
                        nGramSub = nGramSub + s[i + j];
                    }
                }
                nGrams.Add(nGramSub);
                nGramSub = "";
            }
            return nGrams;
        }

    }
}
