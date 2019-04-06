using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoiPoiGet.Model
{
    public class Citys
    {
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        public string CityName { get; set; }

        public string Country { get; set; }

        public string citycode { get; set; }

        public string adcode { get; set; }
    }

    public class Scenes
    {
        /// <summary>
        /// 大类
        /// </summary>
        public string l_class { get; set; }
        /// <summary>
        /// 中类
        /// </summary>
        public string m_class { get; set; }
        /// <summary>
        /// 小类
        /// </summary>
        public string s_class { get; set; }

        public string code { get; set; }
    }
}
