using System;
using System.IO;

namespace DeleteMetaFile
{
    class Program
    {
        static void Main(string[] args)
        {

            string path;
            do
            {
                Console.WriteLine("don't have directory");
                Console.WriteLine("please input directory :");
                path = Console.ReadLine();

            } while (!Directory.Exists(path));


            RemoveMetaFileFromPath(path);

            Console.WriteLine("Success");
        }

        static void FindAllDefine(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (var f in root.GetFiles())
            {
                var content = File.ReadAllText(f.FullName);

            }
        }

        static void RemoveMetaFileFromPath(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (var f in root.GetFiles())
            {
                if (f.Extension == ".meta")
                {
                    File.Delete(f.FullName);
                    Console.WriteLine(string.Format("delete {0} success", f.FullName));
                }
            }

            foreach (var d in root.GetDirectories())
            {
                RemoveMetaFileFromPath(d.FullName);
            }
        }
    }
}
