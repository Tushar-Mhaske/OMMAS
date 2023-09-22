using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_QUALITY_ATTENTION_VIEW
    {
        public string IS_ALL_TEST_CONDUCTED { get; set; }
        public string IS_QC_REG_P1_MAINTAINED { get; set; }
        public string IS_QC_REG_P2_MAINTAINED { get; set; }
        public string IS_NEGLIGENCE { get; set; }
        public string IS_LOE { get; set; }
        public string IS_LOK { get; set; }
        public string IS_OTHER { get; set; }
        public string OTHER_REASON { get; set; }
        public string IS_NON_CONFORMITIES_QCR2 { get; set; }
        public string IS_NDT_CONDUCTED { get; set; }
        public string ITEM_GRADING_3 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }

    }
}