﻿using System;
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
                string path = "Files/" + file_name;

                // Delete possible file with similiar name:
                DeleteFile(path);

                // Create the file:
                FileStream fs = CreateFileStream(path);
                Byte[] info = Encoding.Unicode.GetBytes(file_content);
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

        public static string GetFileBody(string file_path)
        {
            try
            {
                string text = "";
                using (StreamReader sr = new StreamReader(file_path))
                {
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        text += line;
                    }
                }

                return text;
            }
            catch(Exception ex)
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
