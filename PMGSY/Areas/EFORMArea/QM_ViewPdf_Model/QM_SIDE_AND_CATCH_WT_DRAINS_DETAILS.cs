using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_SIDE_AND_CATCH_WT_DRAINS_DETAILS
    {
        public Nullable<decimal> LOCATION_RD_FROM { get; set; }
        public Nullable<decimal> LOCATION_RD_TO { get; set; }
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string IS_GEN_QUAL_ACCEPTABLE { get; set; }
        public string IS_SIDE_DRAINS_INTEGRATED { get; set; }

    }
}