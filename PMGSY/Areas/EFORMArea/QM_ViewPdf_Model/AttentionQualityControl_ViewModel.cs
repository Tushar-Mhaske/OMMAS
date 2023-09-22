using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class AttentionQualityControl_ViewModel
    {
        public int WORK_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int INFO_ID { get; set; }
        public int QM_USER_ID { get; set; }
        public short ITEM_ID { get; set; }
        public string DPR_QUANTITY { get; set; }
        public string EXECUTED_QUANTITY { get; set; }
        public string TEST_NAME { get; set; }
        public string REQD_TEST_NUMBER { get; set; }
        public string CONDUCTED_TEST_NUMBER { get; set; }
        public string IS_TESTING_ADEQUATE { get; set; }
        public string IPADD { get; set; }
        public int PR_ROAD_CODE { get; set; }
    }
}