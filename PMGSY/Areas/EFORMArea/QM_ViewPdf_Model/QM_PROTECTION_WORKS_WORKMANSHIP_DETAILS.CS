using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_PROTECTION_WORKS_WORKMANSHIP_DETAILS
    {
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string WORKMANSHIP_RS { get; set; }
        public string IS_SURFACE_HONEYCOMBING_FREE { get; set; }
        public string IS_WEEP_HOLES_PROVIDED { get; set; }
        public Nullable<decimal> WEEP_HOLES_SPACING_ASPER_DRAWING { get; set; }
        public Nullable<decimal> WEEP_HOLES_SPACING_ACTUAL { get; set; }

    }
}