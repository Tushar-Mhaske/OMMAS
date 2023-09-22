using System;
using System.Collections.Generic;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{  
    public class QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG
    {       
        public string WORK_STATUS { get; set; }
        public string C_IS_COMPLETED_WITH_DELAY { get; set; }
        public Nullable<decimal> C_PERIOD_OF_DELAY { get; set; }
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
