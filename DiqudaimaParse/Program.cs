using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DiqudaimaParse
{
    /// <summary>
    /// 分析 从 diqudaima网站下载的数据.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string savedFileName = DateTime.Now.ToString("yyyyMMddhhmmss")+".txt";
            string[] childdata = Directory.GetFiles(@"D:\Code\SimpleCrawler\DiqudaimaParse\datas");
            foreach (string data in childdata)
            { 
                string filepath =   data;// 20170319100342Json.txt";
            string all = File.ReadAllText(filepath);
            string[] sections = all.Split(new string[] { "---------------------------------------------------------" }, StringSplitOptions.RemoveEmptyEntries);

            List<AreaPostPhone> results = new List<AreaPostPhone>();
            SectionParser_Level2 parser = new SectionParser_Level2();
            foreach (string s in sections)
            {
                IList<AreaPostPhone> section_results = parser.Parse(s);
                results.AddRange(section_results);


            }

            foreach (AreaPostPhone a in results)
            {
                File.AppendAllText(savedFileName
                    , string.Format("{{\"AreaName\":\"{0}\",\"AreaCode\":\"{1}\",\"PostCode\":\"{2}\",\"ZoneCode\":\"{3}\"}}",
                    a.Name, a.AreaCode, a.PostCode, a.PhoneCode) + "," + Environment.NewLine );
            }
            }
        }
    }
}
