using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_QUALITY_ATTENTION_QM
    {

        public int INFO_ID { get; set; }
        public int DETAIL_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int QM_USER_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string IS_ALL_TEST_CONDUCTED { get; set; }
        public string IS_QC_REG_P1_MAINTAINED { get; set; }
        public string IS_QC_REG_P2_MAINTAINED { get; set; }
        public string LESS_TESTING_FLAG { get; set; }
        public string LESS_TESTING_REASON { get; set; }
        public string IS_NON_CONFORMITIES_QCR2 { get; set; }
        public string ITEM_GRADING_3 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }
        public string IPADD { get; set; }
    }
}