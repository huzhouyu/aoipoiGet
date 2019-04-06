using AoiPoiGet.Model;
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
        static List<string> allIds = new List<string>();
        static List<string> urlList = new List<string> { "https://ditu.amap.com/detail/get/detail?id={0}", "https://www.amap.com/detail/get/detail?id={0}" };
        public static int OverTimes = 1;
        public static void GetAOI(string fileName, Citys city, Scenes scene)
        {
            try
            {
                string poiFileName = fileName.Replace("-AOI", "");
                if (!File.Exists(poiFileName)) return;
                List<string> ids = Getids(poiFileName);
                List<string> needWre = Getids(poiFileName);
                List<string> HttpUnit = new List<string>();
                var tmpFile = fileName.Replace("-AOI", $"-OVER{OverTimes}");
                //创建文件
                if (!File.Exists(fileName))
                    File.Create(fileName).Dispose();
                if (File.Exists(tmpFile)&& IsAoiDataGood(fileName))
                {
                    allIds.AddRange(Getids(tmpFile));
                }
                else
                {
                    File.Create(tmpFile).Dispose();
                }
                foreach (string id in ids)
                {
                    if (allIds.Contains(id))
                        continue;
                    allIds.Add(id);
                    HttpUnit.Add(id);
                }
                if (HttpUnit.Count > 0)
                {
                    RequestAOI(HttpUnit, fileName, needWre);
                }
                using (StreamWriter sw=new StreamWriter(tmpFile))
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
                new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
        }



        public static void GetAOIDtl(string fileName, Citys city, Scenes scene)
        {
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
                new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
        }


        public static void TongJiAOIDtl(string fileName, Citys city, Scenes scene)
        {
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
                List<AOIDATA> listAoi = new List<AOIDATA>();
                foreach (string poiid in ids.Keys)
                {
                    var tmp = ids[poiid];
                    var groupByTmp= tmp.GroupBy(u => u.shape);
                    var aoi = tmp.FirstOrDefault();
                    int maxCount = 0;
                    foreach (var item in groupByTmp)
                    {

                        if(maxCount< item.Count())
                        {
                            maxCount = item.Count();
                            aoi = item.FirstOrDefault();
                        }
                    }
                    listAoi.Add(aoi);
                }
                using(StreamWriter sw=new StreamWriter(fileName))
                {
                    foreach (var item in listAoi)
                    {
                        sw.WriteLine(item.ToString());
                    }
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-AOI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
        }



        public static void RequestAOI(List<string> ids, string fileName,List<string> exit)
        {
            List<Model.AOI> list = new List<AOI>();
            for (var i = 0; i < ids.Count; i++)
            {
                var a = GetAoi(ids[i]);
                if (a == null) exit.RemoveAll(u=>u==ids[i]);
                 Console.WriteLine($"当前查询了到第{i + 1}条，查询到有AOI的数据共{list.Count}条，共{ids.Count}条");
                if (a==null||a.data.spec == null || a.data.spec.mining_shape == null || a.data.spec.mining_shape.shape == null)
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
                string httpUrl = string.Format(urlList[random.Next(urlList.Count)], id);
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
                        System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
                        var tmp = processList.Where(u => u.ProcessName.ToLower() == "iexplore");
                        if (tmp.Count() <2)
                        {
                            if(boo)
                            {
                                if (bo)
                                {
                                    Process.Start("iexplore.exe", "https://www.amap.com/place/B0FFG7R3O4");  //直接打开IE浏览器，打开指定页
                                    bo = false;
                                }
                            }
                            else
                            {
                                if (bo1)
                                {
                                    Process.Start("iexplore.exe", "https://ditu.amap.com/verify/?from=https%3A%2F%2Fditu.amap.com%2Fplace%2FB00154DR22&channel=newpc&uuid=1a982e5b-8a33-4753-9cd2-900bcf8e6c5c&url=/detail/get/detail?id=B00154DR22");  //直接打开IE浏览器，打开指定页
                                    bo1 = false;
                                }
                            }
                        }
                    }
                    httpUrl = string.Format(urlList[random.Next(urlList.Count)], id);
                    Console.WriteLine($"第{i}次尝试查询，请用web访问 https://ditu.amap.com/search?query=酒店&city=310000 和 https://www.amap.com  完成认证");
                    i++;
                    Thread.Sleep(2000);
                    s = HttpUtil.HTTPAOIGet(httpUrl);
                }
                a =s!=""?JsonConverter.FromJson<Model.AOI>(s):null;
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
                        s += AppConst.GetWGSPointStr(a.data.spec.mining_shape.shape) + "\r\n";
                        sb.Append(s);
                        con += 1;
                    }
                    catch(Exception ex)
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
        public static bool IsAoiDataGood(string filePath)
        {
            StreamReader reader = File.OpenText(filePath);
            var t= reader.ReadToEnd();
            reader.Close();
            bool b= t.Contains("{");
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


        public static Dictionary<string,List<AOIDATA>> GetAOIDATA(string fileName)
        {
            Dictionary<string, List<AOIDATA>> ids = new Dictionary<string, List<AOIDATA>>();
            StreamReader reader = File.OpenText(fileName);
            string str = "";
            while ((str = reader.ReadLine()) != null)
            {
                if (str == null || str == "") continue;
                char[] separator = new char[] { ' ' };
                string[] arr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 3)
                {
                    if (ids.Keys.Contains(arr[0]))
                    {
                        ids[arr[0]].Add(new AOIDATA() { poiid = arr[0], area = arr[1], center = arr[2], shape = arr[3] });
                    }
                    else
                    {
                        List<AOIDATA> oIDATAs = new List<AOIDATA>() { new AOIDATA() { poiid = arr[0], area = arr[1], center = arr[2], shape = arr[3] } };
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
