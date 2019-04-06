using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Jurassic;


namespace AoiPoiGet
{
    public class HttpUtil
    {

        /// <summary>
        /// https Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HTTPSGet(string url)
        {
            string s = "";
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpRequest.Method = "GET";
                httpRequest.KeepAlive = true;
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                //发送请求
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream stream = httpResponse.GetResponseStream();
                if (stream == null)
                {
                    s = "";
                }
                else
                {
                    StreamReader sr = new StreamReader(stream);
                    s = sr.ReadToEnd();
                }
                stream.Dispose();
            }
            catch (Exception ex)
            {
                new Log().PageLog.Error(ex);
                //HTTPGet(url);
            }
            return s;
        }
        public static object DoJs(string jscode)
        {

            var engine = new Jurassic.ScriptEngine();

            return engine.Evaluate(jscode);// var addResult= engine.CallGlobalFunction(" main", 5, 6);//结果11
        }

        public static string HTTPAOIGet(string url)
        {
            Thread.Sleep(10);
            string s = "";
            try
            {
                CookieCollection cookies = new CookieCollection();
                //Cookie ck = new Cookie("key", "6a7665aa7301eae686d9e79884d0445b");
                //Cookie ck = null;
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("UM_distinctid", "166f66bcd4079-04d7ae562f7937-737356c-1fa400-166f66bcd41309");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("guid", "fbf7-aa61-04f9-1054");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("cna", "q+RqFMg80EgCAd9YCicBrQ0d");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("_uab_collina", "154173575888395752485777");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("x5sec", "7b22617365727665723b32223a223337343433373339326466613063356136306264323663383666643362653133434f44596f393846454e7a666e3447396c3443656a51453d227d");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("CNZZDATA1255626299", "121356895-1541732484-https%253A%252F%252Fwww.baidu.com%252F%7C1541852061");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                //ck = new Cookie("isg", "BPn5nNWUhgByRFqZjpPtu5LyCGUTru3e6MeVdxsuOyCfohk0Y1fdiGT7IObxGoXw");
                //ck.Domain = ".amap.com";
                //cookies.Add(ck);
                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpRequest.Method = "GET";
                httpRequest.KeepAlive = true;
                //httpRequest.Host = "www.amap.com";
                httpRequest.ContentType = "application/json; charset=utf-8";
                httpRequest.UserAgent = Usage.getUsage();
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                //httpRequest.CookieContainer = new CookieContainer();
                //httpRequest.CookieContainer.Add(cookies);
                //发送请求
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream stream = httpResponse.GetResponseStream();
                if (stream == null)
                {
                    s = "";
                }
                else
                {
                    StreamReader sr = new StreamReader(stream);
                    s = sr.ReadToEnd();
                }
                stream.Dispose();
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("未能解析此远程名称: 'www.amap.com'") || ex.Message.Contains("套接字操作尝试一个无法连接的主机") || ex.Message.Contains("无法连接到远程服务器"))
                {
                    Thread.Sleep(2000);
                    HTTPAOIGet(url);
                }
                else
                {
                    new Log().PageLog.Error(ex.Message);
                }
            }
            return s;
        }
    }
}
