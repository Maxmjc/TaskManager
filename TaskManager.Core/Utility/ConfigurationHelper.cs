using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TaskManager.Core.Utility
{
    public static class ConfigurationHelper
    {
        private static IConfigurationRoot _configuration =null;

        static ConfigurationHelper()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration= builder.Build();
        }
        /// <summary>
        /// 获取配置项 格式：a:b:c
        /// </summary>
        /// <returns></returns>
        public static string GetConfigurationValue(string key)
        {
            return _configuration[key];
        }
    }
}
