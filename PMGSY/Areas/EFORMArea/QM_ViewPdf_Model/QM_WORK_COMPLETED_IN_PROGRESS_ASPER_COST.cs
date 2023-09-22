using System;
using System.Collections.Generic;


namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST
    {
        public string WORK_STATUS { get; set; }
        public Nullable<decimal> SANCTION_COST { get; set; }
        public Nullable<decimal> COMPLETION_COST { get; set; }
        public string REASON_FOR_EXTRA { get; set; }
        public string IS_REVISED_DPR_PREPARED { get; set; }
        public string IS_CHANGED_SCOPEOFWORK { get; set; }
        public string IS_VARIATION_IN_QTY { get; set; }
        public string IS_COST_APPROVED { get; set; }
        public string OTHER_DESCRIBTION { get; set; }
        public string IS_OTHER { get; set; }
      
    }
}
