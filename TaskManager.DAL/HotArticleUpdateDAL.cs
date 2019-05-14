using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TaskManager.Core.Db;

namespace TaskManager.DAL.HotArticleUpdate
{
    public class HotArticleUpdateDAL
    {
        private static IDbConnection GetDbConnection()
        {
            return ConnectionStrHelper.GetJZDataConnection();
        }

        public static int UpdateHotArticle(out int articleTotal)
        {
            int updateArticleCount = 0;
            var artileIdList = GetTopArticle();
            articleTotal = artileIdList.Count();

            using (var conn = GetDbConnection())
            {
                foreach (var articleId in artileIdList)
                {
                    string sql = "UPDATE [JZData].[dbo].[News_Article] SET IsHot = 1,Points=Points+10 WHERE Id = @Id AND IsHot = 0";

                    updateArticleCount += conn.Execute(sql, new { Id = articleId });
                }
            }
            return updateArticleCount;
        }

        private static IEnumerable<string> GetTopArticle()
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day);
            var endDate = startDate.AddDays(-7);
            string sql = "SELECT TOP 30 Id FROM[JZData].[dbo].[News_Article] WHERE CreateTime< @startDate AND CreateTime > @endDate ORDER BY Points DESC";

            using (var conn = GetDbConnection())
            {
                return conn.Query<string>(sql, new { startDate, endDate }).AsList();
            }
        }
    }
}
