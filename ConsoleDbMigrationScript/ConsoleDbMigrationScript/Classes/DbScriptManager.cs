using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleDbMigrationScript.Classes
{
    /// <summary>
    /// Class whom retrieve script to run and execute them
    /// </summary>
    public class DbScriptManager
    {
        private const string MIGRATION_ALREADY_RUN = "MIGRATION ALREADY RUNNED ON THIS DB!!! STOP EXECUTION SCRIPT";
        private const string MIGRATION_PREREQUISITE = "YOU HAVE TO RUN SCRIPT";

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">settings</param>
        public DbScriptManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieve scripts to run
        /// </summary>
        /// <param name="basePath">base path where files are</param>
        /// <returns>boolean of success</returns>
        public bool RunScripts(string basePath)
        {
            var connectionString = _configuration.GetConnectionString("Database");
            var scripts = _configuration.GetSection("SqlScripts:Planned").Get<List<string>>();

            if (scripts != null && scripts.Any())
            {
                foreach (string script in scripts)
                {
                    if (!String.IsNullOrWhiteSpace(script))
                    {
                        string pathScript = Path.Combine(basePath, script);

                        // if ends for .sql is a file
                        if (script.EndsWith(".sql"))
                        {
                            this.runSqlScriptFile(pathScript, connectionString);
                        }
                        else // it's a folder
                        {
                            DirectoryInfo folder = new DirectoryInfo(pathScript);
                            if (folder.Exists)
                            {
                                FileInfo[] files = folder.GetFiles("*.sql");
                                foreach (FileInfo pathFile in files)
                                {
                                    this.runSqlScriptFile(pathFile.FullName, connectionString);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Folder '{script}' doesn't exists. No file founded.");
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Execute one script
        /// </summary>
        /// <param name="pathStoreProceduresFile">path to script to execute</param>
        /// <param name="connectionString">connection string where execute script</param>
        /// <returns>boolean of success</returns>
        public bool runSqlScriptFile(string pathStoreProceduresFile, string connectionString)
        {
            // Create a StringBuilder for log
            var printOutput = new StringBuilder();
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Script going to execute: '{pathStoreProceduresFile}'");
            try
            {
                string script = File.ReadAllText(pathStoreProceduresFile);

                System.Collections.Generic.IEnumerable<string> commandStrings = 
                    Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.FireInfoMessageEventOnUserErrors = true;
                    connection.StatisticsEnabled = true;
                    connection.InfoMessage += (object obj, SqlInfoMessageEventArgs e) => {
                        printOutput.AppendLine(e.Message);
                    };
                    connection.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            using (var command = new SqlCommand(commandString, connection))
                            {
                                try
                                {
                                    command.ExecuteNonQuery();

                                    var output = printOutput.ToString();
                                    if (output.Contains(MIGRATION_ALREADY_RUN))
                                    {
                                        Console.WriteLine($"Script '{pathStoreProceduresFile}' already run on this database. Skip to the next script.");
                                        break;
                                    }
                                    else if (output.Contains(MIGRATION_PREREQUISITE))
                                    {
                                        // if is required a prerequisite script, try to execute it
                                        int pFrom = output.IndexOf("'") + 1;
                                        int pTo = output.IndexOf("'", pFrom);
                                        string scriptToRun = output.Substring(pFrom, pTo - pFrom) + ".sql";

                                        pTo = pathStoreProceduresFile.LastIndexOf("\\") + 1;

                                        string pathScriptToRun = pathStoreProceduresFile.Substring(0, pTo) + scriptToRun;

                                        Console.WriteLine($"{MIGRATION_PREREQUISITE}: {scriptToRun}");
                                        if (pathScriptToRun != pathStoreProceduresFile)
                                        {
                                            if (File.Exists(pathScriptToRun))
                                            {
                                                var success = runSqlScriptFile(pathScriptToRun, connectionString);
                                                if (!success)
                                                {
                                                    throw new ArgumentException($"Fail to run: {pathScriptToRun}!");
                                                }
                                            }
                                            else
                                            {
                                                throw new ArgumentException($"File {scriptToRun} doesn't exists! Cannot execute it");
                                            }
                                        }
                                        else
                                        {
                                            throw new ArgumentException($"PREREQUISITES recursion problem! Cannot execute it");
                                        }
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    if (ex.Message == MIGRATION_ALREADY_RUN)
                                    {
                                        Console.WriteLine($"Script '{pathStoreProceduresFile}' already run on this database. Skip to the next script.");
                                    }
                                    else
                                    {
                                        string spError = commandString.Length > 100 ? commandString.Substring(0, 100) + " ...\n..." : commandString;
                                        System.Console.WriteLine(string.Format("Please check the SqlServer script.\nFile: {0} \nLine: {1} \nError: {2} \nSQL Command: \n{3}", pathStoreProceduresFile, ex.LineNumber, ex.Message, spError));
                                        throw new ArgumentException("Fail to execute script: " + pathStoreProceduresFile);
                                    }
                                }
                                finally
                                {
                                    Console.WriteLine("************************* START GO Command ************************");
                                    Console.WriteLine(printOutput.ToString());
                                    Console.WriteLine("************************* END GO Command ************************");
                                    Console.WriteLine();
                                    printOutput.Clear();
                                }
                            }
                        }
                    }

                    Console.WriteLine($"Run script '{pathStoreProceduresFile}' with success");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();

                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw new ArgumentException("Fail to execute script: " + pathStoreProceduresFile);
            }
        }

    }
}
