using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_SHOULDERS_UCS_DETAILS
    {
        public Nullable<decimal> LOCATION_RD_FROM { get; set; }
        public Nullable<decimal> LOCATION_RD_TO { get; set; }
        public Nullable<decimal> UCS_ASPER_MIX_DESIGN { get; set; }
        public Nullable<decimal> UCS_ACHIEVED { get; set; }
        public string IS_UCS_ACCEPTABLE { get; set; }
    }
}