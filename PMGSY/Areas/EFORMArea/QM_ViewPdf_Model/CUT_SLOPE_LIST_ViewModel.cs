using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class CUT_SLOPE_LIST_ViewModel
    {

        public int CUT_DETAIL_ID { get; set; }
        public int SIDE_SLOP_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<decimal> LOCATION_RD_13_2 { get; set; }
        public string IS_STABLE { get; set; }

        public int RowId { get; set; }
    }
}