using System.IO;
using System.Threading.Tasks;

namespace Skidbladnir.IO.File
{
    public static class Deletion
    {
        public static Task DeleteFile(string path, bool allowNotFound = true)
        {
            return Task.Run(() =>
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
            });
        }

        public static Task DeleteDirectory(string path, bool allowNotFound = true)
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

                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                }

                Directory.Delete(path, true);
            });
        }
    }
}
