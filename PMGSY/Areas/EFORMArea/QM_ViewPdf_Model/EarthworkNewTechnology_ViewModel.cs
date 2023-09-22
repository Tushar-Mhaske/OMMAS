using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class EarthworkNewTechnology_ViewModel
    {
        public int TECH_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public string IS_NEW_TECH_USED { get; set; }
        public string TECH_NAME { get; set; }
        public string PROVIDER_NAME { get; set; }
        public string STABILIZER_NAME { get; set; }
        public string STABILIZER_QTY_DPR { get; set; }
        public string STABILIZER_QTY_USED { get; set; }
        public Nullable<decimal> UCS { get; set; }
        public string JC_TECH_NAME { get; set; }
        public string JC_TECH_PROVIDER { get; set; }
        public string JC_TECH_QTY_DPR { get; set; }
        public string JC_TECH_QTY_USED { get; set; }
        public string IS_TEST_CERT_PROVIDED { get; set; }
        public Nullable<decimal> SUBGRADE_CBR { get; set; }
        public string SUBITEM_GRADING_5I { get; set; }
        public string GUIDING_NOTE { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    }
}