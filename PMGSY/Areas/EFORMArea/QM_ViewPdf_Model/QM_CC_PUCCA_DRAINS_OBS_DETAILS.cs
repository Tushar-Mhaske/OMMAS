using System;
using System.Collections.Generic;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{  
    public class QM_CC_PUCCA_DRAINS_OBS_DETAILS
    {
        public Nullable<decimal> LOCATION_RD_FROM { get; set; }
        public Nullable<decimal> LOCATION_RD_TO { get; set; }
        public Nullable<decimal> LOCATION_RD { get; set; }
        public Nullable<decimal> CS_SIZE_B_ASPER_DPR { get; set; }
        public Nullable<decimal> CS_SIZE_D_ASPER_DPR { get; set; }
        public Nullable<decimal> CS_SIZE_B_MEAS { get; set; }
        public Nullable<decimal> CS_SIZE_D_MEAS { get; set; }
        public string IS_SIZE_DRAINS_ACCEPTABLE { get; set; }
        public Nullable<decimal> SOC_ASPER_QCR1 { get; set; }
        public string GRADING_GEN_QOM { get; set; }
      
    }
}
