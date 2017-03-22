﻿using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace DiqudaimaParse
{

    public interface SectionParser
    {
        IList<AreaPostPhone> Parse(string sectionContent);
    }

    //直辖市地区, 非直辖市城市
    public class SectionParser_Level1 : SectionParser
    {

        string re_areaname = "";
        string re_phone = "(?<=<strong>长途区号</strong>:).+?(?=</td>)";
        string re_post = "(?<=<strong>邮政编码</strong>:).+?(?=</td>)";
        string re_area = "(?<=<strong>行政代码</strong>:).+?(?=</td>)";
        string re_table = "<div align=\"center\"><h1>(.+?)介绍.+?<strong>长途区号</strong>:(.+?)</td>.+<strong>邮政编码</strong>:(.+?)</td>.+<strong>行政代码</strong>:(.+?)</td>";
        string angle_selector_child = "ul>li";
        string child_re_vallege = @"(?<=<li>)(.+?)\[(\d+)(?=\]</li>)";

        public SectionParser_Level1()
        {

        }
        HtmlParser parser = new HtmlParser();
        public IList<AreaPostPhone> Parse(string sectionContent)
        {
            IList<AreaPostPhone> results = new List<AreaPostPhone>();
            string areaname = "", areacode = "", phonecode = "", postcode = "";
            var mTable = Regex.Match(sectionContent, re_table, RegexOptions.Singleline);

            if (mTable.Success)
            {
                areaname = mTable.Groups[1].Value;
                phonecode = mTable.Groups[2].Value;
                postcode = mTable.Groups[3].Value;
                areacode = mTable.Groups[4].Value;
                //results.Add(new AreaPostPhone { AreaCode = areacode, Name = areaname, PhoneCode = phonecode, PostCode = postcode });

            }

            var matches = Regex.Matches(sectionContent, "--&gt;");
            //如果是乡镇
            if (matches.Count == 4)
            {

                var vallegeMatches = Regex.Matches(sectionContent, child_re_vallege);
                foreach (Match m in vallegeMatches)
                {
                    string vallege_name = m.Groups[1].Value;
                    string vallege_code = m.Groups[2].Value;

                    AreaPostPhone vallege = new AreaPostPhone { Name = vallege_name, AreaCode = vallege_code };
                    if (!string.IsNullOrEmpty(phonecode))
                    {
                        vallege.PhoneCode = phonecode;
                        vallege.PostCode = postcode;
                    }
                    results.Add(vallege);

                }


            }
            return results;
        }
    }
    //直辖市街道, 非直辖市 区,县
    public class SectionParser_Level2 : SectionParser
    {

        string re_areaname = "";
        string re_phone = "(?<=<strong>长途区号</strong>:).+?(?=</td>)";
        string re_post = "(?<=<strong>邮政编码</strong>:).+?(?=</td>)";
        string re_area = "(?<=<strong>行政代码</strong>:).+?(?=</td>)";
        string re_table = "<div align=\"center\"><h1>(.+?)介绍.+?<strong>长途区号</strong>:(.+?)</td>.+<strong>邮政编码</strong>:(.+?)</td>.+<strong>行政代码</strong>:(.+?)</td>";
        string angle_selector_child = "ul>li";
        string child_re_vallege = @"(?<=<li>)(.+?)\[(\d+)(?=\]</li>)";
        string child_three_split = "(?<=\\d+\\.html\">)(.+?)</a>\\[(\\d+)(?=\\]</li>)";
        string child_two_split = "target=\"_blank\">(.+?)</a> 地区编码：(\\d+)? 邮编：(\\d+)? 电话区号：(\\d+)?</li>";

        public SectionParser_Level2()
        {

        }
        HtmlParser parser = new HtmlParser();
        public IList<AreaPostPhone> Parse(string sectionContent)
        {
            List<AreaPostPhone> results = new List<AreaPostPhone>();
            string areaname = "", areacode = "", phonecode = "", postcode = "";
            var matches = Regex.Matches(sectionContent, "--&gt;");//层级判断
            var mTable = Regex.Match(sectionContent, re_table, RegexOptions.Singleline);

            if (mTable.Success)
            {
                areaname = mTable.Groups[1].Value;
                phonecode = mTable.Groups[2].Value;
                postcode = mTable.Groups[3].Value;
                areacode = mTable.Groups[4].Value;


            }
            if (matches.Count == 2)
            {
                var twosplit = Regex.Matches(sectionContent, child_re_vallege);



                var matchesSplit2 = Regex.Matches(sectionContent, child_two_split);
                IList<AreaPostPhone> level2List = new List<AreaPostPhone>();
                foreach (Match m2 in matchesSplit2)
                {

                    level2List.Add(new AreaPostPhone
                    {
                        AreaCode = m2.Groups[2].Value
                        ,
                        Name = m2.Groups[1].Value,
                        PhoneCode = m2.Groups[4].Value,
                        PostCode = m2.Groups[3].Value
                    });
                }
                if (phonecode == "0")
                {
                    if (level2List.Count > 0)
                    {
                        phonecode = level2List[0].PhoneCode;
                        postcode = level2List[0].PostCode;
                        areacode = level2List[0].AreaCode.Substring(0, 4) + "00";
                    }
                    else
                    {
                        phonecode = postcode = "";
                    }
                }
                level2List.Add(new AreaPostPhone { AreaCode = areacode, Name = areaname, PhoneCode = phonecode, PostCode = postcode });
                results.AddRange(level2List);
            }


            //如果是乡镇
            if (matches.Count == 4 || matches.Count == 3)
            {
                string re3or4 = child_three_split;
                if (matches.Count == 4)
                {
                    re3or4 = child_re_vallege;
                }


                var vallegeMatches = Regex.Matches(sectionContent, re3or4);
                foreach (Match m in vallegeMatches)
                {
                    string vallege_name = m.Groups[1].Value;



                    string vallege_code = m.Groups[2].Value;

                    AreaPostPhone vallege = new AreaPostPhone { Name = vallege_name, AreaCode = vallege_code };
                    if (string.IsNullOrEmpty(phonecode) || phonecode == "0")
                    {
                        phonecode = postcode = "";
                    }
                    
                    vallege.PhoneCode = phonecode;
                    vallege.PostCode = postcode;
                    results.Add(vallege);

                }


            }
            return results;
        }
        public string AdjustName(string rawName)
        {/*
            江苏省徐州市贾汪区鹿庄粮棉原种场
            江苏省徐州市贾汪区青山泉镇
            江苏省徐州市鼓楼区丰财街道
            江苏省常州市武进区横林镇前丰村村委会
            江苏省常州市武进区芙蓉镇芙蓉社区居委会
            四川省甘孜县泥柯乡
            村委会 村名委员会 居名委员会 居委会

            省 县 乡 村
            省 县 镇 村
            省 县 镇 居委会
            省 县 乡 村委会
            省 县 乡 村 村委会
            省 市 乡 村委会
            省 市 镇 村委会
            省 市 区 街道 居委会
            自治区 市 办事处
            自治区 县 镇 村委会
            自治区 县 镇 居委会
            自治区 市 区 镇 村委会
            自治区 市 区 办事处 居委会
            自治区 市 区 镇 村委会
            自治区 市 乡   村委会
            自治区 市 办事处 社区
             */
            string adjusted = rawName;

            //去除上级


            return adjusted;
        }
    }
    //乡,镇
   
    public class AreaPostPhone
    {
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public string PostCode { get; set; }
        public string PhoneCode { get; set; }
    }
}
