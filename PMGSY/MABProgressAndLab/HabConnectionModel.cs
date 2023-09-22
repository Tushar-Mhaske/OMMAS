using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.MABProgressAndLab
{
    public class HabConnectionModel
    {
        public string HabCode { get; set; }
        public string HabName { get; set; }
        public string HabPopulation { get; set; }
        public string HabMaxOrder { get; set; }
    }

    public class ImageDecscrptionModel
    {
        public string HeadCode { get; set; }
        public string HeadSHDesc { get; set; }
        public string HeadType { get; set; }
    }
}