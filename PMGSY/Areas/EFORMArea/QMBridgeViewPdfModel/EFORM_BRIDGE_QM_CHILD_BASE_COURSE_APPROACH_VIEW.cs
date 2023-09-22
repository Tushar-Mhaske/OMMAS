using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel
{
    public class EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_VIEW
    {
        public Nullable<decimal> RD_LOC { get; set; }
        public string GRADING_AGGREGATE { get; set; }
        public string PLASTICITY_FILLER { get; set; }
        public Nullable<decimal> MATERIAL_VOLUME_AGGR { get; set; }
        public string COMP_BASED_VOLUMETRIC { get; set; }
        public Nullable<decimal> DESIGN_THICKNESS { get; set; }
        public Nullable<decimal> WBM_THICKNESS { get; set; }
        public string IS_THICKNESS_ADEQUATE { get; set; }

    }
}