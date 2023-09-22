using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_SLAB_CULVERTS_DETAILS
    {
        public Nullable<decimal> LOCATION_RD { get; set; }
        public Nullable<decimal> SLAB_THICKNESS_ASPER_DPR { get; set; }
        public Nullable<decimal> SLAB_THICKNESS_MEAS_QM { get; set; }
        public string GRADING_SLAB_THICKNESS { get; set; }
        public string CONCRETE_GRADE_ASPER_DPR { get; set; }
        public string STRENGTH_OF_CONCRETE { get; set; }
        public string QOM { get; set; }
    }
}