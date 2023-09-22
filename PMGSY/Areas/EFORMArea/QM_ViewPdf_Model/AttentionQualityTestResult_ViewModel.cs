using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class AttentionQualityTestResult_ViewModel
    {
        public int TEST_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int INFO_ID { get; set; }
        public string ROAD_LOC { get; set; }
        public string TEST_NAME { get; set; }
        public string TEST_CONDUCTED_RESULT { get; set; }
        public string TEST_RESULT_QCR1 { get; set; }
        public string TEST_RESULT_PREVIOUS { get; set; }
        public string TEST_RESULT_CONFRM { get; set; }       
        public int PR_ROAD_CODE { get; set; }
        public int QM_USER_ID { get; set; }
    }
}