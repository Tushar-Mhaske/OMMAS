using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EformInspDetailsViewModel
    {
        public System.DateTime VISIT_DATE { get; set; }
        public string VISITOR_NAME_DESG { get; set; }
        public Nullable<decimal> ROAD_FROM { get; set; }
        public Nullable<decimal> ROAD_TO { get; set; }
        public string INSP_LEVEL { get; set; }
        public string OBSERVATIONS { get; set; }
        public string ACTION { get; set; }
    }
}