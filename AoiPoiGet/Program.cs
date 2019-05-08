using AoiPoiGet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoiPoiGet
{
    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {

            //启动时加载日志
            string logpath = AppDomain.CurrentDomain.BaseDirectory + "log4net.xml";
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(logpath));
            string resultPath = AppDomain.CurrentDomain.BaseDirectory + "\\Result\\";
            //获取配置文件
            List<Citys> citys = AppConst.GetConfigCitys();
            List<Scenes> scens = AppConst.GetConfigScenes();
            if (AppConst.IsDownPOI == "是")
            {
                ThreadPool.SetMaxThreads(10, 10);
                ThreadPool.SetMinThreads(5, 10);
                List<WaitHandle> waitHandles = new List<WaitHandle>();
                Console.WriteLine("开始下载POI数据.....");
                foreach (Scenes scene in scens)
                {
                    foreach (Citys city in citys)
                    {
                        try
                        {
                            // 场景大类/场景中类/场景小类/省份/地市-区县-.txt
                            string subPath = string.Format("{0}/{1}/{2}/{3}/{4}/", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName);
                            string fileName = resultPath + subPath + city.Country + ".txt";
                            //先判断文件是否存在 存在的话 即定位改地市已经下载
                            if (File.Exists(fileName))
                            {
                                //File.Delete(fileName);
                                continue;
                            }
                            //创建目录
                            if (!Directory.Exists(resultPath + subPath))
                                Directory.CreateDirectory(resultPath + subPath);
                            //创建文件
                            File.Create(fileName).Dispose();
                            
                            //下载POI数据
                            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - ");
                            Console.WriteLine(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-POI数据", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country));

                            if (waitHandles.Count > 40)
                            {
                                WaitHandle.WaitAll(waitHandles.ToArray());
                                waitHandles = new List<WaitHandle>();
                            }
                            AutoResetEvent wh = new AutoResetEvent(false);
                            ThreadPameM pameM = new ThreadPameM() { FilePath = fileName, Wait = wh, city=city,scene=scene };
                            ThreadPool.QueueUserWorkItem(new WaitCallback(POIA.DownPoiData), pameM);
                            waitHandles.Add(wh);
                        }
                        catch (Exception ex)
                        {
                            new Log().PageLog.Error(ex);
                        }

                    }
                }
                if (waitHandles.Count != 0)
                {
                    WaitHandle.WaitAll(waitHandles.ToArray());
                }
                waitHandles = new List<WaitHandle>();

            }
            if (AppConst.IsDownAOI == "是")
            {
                ThreadPool.SetMaxThreads(AppConst.AoiThreadTimes, AppConst.AoiThreadTimes);
                ThreadPool.SetMinThreads(AppConst.AoiThreadTimes, AppConst.AoiThreadTimes);
                List<WaitHandle> waitHandles =new List<WaitHandle>();
                Console.WriteLine("开始下载AOI数据.....");
                for (int i = 0; i < AppConst.DownAOITimes; i++)
                {
                    foreach (Scenes scene in scens)
                    {
                        foreach (Citys city in citys)
                        {
                            try
                            {
                                // 场景大类/场景中类/场景小类/省份/地市-区县-.txt
                                string subPath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}\\", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName);
                                string fileName = resultPath + subPath + city.Country + "-AOI.txt";
                                //下载AOI数据
                                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - -");
                                Console.WriteLine(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country));
                                if (waitHandles.Count > 40)
                                {
                                    WaitHandle.WaitAll(waitHandles.ToArray());
                                    waitHandles = new List<WaitHandle>();
                                }
                                AutoResetEvent wh = new AutoResetEvent(false);
                                ThreadPameM pameM = new ThreadPameM() { FilePath = fileName, Wait = wh,OverTimes=i+1 };
                                ThreadPool.QueueUserWorkItem(new WaitCallback(AOIAction.GetAOI), pameM);
                                waitHandles.Add(wh);
                            }
                            catch (Exception ex)
                            {
                                new Log().PageLog.Error(ex);
                            }

                        }
                    }
                }
                if (waitHandles.Count != 0)
                {
                    WaitHandle.WaitAll(waitHandles.ToArray());
                }
                waitHandles = new List<WaitHandle>();
                for (int i = 0; i < AppConst.CalDownAOITimes; i++)
                {
                    foreach (Scenes scene in scens)
                    {
                        foreach (Citys city in citys)
                        {
                            try
                            {
                                // 场景大类/场景中类/场景小类/省份/地市-区县-.txt
                                string subPath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}\\", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName);
                                string fileName = resultPath + subPath + city.Country + "-AOI.txt";
                                //下载AOI数据
                                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - -");
                                Console.WriteLine(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country));
                                if (waitHandles.Count > 40)
                                {
                                    WaitHandle.WaitAll(waitHandles.ToArray());
                                    waitHandles = new List<WaitHandle>();
                                }
                                AutoResetEvent wh = new AutoResetEvent(false);
                                ThreadPameM pameM = new ThreadPameM() { FilePath = fileName, Wait = wh };
                                ThreadPool.QueueUserWorkItem(new WaitCallback(AOIAction.GetAOIDtl), pameM);
                                waitHandles.Add(wh);
                            }
                            catch (Exception ex)
                            {
                                new Log().PageLog.Error(ex);
                            }

                        }
                    }
                }
                if (waitHandles.Count != 0)
                {
                    WaitHandle.WaitAll(waitHandles.ToArray());
                }
                int allCount = 0;
                foreach (Scenes scene in scens)
                {
                    foreach (Citys city in citys)
                    {
                        try
                        {
                            // 场景大类/场景中类/场景小类/省份/地市-区县-.txt
                            string subPath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}\\", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName);
                            string fileName = resultPath + subPath + city.Country + "-AOI.txt";
                            //下载AOI数据
                            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - -");
                            Console.WriteLine(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country));
                            allCount+= AOIAction.TongJiAOIDtl(fileName);
                        }
                        catch (Exception ex)
                        {
                            new Log().PageLog.Error(ex);
                        }

                    }
                }
                Console.WriteLine($"本次共找到AOI数据{allCount}条");
                Console.ReadKey();

            }

        }


        public void doInmg(string url, string saveImgPath)
        {
            WebBrowser wb = new WebBrowser();  // 创建一个WebBrowser
            wb.ScrollBarsEnabled = false;  // 隐藏滚动条

            wb.Navigate(url);  // 打开网页
            wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);  // 增加网页加载完成
        }
        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            // 网页加载完毕才保存
            if (wb.ReadyState == WebBrowserReadyState.Complete)
            {
                // 获取网页高度和宽度,也可以自己设置
                int height = wb.Document.Body.ScrollRectangle.Height;
                int width = wb.Document.Body.ScrollRectangle.Width;

                // 调节webBrowser的高度和宽度
                wb.Height = height;
                wb.Width = width;

                Bitmap bitmap = new Bitmap(width, height);  // 创建高度和宽度与网页相同的图片
                Rectangle rectangle = new Rectangle(0, 0, width, height);  // 绘图区域
                wb.DrawToBitmap(bitmap, rectangle);  // 截图

                // 保存图片对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
                saveFileDialog.ShowDialog();

                bitmap.Save(saveFileDialog.FileName);  // 保存图片
            }
        }
    }
}
