using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class AttentionToQuality_ViewModel
    {       
        public int EFORM_ID { get; set; }      
        public int PR_ROAD_CODE { get; set; }
        public string IS_ALL_TEST_CONDUCTED { get; set; }
        public string IS_QC_REG_P1_MAINTAINED { get; set; }
        public string IS_QC_REG_P2_MAINTAINED { get; set; }
        public string IS_NEGLIGENCE { get; set; }
        public string IS_LOE { get; set; }
        public string IS_LOK { get; set; }
        public string IS_OTHER { get; set; }
        public string OTHER_REASON { get; set; }
        public string IS_NON_CONFORMITIES_QCR2 { get; set; }
        public string ITEM_GRADING_3 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }
    }
}