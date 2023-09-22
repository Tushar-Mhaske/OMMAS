using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Report
{
    public class ReportFilter
    {
        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int32 LowerAdminNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public String CreditDebit { get; set; }
        public Int16 Head { get; set; }
        
        public char RoadStatus { get; set; }

        public int State { get; set; }
        public int Dpiu { get; set; }

        public String monthlyStateSrrdaDpiu { get; set; }
        public string Selection { get; set; }

        public string DPIUSelection { get; set; }
        public string HeadName { get; set; }

        public string AgencyCode { get; set; }
    }
}