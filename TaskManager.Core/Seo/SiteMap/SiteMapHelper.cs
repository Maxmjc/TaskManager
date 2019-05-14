using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TaskManager.Core.Seo.SiteMap
{
    public class SiteMapHelper
    {
        private string siteMapName = "SiteMap";
        private int count = 0;
        private int writeCount = 50000;

        public SiteMapHelper(string sitemapName)
        {
            siteMapName = sitemapName;
        }

        /// <summary>
        /// 返回可追加数据的文件名称和数量
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isForceToNew">是否强制新建文件后添加数据</param>
        /// <returns></returns>
        private Tuple<int, string> GetWriteCount(string path, bool isForceToNew)
        {
            var siteMapPath = string.Format(@"{0}\{1}{2}.xml", path, siteMapName, count <= 0 ? string.Empty : count.ToString());
            while (System.IO.File.Exists(siteMapPath))
            {
                XmlDocument document = new XmlDocument();
                document.Load(siteMapPath);
                if (document.DocumentElement.ChildNodes.Count < writeCount && !isForceToNew)
                {
                    return new Tuple<int, string>(writeCount - document.DocumentElement.ChildNodes.Count, siteMapPath);
                }
                count = count + 1;
                siteMapPath = string.Format(@"{0}/{1}{2}.xml", path, siteMapName, count <= 0 ? string.Empty : count.ToString());
            }
            return new Tuple<int, string>(writeCount, siteMapPath);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="isFirstToNew">第一次写入是否新建文件</param>
        public void WriteData(List<SiteMap> data, string path, bool isFirstToNew = false)
        {
            if (data == null || data.Count <= 0) return;

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            while (data.Count > 0)
            {
                var result = GetWriteCount(path, isFirstToNew);
                if (isFirstToNew) isFirstToNew = false;
                var count = Math.Min(result.Item1, data.Count);
                var temp = data.Take(count).ToList();
                data.RemoveRange(0, count);
                if (result.Item1 == writeCount)
                    WriteNewXml(temp, result.Item2);
                else
                    WriteAppendXml(temp, result.Item2);
            }
        }

        /// <summary>
        /// 把数据写入新文档
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        private void WriteNewXml(IList<SiteMap> data, string path)
        {
            XmlTextWriter writer = new XmlTextWriter(path, null);
            //使用自动缩进便于阅读
            //writer.Formatting = Formatting.Indented;
            //写入版本声明
            writer.WriteStartDocument();
            String PItext = "type='text/xsl' href='sitemap.xsl'";
            //声明引入样式
            writer.WriteProcessingInstruction("xml-stylesheet", PItext);
            //写入根元素
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            //加入子元素
            foreach (var item in data)
            {
                writer.WriteStartElement("url");
                writer.WriteElementString("loc", item.Url);
                writer.WriteElementString("lastmod", string.IsNullOrWhiteSpace(item.Date) ? DateTime.Now.Date.ToString("yyyy-MM-dd") : item.Date);
                writer.WriteElementString("changefreq", "daily");
                if (item.Type == 1)
                {
                    writer.WriteElementString("priority", "1");
                }
                else
                {
                    writer.WriteElementString("priority", "0.8");
                }
                //关闭根元素，并书写结束标签
                writer.WriteEndElement();
            }
            ////关闭根元素，并书写结束标签
            writer.WriteEndElement();
            ////将XML写入文件并且关闭XmlTextWriter
            writer.Close();
        }
        /// <summary>
        /// 把数据追加到旧文档
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        private void WriteAppendXml(IList<SiteMap> data, string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            var tempData = new List<SiteMap>();
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                XmlElement xe = (XmlElement)node;
                var temp = new SiteMap();
                temp.Url = xe.FirstChild.InnerText;
                temp.Date = xe.FirstChild.NextSibling.InnerText;
                temp.Type = xe.LastChild.InnerText == "1" ? 1 : 2;
                tempData.Add(temp);
            }
            tempData.AddRange(data);
            WriteNewXml(tempData, path);
        }
    }

    /// <summary>
    /// sitemap实体
    /// </summary>
    public class SiteMap
    {
        public double Type
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public string Date
        {
            get;
            set;
        }
    }
}
