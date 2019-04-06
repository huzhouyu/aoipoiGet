using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoiPoiGet.Model
{
    public class POI
    {
        public string status { get; set; }

        public string count { get; set; }

        public string info { get; set; }

        public List<poi> pois { get; set; }
    }

    public class poi
    {
        public string id { get; set; }
        public string name { get; set; }

        public string type { get; set; }

        public string address { get; set; }

        public string location { get; set; }

        public string pname { get; set; }

        public string cityname { get; set; }

        public string adname { get; set; }
    }
}
