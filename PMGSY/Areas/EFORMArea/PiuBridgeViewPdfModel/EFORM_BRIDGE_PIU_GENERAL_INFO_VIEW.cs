using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel
{
    public class EFORM_BRIDGE_PIU_GENERAL_INFO_VIEW
    {
        public string WORK_STATUS { get; set; }
        public Nullable<System.DateTime> INSPECTION_DATE { get; set; }
        public string QM_CODE { get; set; }
        public Nullable<decimal> DEVIATION_LENGTH { get; set; }
        public string DEVIATION_REASON { get; set; }
        public Nullable<decimal> TECHNICAL_SANC_COST { get; set; }
        public Nullable<decimal> EXPENDITURE_DONE { get; set; }
        public Nullable<decimal> TOTAL_EXPENDITURE { get; set; }
        public Nullable<decimal> BILLS_PENDING { get; set; }
        public string AWARD_OF_WORK_DATE { get; set; }
        public string START_OF_WORK_DATE { get; set; }
        public string STIPULATED_COMPLETION_DATE { get; set; }
        public string ACTUAL_COMPLETION_DATE { get; set; }
        public string PIU_HEAD_NAME { get; set; }
        public string PIU_HEAD_MOBILE_NO { get; set; }
        public string PIU_HEAD_EMAIL { get; set; }
        public string PIU_ADDR { get; set; }
        public string PIU_SIGN_DATE { get; set; }

    }
}