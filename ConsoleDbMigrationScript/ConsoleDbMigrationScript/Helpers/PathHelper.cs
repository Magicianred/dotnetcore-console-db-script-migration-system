using System.IO;

namespace ConsoleDbMigrationScript.Helpers
{
    /// <summary>
    /// Helper to handle Path
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Retrieve path of the assembly
        /// </summary>
        /// <returns>string of the path</returns>
        public static string GetRootFolderPath()
        {
            return Path.GetDirectoryName(typeof(Program).Assembly.Location);
        }

        /// <summary>
        /// Return path to call in function, if is relative path add rootPath
        /// </summary>
        /// <param name="path">path to call</param>
        /// <param name="rootPath">root path to add</param>
        /// <returns>normalized path</returns>
        public static string GetNormalizedPath(string path, string rootPath)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.GetFullPath(Path.Combine(rootPath, path));
            }

        }
    }
}
