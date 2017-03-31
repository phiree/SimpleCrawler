using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DiqudaimaParse
{
    /// <summary>
    /// 分析 从 diqudaima网站下载的数据.
    /// </summary>
    class Program
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Craw.JsonData");
        static log4net.ILog logRaw = log4net.LogManager.GetLogger("Craw.JsonDataRaw");
        static  log4net.ILog logWe = log4net.LogManager.GetLogger("Craw.WarnError");
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
          

            string[] childdata = Directory.GetFiles(@"..\..\datas");
            foreach (string data in childdata)
            { 
                string filepath =   data;// 20170319100342Json.txt";
            string all = File.ReadAllText(filepath,Encoding.Default);
            string[] sections = all.Split(new string[] { "-------------------" }, StringSplitOptions.RemoveEmptyEntries);

            List<AreaPostPhone> results = new List<AreaPostPhone>();
                List<AreaPostPhone> resultsCopy = new List<AreaPostPhone>();
            SectionParser_Level2 parser = new SectionParser_Level2();
            foreach (string s in sections)
            {
                IList<AreaPostPhone> section_results = parser.Parse(s);
                results.AddRange(section_results);
                    resultsCopy.AddRange(section_results);

            }
               

            foreach (AreaPostPhone a in resultsCopy)
            {
                    
                    string fullAreaCode = a.AreaCode;
                    if (a.AreaCode.Length == 12)
                    {
                        
                    }
                    else
                    {
                        fullAreaCode = a.AreaCode + "000000";
                    }
                    string shortName = AdjustName(a.Name,a.AreaCode);

                    log.Debug(string.Format(
                       "{{\"AreaName\":\"{0}\",\"AreaCode\":\"{1}\",\"PostCode\":\"{2}\",\"ZoneCode\":\"{3}\"}}",
                    shortName, fullAreaCode, a.PostCode, a.PhoneCode) + ",");
                    logRaw.Debug(string.Format(
                       "{{\"AreaName\":\"{0}\",\"AreaCode\":\"{1}\",\"PostCode\":\"{2}\",\"ZoneCode\":\"{3}\"}}",
                    a.Name, a.AreaCode, a.PostCode, a.PhoneCode) + ",");

                }
            }
        }
        static string AdjustName(string rawName,string rawAreaCode)
        {/*
            江苏省徐州市贾汪区鹿庄粮棉原种场
            江苏省徐州市贾汪区青山泉镇
            江苏省徐州市鼓楼区丰财街道
            江苏省常州市武进区横林镇前丰村村委会
            江苏省常州市武进区芙蓉镇芙蓉社区居委会
            四川省甘孜县泥柯乡
             */
         /*(.+?省)?(.+?市)?(.+?市|县|区)?(.+?(镇|街道办事处|街道|乡))?(.+?(村委会|居委会|居民委员会|村民委员会|场|地区办事处|村村委会))?*/

            string rep = "(?<Province>.+?省|.+?自治区)?(?<City>^[县镇]+?市)?(?<District>[^社库管理]+?区)?(?<Country>.+?县)?(?<Town>.+?镇|.+?街道办事处|.+?街道|.+?乡)?(?<Village>.+?渔委会|.+?经济开发试验区|.+?开发区|.+?社区居委会|.+?社区|.+?村委会|.+?居委会|.+?居民委员会|.+?村民委员会|.+?自然保护区|.+?场|.+?地区办事处|.+?村村委会|.+?村委|.+?村|.+?监狱|.+?经营所|.+?作业区|.+?管理区|.+?风景区|.+?公园|.+?矿)?";

            var matches = Regex.Matches(rawName, rep);

            bool isCityOrDistrictOrCountry = rawAreaCode.Length == 6;
            bool isTown = rawAreaCode.EndsWith("000");
            bool isVillage = rawAreaCode.Length == 12 && !rawAreaCode.EndsWith("000");

            foreach (Match m in matches)
            {
                var gpProvince = m.Groups["Province"];
                var gpCity = m.Groups["City"];
                var gpCountry = m.Groups["Country"];
                var gpTown = m.Groups["Town"];
                var gpVillage = m.Groups["Village"];
                if (gpVillage != null && gpVillage.Value != "")
                {
                    if (gpProvince == null)
                    {
                        logWe.Error("village without province,ignore:" + rawName + "," + rawAreaCode);
                        break;
                    }
                    if (!isVillage)
                    {
                        logWe.Error("should not be village,but is:" + rawName + "," + rawAreaCode);
                    }
                    return gpVillage.Value;

                }
                if (gpTown != null && gpTown.Value != "")
                {
                    if (!isTown)
                    {
                        logWe.Error("should not be town,but is:" + rawName + "," + rawAreaCode);
                    }
                 
                    return gpTown.Value;
                }
                if (gpCountry != null && gpCountry.Value != "")
                {
                    if (!isCityOrDistrictOrCountry)
                    {
                        logWe.Error("should not be Country,but is:" + rawName + "," + rawAreaCode);
                    }
                    return gpCountry.Value;
                }
                if (gpCity != null && gpCity.Value != "")
                {
                    if (!isCityOrDistrictOrCountry)
                    {
                        logWe.Error("should not be city,but is:" + rawName + "," + rawAreaCode);
                    }
                    return gpCity.Value;
                }


            }
            logWe.Warn("匹配失败:" + rawName);
            return rawName;



        }
        static string ShortenName( string rawName,string areaCode,  List<AreaPostPhone> results)
        {
            string parentCode = string.Empty;
            if (areaCode.EndsWith("000"))
            {
                parentCode = areaCode.Substring(0, 6) + "000000";
            }
            else
            {
                parentCode = areaCode.Substring(0, 9) + "000";
            }
            var parentItemResult = results.Where(x => x.AreaCode == parentCode);
            if (parentItemResult.Count() == 0)
            {
                return "(无父级)" + rawName;
            }
            else if (parentItemResult.Count() > 1)
            {
                return "(多父级)" + rawName;
            }
            else
            {
                string parentName = parentItemResult.First().Name;
                return rawName.Replace(parentName, string.Empty);
            }
        }
        
    }
}
