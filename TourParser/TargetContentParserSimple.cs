using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TourParser
{
    /// <summary>
    /// 从页面提取需要分析的内容。
    /// </summary>
   public class TargetContentParserSimple:ITargetContentParser
    {
        
         public string  Parse(string rawHtml)
        {
            var result = string.Empty;
            result = rawHtml.Substring(0, 100);
            return result;
        }

 
    }
    
}
