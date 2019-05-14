using log4net.Config;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Db;
using TaskManager.Core.Log;
using TaskManager.Core.Seo;
using TaskManager.Core.Utility;
using TaskManager.Model;

namespace TaskManager.Job.SeoTuiSong
{

    class Program
    {
        static void Main(string[] args)
        {
            var xmlPath = $@"{AppDomain.CurrentDomain.BaseDirectory}Config\AppConfig.xml";
            var lastMaxNewsIdXmlPath = "root/LastMaxNewsId";
            var url = "www.abc.com";

            var sql = "select * from News_Article where StateFlag=1";
            var posturls = new List<string>();
            var lastMaxNewsId = XmlHelper.GetNodeInnerText(xmlPath, lastMaxNewsIdXmlPath);
            var lastMaxNewsIdInt = 0;
            int.TryParse(lastMaxNewsId, out lastMaxNewsIdInt);
            if (lastMaxNewsIdInt > 0)
            {
                sql += $" and id >{lastMaxNewsIdInt}";
            }
            else
            {
                var listUrl = CommonUrl.GetNewsListUrl(url);

                posturls.AddRange(listUrl);
            }

            var articles = DapperHelper<News_Article>.Query(sql, null);

            if (articles != null && articles.Count > 0)
            {
                posturls.AddRange( articles.Select(s=> string.Format("http://{0}/zixun/{1}.html", url, s.Id)));

                XmlHelper.UpdateNodeInnerText(xmlPath, lastMaxNewsIdXmlPath, articles.Max(m => m.Id).ToString());
            }

            var domain = "www.abc.com";
            var token = "token";
           var postResult= BaiduTuiSon.PostUrlComm(domain, token, posturls);
            if(postResult==null || postResult.Count <= 0)
            {
                Logger.Info($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}】资讯站主动推送未找到数据 未推送!");
            }
            foreach(var item in postResult)
            {
                Logger.Info($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}】资讯站主动推送结果!{item.Serializer()}");
            }
        }
    }
}
