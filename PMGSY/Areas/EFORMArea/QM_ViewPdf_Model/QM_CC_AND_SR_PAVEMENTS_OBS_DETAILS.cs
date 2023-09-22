using System;
using System.Collections.Generic;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{    
    public class QM_CC_AND_SR_PAVEMENTS_OBS_DETAILS
    {
        public Nullable<decimal> REF_RD_FROM { get; set; }
        public Nullable<decimal> REF_RD_TO { get; set; }
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string GRADING_QOM { get; set; }
        public string CONCRETE_STRENGTH_ASPER_QCR1 { get; set; }
        public string GRADING_QOW { get; set; }
        public Nullable<decimal> THICKNESS_ASPER_DPR { get; set; }
        public Nullable<decimal> THICKNESS_MEAS_QM { get; set; }
        public string IS_THICKNESS_ACCEPTABLE { get; set; }
    }
}
