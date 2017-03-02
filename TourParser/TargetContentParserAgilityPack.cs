using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AngleSharp;
using AngleSharp.Parser.Html;
using AngleSharp.Dom;

namespace TourParser
{
    /// <summary>
    /// 从页面提取需要分析的内容。
    /// </summary>
   public class TargetContentParserAgilityPack : ITargetContentParser
    {

        string parseRule;
        public TargetContentParserAgilityPack(string parseRule)
        {
            this.parseRule = parseRule;
        }
        public static string ParsePlainText(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, "<[^>]*>", string.Empty);
        }
         public IList<string> Parse(string rawHtml)
        {
            IList<string> result;

            //  doc.DocumentNode.SelectSingleNode("//")
            var parser = new HtmlParser();
            var doc = parser.Parse(rawHtml);
            var cssSelector = doc.QuerySelectorAll(parseRule);
            result = cssSelector.Select(x => x.InnerHtml).ToList();
            return result;
        }

 
    }
    
}
