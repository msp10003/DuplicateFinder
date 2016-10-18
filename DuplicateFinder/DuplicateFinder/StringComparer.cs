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
            return proximity(r1.getFullName(), r2.getFullName());
        }

        /*public double jaroWinklerCompare(String s1, String s2)
        {
            //TODO: for-loops everywhere in this overlong method, refine and improve
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
        }*/


        private static readonly double mWeightThreshold = 0.7;

        /* Size of the prefix to be concidered by the Winkler modification. 
         * Winkler's paper used a default value of 4
         */
        private static readonly int mNumChars = 4;


        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (perfect match) to 1 (no match). 
        /// </summary>
        /// <param name="aString1">First String</param>
        /// <param name="aString2">Second String</param>
        /// <returns></returns>
        public static double distance(string aString1, string aString2)
        {
            return 1.0 - proximity(aString1, aString2);
        }


        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (no match) to 1 (perfect match). 
        /// </summary>
        /// <param name="aString1">First String</param>
        /// <param name="aString2">Second String</param>
        /// <returns></returns>
        public static double proximity(string aString1, string aString2)
        {
            int lLen1 = aString1.Length;
            int lLen2 = aString2.Length;
            if (lLen1 == 0)
                return lLen2 == 0 ? 1.0 : 0.0;

            int lSearchRange = Math.Max(0, Math.Max(lLen1, lLen2) / 2 - 1);

            // default initialized to false
            bool[] lMatched1 = new bool[lLen1];
            bool[] lMatched2 = new bool[lLen2];

            int lNumCommon = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                int lStart = Math.Max(0, i - lSearchRange);
                int lEnd = Math.Min(i + lSearchRange + 1, lLen2);
                for (int j = lStart; j < lEnd; ++j)
                {
                    if (lMatched2[j]) continue;
                    if (aString1[i] != aString2[j])
                        continue;
                    lMatched1[i] = true;
                    lMatched2[j] = true;
                    ++lNumCommon;
                    break;
                }
            }
            if (lNumCommon == 0) return 0.0;

            int lNumHalfTransposed = 0;
            int k = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                if (!lMatched1[i]) continue;
                while (!lMatched2[k]) ++k;
                if (aString1[i] != aString2[k])
                    ++lNumHalfTransposed;
                ++k;
            }
            // System.Diagnostics.Debug.WriteLine("numHalfTransposed=" + numHalfTransposed);
            int lNumTransposed = lNumHalfTransposed / 2;

            // System.Diagnostics.Debug.WriteLine("numCommon=" + numCommon + " numTransposed=" + numTransposed);
            double lNumCommonD = lNumCommon;
            double lWeight = (lNumCommonD / lLen1
                             + lNumCommonD / lLen2
                             + (lNumCommon - lNumTransposed) / lNumCommonD) / 3.0;

            if (lWeight <= mWeightThreshold) return lWeight;
            int lMax = Math.Min(mNumChars, Math.Min(aString1.Length, aString2.Length));
            int lPos = 0;
            while (lPos < lMax && aString1[lPos] == aString2[lPos])
                ++lPos;
            if (lPos == 0) return lWeight;
            return lWeight + 0.1 * lPos * (1.0 - lWeight);

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

        /// <summary>
        /// Calculates the N-Gram similarity between two descriptions (which are just strings). Returns a similarity
        /// in range 0 - 1 with 1 being a perfect match
        /// </summary>
        public double nGramCompareDesc(Record r1, Record r2)
        {
            return nGramCompareDesc(r1.getDescription(), r2.getDescription());
        }

        /// <summary>
        /// The underlying method, which uses two strings to compare as opposed to records.
        /// would make it private but i need it for unit testing
        /// </summary>
        public double nGramCompareDesc(string s1, string s2)
        {
            NameParser parser = new NameParser();
            List<String> r1NGrams = parser.parseNGrams(s1, 3);
            List<String> r2NGrams = parser.parseNGrams(s2, 3);
            //TODO code reuse here
            double rc = IntersectNonDistinct(r1NGrams, r2NGrams);
            double r1c = r1NGrams.Count;
            double r2c = r2NGrams.Count;
            return (2* rc / (r1c + r2c));
        }

        private double IntersectNonDistinct(List<String> l1, List<String> l2)
        {
            //TODO do this for real, not just copied from stack overflow
            ILookup<String, String> lookup1 = l1.ToLookup(i => i);
            ILookup<String, String> lookup2 = l2.ToLookup(i => i);

            var result =
            (
                from group1 in l1.GroupBy(i => i)
                let group2 = lookup2[group1.Key]
                from i in (group1.Count() < group2.Count() ? group1 : group2)
                select i
            ).ToList();

            return result.Count();
        }
    }
}
