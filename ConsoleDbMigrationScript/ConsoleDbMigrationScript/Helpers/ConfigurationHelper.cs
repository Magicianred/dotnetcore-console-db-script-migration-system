using Microsoft.Extensions.Configuration;
using System;

namespace ConsoleDbMigrationScript.Helpers
{
    /// <summary>
    /// Handle json configuration files by environment
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Retrieve configuration settings from json file based on environment
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="build"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(IConfigurationBuilder builder = null, bool build = true, params string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            builder ??= new ConfigurationBuilder();
            builder.AddJsonFile("consoleSettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"consoleSettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (args != null && args.Length > 0)
                builder.AddCommandLine(args);

            if (build)
                return builder.Build();
            return null;
        }
    }

}
