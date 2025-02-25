using System;
using System.Diagnostics;
using System.IO;

namespace Xanadu.Skidbladnir.IO.File
{
    /// <summary>
    /// The extension of the IO operations.
    /// </summary>
    public static class IOExtension
    {
        /// <summary>
        /// Path for %APPDATA%/&lt;ProcessName&gt;
        /// </summary>
        public static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Process.GetCurrentProcess().ProcessName);

        /// <summary>
        /// Copy a directory including files.
        /// </summary>
        /// <param name="sourceDir">Source Directory</param>
        /// <param name="destinationDir">Destination Directory. Create if not exist.</param>
        /// <param name="recursive">Recursive copy the directory. Default is true.</param>
        /// <exception cref="DirectoryNotFoundException">Throw if source directory does not exist!</exception>
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
        /// Delete a file. Able to allow not found and the file with weird attributes.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="allowNotFound">Allow not found then do nothing.</param>
        /// <exception cref="FileNotFoundException">If not allow not found but do not found, throw it.</exception>
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
        /// Delete a directory. Able to allow not found and the files with weird attributes.
        /// </summary>
        /// <param name="path">Directory path.</param>
        /// <param name="allowNotFound">Allow not found then do nothing.</param>
        /// <param name="force">true is for deleting the file with weird attributes.</param>
        /// <exception cref="FileNotFoundException">If not allow not found but do not found, throw it.</exception>
        public static void DeleteDirectory(string path, bool allowNotFound = true, bool force = false)
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
        }
    }
}
