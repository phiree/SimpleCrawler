using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiqudaimaParse;
using System.IO;
namespace Tests
{
    [TestFixture()]
    public class SectionParser_Level1Tests
    {
        string test_data_base_uri = @"D:\Code\SimpleCrawler\DiqudaimaParseTests\testdata\";
        [Test()]
        public void ParseTest_NotVallege()
        {
            string context = File.ReadAllText(test_data_base_uri+"北京市-县.txt");
            SectionParser_Level2 sp = new SectionParser_Level2();
           var results= sp.Parse(context);
            Assert.AreEqual(3,results.Count);
            Assert.AreEqual("010", results[1].PhoneCode);
            Assert.AreEqual("101500", results[1].PostCode);
            Assert.AreEqual("110228", results[1].AreaCode);
        }
        [Test()]
        public void ParseTest_NotVallege2()
        {
            string context = File.ReadAllText(test_data_base_uri + "湖南湘潭.txt");
            SectionParser_Level2 sp = new SectionParser_Level2();
            var results = sp.Parse(context);
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("0732", results[0].PhoneCode);
            Assert.AreEqual("411100", results[0].PostCode);
            Assert.AreEqual("430381", results[0].AreaCode);

            Assert.AreEqual("0732", results[5].PhoneCode);
            Assert.AreEqual("411300", results[5].PostCode);
            Assert.AreEqual("430382", results[5].AreaCode);
        }
        [Test()]
        public void ParseTest_Vallege()
        {
            string context = File.ReadAllText(test_data_base_uri + "河北省石家庄市长安区建北街道办.txt");
            SectionParser_Level2 sp = new SectionParser_Level2();
            var results = sp.Parse(context);
            Assert.AreEqual(7, results.Count);
            Assert.AreEqual("130102001001", results[0].AreaCode);
            Assert.AreEqual("130102001012", results[6].AreaCode);
 
        }
        [Test]
        public void ParseTest_Vallege2()
        {
            string context = File.ReadAllText(test_data_base_uri + "湖南省-衡阳市-衡东县.txt");
            SectionParser_Level2 sp = new SectionParser_Level2();
            var results = sp.Parse(context);
            Assert.AreEqual(24, results.Count);
            Assert.AreEqual("430424100000", results[0].AreaCode);
            Assert.AreEqual("421000", results[0].PostCode);
            Assert.AreEqual("430424212000", results[23].AreaCode);
            Assert.AreEqual("0734", results[23].PhoneCode);

        }
    }
}