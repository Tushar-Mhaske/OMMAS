using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class BITUMINOUS_BASECOURSE_OBSERVATION_ViewModel
    {
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string GRADING_COARSE_AGG { get; set; }
        public Nullable<decimal> DENSITY_ACHIEVED { get; set; }
        public string PERCENT_COMPAQ { get; set; }
        public Nullable<decimal> PERCENT_BITCONT_ASPER_QCR1 { get; set; }
        public Nullable<decimal> PERCENT_BITCONT_MEAS_QM { get; set; }
        public string GRADING_PERCENT_BITCONT { get; set; }
        public Nullable<decimal> OBSERV_TOL_MEAS_QM { get; set; }
        public string GRADING_OBSERV_TOL { get; set; }
        
    }
}