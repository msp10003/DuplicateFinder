/*Class: NameParser
 * Class called to split raw name string into a first, last, and middle name
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
                        tokens.Add(currentToken);
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
                        tokens.Add(currentToken);
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
                    if (currentToken.Length > 0) tokens.Add(currentToken);
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

    }
}
