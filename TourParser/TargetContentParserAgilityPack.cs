using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using AngleSharp;
using AngleSharp.Parser.Html;

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
         public string  Parse(string rawHtml)
        {
            var result = string.Empty;
            
            //  doc.DocumentNode.SelectSingleNode("//")
            var parser = new HtmlParser();
            var doc = parser.Parse(rawHtml);
            var cssSelector = doc.QuerySelector(parseRule);
            result=cssSelector. TextContent;
            return result;
        }

 
    }
    
}
