using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class SIDE_SLOPE_LIST_ViewModel
    {
     
        public int SS_DETAIL_ID { get; set; }
        public int SIDE_SLOP_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<decimal> LOCATION_RD_13_1 { get; set; }
        public string SS_OBSERVED_BY_QM { get; set; }
        public string IS_SS_SATISFACTORY { get; set; }
        public string IS_PROFILE_SATISFACTORY { get; set; }
        public string GRADING { get; set; }
        
        public int RowId { get; set; }

    }
}