﻿// --------------------------------------------------------------------------------------------------------------------
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
            
            Settings.Seeds.Add(
                new Seed
                {
                    Name = "山东",
                    StartUrl = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2015/index.html",
                    FollowLinkRules = new List<FollowLinkRule>() {
                        new FollowLinkRule(@"\d+\.html",true,"tr.citytr>td>a[href$='html'],tr.countytr>td>a[href$='html'],tr.towntr>td>a[href$='html'],tr.villagetr>td>a[href$='html']"
                         )
                       },
                    ContentParseRules="",
                   
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
            master.DataReceivedEvent += MasterDataReceivedEvent;
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
        static string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
        /// <summary>
        /// The master data received event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void MasterDataReceivedEvent(DataReceivedEventArgs args)
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
                    areaPaire.Add(key, value);

              
                    //是否包含关键字
                }
                
                
                foreach (var item in areaPaire)
                {

                    string aa = string.Format("{{AreaCode:\"{0}\",AreaName:\"{1}\"}},", item.Key,item.Value)+Environment.NewLine;

                    System.IO.File.AppendAllText(fileName, aa);

                }
               

            }


            downloadedPageAmount++;
            // 在此处解析页面，可以用类似于 HtmlAgilityPack（页面解析组件）的东东、也可以用正则表达式、还可以自己进行字符串分析
        }

        #endregion
    }
}