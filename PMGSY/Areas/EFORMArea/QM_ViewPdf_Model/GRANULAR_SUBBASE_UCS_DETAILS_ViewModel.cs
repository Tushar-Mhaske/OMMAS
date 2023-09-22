using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class GRANULAR_SUBBASE_UCS_DETAILS_ViewModel
    {
       
        public Nullable<decimal> LOCATION_RD_FROM_14 { get; set; }
        public Nullable<decimal> LOCATION_RD_TO_14 { get; set; }
        public Nullable<decimal> UCS_ASPER_MIX_DESIGN_14 { get; set; }
        public Nullable<decimal> UCS_ACHIEVED_14 { get; set; }
        public string IS_UCS_ACCEPTABLE_14 { get; set; }

    }
}