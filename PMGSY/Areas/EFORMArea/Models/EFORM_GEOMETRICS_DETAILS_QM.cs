using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_GEOMETRICS_DETAILS_QM
    {
        public int INFO_ID { get; set; }
        public int DETAIL_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int QM_USER_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string ITEM_GRADING_4 { get; set; }
        public string REASON_FOR_DEVIATION { get; set; }
        public string IPADD { get; set; }
    }
}