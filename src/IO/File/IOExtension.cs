using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Xanadu.Skidbladnir.IO.File
{
    /// <summary>
    /// 
    /// </summary>
    public static class IOExtension
    {
        /// <summary>
        /// Path for %APPDATA%/&lt;ProcessName&gt;
        /// </summary>
        public static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Process.GetCurrentProcess().ProcessName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            var dirs = dir.GetDirectories();

            // Create the destination directory
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (var file in dir.GetFiles())
            {
                var targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (!recursive)
            {
                return;
            }

            foreach (var subDir in dirs)
            {
                var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                IOExtension.CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="allowNotFound"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static void DeleteFile(string path, bool allowNotFound = true)
        {
            if (!System.IO.File.Exists(path))
            {
                if (!allowNotFound)
                {
                    throw new FileNotFoundException("File Not Found!", path);
                }

                return;
            }

            System.IO.File.SetAttributes(path, FileAttributes.Normal);
            System.IO.File.Delete(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="allowNotFound"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static Task DeleteDirectory(string path, bool allowNotFound = true, bool force = false)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(path))
                {
                    if (!allowNotFound)
                    {
                        throw new FileNotFoundException("Directory Not Found!", path);
                    }

                    return;
                }

                if (force)
                {
                    var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    }

                }

                Directory.Delete(path, true);
            });
        }
    }
}
