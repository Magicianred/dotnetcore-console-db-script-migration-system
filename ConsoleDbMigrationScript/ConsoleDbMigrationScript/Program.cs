using ConsoleDbMigrationScript.Classes;
using ConsoleDbMigrationScript.Helpers;
using System;
using System.IO;

namespace ConsoleDbMigrationScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var guid = Guid.NewGuid();
            var date = DateTime.Now.ToString("yyyy-MM-dd_HH-MM-ss");
            FileStream filestream = new FileStream("logs\\log_" + date + "_" + guid.ToString() + ".log", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            string assemblyPath = PathHelper.GetRootFolderPath();

            var pathDbScripts = String.Empty;
            (args, pathDbScripts) = ArgsHelper.HandlePathDbScripts(args);

            var rootPath = PathHelper.GetNormalizedPath(pathDbScripts, assemblyPath);

            var configuration = ConfigurationHelper.GetConfiguration(args: args);

            var dbScriptManager = new DbScriptManager(configuration);
            dbScriptManager.RunScripts(rootPath);

            Console.WriteLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Program completed with success");
        }
    }
}
