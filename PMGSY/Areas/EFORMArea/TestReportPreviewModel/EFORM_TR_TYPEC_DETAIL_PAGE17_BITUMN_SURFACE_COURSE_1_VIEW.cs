using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.TestReportPreviewModel
{
    public class EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_VIEW
    {   
        public int TYPEC_DETAIL_ID { get; set; }
        public int MAIN_ITEM_ID { get; set; }
        public int SUBITEM_ID { get; set; }
        public int TABLE_ID { get; set; }
        public int ROW_ID { get; set; }

        public int DETAIL_ITEM_ID { get; set; }
        public string SIEVE_DESIGNATION { get; set; }
        public Nullable<decimal> SAMPLE_WEIGHT { get; set; }
        public Nullable<decimal> RETAINED_WEIGHT { get; set; }
        public Nullable<decimal> CUMULATIVE_WEIGHT { get; set; }
        public Nullable<decimal> PASSING_WEIGHT { get; set; }
        public string PERMISSIBLE_RANGE { get; set; }

    }
}