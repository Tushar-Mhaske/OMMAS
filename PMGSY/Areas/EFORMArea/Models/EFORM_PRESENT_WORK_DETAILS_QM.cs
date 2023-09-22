using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_PRESENT_WORK_DETAILS_QM
    {
        public int WORK_ID { get; set; }
        public int DETAIL_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int QM_USER_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public short ITEM_ID { get; set; }
        public Nullable<decimal> ROAD_FROM { get; set; }
        public Nullable<decimal> ROAD_TO { get; set; }
        public string IPADD { get; set; }
    }
}