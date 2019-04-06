using AoiPoiGet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoiPoiGet
{
    public class POIAction
    {

        public void DownPoiData(string fileName, Citys city, Scenes scene)
        {
            try
            {
                //构造web服务的Url
                string httpUrl = string.Format("https://restapi.amap.com/v3/place/text?key=a1cf3409aacac2f92df039ba818e42c5&keywords=&types={0}&city={1}&output=JSON&offset=20&page=1&extensions=all&citylimit=true", scene.code, city.adcode);
                string s = HttpUtil.HTTPSGet(httpUrl);
                while (s == "")
                {
                    s = HttpUtil.HTTPSGet(httpUrl);
                }
                POI p = JsonConverter.FromJson<Model.POI>(s);
                SavePOIData(fileName, city, scene, p.pois);
                double Count = double.Parse(p.count);
                if (Count > 20)
                    GetPageData(fileName, city, scene, Count);
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-POI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
        }

        public void GetPageData(string fileName, Citys city, Scenes scene, double count)
        {
            try
            {
                double ys = count % 20;
                double pageSize = Math.Floor(count / 20);
                if (ys > 0)
                    pageSize = pageSize + 1;
                for (int i = 2; i <= pageSize; i++)
                {
                    string httpUrl = string.Format("https://restapi.amap.com/v3/place/text?key=a1cf3409aacac2f92df039ba818e42c5&keywords=&types={0}&city={1}&output=JSON&offset=20&page={2}&extensions=all&citylimit=true", scene.code, city.adcode, i);
                    string s = HttpUtil.HTTPSGet(httpUrl);
                    while (s == "")
                    {
                        s = HttpUtil.HTTPSGet(httpUrl);
                    }
                    POI p = JsonConverter.JsonDeserialize<Model.POI>(s);
                    SavePOIData(fileName, city, scene, p.pois);
                }
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(string.Format("请求{0}-{1}-{2}-{3}-{4}-{5}-POI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
        }

        public void SavePOIData(string fileName, Citys city, Scenes scene, List<poi> pois)
        {
            try
            {
               
                StringBuilder sb = new StringBuilder();
                foreach (var p in pois)
                {
                    string wgs_loaction = "";
                    if (p.location != "" && p.location.Contains(","))
                    {
                        //火星坐标转为WGS84坐标
                        LngLatPoint wgs = new Transform().GCJ2WGS(double.Parse(p.location.Split(',')[1]), double.Parse(p.location.Split(',')[0]));
                        wgs_loaction = wgs.Lng + "," + wgs.Lat;
                    }
                    string s = p.id + " ";
                    s += p.name + " ";
                    s += p.type + " ";
                    s += p.address + " ";
                    s += p.pname + " ";
                    s += p.cityname + " ";
                    s += p.adname + " ";
                    s += wgs_loaction;
                    s += "\r\n";
                    sb.Append(s);
                }
                FileStream fs = File.OpenWrite(fileName);
                fs.Position = fs.Length;
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(string.Format("保存{0}-{1}-{2}-{3}-{4}-{5}-POI数据：{6}", scene.l_class, scene.m_class, scene.s_class, city.Province, city.CityName, city.Country, ex));
            }
        }
    }
}
