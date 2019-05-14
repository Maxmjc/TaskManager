using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Utility
{
    /// <summary>
    /// Json序列化帮助类
    /// </summary>
    public static class JsonSerializerHelper
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };
        /// <summary>
        /// 将对象序列化为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serializer(this object obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, Settings);
        }
        /// <summary>
        /// 反序列化json字符串为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string json)
        {
            return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json, Settings);
        }
        public static object DeserializeObject(this string json, Type targeType)
        {
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, targeType);
        }
    }
}
