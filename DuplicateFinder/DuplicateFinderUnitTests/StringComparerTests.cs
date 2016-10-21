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
            double sim = DuplicateFinder.StringComparer.proximity("DUSTIN JOE", "DUSTIN" );
            double x = sim;
        }

        [TestMethod]
        public void CompareDescriptionTest()
        {
            DuplicateFinder.StringComparer strComp = new DuplicateFinder.StringComparer();
            string desc1 = "Fell asleep after surgery";
            string desc2 = "Went to sleep aeftyzt the surgery.";
            double sim = strComp.nGramCompareDesc(desc1, desc2);
            double x = sim;
        }

    }
}
