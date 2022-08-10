using System.IO;

namespace BankTech_API_V1
{
    public class FileHelper
    {

        public FileHelper()
        {
            
        }

        public void EnsureDirectoryExists(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                System.IO.Directory.CreateDirectory(fi.DirectoryName);
            }
        }

    }
}
