
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using TaskManager.Core.Utility;

namespace TaskManager.Core.Db
{
    public static class ConnectionStrHelper
    {
        /// <summary>
        /// 获取JZData连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionStr()
        {
            return ConfigurationHelper.GetConfigurationValue("ConnectionStrings:Connection");

        }
        /// <summary>
        /// 获取JZData连接字符串
        /// </summary>
        /// <returns></returns>
        public static IDbConnection GetConnection()
        {
            var connectionStr = ConfigurationHelper.GetConfigurationValue("ConnectionStrings:Connection");

            return new SqlConnection(connectionStr);
        }
    }
}
