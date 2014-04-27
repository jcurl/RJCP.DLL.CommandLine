// $URL$
// $Id$
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RJCP.Core.Datastructures
{
    /// <summary>
    /// Summary description for ReadOnlyDictionary
    /// </summary>
    [TestClass]
    public class ReadOnlyDictionaryTest
    {
        private Dictionary<int, string> GenerateDict()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "One");
            dict.Add(2, "Two");
            dict.Add(5, "Five");
            dict.Add(10, "Ten");
            dict.Add(20, "Twenty");
            dict.Add(50, "Fifty");
            dict.Add(100, "One-Hundred");
            dict.Add(200, "Two-Hundred");
            dict.Add(500, "Five-Hundred");

            return dict;
        }

        [TestMethod]
        [TestCategory("Datastructures.ReadOnlyDictionary")]
        public void RODict_Instantiate()
        {
            Dictionary<int, string> dict = GenerateDict();
            ReadOnlyDictionary<int, string> rdict = new ReadOnlyDictionary<int, string>(dict);

            Assert.IsTrue(rdict.IsReadOnly, "ReadOnlyDictionary is not Read-Only");

            Assert.AreEqual(dict.Count, rdict.Count);
            Assert.AreEqual(dict.Keys.Count, rdict.Keys.Count);
            Assert.AreEqual(dict.Values.Count, rdict.Values.Count);

            foreach (KeyValuePair<int, string> kvp in dict) {
                Assert.AreEqual(dict[kvp.Key], rdict[kvp.Key]);
            }
        }

        [TestMethod]
        [TestCategory("Datastructures.ReadOnlyDictionary")]
        public void RODict_Methods()
        {
            Dictionary<int, string> dict = GenerateDict();
            ReadOnlyDictionary<int, string> rdict = new ReadOnlyDictionary<int, string>(dict);

            bool err;
            err = false;
            try { rdict.Add(new KeyValuePair<int, string>(90, "Ninety")); }
            catch { err = true; }
            Assert.IsTrue(err, "Add(KVP) was writable for a readonly dictionary");

            err = false;
            try { rdict.Add(99, "Ninety-Nine"); }
            catch { err = true; }
            Assert.IsTrue(err, "Add(key, value) was writable for a readonly dictionary");

            err = false;
            try { rdict.Clear(); }
            catch { err = true; }
            Assert.IsTrue(err, "Clear() was successful for a readonly dictionary");

            err = false;
            try { rdict.Remove(10); }
            catch { err = true; }
            Assert.IsTrue(err, "Remove(key) was successful for a readonly dictionary");
        }

        [TestMethod]
        [TestCategory("Datastructures.ReadOnlyDictionary")]
        public void RODict_Keys()
        {
            Dictionary<int, string> dict = GenerateDict();
            ReadOnlyDictionary<int, string> rdict = new ReadOnlyDictionary<int, string>(dict);

            bool err;
            err = false;
            try { rdict.Keys.Add(51); }
            catch { err = true; }
            Assert.IsTrue(err, "Keys.Add(Key) was writable for a readonly dictionary");

            err = false;
            try { rdict.Keys.Clear(); }
            catch { err = true; }
            Assert.IsTrue(err, "Keys.Clear() was successful for a readonly dictionary");

            err = false;
            try { rdict.Keys.Remove(50); }
            catch { err = true; }
            Assert.IsTrue(err, "Keys.Remove(Key) was successful for a readonly dictionary");

            Assert.IsTrue(rdict.Keys.IsReadOnly, "ReadOnlyDictionary.Keys is not readonly");
        }

        [TestMethod]
        [TestCategory("Datastructures.ReadOnlyDictionary")]
        public void RODict_Values()
        {
            Dictionary<int, string> dict = GenerateDict();
            ReadOnlyDictionary<int, string> rdict = new ReadOnlyDictionary<int, string>(dict);

            bool err;
            err = false;
            try { rdict.Values.Add("Fifty-one"); }
            catch { err = true; }
            Assert.IsTrue(err, "Values.Add(Key) was writable for a readonly dictionary");

            err = false;
            try { rdict.Values.Clear(); }
            catch { err = true; }
            Assert.IsTrue(err, "Values.Clear() was successful for a readonly dictionary");

            err = false;
            try { rdict.Values.Remove("Fifty"); }
            catch { err = true; }
            Assert.IsTrue(err, "Values.Remove(Key) was successful for a readonly dictionary");

            Assert.IsTrue(rdict.Values.IsReadOnly, "ReadOnlyDictionary.Values is not readonly");
        }
    }
}
