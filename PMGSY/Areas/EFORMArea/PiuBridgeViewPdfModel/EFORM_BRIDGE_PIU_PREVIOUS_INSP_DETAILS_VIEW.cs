using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel
{
    public class EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS_VIEW
    {
        public string VISIT_DATE { get; set; }
        public string VISITOR_NAME_DESG { get; set; }
        public string WORK_STAGE_INSP { get; set; }

        public decimal? ROAD_FROM { get; set; }

        public decimal? ROAD_TO { get; set; }

        public string INSP_LEVEL { get; set; }
        public string OBSERVATIONS { get; set; }
        public string ACTION { get; set; }

    }
}