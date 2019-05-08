using AoiPoiGet.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AoiPoiGet
{
    public static class AppConst
    {
        public readonly static string IsDownPOI = ConfigurationManager.AppSettings["IsDownPOI"];
        public readonly static string IsDownAOI = ConfigurationManager.AppSettings["IsDownAOI"];
        public readonly static int DownAOITimes =Convert.ToInt32(ConfigurationManager.AppSettings["DownAOITimes"]);
        public readonly static int CalDownAOITimes = Convert.ToInt32(ConfigurationManager.AppSettings["CalDownAOITimes"]);


        public readonly static int AoiThreadTimes = Convert.ToInt32(ConfigurationManager.AppSettings["AoiThreadTimes"]);
        public readonly static int AoiThreadSleepTime = Convert.ToInt32(ConfigurationManager.AppSettings["AoiThreadSleepTime"]);

        /// <summary>
        /// 获取地市配置文件
        /// </summary>
        /// <returns></returns>
        public static List<Citys> GetConfigCitys()
        {
            
            List<Citys> list = new List<Citys>();
            try
            {
                string ProjectionDefinePath = AppDomain.CurrentDomain.BaseDirectory + "/config/City.txt";
                StreamReader reader = File.OpenText(ProjectionDefinePath);
                string str = "";
                while ((str = reader.ReadLine()) != null)
                {
                    char[] separator = new char[] { '|' };
                    string[]  arr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    Citys c = new Citys();
                    c.Province = arr[0];
                    c.CityName = arr[1];
                    c.Country = arr[2];
                    c.citycode = arr[4];
                    c.adcode = arr[3];
                    list.Add(c);
                }
                reader.Close();
                reader = null;
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error("读取地市配置文件：" + ex);
            }

            return list;
        }
        /// <summary>
        /// 获取场景配置文件
        /// </summary>
        /// <returns></returns>
        public static List<Scenes> GetConfigScenes()
        {
            List<Scenes> list = new List<Scenes>();
            try
            {
                string ProjectionDefinePath = AppDomain.CurrentDomain.BaseDirectory + "/config/Scene.txt";
                StreamReader reader = File.OpenText(ProjectionDefinePath);
                string str = "";
                while ((str = reader.ReadLine()) != null)
                {
                    char[] separator = new char[] { '|' };
                    string[] arr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    Scenes c = new Scenes();
                    c.l_class = arr[0];
                    c.m_class = arr[1];
                    c.s_class = arr[2];
                    c.code = arr[3];
                    list.Add(c);
                }
                reader.Close();
                reader = null;
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error("读取场景配置文件：" + ex);
            }

            return list;
        }


        /// <summary>
        /// 获取轮廓点集合
        /// </summary>
        /// <param name="gcjPointstr"></param>
        /// <returns></returns>
        public static string GetWGSPointStr(string gcjPointstr)
        {
            string wgsPoints = string.Empty;
            List<string> listStr = new List<string>();
            string[] s = gcjPointstr.Split(';');
            foreach (string ls in s)
            {
                string[] ss = ls.Split(',');
                LngLatPoint wgs = new Transform().GCJ2WGS(double.Parse(ss[1]), double.Parse(ss[0]));
                listStr.Add(wgs.Lng + "," + wgs.Lat);
            }
            wgsPoints = string.Join(";", listStr);
            return wgsPoints;
        }

        public static CookieCollection GetConfigCookies()
        {
            string cs = ConfigurationManager.AppSettings["cookie"];
            
            CookieCollection cookies = new CookieCollection();
            string[] s = cs.Split(';');
            foreach (var ss in s)
            {
                string[] ccs = ss.Split('=');
                Cookie ck = new Cookie(ccs[0], ccs[1]);
                ck.Domain = "www.amap.com";
                cookies.Add(ck);
            }
            return cookies;
        }
    }
}
