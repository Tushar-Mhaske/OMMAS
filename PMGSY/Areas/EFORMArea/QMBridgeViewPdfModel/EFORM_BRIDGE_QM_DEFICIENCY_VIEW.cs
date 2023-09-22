using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_DEFICIENCY_VIEW
    {
        public string IS_NO_DEFICIENCY { get; set; }
        public string IS_BOQ_NOT_CLEAR { get; set; }
        public string IS_NO_SPANS_INSUFFICIENT { get; set; }
        public string IS_NO_PROVISION_PROTECTION_WORK { get; set; }
        public string IS_HYDROLIC_DESIGN_DPR { get; set; }
        public string IS_GUARD_STONE_IN_DPR { get; set; }
        public string IS_DEVIATION_ALIGNMENT { get; set; }
        public string OTHER_COMMENT { get; set; }

    }
}