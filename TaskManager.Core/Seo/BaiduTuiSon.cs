using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TaskManager.Core.Utility;

namespace TaskManager.Core.Seo
{
    public class BaiduTuiSon
    {
        private static readonly string _url = "http://data.zz.baidu.com";
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlList"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        private static PushUrlBaiduResult Post(string url, List<string> urlList, Encoding encode)
        {
#if DEBUG
            return new PushUrlBaiduResult() { Not_Same_Site = urlList, Success = urlList.Count, message = "本地测试使用 未真正提交！" };
#else
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Post";
            request.UserAgent = "curl/7.12.1";
            request.Host = "data.zz.baidu.com";
            StringBuilder sb = new StringBuilder();
            foreach (var item in urlList)
                sb.AppendLine(item);
            byte[] contentBytes = encode.GetBytes(sb.ToString());
            request.ContentLength = contentBytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(contentBytes,0,contentBytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseStr = string.Empty;
            if(request.HaveResponse)
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseStr = sr.ReadToEnd();
                }
            if (string.IsNullOrWhiteSpace(responseStr)) return null;
            return JsonSerializerHelper.DeserializeObject<PushUrlBaiduResult>(responseStr);
#endif

        }

        public static List<PushUrlBaiduResult> PostUrlComm(string domain, string token, List<string> data, int maxStep = 2000, bool isOriginal = false)
        {
            var result = new List<PushUrlBaiduResult>();
            if (string.IsNullOrWhiteSpace(domain)) return result;
            if (data == null || data.Count <= 0) return result;
            var count = Math.Ceiling((double)data.Count / (double)maxStep);
            var postAddress = string.Format("{0}/urls?site={1}&token={2}", _url, domain, token);
            if (isOriginal) postAddress += "&type=Original";
            for (int i = 1; i <= count; i++)
            {
                var jMax = i * maxStep;
                if (jMax > data.Count) jMax = data.Count;
                var item = new List<string>();
                for (int j = (i - 1) * maxStep; j < jMax; j++) item.Add(data[j]);
                if (item.Count > 0)
                    result.Add(Post(postAddress, item, Encoding.UTF8));
            }
            return result;
        }
    }
    /// <summary>
    /// 推送结果
    /// </summary>
    public class PushUrlBaiduResult
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string error { get; set; }
        public string message { get; set; }
        /// <summary>
        /// 当天剩余的可推送url条数
        /// </summary>
        public int Remain { get; set; }
        /// <summary>
        /// 成功推送的url条数
        /// </summary>
        public int Success { get; set; }
        /// <summary>
        /// 由于不是本站url而未处理的url列表
        /// </summary>
        public List<string> Not_Same_Site { get; set; }
        /// <summary>
        /// 不合法的url列表
        /// </summary>
        public List<string> Not_Valid { get; set; }
    }

}
