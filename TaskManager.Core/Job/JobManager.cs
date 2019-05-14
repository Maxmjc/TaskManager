using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Log;

namespace TaskManager.Core.Job
{
    public class JobManager
    {
        #region 变量
        private static ISchedulerFactory _schedulerFactory = new StdSchedulerFactory();

        private static string _jobGroupName = "jianzhuJob"; //Job群组名
        private static string _triggerGroupName = "jianzhuTrigger"; //触发器群组名
        #endregion

        private static IScheduler GetScheduler()
        {
            return _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
        }

        #region 添加，删除，修改Job方法
        /// <summary>
        /// 添加一个定时任务，使用默认的任务组名，触发器名，触发器组名 
        /// </summary>
        /// <param name="pStrJobName">任务名</param>
        /// <param name="pStrCronExpress">触发器表达式</param>
        public static void AddJob<T>(string pStrJobName, string pStrCronExpress, IDictionary<string, object> pDictionary,bool isStart=false) where T : IJob
        {
            try
            {
                IScheduler sched = GetScheduler();
                // 创建任务
                IJobDetail job = JobBuilder.Create<T>()
                    .WithIdentity(pStrJobName, _jobGroupName)
                    .Build();

                // 创建触发器
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(pStrJobName, _triggerGroupName)
                    .WithCronSchedule(pStrCronExpress)
                    .Build();

                //给任务传参数
                foreach (KeyValuePair<string, object> kvp in pDictionary)
                {
                    job.JobDataMap.Put(kvp.Key, kvp.Value);
                }

                sched.ScheduleJob(job, trigger);
               
                if (isStart)
                {
                    StartJob(sched);
                }
               
            }
            catch (Exception e)
            {
                Logger.Error($"创建任务【{pStrJobName}】失败", e);
            }
        }

        private static void StartJob(IScheduler sched)
        {
            sched.Start();
        }
        /// <summary>
        /// 移除一个任务(使用默认的任务组名，触发器名，触发器组名) 
        /// </summary>
        /// <param name="pStrJobName">任务名称</param>
        public static void RemoveJob(string pStrJobName)
        {
            try
            {
                IScheduler sched = GetScheduler();
                JobKey jobKey = new JobKey(pStrJobName);
                TriggerKey triggerKey = new TriggerKey(pStrJobName, _triggerGroupName);
                sched.PauseTrigger(triggerKey);// 停止触发器  
                sched.UnscheduleJob(triggerKey);// 移除触发器  
                sched.DeleteJob(jobKey);// 删除任务  
            }
            catch (Exception e)
            {
                Logger.Error($"移除任务【{pStrJobName}】失败", e);
            }
        }

        /// <summary>
        /// 修改一个任务的触发时间(使用默认的任务组名，触发器名，触发器组名) 
        /// </summary>
        /// <param name="pStrJobName">任务名</param>
        /// <param name="pStrCronExpress">触发器表达式</param>
        public static void ModifyJobTime<T>(string pStrJobName, string pStrCronExpress, IDictionary<string, object> pDictionary) where T : IJob
        {
            try
            {
                IScheduler sched = GetScheduler();
                TriggerKey triggerKey = new TriggerKey(pStrJobName, _triggerGroupName);
                ICronTrigger trigger = (ICronTrigger)sched.GetTrigger(triggerKey);
                if (trigger == null)
                {
                    return;
                }
                RemoveJob(pStrJobName);
                AddJob<T>(pStrJobName, pStrCronExpress, pDictionary);
            }
            catch (Exception e)
            {
                Logger.Error($"修改任务【{pStrJobName}】失败", e);
            }
        }
        #endregion

        #region 启动，关闭Job
        /// <summary>
        /// 启动所有定时任务 
        /// </summary>
        public static void StartJobs()
        {
            try
            {
                IScheduler sched = GetScheduler();
                sched.Start();
            }
            catch (Exception e)
            {
                Logger.Error($"启动所有任务失败", e);
            }
        }

        /// <summary>
        /// 关闭所有定时任务
        /// </summary>
        public static void ShutdownJobs()
        {
            try
            {
                IScheduler sched = GetScheduler();
                if (!sched.IsShutdown)
                {
                    sched.Shutdown();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"关闭所有任务失败", e);
            }
        }
        #endregion

        public static void Clear()
        {
            IScheduler sched = GetScheduler();
            sched.Clear();
        }
    }
}
