using System.Collections.Generic;

namespace ConsoleDbMigrationScript.Helpers
{
    /// <summary>
    /// Helper to handle Args parameters
    /// </summary>
    public static class ArgsHelper
    {
        public const string PATH_DB_SCRIPTS_KEY = "--pathdbscripts";

        /// <summary>
        /// Handle parameter pathdbscripts
        /// </summary>
        /// <param name="args">command line args</param>
        /// <returns>args, pathDbScripts</returns>
        public static (string[], string) HandlePathDbScripts(string[] args)
        {
            bool hasParameter = false;
            var pathDbScripts = "";

            var argsList = new List<string>();
            foreach (var arg in args)
            {
                argsList.Add(arg);
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == PATH_DB_SCRIPTS_KEY)
                {
                    hasParameter = true;
                    // retrieve the path
                    pathDbScripts = args[i + 1];
                }
            }
            if (!hasParameter)
            {
                argsList.Add(PATH_DB_SCRIPTS_KEY);
                argsList.Add(pathDbScripts);
            }

            return (argsList.ToArray(), pathDbScripts);
        }
    }
}
