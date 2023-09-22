using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_VIEW
    {
        public string WORK_STATUS { get; set; }
        public string C_IS_COMPLETED_WITH_DELAY { get; set; }
        public Nullable<decimal> C_IS_PERIOD_OF_DELAY { get; set; }
        public string C_AMOUNT_STATUS { get; set; }
        public Nullable<decimal> C_AMOUNT { get; set; }
        public string C_COMMENT { get; set; }
        public string P_IS_AS_PER_SCHEDULE { get; set; }
        public Nullable<decimal> P_EXT_MONTHS { get; set; }
        public string P_IS_AMOUNT_REFUNDED { get; set; }
        public Nullable<decimal> P_AMOUNT { get; set; }
        public Nullable<decimal> P_PANELTY_AMOUNT { get; set; }
        public string P_COMMENT { get; set; }

    }
}