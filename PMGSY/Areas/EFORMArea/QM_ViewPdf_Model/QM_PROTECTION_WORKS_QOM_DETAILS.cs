using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_PROTECTION_WORKS_QOM_DETAILS
    {
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string STRUCTURE_TYPE { get; set; }
        public string PROTECTION_TYPE { get; set; }
        public string GENERAL_QOM { get; set; }
        public Nullable<decimal> AVG_WIDTH_ASPER_DPR { get; set; }
        public Nullable<decimal> AVG_HEIGHT_ASPER_DPR { get; set; }
        public Nullable<decimal> AVG_WIDTH_ASPER_REC { get; set; }
        public Nullable<decimal> AVG_HEIGHT_ASPER_REC { get; set; }
        public string IS_COMPR_SOM { get; set; }
    }
}