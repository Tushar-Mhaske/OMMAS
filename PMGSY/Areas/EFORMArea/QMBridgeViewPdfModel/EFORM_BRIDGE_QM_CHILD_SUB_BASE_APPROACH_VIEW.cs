using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH_VIEW
    {
        public Nullable<decimal> RD_LOC { get; set; }
        public string IS_GRADING_CONFORM { get; set; }
        public string IS_MATERIAL_SUITABLE { get; set; }
        public Nullable<decimal> IS_DRY_DENSITY { get; set; }
        public Nullable<decimal> COMPACTION { get; set; }
        public string IS_COMPACTION_INADEQUATE { get; set; }
        public Nullable<decimal> THICKNESS_PER_DPR { get; set; }
        public Nullable<decimal> MEASURED_THICKNESS { get; set; }
        public string IS_PRESCRIBED_THICKNESS_PROV { get; set; }

    }
}