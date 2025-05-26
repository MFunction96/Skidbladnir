using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        public static string AppDataFolder {
            get
            {
                var processName = Process.GetCurrentProcess().ProcessName;
                var sanitized = string.Concat(processName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), sanitized);
            }
        }

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
                IOExtension.CopyDirectory(subDir.FullName, newDestinationDir, recursive);
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

            try
            {
                System.IO.File.SetAttributes(path, FileAttributes.Normal);
                System.IO.File.Delete(path);
            }
            catch (IOException) when (allowNotFound)
            {

            }
            catch (UnauthorizedAccessException) when (allowNotFound)
            {

            }

        }

        /// <summary>
        /// Delete a directory. Able to allow not found and the files with weird attributes.
        /// </summary>
        /// <param name="path">Directory path.</param>
        /// <param name="allowNotFound">Allow not found then do nothing.</param>
        /// <param name="force">true is for deleting the file with weird attributes.</param>
        /// <exception cref="DirectoryNotFoundException">If not allow not found but do not found, throw it.</exception>
        public static void DeleteDirectory(string path, bool allowNotFound = true, bool force = false)
        {
            if (!Directory.Exists(path))
            {
                if (!allowNotFound)
                {
                    throw new DirectoryNotFoundException($"{path} Not Found!");
                }

                return;
            }

            if (force)
            {
                foreach (var dir in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                {
                    _ = new DirectoryInfo(dir)
                    {
                        Attributes = FileAttributes.Normal
                    };
                }

                foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                }

            }

            Directory.Delete(path, true);
        }
    }
}
