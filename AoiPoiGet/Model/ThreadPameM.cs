﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoiPoiGet.Model
{
    public class ThreadPameM
    {
        public string FilePath { get; set; }
        public AutoResetEvent Wait { get; set; }
        public int OverTimes { get; set; }
        public Citys city { get; set; }
        public Scenes scene { get; set; }
    }
}
