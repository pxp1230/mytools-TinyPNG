using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TinyPng;

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
                    //try
                    //{
                    //    DisplayTinyPNG(apiKey, item).Wait();
                    //}
                    //catch (Exception)
                    //{
                    //    Console.WriteLine("API Key 暂时无法使用，按任意键退出！");
                    //    Console.ReadKey();
                    //    return;
                    //}


                    //参考：https://blog.csdn.net/lhtzbj12/article/details/54143044
                    try
                    {
                        Image srcImage = Image.FromFile(item);
                        ImageFormat format = srcImage.RawFormat;
                        Bitmap destImage = new Bitmap(srcImage.Width, srcImage.Height);
                        var graphics = GetGraphics(destImage);
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                        srcImage.Dispose();
                        if (format.Guid == ImageFormat.Jpeg.Guid)
                        {
                            destImage.Save(item, ImageFormat.Jpeg);
                            destImage.Dispose();
                            FileInfo f = new FileInfo(item);
                            f.LastWriteTime = lastWriteTime;
                        }
                        else if (format.Guid == ImageFormat.Png.Guid)
                        {
                            destImage.Save(item, ImageFormat.Png);
                            destImage.Dispose();
                            FileInfo f = new FileInfo(item);
                            f.LastWriteTime = lastWriteTime;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadKey();
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("失败：参数不正确，请输入需要处理的文件或文件夹");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 获取高清的Graphics
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        static Graphics GetGraphics(Image img)
        {
            var g = Graphics.FromImage(img);
            //设置质量
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            //InterpolationMode不能使用High或者HighQualityBicubic,如果是灰色或者部分浅色的图像是会在边缘处出一白色透明的线
            //用HighQualityBilinear却会使图片比其他两种模式模糊（需要肉眼仔细对比才可以看出）
            g.InterpolationMode = InterpolationMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            return g;
        }

        //static async Task DisplayTinyPNG(string apiKey, string path)
        //{
        //    using (var png = new TinyPngClient(apiKey))
        //    {
        //        var result = await png.Compress(path);
        //        var compressedImage = await png.Download(result);
        //        await compressedImage.SaveImageToDisk(path);
        //    }
        //}
    }
}
