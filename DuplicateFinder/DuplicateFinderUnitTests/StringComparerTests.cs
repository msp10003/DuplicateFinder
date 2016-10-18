using System;
using DuplicateFinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DuplicateFinderUnitTests
{
    [TestClass]
    public class StringComparerTests
    {
        [TestMethod]
        public void jaroWinklerTest()
        {
            DuplicateFinder.StringComparer strComp = new DuplicateFinder.StringComparer();
            double sim = DuplicateFinder.StringComparer.proximity("CAARTER AMY", "CARTER AMY J");
        }

        [TestMethod]
        public void CompareDescriptionTest()
        {
            DuplicateFinder.StringComparer strComp = new DuplicateFinder.StringComparer();
            string desc1 = "string azfg cvs zxae utr 43 and some random text";
            string desc2 = "sttah cvs zxae ufgjeitr ertpo random bext";
            double sim = strComp.nGramCompareDesc(desc1, desc2);
            double x = sim;
        }

    }
}
