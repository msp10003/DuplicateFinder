/*Class: Similarity Comparer
 * Contains methods used to detect the similarity between two records
 * Makes use of two different types of comparisons
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    //TODO refactor all methods to take strings instead of records
    public class StringComparer
    {
        delegate int del();

        public double jaroWinklerCompare(Record r1, Record r2)
        {
            return jaroWinklerCompare(r1.getFullName(), r2.getFullName());
        }

        public double jaroWinklerCompare(String s1, String s2)
        {
            String maxStr, minStr;

            if (s1.Length >= s2.Length)
            {
                maxStr = s1;
                minStr = s2;
            }
            else
            {
                maxStr = s2;
                minStr = s1;
            }

            int w = (maxStr.Length / 2) - 1;
            int t = 0;
            int m = 0;
            char[] matches1 = new char[maxStr.Length];
            char[] matches2 = new char[maxStr.Length];
            double dj, dw;
            int l = 0;

            //first get the matches
            for (int i = 0; i < maxStr.Length; i++)
            {
                del winMax = () => { return Math.Min(i + w, minStr.Length-1); };
                del winMin = () => { return Math.Max(i - w, 0); };
                int z = winMax();

                //TODO improve speed by starting at i and checking to left and right simultaneously
                //the reasoning being that most of the time that match will be at i
                for (int j = winMin(); j < winMax()+1; j++)
                {
                    if (minStr[j] == maxStr[i])
                    {
                        matches1[i] = maxStr[i];
                        matches2[j] = minStr[j];
                        m++;
                        break;
                    }
                }

            }

            char[] matches1Normalized = new char[matches1.Length];
            char[] matches2Normalized = new char[matches2.Length];
            int m1nIndex= 0;
            int m2nIndex = 0;
            for (int i = 0; i < matches1.Length; i++)
            {
                if (matches1[i] !=  '\0')
                {
                    matches1Normalized[m1nIndex] = matches1[i];
                    m1nIndex++;
                }
                if (matches2[i] != '\0')
                {
                    matches2Normalized[m2nIndex] = matches2[i];
                    m2nIndex++;
                }
            }

            //next get the transpositions
            for (int i = 0; i < m; i++)
            {
                if (matches1Normalized[i] != matches2Normalized[i])
                {
                    t++;
                }
            }
            //get transposition count
            t = (t / 2);

            //calculate the length of the opening substring
            for (int i = 0; i < minStr.Length; i++)
            {
                if (minStr[i] != maxStr[i])
                {
                    break;
                }
                l++;
            }

            //now calculate the jaro distance
            if (m == 0)
            {
                dj = 0;
            }
            else
            {
                double firstTerm = (double) m /(double) maxStr.Length;
                double secondTerm = (double) m /(double) minStr.Length;
                double thirdTerm = ((double)m - (double)t) / (double) m;
                dj = (1.0/3.0)*(firstTerm + secondTerm + thirdTerm);
            }
            
            //and lastly calculate the Winkler factor
            dw = dj + ((l * 0.1) * (1 - dj));
            
            return dw;
        }

        //TODO use a hashtable here
        public double nGramCompare(Record r1, Record r2)
        {
            //handle numbers
            int x, y;
            if (Int32.TryParse(r1.getFullName(),out x) && Int32.TryParse(r2.getFullName(), out y))
            {
                if (x == y)
                {
                    return 0.999;
                }
                else
                {
                    return 0.00;
                }
            }
            else if(Int32.TryParse(r1.getFullName(), out x) || Int32.TryParse(r2.getFullName(), out y))
            {
                return 0.00;
            }
            var result = r1.nGrams.Intersect(r2.nGrams);
            double rc = result.Count();
            double r1c = r1.nGrams.Count();
            double r2c = r2.nGrams.Count();
            double similarity = (rc*2)/(r1c+r2c);
            return similarity;
        }

        public double nGramCompareDesc(Record r1, Record r2)
        {
            NameParser parser = new NameParser();
            List<String> r1NGrams = parser.parseNGrams(r1.getDescription(), 3);
            List<String> r2NGrams = parser.parseNGrams(r2.getDescription(), 3);
            //TODO code reuse here
            var result = r1NGrams.Intersect(r2NGrams);
            double rc = result.Count();
            double r1c = r1NGrams.Count;
            double r2c = r2NGrams.Count;
            double similarity = (rc * 2) / (r1c + r2c);
            return similarity;
        }
    }
}
