using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TaskManager.Core.File;
using TaskManager.Core.Job;
using TaskManager.Core.Log;

namespace TaskManager.Service
{
    public class TopshelfService
    {
        private static List<JobDetail> services = new List<JobDetail>();

        /// <summary>
        /// 服务启动
        /// </summary>
        public void Start()
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange:true);
                var configuration = builder.Build();

                try
                {
                    //首次启动 加载
                    InitServices(configuration);
                }
                catch(Exception ex)
                {
                    Logger.Error("服务启动加载配置异常", ex);
                }

                IFileProvider fileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);

                ChangeToken.OnChange(() => fileProvider.Watch("appsettings.json"), () =>
                {
                    //等待程序重新加载json
                    Thread.Sleep(500);
                    try
                    {
                        ServicesChanged(configuration);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("配置文件更改重新加载配置异常", ex);
                    }

                });
            }
            catch(Exception ex)
            {
                Logger.Error("服务启动异常",ex);
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            try
            {
                JobManager.ShutdownJobs();
                JobManager.Clear();
            }
            catch(Exception ex)
            {
                Logger.Error("服务停止异常", ex);
            }
        }

        #region 加载服务列表
        /// <summary>
        /// 首次启动 加载服务列表
        /// </summary>
        /// <param name="configuration"></param>
        static void InitServices(IConfigurationRoot configuration)
        {
            foreach (var item in configuration.GetSection("Services").GetChildren())
            {
                var job = new JobDetail();
                job.JobName = item.GetSection("JobName").Value;
                job.Cron = item.GetSection("Cron").Value;
                job.ConsolePath = item.GetSection("ConsolePath").Value;
#if DEBUG
                Console.WriteLine($"首次加载【{job.JobName}】任务 cron{job.Cron} ");
#else
#endif
                services.Add(job);

                JobManager.AddJob<ConsoleJob>(job.JobName, job.Cron,
                    new Dictionary<string, object>() {
                        { "ConsolePath", job.ConsolePath }
                    });
            }
            JobManager.StartJobs();
        }
        /// <summary>
        /// 配置文件更新 更新服务列表
        /// </summary>
        /// <param name="configuration"></param>
        static void ServicesChanged(IConfigurationRoot configuration)
        {
            var jobDetails = new List<JobDetail>();

            foreach (var item in configuration.GetSection("Services").GetChildren())
            {
                var job = new JobDetail();
                job.JobName = item.GetSection("JobName").Value;
                job.Cron = item.GetSection("Cron").Value;
                job.ConsolePath = item.GetSection("ConsolePath").Value;
#if DEBUG
                Console.WriteLine($"重新加载【{job.JobName}】任务 cron{job.Cron} ");
#else
#endif
                jobDetails.Add(job);

                var dicPara = new Dictionary<string, object>() { { "ConsolePath", job.ConsolePath } };

                //配置更改后刷新
                var oldJob = services.FirstOrDefault(f => f.JobName.Equals(job.JobName));

                //原服务不存在 新添加服务
                if (oldJob == null)
                {
                    JobManager.AddJob<ConsoleJob>(job.JobName, job.Cron, dicPara, true);
                    continue;
                }
                //配置未改变 不用做改动
                if (oldJob.Cron.Equals(job.Cron) && oldJob.ConsolePath.Equals(job.ConsolePath))
                {
                    continue;
                }

                //删除服务 
                JobManager.RemoveJob(oldJob.JobName);
                //重新添加服务
                JobManager.AddJob<ConsoleJob>(job.JobName, job.Cron, dicPara,true);
            }

            services = jobDetails;
        }

        #endregion
    }
}
