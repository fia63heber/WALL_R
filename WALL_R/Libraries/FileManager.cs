using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WALL_R.Models;
using System.IO;

namespace WALL_R.Libraries
{
    public static class FileManager
    // partially copied from microsoft documentary: https://docs.microsoft.com/en-us/dotnet/api/system.io.file.create?view=netframework-4.7.2
    {
        public static FileStream CreateFileStream(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate);
        }

        public static bool CreateFile(string file_name, string file_content)
        {
            try
            {
                string path = "" + file_name;

                // Delete possible file with similiar name:
                DeleteFile(file_name);

                // Create the file:
                FileStream fs = CreateFileStream(file_name);
                Byte[] info = new UTF8Encoding(true).GetBytes(file_content);
                // Add some information to the file:
                fs.Write(info, 0, info.Length);
                fs.Close();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetFilePath(string file_name)
        {
            try
            {
                string path = "" + file_name;
                return path;
            }
            catch
            {
                return "";
            }
        }


        public static bool DeleteFile(string file_name)
        {
            try
            {
                string path = "" + file_name;

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
