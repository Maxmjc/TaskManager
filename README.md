# TaskManager
.net core 轻量任务调度框架（基于Quartz.net ）

要写任务了吗？ 下面是过关秘籍：


1 在Jobs文件夹下找个合适地方 创建一个 netcore控制台应用   名字类似：TaskManager.Job.*

2 这个控制台默认运行使用dotnet运行的  要编辑 【Task.Manager.Job.*.csproj】文件
  在  【 <TargetFramework>netcoreapp2.2</TargetFramework> 】下方 
  添加一行 ：【<RuntimeIdentifier>win7-x64</RuntimeIdentifier>】
  这样生成项目就会在这个目录 bin\Debug\netcoreapp2.2\win7-x64  生成exe文件了 

3 右键项目 点击属性 选择【生成事件】 在后期生成事件命令 粘贴下面代码
【 xcopy /r /y $(TargetDir)* $(SolutionDir)\TaskManager.Service\$(OutDir)Jobs\$(ProjectName)\ /s /e 】

4 在控制台中写你任务的逻辑…

5 在 【TaskManager.Service】项目下面的 【appsettings.json】中 配置你添加的任务
 在 Services配置节中 
 添加结点：
     {
      "JobName": "你的任务名字",
      "Cron": "cron表达式",
      "ConsolePath": "/Jobs/TaskManager.Job.*/TaskManager.Job.*.exe"
    }

6 齐活 完美！



说明 :

第一点：

TaskManager.Service 为任务调度主程序
本地调试 就控制台运行就OK， 线上可以部署成服务。

部署服务步骤:
1 生成项目
2 找到【TaskManager.Service\bin\Debug\netcoreapp2.2\win7-x64】
3 管理员运行cmd ，切换到工作目录为【TaskManager.Service\bin\Debug\netcoreapp2.2\win7-x64】
4 在cmd中输入 【TaskManager.Service.exe install】 ps：如需卸载 在cmd中 输入【TaskManager.Service.exe uninstall】




第二点：

cron表达式生成url：http://www.bejson.com/othertools/cron/

常用表达式例子

（1）0/2 **** ? 表示每2秒 执行任务

（1）0 0/2 *** ? 表示每2分钟 执行任务

（1）0 0 2 1 * ? 表示在每月的1日的凌晨2点调整任务

（2）0 15 10 ?* MON-FRI 表示周一到周五每天上午10:15执行作业

（3）0 15 10 ? 6L 2002-2006   表示2002-2006年的每个月的最后一个星期五上午10:15执行作

（4）0 0 10,14,16 ** ? 每天上午10点，下午2点，4点 

（5）0 0/30 9-17 ** ? 朝九晚五工作时间内每半小时

（6）0 0 12 ?* WED    表示每个星期三中午12点 

（7）0 0 12 ** ? 每天中午12点触发

（8）0 15 10 ?** 每天上午10:15触发 

（9）0 15 10 ** ? 每天上午10:15触发 

（10）0 15 10 ** ? 每天上午10:15触发 

（11）0 15 10 ** ? 2005    2005年的每天上午10:15触发 

（12）0 * 14 ** ? 在每天下午2点到下午2:59期间的每1分钟触发 

（13）0 0/5 14 ** ? 在每天下午2点到下午2:55期间的每5分钟触发 

（14）0 0/5 14,18 ** ? 在每天下午2点到2:55期间和下午6点到6:55期间的每5分钟触发 

（15）0 0-5 14 ** ? 在每天下午2点到下午2:05期间的每1分钟触发 

（16）0 10,44 14 ? 3 WED 每年三月的星期三的下午2:10和2:44触发 

（17）0 15 10 ?* MON-FRI 周一至周五的上午10:15触发 

（18）0 15 10 15 * ? 每月15日上午10:15触发 

（19）0 15 10 L* ? 每月最后一日的上午10:15触发 

（20）0 15 10 ?* 6L    每月的最后一个星期五上午10:15触发 

（21）0 15 10 ?* 6L 2002-2005   2002年至2005年的每月的最后一个星期五上午10:15触发 

（22）0 15 10 ?* 6#3   每月的第三个星期五上午10:15触发
