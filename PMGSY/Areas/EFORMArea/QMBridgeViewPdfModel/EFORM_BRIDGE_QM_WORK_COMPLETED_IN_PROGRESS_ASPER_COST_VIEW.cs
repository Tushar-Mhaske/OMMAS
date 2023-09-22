using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_VIEW
    {
        public string WORK_STATUS { get; set; }
        public Nullable<decimal> SANCTION_COST { get; set; }
        public Nullable<decimal> COMPLETION_COST { get; set; }
        public string REASON_EXTRA_COST { get; set; }
        public string ACTION_BY_PIU { get; set; }

    }
}