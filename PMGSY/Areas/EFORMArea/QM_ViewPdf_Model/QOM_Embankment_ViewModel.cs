using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class QOM_Embankment_ViewModel
    {

        public int QOM_ID { get; set; }
    
        public int EFORM_ID { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        public string NAME_LOCATION_SRC { get; set; }
        public decimal DIST_SOE { get; set; }
        public string IS_JUSTIFIED { get; set; }

        public string APPROVED_SRC_REMARKS { get; set; }

        public string SUBITEM_GRADING_5_II { get; set; }

        public string IMPROVEMENT_REMARK_12_1 { get; set; }

        public int QM_USER_ID { get; set; }


        public string IPADD { get; set; }


    }
}