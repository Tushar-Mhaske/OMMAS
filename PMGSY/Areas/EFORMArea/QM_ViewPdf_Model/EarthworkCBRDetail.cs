using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class EarthworkCBRDetail
    {
        public int CBR_DETAIL_ID { get; set; }
        public int TECH_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public Nullable<decimal> LOCATION_RD_FROM { get; set; }
        public Nullable<decimal> LOCATION_RD_TO { get; set; }
        public Nullable<decimal> CBR_ASPER_MIX_DESIGN { get; set; }
        public Nullable<decimal> CBR_ASPER_PIU { get; set; }
        public string IS_CBR_ACCEPTABLE { get; set; }
        public int QM_USER_ID { get; set; }
        public string IPADD { get; set; }
    }
}