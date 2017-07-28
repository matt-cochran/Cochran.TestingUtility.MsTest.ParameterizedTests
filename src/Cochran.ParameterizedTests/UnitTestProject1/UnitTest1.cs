using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public static IEnumerable<Object[]> OtherParams()
        {
            var result = from r in Enumerable.Range(0, 10)
                         select new Object[] { 5, r };

            return result;
        }

        public static IEnumerable<Object[]> Params()
        {
            yield return new Object[] { 1, 3 };
            yield return new Object[] { 2, 3 };
            yield return new Object[] { 3, 3 };
            yield return new Object[] { 4, 3 };
            yield return new Object[] { 5, 3 };
            yield return new Object[] { 6, 3 };
        }


        [ParameterizedTest]
        [ParameterizedTestSource("Params")]
        [ParameterizedTestSource("OtherParams")]
        public void Test(Int32 expected, Int32 actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [ParameterizedTest]
        [ParameterizedTestSource("Invalid")]
        public void Test2(Int32 expected, Int32 actual)
        {
            Assert.AreEqual(expected, actual);
        }
    }
}