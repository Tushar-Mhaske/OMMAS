using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel
{
    public class EFORM_BRIDGE_PIU_PRGS_DETAILS_VIEW
    {
        public Nullable<int> ITEM_ID { get; set; }
        public string IEM_UNIT { get; set; }
        public Nullable<decimal> DPR_QUANTITY { get; set; }
        public Nullable<decimal> EXECUTED_QUANTITY { get; set; }
        public Nullable<decimal> COMPLETED_PERC { get; set; }
        public string DUE_START_DATE { get; set; }
        public string DUE_END_DATE { get; set; }
        public string ACTUAL_START_DATE { get; set; }
        public string ACTUAL_END_DATE { get; set; }
        public string DELAY_MONTH { get; set; }
    }
}