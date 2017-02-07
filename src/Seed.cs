using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCrawler
{
    /// <summary>
    /// 种子信息
    /// </summary>
    public class Seed
    {
        public Guid SeedId { get; set; }
        public string Name { get; set; }
        public string StartUrl { get; set; }
        /// <summary>
        /// 链接跟踪规则
        /// </summary>
        public IList<FollowLinkRule> FollowLinkRules { get; set; }
        
        /// <summary>
        /// 内容提取规则。
        /// </summary>
        public string ContentParseRules { get; set; }
    }
}
