using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_PIPE_CULVERTS
    {
        public Nullable<decimal> TOTAL_PIPE_CULVERTS { get; set; }
        public Nullable<decimal> MINIMUM_CUSHION { get; set; }
        public Nullable<decimal> CLASS_OF_PIPES_NP2 { get; set; }
        public Nullable<decimal> CLASS_OF_PIPES_NP3 { get; set; }
        public string GRADE_OF_CONCRETE { get; set; }
        public string IS_PROTECTION_PROVIDED { get; set; }
        public string IS_PIPE_PROPERLY_PLACED { get; set; }
        public string ITEM_GRADING_13 { get; set; }
        public string IMPROVE_SUGGESTIONS { get; set; }

    }
}