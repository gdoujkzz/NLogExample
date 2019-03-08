using NLog;
using System;

namespace NLConsoleApp_Framework
{
    class Program
    {
        static void Main(string[] args)
        {
            //加载配置
            #region 代码加载配置
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "1.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
            #endregion
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Hello Wrold");
            Console.ReadKey();
        }
    }


    public class MyClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void MyMethod1()
        {
            logger.Trace("Sampel Trace message");
            logger.Trace("Sampel Debug message");
            logger.Info("Sample Info message");
            logger.Warn("Sample Warning message");
            logger.Error("Sample Error message");
            logger.Fatal("Sampel fatal error message");
            logger.Log(LogLevel.Info, "Sample informational message");
            try
            {

            }
            catch(Exception ex)
            {
                logger.Error(ex, "异常");
            }
        }
    }

    //NLog的比较好的做法
    //1 Logger 在每个类中应该是以静态变量的形式存在，重复创建也有开销
    //namespace MyNamespace
    //{
    //    public class MyClass
    //    {
    //        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    //    }
    //}
    //2 Logger 输出的东西应该格式化。
    //为了避免字符串的重新分配和字符串的连接。写logger的时候应该格式化好来，类似下面这样。
    //logger.Info("Hello {0}", "Earth");
    //3 避免将Exception作为格式化参数提供，而是明确的将其做为第一个参数提供，
    //这将有助于NLog目标更好的日志记录
    //4 NLog 默认会吞掉所有异常，记录日志是不会导致应用程序停止的。
    //但是对很多应用程序而已，日志记录是非常关键的。因此如果初始化Nlog配置失败时候，那么它是致命的
    //为了检查是不是配置文件的问题，它提供了一个Throw Exception=true。 这个永远都不应该在生产环境使用
    //5 记得去清理缓冲区 Remember to Flush
    //默认情况下，NLog会在应用程序关闭时候，自动Flush，Windows在中止.Net应用程序提供了有限时间来
    //执行关闭（通常为两秒）。 如果NLog 是要写到数据库，邮件，要用到网络（Http,Mail
    //Tcp ）通常要手动进行关闭
    //NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
}    //在linux上，在进入应用程序关闭之前需要应用程序停止线程和定时器，如果不这么做，
    //可能会导致其异常
