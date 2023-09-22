using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QM_PIPE_CULVERTS_DETAILS
    {
        public Nullable<decimal> LOCATION_RD { get; set; }
        public string CLASS_OF_PIPE { get; set; }
        public Nullable<decimal> MEAS_CUSHION { get; set; }
        public string STRENGTH_OF_CONCRETE { get; set; }
        public string QOM { get; set; }

    }
}