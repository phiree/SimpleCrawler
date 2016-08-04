// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlInfo.cs" company="pzcast">
//   (C) 2015 pzcast. All rights reserved.
// </copyright>
// <summary>
//   The url info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
namespace SimpleCrawler
{
    
    /// <summary>
    /// 跟踪的链接。
    /// </summary>
    public class FollowLinkRule
    {
        #region Fields

        /// <summary>
        /// The url.
        /// </summary>
        private string rule;
        public   string Rule { get { return rule; } }
        //是否是需要提取数据的页面
        private bool needParseContent;
        public bool NeedParseContent {
            get { return needParseContent; }
        }
        private string parseSelector;
        public string ParseSelector { get { return parseSelector; } }

        #endregion

        public FollowLinkRule(string rule, bool needParseContent ): this(rule, needParseContent,string.Empty)
        {

        }
        /// </param>
        public FollowLinkRule(string rule, bool needParseContent,string parseSelector)
        {
            this.rule = rule;
            this.needParseContent = needParseContent;
            if (this.needParseContent == true)
            {
                if (string.IsNullOrEmpty(parseSelector))
                {
                    throw new System.Exception("需要分析内容的网页必须提供提取规则");
                }
                this.parseSelector = parseSelector;
            }
            
            
        }
 
    }
}