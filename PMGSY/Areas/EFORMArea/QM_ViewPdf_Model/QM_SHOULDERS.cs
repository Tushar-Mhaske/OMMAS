using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_SHOULDERS
    {
        public string ITEM_EXECUTION_STATUS { get; set; }
        public string IS_NEW_TECH_USED { get; set; }
        public string NEW_TECH_NAME { get; set; }
        public string NEW_TECH_PROVIDER { get; set; }
        public string NAME_STABILISER_USED { get; set; }
        public string STABILISER_QTY_ASPER_DPR { get; set; }
        public string STABILISER_QTY_USED { get; set; }
        public Nullable<decimal> UCS_ASPER_DPR { get; set; }
        public string MATERIAL_TYPE { get; set; }
        public decimal MATERIAL_WIDTH { get; set; }
        public decimal MATERIAL_THICKNESS { get; set; }
        public string ITEM_GRADING_12 { get; set; }
        public string IMPROVE_SUGGESTION { get; set; }

    }
}