using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using TaskManager.Core.Log;
using TaskManager.Core.Seo.SiteMap;
using TaskManager.Core.Utility;

namespace TaskManager.Job.SeoSiteMap
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlPath = $@"{AppDomain.CurrentDomain.BaseDirectory}Config\AppConfig.xml";

            var newsSiteMapName = XmlHelper.GetNodeInnerText(xmlPath, "root/NewsSiteMapName");
            var newsSiteMapPath = XmlHelper.GetNodeInnerText(xmlPath, "root/NewsSiteMapPath");


            //定义 siteMap生成顺序
            var siteMapSort = new List<ISiteMap>()
            {
                new NewsSiteMap()
            };
            var siteMapHelper = new SiteMapHelper(newsSiteMapName);
            foreach (var instance in siteMapSort)
            {
                var siteMapData = instance.GetSiteMapList();
                siteMapHelper.WriteData(siteMapData, newsSiteMapPath);
            }
            Logger.Info($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}】资讯站siteMap生成成功!");
        }
    }
}
