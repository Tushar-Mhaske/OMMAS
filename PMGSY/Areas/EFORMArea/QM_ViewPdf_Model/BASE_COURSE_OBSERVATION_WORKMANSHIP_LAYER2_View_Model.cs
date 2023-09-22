using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class BASE_COURSE_OBSERVATION_WORKMANSHIP_LAYER2_View_Model
    {
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string GRADING_AGRI { get; set; }
        public string GRADING_PLASTICITY { get; set; }
        public Nullable<decimal> PERCENT_VOL_FILLER_MATERIAL { get; set; }
        public string GRADING_COMPACTION { get; set; }
        public Nullable<decimal> DESIGN_THICKNESS { get; set; }
        public Nullable<decimal> WBM_THICKNESS { get; set; }
        public string ADEQUATE_THICKNESS { get; set; }
       
    }
}