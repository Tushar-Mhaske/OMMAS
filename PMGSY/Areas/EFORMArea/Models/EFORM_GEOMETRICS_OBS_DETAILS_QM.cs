using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_GEOMETRICS_OBS_DETAILS_QM
    {
        public int OBS_ID { get; set; }
        public int INFO_ID { get; set; }
        public string ROAD_LOC { get; set; }
        public Nullable<decimal> C4IA_ROAD_WIDTH_DPR { get; set; }
        public Nullable<decimal> C4IA_ROAD_WIDTH_ACTUAL { get; set; }
        public string C4IA_ROAD_WIDTH_GRADE { get; set; }
        public Nullable<decimal> C4IB_CARRIAGE_WIDTH_DPR { get; set; }
        public Nullable<decimal> C4IB_CARRIAGE_WIDTH_ACTUAL { get; set; }
        public string C4IB_CARRIAGE_WIDTH_GRADE { get; set; }
        public Nullable<decimal> C4IC_CAMBER_PER_DPR { get; set; }
        public Nullable<decimal> C4IC_CAMBER_PER_ACTUAL { get; set; }
        public string C4IC_CAMBER_PER_GRADE { get; set; }
        public Nullable<decimal> C4IIA_ELEVATION_PER_DPR { get; set; }
        public Nullable<decimal> C4IIA_ELEVATION_PER_ACTUAL { get; set; }
        public string C4IIA_ELEVATION_PER_GRADE { get; set; }
        public Nullable<decimal> C4IIB_EXTRA_WIDENING_DPR { get; set; }
        public Nullable<decimal> C4IIB_EXTRA_WIDENING_ACTUAL { get; set; }
        public string C4IIB_EXTRA_WIDENING_GRADE { get; set; }
        public Nullable<decimal> C4IIIA_LONG_GRAD_PER_DPR { get; set; }
        public Nullable<decimal> C4IIIA_LONG_GRAD_PER_ACTUAL { get; set; }
        public string C4IIIA_LONG_GRAD_PER_GRADE { get; set; }
        public string TABLE_FLAG { get; set; }
        public Nullable<decimal> ROAD_FROM { get; set; }
        public Nullable<decimal> ROAD_TO { get; set; }
        public string IPADD { get; set; }
        public int PR_ROAD_CODE { get; set; }

    }
}