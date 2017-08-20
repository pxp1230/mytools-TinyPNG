using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyPng;

namespace TinyPNG
{
    class Program
    {
        const string apiKey = "[apiKey请到官网注册后，到后台获取]";
        static readonly DateTime lastWriteTime = new DateTime(1993, 5, 12);

        static void Main(string[] args)
        {
            //示例：
#if DEBUG
            args = new string[1];
            args[0] = @"F:\new_desktop\临时\2503750369jingxuan_yxjx_272970_15.jpg";
#endif
            Console.WriteLine("正在压缩图片中...");

            if (args != null && args.Length >= 1)
            {
                List<string> files = new List<string>();

                foreach (string arg in args)
                {
                    string path = arg;
                    DirectoryInfo rootDirectory = new DirectoryInfo(path);
                    FileInfo curFile = new FileInfo(path);
                    if (curFile.Exists && (curFile.Extension.ToLower() == ".jpg" || curFile.Extension.ToLower() == ".png"))
                    {
                        if (curFile.LastWriteTime != lastWriteTime)
                            files.Add(curFile.FullName);
                    }
                    else if (rootDirectory.Exists)
                    {
                        FileInfo[] fs = rootDirectory.GetFiles();
                        for (int i = 0; i < fs.Length; i++)
                        {
                            if (fs[i].LastWriteTime == lastWriteTime)
                                continue;
                            if (fs[i].Extension.ToLower() == ".jpg" || fs[i].Extension.ToLower() == ".png")
                                files.Add(fs[i].FullName);
                        }
                    }
                }

                foreach (var item in files)
                {
                    try
                    {
                        DisplayTinyPNG(apiKey, item).Wait();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("API Key 暂时无法使用，按任意键退出！");
                        Console.ReadKey();
                        return;
                    }
                    FileInfo f = new FileInfo(item);
                    f.LastWriteTime = lastWriteTime;
                }
            }
            else
            {
                Console.WriteLine("失败：参数不正确，请输入需要处理的文件或文件夹");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }

        static async Task DisplayTinyPNG(string apiKey, string path)
        {
            using (var png = new TinyPngClient(apiKey))
            {
                var result = await png.Compress(path);
                var compressedImage = await png.Download(result);
                await compressedImage.SaveImageToDisk(path);
            }
        }
    }
}
