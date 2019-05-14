using System;
using Topshelf;

namespace TaskManager.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(c =>
            {
                c.SetServiceName("TaskManagerService");
                c.SetDisplayName("任务调度服务");
                c.Service<TopshelfService>(s =>
                {
                    s.ConstructUsing(b => new TopshelfService());
                    s.WhenStarted(o => o.Start());
                    s.WhenStopped(o => o.Stop());
                });
            });
#if DEBUG
            //本地调试 为了留住黑框
            Console.ReadKey();
#else
#endif
        }
    }
}


