using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Core.Db;
using TaskManager.Core.Log;
using TaskManager.Core.Seo;
using TaskManager.Core.Seo.SiteMap;
using TaskManager.Core.Utility;
using TaskManager.Model;

namespace TaskManager.Job.SeoSiteMap
{
    public class NewsSiteMap : ISiteMap
    {
        public List<SiteMap> GetSiteMapList()
        {
            var url = "www.abc.com";
            var lastMaxNewsIdXmlPath = "root/LastMaxNewsId";
            var xmlPath = $@"{AppDomain.CurrentDomain.BaseDirectory}Config\AppConfig.xml";
            var lastMaxNewsId = XmlHelper.GetNodeInnerText(xmlPath, lastMaxNewsIdXmlPath);

            var resultSiteMap = new List<SiteMap>();
            var now = DateTime.Now.ToString("yyyy-MM-dd");

            var sql = "select * from News_Article where StateFlag=1";

            var lastMaxNewsIdInt = 0;
            int.TryParse(lastMaxNewsId, out lastMaxNewsIdInt);
            if (lastMaxNewsIdInt > 0)
            {
                sql += $" and id >{lastMaxNewsIdInt}";
            }
            else
            {
                var listUrl = CommonUrl.GetNewsListUrl(url);

                resultSiteMap.AddRange(listUrl.Select(s => new SiteMap() { Date = now, Type = 1.0, Url = s }));
            }


            var articles = DapperHelper<News_Article>.Query(sql, null);

            if (articles == null || articles.Count <= 0)
            {
                return resultSiteMap;
            }

            XmlHelper.UpdateNodeInnerText(xmlPath, lastMaxNewsIdXmlPath, articles.Max(m=>m.Id).ToString());

            resultSiteMap.AddRange(articles.Select(s => new SiteMap()
            {
                Type = 0.8,
                Url = string.Format("http://{0}/zixun/{1}.html", url, s.Id)
            }).ToList());

            return resultSiteMap;
   
        }
    }
}
