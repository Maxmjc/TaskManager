using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Log;

namespace TaskManager.Core.Job
{
    public class ConsoleJob : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string content = dataMap.GetString("ConsolePath");
                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.Info($"【{context.JobDetail.Key}】任务的ConsolePath参数为空 取消任务");
                    return null;
                }

                ProcessStartInfo state = new ProcessStartInfo();
                state.FileName = $@"{AppDomain.CurrentDomain.BaseDirectory}{content}";
                state.WindowStyle = ProcessWindowStyle.Hidden;

                var lastIndex = content.LastIndexOf("/");
                if (lastIndex != -1)
                {
                    var workDir = content.Substring(0, content.LastIndexOf("/"));
                    if (!string.IsNullOrWhiteSpace(workDir))
                    {
                        state.WorkingDirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}{workDir}";
                    }
                }
                Process.Start(state);
                Logger.Info($"【{context.JobDetail.Key}】任务运行成功");
#if DEBUG
                Console.WriteLine($"【{context.JobDetail.Key}】任务运行成功{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
#else
#endif
            }
            catch(Exception ex)
            {
#if DEBUG
                Console.WriteLine($"【{context.JobDetail.Key}】任务运行失败{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}异常：{ex.ToString()}");
#else
#endif
                Logger.Error($"【{context.JobDetail.Key}】任务运行异常", ex);
            }

            return null;
        }
    }
}
