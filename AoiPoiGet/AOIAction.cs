﻿using AoiPoiGet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace AoiPoiGet
{

    public static class AOIAction
    {
        static object LockObj = new object();
        static Dictionary<int, List<string>> allIds = new Dictionary<int, List<string>>();
        static List<string> urlList = new List<string> { "https://ditu.amap.com/detail/get/detail?id={0}", "https://www.amap.com/detail/get/detail?id={0}" };
        static int isNowDoYanzhengMa = 0;
        static int nowIndex = 0;
        static bool isLock = false;
        static object LockAddallIdsKey = new object();
        public static void GetAOI(object obj)
        {
            var tmp = (ThreadPameM)obj;
            string fileName = tmp.FilePath;
            lock (LockAddallIdsKey)
            {
                try
                {
                    if (!allIds.ContainsKey(tmp.OverTimes))
                    {
                        allIds.Add(tmp.OverTimes, new List<string>());
                    }
                }
                catch
                {

                }
            }
            try
            {
                string poiFileName = fileName.Replace("-AOI", "");
                if (!File.Exists(poiFileName))
                {
                    tmp.Wait.Set();
                    return;
                }
                List<string> ids = Getids(poiFileName);
                List<string> needWre = Getids(poiFileName);
                List<string> HttpUnit = new List<string>();
                var tmpFile = fileName.Replace("-AOI", $"-OVER{tmp.OverTimes}");
                //创建文件
                if (!File.Exists(fileName))
                    File.Create(fileName).Dispose();
                if (File.Exists(tmpFile) && IsAoiDataGood(fileName, tmp.OverTimes))
                {

                    allIds[tmp.OverTimes].AddRange(Getids(tmpFile));

                }
                else
                {
                    File.Create(tmpFile).Dispose();
                }
                foreach (string id in ids)
                {
                    if (allIds[tmp.OverTimes].Contains(id))
                        continue;
                    allIds[tmp.OverTimes].Add(id);
                    HttpUnit.Add(id);
                }
                if (HttpUnit.Count > 0)
                {
                    RequestAOI(HttpUnit, fileName, needWre);
                }
                using (StreamWriter sw = new StreamWriter(tmpFile))
                {
                    foreach (var item in needWre)
                    {
                        sw.WriteLine(item);
                    }
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                //new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
            tmp.Wait.Set();
        }



        public static void GetAOIDtl(object obj)
        {

            var tmp = (ThreadPameM)obj;
            string fileName = tmp.FilePath;
            try
            {
                if (!File.Exists(fileName))
                {
                    File.Create(fileName).Dispose();
                }
                List<string> ids = Getids(fileName);
                var tmpFile = fileName.Replace("-AOI", $"-CalAOI");
                //创建文件
                if (!File.Exists(tmpFile))
                    File.Create(tmpFile).Dispose();
                List<string> HttpUnit = new List<string>();
                foreach (string id in ids)
                {
                    HttpUnit.Add(id);
                }
                if (HttpUnit.Count > 0)
                {
                    RequestAOI(HttpUnit, tmpFile, new List<string>());
                }
            }
            catch (Exception ex)
            {
                //new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
            tmp.Wait.Set();
        }


        public static int TongJiAOIDtl(object filePath)
        {
            string fileName = filePath.ToString();
            try
            {
                if (!File.Exists(fileName))
                {
                    File.Create(fileName).Dispose();
                }
                var tmpFile = fileName.Replace("-AOI", $"-CalAOI");
                //创建文件
                if (!File.Exists(tmpFile))
                    File.Create(tmpFile).Dispose();
                var ids = GetAOIDATA(tmpFile);
                List<string> HttpUnit = new List<string>();
                List<AOIHelp> listAoi = new List<AOIHelp>();
                foreach (string poiid in ids.Keys)
                {
                    var tmp = ids[poiid];
                    var groupByTmp = tmp.GroupBy(u => u.shape);
                    var aoi = tmp.FirstOrDefault();
                    int maxCount = 0;
                    foreach (var item in groupByTmp)
                    {

                        if (maxCount < item.Count())
                        {
                            maxCount = item.Count();
                            aoi = item.FirstOrDefault();
                        }
                    }
                    listAoi.Add(aoi);
                }
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    foreach (var item in listAoi)
                    {
                        sw.WriteLine(item.ToString());
                    }
                    sw.Flush();
                }
                return listAoi.Count;
            }
            catch (Exception ex)
            {
                //new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
            return 0;
        }



        public static void RequestAOI(List<string> ids, string fileName, List<string> exit)
        {
            List<Model.AOI> list = new List<AOI>();
            for (var i = 0; i < ids.Count; i++)
            {
                var a = GetAoi(ids[i]);
                if (a == null) exit.RemoveAll(u => u == ids[i]);
                Console.WriteLine($"当前查询了到第{i + 1}条，查询到有AOI的数据共{list.Count}条，共{ids.Count}条");
                if (a == null || a.data.spec == null || a.data.spec.mining_shape == null || a.data.spec.mining_shape.shape == null)
                    continue;
                list.Add(a);
            }

            SaveAOI(list, fileName);

        }
        public static Model.AOI GetAoi(string id)
        {
            Model.AOI a = null;
            try
            {
                Dictionary<bool, int> failDic = new Dictionary<bool, int>();
                failDic.Add(true, 0);
                failDic.Add(false, 0);
                Random random = new Random();
                string httpUrl = string.Format(urlList[nowIndex], id);
                if (!isLock)
                {
                    nowIndex = (nowIndex + 1) % 2;
                }
                string s = HttpUtil.HTTPAOIGet(httpUrl);
                int i = 1;
                bool bo = true;
                bool bo1 = true;
                while (s.Contains("\"status\":\"6\",\"data\":\"too fast\""))
                {
                    var boo = httpUrl.Contains("www.amap.com");
                    failDic[boo]++;
                    if (failDic[boo] > 0)
                    {
                        //System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();


                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                        p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                        p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                        p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                        p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                        lock (LockObj)
                        {
                            if (isNowDoYanzhengMa % 10 == 0)
                            {
                                isLock = true;
                                nowIndex = (nowIndex + 1) % 2;
                                isNowDoYanzhengMa = 0;
                                if (boo)
                                {
                                    if (bo || i / 4 == 0)
                                    {
                                        bo = false;
                                        p.Start();//启动程序
                                                  //向cmd窗口发送输入信息
                                        p.StandardInput.WriteLine($@"python {AppDomain.CurrentDomain.BaseDirectory}破解极验滑动验证码完整代码.py https://www.amap.com/place/B0FFG7R3O4");
                                        p.StandardInput.WriteLine("exit");
                                    }

                                }
                                else
                                {
                                    if (bo1 || i / 4 == 0)
                                    {
                                        bo1 = false;
                                        p.Start();//启动程序
                                                  //向cmd窗口发送输入信息
                                        p.StandardInput.WriteLine($@"python {AppDomain.CurrentDomain.BaseDirectory}破解极验滑动验证码完整代码.py https://ditu.amap.com/search?query=酒店&city=310000");
                                        p.StandardInput.WriteLine("exit");
                                    }
                                }
                                Thread.Sleep(30000);
                                isLock = false;
                            }
                            isNowDoYanzhengMa++;
                        }

                    }
                    httpUrl = string.Format(urlList[nowIndex], id);
                    Console.WriteLine($"第{i}次尝试查询，请用web访问 https://ditu.amap.com/search?query=酒店&city=310000 和 https://www.amap.com  完成认证");
                    i++;
                    Thread.Sleep(2000);
                    s = HttpUtil.HTTPAOIGet(httpUrl);
                }
                a = s != "" ? JsonConverter.FromJson<Model.AOI>(s) : null;
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(string.Format("请求{0}-AOI数据：{1}", id, ex));
            }
            return a;
        }


        public static void SaveAOI(List<Model.AOI> aois, string fileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                int con = 0;
                //var idsList = Getids(fileName);
                foreach (var a in aois)
                {
                    try
                    {
                        //if (idsList.Contains(a.data.@base.poiid)) continue;
                        if (a.data.spec == null || a.data.spec.mining_shape == null || a.data.spec.mining_shape.shape == null)
                            continue;
                        string wgs_loaction = "";
                        string cen = a.data.spec.mining_shape.center;
                        if (cen != "" && cen.Contains(","))
                        {
                            //火星坐标转为WGS84坐标
                            LngLatPoint wgs = new Transform().GCJ2WGS(double.Parse(cen.Split(',')[1]), double.Parse(cen.Split(',')[0]));
                            wgs_loaction = wgs.Lng + "," + wgs.Lat;
                        }
                        string s = string.Empty;
                        s += a.data.@base.poiid + " ";
                        s += a.data.spec.mining_shape.area + " ";
                        s += wgs_loaction + " ";
                        s += a.data.@base.name + " " + a.data.@base.city_name + " " + a.data.@base.address + " ";
                        s += AppConst.GetWGSPointStr(a.data.spec.mining_shape.shape) + "\r\n";
                        sb.Append(s);
                        con += 1;
                    }
                    catch (Exception ex)
                    {
                        new Log().PageLog.Error(string.Format("保存AOI数据：{0}", ex));
                    }
                }
                Console.WriteLine(string.Format("下载{0}条数据", con));
                FileStream fs = File.OpenWrite(fileName);
                fs.Position = fs.Length;
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(string.Format("保存AOI数据：{0}", ex));
            }
        }
        public static bool IsAoiDataGood(string filePath, int OverTimes)
        {
            StreamReader reader = File.OpenText(filePath);
            var t = reader.ReadToEnd();
            reader.Close();
            bool b = t.Contains("{");
            if (b)
            {
                File.Delete(filePath);
                File.Delete(filePath.Replace("-AOI", $"-OVER{OverTimes}"));
                File.Create(filePath).Dispose();
            }
            b = !b;
            return b;
        }

        public static List<string> Getids(string fileName)
        {
            List<string> ids = new List<string>();
            StreamReader reader = File.OpenText(fileName);
            string str = "";
            while ((str = reader.ReadLine()) != null)
            {
                if (str == null || str == "") continue;
                char[] separator = new char[] { ' ' };
                string[] arr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                ids.Add(arr[0]);
            }
            reader.Close();
            reader = null;
            return ids;
        }


        public static Dictionary<string, List<AOIHelp>> GetAOIDATA(string fileName)
        {
            Dictionary<string, List<AOIHelp>> ids = new Dictionary<string, List<AOIHelp>>();
            StreamReader reader = File.OpenText(fileName);
            string str = "";
            while ((str = reader.ReadLine()) != null)
            {
                if (str == null || str == "") continue;
                char[] separator = new char[] { ' ' };
                string[] arr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 6)
                {
                    if (ids.Keys.Contains(arr[0]))
                    {
                        ids[arr[0]].Add(new AOIHelp(str));
                    }
                    else
                    {
                        List<AOIHelp> oIDATAs = new List<AOIHelp>() { new AOIHelp(str) };
                        ids.Add(arr[0], oIDATAs);
                    }
                }
            }
            reader.Close();
            reader = null;
            return ids;
        }

    }

}
