using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.PMIS.Models
{
    //public class GanttChartResponseModel
    //{ }
        public class GanttChartResponseModel
        {
            public List<GanttChartData> ListForChart = new List<GanttChartData>();
        }


        public class GanttChartData
        {
            public string name { get; set; }
            public double start { get; set; }
            public double end { get; set; }
            public string State { get; set; }
            public string District { get; set; }
            public string BlockName { get; set; }
            public string PiuName { get; set; }
            public int SanctionYear { get; set; }
            public string SanctionDate { get; set; }
            public int RoadCode { get; set; }
            public string RoadName { get; set; }
            public decimal SanctionedLength { get; set; }
            public int ImsBatch { get; set; }
            public decimal? SanctionCost { get; set; }
            public decimal? AgreementCost { get; set; }
            public string PackageNo { get; set; }
            public CompletedClass completed { get; set; }
            public string color { get; set; }

        }


        public class CompletedClass
        {
            public decimal amount { get; set; }
            public string fill { get; set; }
        }

}