using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class NewTechPIU_ViewModel
    {
        public int RowId { get; set; }
        public int TECH_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        
        public string MAST_TECH_NAME { get; set; }
        public Nullable<decimal> RD_FROM { get; set; }
        public Nullable<decimal> RD_TO { get; set; }
    }
}