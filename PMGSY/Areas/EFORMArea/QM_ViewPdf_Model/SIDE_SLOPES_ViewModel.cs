using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class SIDE_SLOPES_ViewModel
    {

        public int SIDE_SLOP_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string SIDE_SLOPS_ASPER_DPR { get; set; }
        public string IS_ANALYSIS_DONE { get; set; }
        public string OBSERVATIONS { get; set; }
        public string SUBITEM_GRADING_5IV { get; set; }
        public string IMPROVEMENT_REMARK_5IV { get; set; }
        public string ITEM_GRADING_5 { get; set; }
        public string IMPROVEMENT_REMARK_5 { get; set; }


    }
}