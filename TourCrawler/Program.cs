// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="pzcast">
//   (C) 2015 pzcast. All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleCrawler.Demo
{
    using TourParser;
    using SimpleCrawler;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;
    using global::TourCrawler;

    /// <summary>
    /// The program.
    /// </summary>
    internal class TourCrawler
    {
        #region Static Fields

        /// <summary>
        /// The settings.
        /// </summary>
        private static readonly CrawlSettings Settings = new CrawlSettings();

        /// <summary>
        /// The filter.
        /// 关于使用 Bloom 算法去除重复 URL： 
        /// </summary>
        private static BloomFilter<string> filter;
          #endregion
        private static Guid CrawlerId = Guid.NewGuid();
        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            filter = new BloomFilter<string>(200000);


            /*
            //设置种子地址和爬取规则
            Settings.SeedsAddress.Add(
                "http://www.ctszj.com.cn/ajax/soso_more_group.php?pageSize=99999&pageIndex=0"
                , new string[] { "route.php?gid=" }
                );
            */

            //Settings.Seeds.Add(
            //    new Seed
            //    {
            //        Name = "全国行政编码",
            //        StartUrl = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2015/index.html",
            //        FollowLinkRules = new List<FollowLinkRule>() {
            //            new FollowLinkRule(@"\d+\.html",true,"tr.citytr>td>a[href$='html'],tr.countytr>td>a[href$='html'],tr.towntr>td>a[href$='html'],tr.villagetr>td:nth-child(odd)"
            //             )
            //           },
            //        ContentParseRules="",

            //    }

            //    );

            //Settings.Seeds.Add(
            //  new Seed
            //  {
            //      Name = "全国邮政编码",

            //      StartUrl = "http://www.ip138.com/post/",
            //      FollowLinkRules = new List<FollowLinkRule>() {
            //            new FollowLinkRule(@"/(\d{2}|xianggang|aomen|taiwan)/",true,"table.t12>tbody>tr[bgcolor='#ffffff']"
            //             )
            //         },
            //      ContentParseRules = "",

            //  }

            //  );



            Settings.Seeds.Add(
              new Seed
              {
                  Name = "全国行政编码_邮编_区号_车牌",

                  StartUrl = "http://diqudaima.com/",
                  FollowLinkRules = new List<FollowLinkRule>() {
                      //第一级, 不需要分析内容.
                       new FollowLinkRule(@"(^/([^/]+?/)$)",false,string.Empty),
                        //第二级, 也不需要分析内容.

                        new FollowLinkRule(@"^/[^/]+?/[^/]+?/index\.html",true, "div.all3"),
                        //第三级, 
                          new FollowLinkRule(@"^/[^/]+?/[^/]+?/[^/]+?/$",true, "div.all2"),
                          //第四级别
                           new FollowLinkRule(@"^/address/\d+?\.html$",true, "div.all3")
                              //第五级别
                           //new FollowLinkRule(string.Empty,true,
                           //"div.all3>ul>li,div.all3>table>tbody"),

                     },
                  ContentParseRules = "",

              }

              );

            //Settings.Seeds.Add(
            //    new Seed
            //    {
            //        Name = "浙江省中国旅行社集团有限公司",
            //        StartUrl = "http://www.ctszj.com.cn/domestic_tours.php?page=1",
            //        FollowLinkRules = new List<FollowLinkRule>() {
            //            new FollowLinkRule(@"route\.php\?gid=",true,".cjxl>p"),
            //            new FollowLinkRule(@"page=",false) },
            //        ContentParseRules="",

            //    }

            //    );


            // 设置 URL 关键字


            // 设置爬取线程个数
            Settings.ThreadCount = 5;

            // 设置爬取深度
            Settings.Depth = 7;

            // 设置爬取时忽略的 Link，通过后缀名的方式，可以添加多个
            Settings.EscapeLinks.Add(".jpg");

            // 设置自动限速，1~5 秒随机间隔的自动限速
            Settings.AutoSpeedLimit = false;

            // 设置都是锁定域名,去除二级域名后，判断域名是否相等，相等则认为是同一个站点
            // 例如：mail.pzcast.com 和 www.pzcast.com
            Settings.LockHost = false;

            // 设置请求的 User-Agent HTTP 标头的值
            // settings.UserAgent 已提供默认值，如有特殊需求则自行设置

            // 设置请求页面的超时时间，默认值 15000 毫秒
            // settings.Timeout 按照自己的要求确定超时时间

            // 设置用于过滤的正则表达式
            // settings.RegularFilterExpressions.Add("");
            var master = new CrawlMaster(Settings);
            master.AddUrlEvent += MasterAddUrlEvent;
            //  master.DataReceivedEvent += MasterDataReceivedEventAreaCode;
            // master.DataReceivedEvent += MasterDataReceivedEventPostCode;
            master.DataReceivedEvent += MasterDataReceivedEventDiqubianma;
            master.Crawl();

            Console.ReadKey();
        }
        static int addedLinksAmount;
        /// <summary>
        /// The master add url event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool MasterAddUrlEvent(AddUrlEventArgs args)
        {
            if (!filter.Contains(args.Url))
            {
                filter.Add(args.Url);
                Console.WriteLine(addedLinksAmount + args.Url);
                addedLinksAmount++;
                return true;
            }

            return false; // 返回 false 代表：不添加到队列中
        }
        static int downloadedPageAmount;
        static string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "Json.txt";
        static string simpleFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "Plain.txt";
        /// <summary>
        /// The master data received event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void MasterDataReceivedEventAreaCode(DataReceivedEventArgs args)
        {

            Console.WriteLine(downloadedPageAmount + "下载完毕:" + args.Url);
            ITargetContentParser parser = new TargetContentParserAgilityPack(args.PraseSelector);
            if (args.NeedParseContent)
            {
               IList<string> parsedContentList = parser.Parse(args.Html);
                Dictionary<string, string> areaPaire = new Dictionary<string, string>();
                for(int i=1;i<=parsedContentList.Count;i=i+2)// string parsedContent in parsedContentList )
                {
                    
                    string key = parsedContentList[i - 1];
                    string value = parsedContentList[i];
                    if (value == "市辖区" || value == "县") { continue; }

                    areaPaire.Add(key, value);

              
                    //是否包含关键字
                }
                
                
                foreach (var item in areaPaire)
                {

                    string aa = string.Format("{{\"AreaCode\":\"{0}\",\"AreaName\":\"{1}\"}},", item.Key,item.Value)+Environment.NewLine;
                    
                    System.IO.File.AppendAllText(fileName, aa);
                    string simple = item.Key + " " + item.Value+Environment.NewLine;
                    System.IO.File.AppendAllText(simpleFileName, simple);

                }
               

            }


            downloadedPageAmount++;
            // 在此处解析页面，可以用类似于 HtmlAgilityPack（页面解析组件）的东东、也可以用正则表达式、还可以自己进行字符串分析
        }
        private static void MasterDataReceivedEventPostCode(DataReceivedEventArgs args)
        {

            Console.WriteLine(downloadedPageAmount + "下载完毕:" + args.Url);
            ITargetContentParser parser = new TargetContentParserAgilityPack(args.PraseSelector);
            if (args.NeedParseContent)
            {
                IList<string> parsedContentList = parser.Parse(args.Html);


                Dictionary<string, string> areaPaire = new Dictionary<string, string>();
                IList<PostCodeAndPhonePre> PostCodeResults = new List<PostCodeAndPhonePre>();
                for (int i = 0; i < parsedContentList.Count; i++)// string parsedContent in parsedContentList )
                {
                    string rowContent = parsedContentList[i];
                    ITargetContentParser parserdetail = new TargetContentParserAgilityPack("td");
                    MatchCollection matches = Regex.Matches(rowContent, "<td>(.+?)</td>");
                   
                    if (matches.Count == 4|| matches.Count==6)
                    {
                        
                        PostCodeAndPhonePre oneResult = new PostCodeAndPhonePre();
                        oneResult.AreaName = matches[0].Groups[1].Value;
                        oneResult.AreaPostCode =TargetContentParserAgilityPack.ParsePlainText(matches[1].Groups[1].Value);
                        oneResult.AreaPhonePre = TargetContentParserAgilityPack.ParsePlainText(matches[2].Groups[1].Value);
                        PostCodeResults.Add(oneResult);
                        if (matches.Count  == 6)
                        {
                            PostCodeAndPhonePre oneResult2 = new PostCodeAndPhonePre();
                            oneResult2.AreaName = matches[3].Groups[1].Value;
                            oneResult2.AreaPostCode = TargetContentParserAgilityPack.ParsePlainText(matches[4].Groups[1].Value);
                            oneResult2.AreaPhonePre = TargetContentParserAgilityPack.ParsePlainText(matches[5].Groups[1].Value);
                            PostCodeResults.Add(oneResult2);
                        }
                    }
                    


                    //是否包含关键字
                }


                foreach (var item in PostCodeResults)
                {

                    string aa = string.Format("{{\"AreaName\":\"{0}\",\"PostCode\":\"{1}\",,\"PhoneCode\":\"{2}\"}},", item.AreaName, item.AreaPostCode,item.AreaPhonePre) + Environment.NewLine;

                    System.IO.File.AppendAllText(fileName, aa);
                    string simple = item.AreaName + " " + item.AreaPostCode+"  "+item.AreaPhonePre + Environment.NewLine;
                    System.IO.File.AppendAllText(simpleFileName, simple);

                }


            }


            downloadedPageAmount++;
            // 在此处解析页面，可以用类似于 HtmlAgilityPack（页面解析组件）的东东、也可以用正则表达式、还可以自己进行字符串分析
        }

        private static void MasterDataReceivedEventDiqubianma(DataReceivedEventArgs args)
        {

            Console.WriteLine(downloadedPageAmount + "下载完毕:" + args.Url);
            ITargetContentParser parser = new TargetContentParserAgilityPack(args.PraseSelector);
            if (args.NeedParseContent)
            {
                IList<string> parsedContentList = parser.Parse(args.Html);

 
                for (int i = 0; i < parsedContentList.Count; i++)// string parsedContent in parsedContentList )
                {
                    string parsedContent = parsedContentList[i]
                        +Environment.NewLine+"---------------------------------------------------------"+Environment.NewLine;

                   
                    System.IO.File.AppendAllText(fileName, parsedContent);
                    

                }


            }


            downloadedPageAmount++;
            // 在此处解析页面，可以用类似于 HtmlAgilityPack（页面解析组件）的东东、也可以用正则表达式、还可以自己进行字符串分析
        }

        public class PostCodeAndPhonePre
        {
            public string AreaName { get; set; }
            public string AreaPostCode { get; set; }
            public string AreaPhonePre { get; set; }
        }
        #endregion
    }
}