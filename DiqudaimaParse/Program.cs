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
            log4net.Config.XmlConfigurator.Configure();
            log4net.ILog log = log4net.LogManager.GetLogger("CodeResult");
           
            string[] childdata = Directory.GetFiles(@"..\..\datas");
            foreach (string data in childdata)
            { 
                string filepath =   data;// 20170319100342Json.txt";
            string all = File.ReadAllText(filepath,Encoding.Default);
            string[] sections = all.Split(new string[] { "-------------------" }, StringSplitOptions.RemoveEmptyEntries);

            List<AreaPostPhone> results = new List<AreaPostPhone>();
            SectionParser_Level2 parser = new SectionParser_Level2();
            foreach (string s in sections)
            {
                IList<AreaPostPhone> section_results = parser.Parse(s);
                results.AddRange(section_results);


            }

            foreach (AreaPostPhone a in results)
            {
                    string printName=a.Name, printAreaCode=a.AreaCode;
                    if (a.AreaCode.Length == 6)
                    {
                        printAreaCode = a.AreaCode + "000000";
                    }
                    if (a.AreaCode.Length == 12)
                    {
                        printName = ShortenName(a.Name, a.AreaCode, results);
                    }

                    log.Debug(string.Format(
                       "{{\"AreaName\":\"{0}\",\"AreaCode\":\"{1}\",\"PostCode\":\"{2}\",\"ZoneCode\":\"{3}\"}}",
                     printName, printAreaCode, a.PostCode, a.PhoneCode) + ",");
              
            }
            }
        }
        //根据areacode的关联性, 获得上一级areacode,继而获得上一级名称, 从自身名称中移除
        static string ShortenName(string rawAreaName, string areacode, IList<AreaPostPhone> wholeList)
        {
            bool isLevel3;
            string shortenedName = rawAreaName;
            string parentCode = GetParentAreaCode(areacode,out isLevel3);
           
                string parentName;
                    var a= wholeList.Where(x => x.AreaCode == parentCode);
                if (a.Count() != 1)
                {
                    return "(无父级)"+shortenedName;
                }
                else
                {
                    parentName = a.ToArray()[0].Name;
                }
                if (isLevel3)
                {
                    shortenedName = rawAreaName.Substring(rawAreaName.IndexOf(parentName) + parentName.Length);
                }
                else
                {
                    shortenedName = rawAreaName.Replace(parentName, string.Empty);
                }
                return shortenedName;
            
        }
        static string GetParentAreaCode(string areacode,out bool isLevel3)
        {
            string parentCode = areacode;
            isLevel3 = false;
            if (areacode.EndsWith("000"))
            {
                parentCode = areacode.Substring(0, 6);
                isLevel3 = true;
            }
            else
            {
                parentCode = areacode.Substring(0,9) + "000";
            }
            return parentCode;
            
        }
    }
}
