﻿using System;
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
                    log.Debug(string.Format(
                       "{{\"AreaName\":\"{0}\",\"AreaCode\":\"{1}\",\"PostCode\":\"{2}\",\"ZoneCode\":\"{3}\"}}",
                     a.Name, a.AreaCode, a.PostCode, a.PhoneCode) + ",");
              
            }
            }
        }
    }
}
