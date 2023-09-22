using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_QC_TEST_DETAILS_QM
    {
        public int WORK_ID { get; set; }
        public int INFO_ID { get; set; }
        public short ITEM_ID { get; set; }
        public Nullable<decimal> DPR_QUANTITY { get; set; }
        public Nullable<decimal> EXECUTED_QUANTITY { get; set; }
        public string TEST_NAME { get; set; }
        public Nullable<int> REQD_TEST_NUMBER { get; set; }
        public Nullable<int> CONDUCTED_TEST_NUMBER { get; set; }
        public string IS_TESTING_ADEQUATE { get; set; }
        public string IPADD { get; set; }
        public int PR_ROAD_CODE { get; set; }
    }
}