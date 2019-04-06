using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoiPoiGet
{
   public  class Transform
   {
       private static double x_pi = 52.359877559829883;

       public LngLatPoint bdToGcj(double bd_lat, double bd_lon)
       {
           double x = bd_lon - 0.0065;
           double y = bd_lat - 0.006;
           double num3 = Math.Sqrt((x * x) + (y * y)) - (2E-05 * Math.Sin(y * x_pi));
           double d = Math.Atan2(y, x) - (3E-06 * Math.Cos(x * x_pi));
           double lng = num3 * Math.Cos(d);
           return new LngLatPoint(num3 * Math.Sin(d), lng);
       }

       private LngLatPoint Delta(double lat, double lng)
       {
           LngLatPoint point = new LngLatPoint();
           double num = 6378245.0;
           double num2 = 0.0066934216229659433;
           double num3 = this.TransformLat(lng - 105.0, lat - 35.0);
           double num4 = this.TransformLon(lng - 105.0, lat - 35.0);
           double a = (lat / 180.0) * 3.1415926535897931;
           double d = Math.Sin(a);
           d = 1.0 - ((num2 * d) * d);
           double num7 = Math.Sqrt(d);
           num3 = (num3 * 180.0) / (((num * (1.0 - num2)) / (d * num7)) * 3.1415926535897931);
           num4 = (num4 * 180.0) / (((num / num7) * Math.Cos(a)) * 3.1415926535897931);
           point.Lat = num3;
           point.Lng = num4;
           return point;
       }

       public double Distance(double latA, double lngA, double latB, double lngB)
       {
           double num = 6371000.0;
           double num2 = (Math.Cos((latA * 3.1415926535897931) / 180.0) * Math.Cos((latB * 3.1415926535897931) / 180.0)) * Math.Cos(((lngA - lngB) * 3.1415926535897931) / 180.0);
           double num3 = Math.Sin((latA * 3.1415926535897931) / 180.0) * Math.Sin((latB * 3.1415926535897931) / 180.0);
           double d = num2 + num3;
           if (d > 1.0)
           {
               d = 1.0;
           }
           if (d < -1.0)
           {
               d = -1.0;
           }
           return (Math.Acos(d) * num);
       }

       public LngLatPoint GCJ2WGS(double gcjLat, double gcjLng)
       {
           if (this.OutOfChina(gcjLat, gcjLng))
           {
               return new LngLatPoint(gcjLat, gcjLng);
           }
           LngLatPoint point = this.Delta(gcjLat, gcjLng);
           return new LngLatPoint(gcjLat - point.Lat, gcjLng - point.Lng);
       }

       public LngLatPoint GCJ2WGSExact(double gcjLat, double gcjLng)
       {
           double num = 0.01;
           double num2 = 1E-06;
           double num3 = num;
           double num4 = num;
           double num5 = gcjLat - num3;
           double num6 = gcjLng - num4;
           double num7 = gcjLat + num3;
           double num8 = gcjLng + num4;
           double wgsLat = 0.0;
           double wgsLng = 0.0;
           for (int i = 0; i < 0x3e8; i++)
           {
               wgsLat = (num5 + num7) / 2.0;
               wgsLng = (num6 + num8) / 2.0;
               LngLatPoint point = this.WGS2GCJ(wgsLat, wgsLng);
               num3 = point.Lat - gcjLat;
               num4 = point.Lng - gcjLng;
               if ((Math.Abs(num3) < num2) && (Math.Abs(num4) < num2))
               {
                   return new LngLatPoint(wgsLat, wgsLng);
               }
               if (num3 > 0.0)
               {
                   num7 = wgsLat;
               }
               else
               {
                   num5 = wgsLat;
               }
               if (num4 > 0.0)
               {
                   num8 = wgsLng;
               }
               else
               {
                   num6 = wgsLng;
               }
           }
           return new LngLatPoint(wgsLat, wgsLng);
       }

       public LngLatPoint gcjToBd(double gg_lat, double gg_lon)
       {
           double x = gg_lon;
           double y = gg_lat;
           double num5 = Math.Sqrt((x * x) + (y * y)) + (2E-05 * Math.Sin(y * x_pi));
           double d = Math.Atan2(y, x) + (3E-06 * Math.Cos(x * x_pi));
           double lng = (num5 * Math.Cos(d)) + 0.0065;
           return new LngLatPoint((num5 * Math.Sin(d)) + 0.006, lng);
       }

       private bool OutOfChina(double lat, double lng)
       {
           return (((lng < 72.004) || (lng > 137.8347)) || ((lat < 0.8293) || (lat > 55.8271)));
       }

       private double TransformLat(double x, double y)
       {
           double num = ((((-100.0 + (2.0 * x)) + (3.0 * y)) + ((0.2 * y) * y)) + ((0.1 * x) * y)) + (0.2 * Math.Sqrt(Math.Abs(x)));
           num += (((20.0 * Math.Sin((6.0 * x) * 3.1415926535897931)) + (20.0 * Math.Sin((2.0 * x) * 3.1415926535897931))) * 2.0) / 3.0;
           num += (((20.0 * Math.Sin(y * 3.1415926535897931)) + (40.0 * Math.Sin((y / 3.0) * 3.1415926535897931))) * 2.0) / 3.0;
           return (num + ((((160.0 * Math.Sin((y / 12.0) * 3.1415926535897931)) + (320.0 * Math.Sin((y * 3.1415926535897931) / 30.0))) * 2.0) / 3.0));
       }

       private double TransformLon(double x, double y)
       {
           double num = ((((300.0 + x) + (2.0 * y)) + ((0.1 * x) * x)) + ((0.1 * x) * y)) + (0.1 * Math.Sqrt(Math.Abs(x)));
           num += (((20.0 * Math.Sin((6.0 * x) * 3.1415926535897931)) + (20.0 * Math.Sin((2.0 * x) * 3.1415926535897931))) * 2.0) / 3.0;
           num += (((20.0 * Math.Sin(x * 3.1415926535897931)) + (40.0 * Math.Sin((x / 3.0) * 3.1415926535897931))) * 2.0) / 3.0;
           return (num + ((((150.0 * Math.Sin((x / 12.0) * 3.1415926535897931)) + (300.0 * Math.Sin((x / 30.0) * 3.1415926535897931))) * 2.0) / 3.0));
       }

       public LngLatPoint WGS2GCJ(double wgsLat, double wgsLng)
       {
           if (this.OutOfChina(wgsLat, wgsLng))
           {
               return new LngLatPoint(wgsLat, wgsLng);
           }
           LngLatPoint point = this.Delta(wgsLat, wgsLng);
           return new LngLatPoint(wgsLat + point.Lat, wgsLng + point.Lng);
       }
    }

   public class LngLatPoint
   {

       public LngLatPoint() { }
       public LngLatPoint(double lat, double lng)
       {
           this.Lng = lng;
           this.Lat = lat;
       }
       public double Lng { get; set; }

       public double Lat { get; set; }
   }
}
