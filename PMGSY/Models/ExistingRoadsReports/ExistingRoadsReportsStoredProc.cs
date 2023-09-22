using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.ExistingRoadsReports
{
    public class ExistingRoadsReportsStoredProc
    {

       
    }
    public partial class USP_DRRP_R7_StateLevel_REPORT
    {
        public int LOCATION_CODE { get; set; }
        public string LOCATION_NAME { get; set; }
        public int ROAD_PLAIN { get; set; }
        public int ROAD_CPLAIN { get; set; }
        public int ROAD_ROLL { get; set; }
        public int ROAD_CROLL { get; set; }      
        public int ROAD_TOTAL { get; set; }
        public int ROAD_CTOTAL { get; set; }
    }
}