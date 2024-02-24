using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

namespace PlexMediaArchiver.Classes
{
    public static class AppLogger
    {
        public static Logger log;

        public static void Setup()
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(System.IO.Directory.GetCurrentDirectory())
              .AddJsonFile("nlog.json", optional: true, reloadOnChange: true).Build();
            
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            log = LogManager.Setup()
                    .LoadConfigurationFromSection(config)
                    .GetCurrentClassLogger();
        }
    }
}
