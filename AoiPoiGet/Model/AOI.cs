using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoiPoiGet.Model
{
    public class AOI
    {

        public string status { get; set; }

        public data data { get; set; }
    }

    public class data
    {
        public spec spec { get; set; }

        public jichu @base { get; set; }

    }

    public class spec
    {
        public mining_shape mining_shape { get; set; }
    }

    public class mining_shape
    {
        public string center { get; set; }

        public string area { get; set; }

        public string shape { get; set; }
    }

    public class jichu
    {
        public string poiid { get; set; }
        public string name { get; set; }

        public string city_name { get; set; }

        public string address { get; set; }
    }


    public class AOIDATA: mining_shape
    {
        public string poiid { get; set; }

        public override string ToString()
        {
            return $"{this.poiid} {this.area} {this.center} {this.shape}";
        }

    }

    public class AOIHelp : mining_shape
    {

        public AOIHelp(string str)
        {
            var arr = str?.Split(' ');
            this.poiid = arr?[0];
            this.area = arr?[1];
            this.center = arr?[2];
            this.shape = arr?[3];
            this.name = arr?[4];
            this.city_name = arr?[5];
            this.address = arr?[6];
        }
        public string poiid { get; set; }
        public string name { get; set; }

        public string city_name { get; set; }

        public string address { get; set; }
        public override string ToString()
        {
            return $"{this.poiid} {this.area} {this.center} {this.shape} {this.name} {this.city_name} {this.address}";
        }

    }


}
