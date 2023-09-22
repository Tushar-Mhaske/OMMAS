using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_PREFILLED_PIU
    {
        public int PREFILLED_ID { get; set; }
        public int EFORM_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int ADMIN_SCHEDULE_CODE { get; set; }
        public string STATE { get; set; }
        public string DISTRICT { get; set; }
        public string BLOCK { get; set; }
        public string ROAD_NAME { get; set; }
        public string QM_NAME { get; set; }
        public string PACKAGE_ID { get; set; }
        public string QM_TYPE { get; set; }
        public string PHYSICAL_WORK_STATUS { get; set; }
        public string CURRENT_STAGE { get; set; }                    
        public Nullable<decimal> SANCTION_LENGTH { get; set; }
        public Nullable<decimal> EXECUTED_LENGTH { get; set; }
        public Nullable<decimal> ESTIMATED_COST { get; set; }
        public Nullable<decimal> AWARDED_COST { get; set; }
        public Nullable<decimal> EXPENDITURE_DONE { get; set; }
        public Nullable<decimal> COMPLETION_COST { get; set; }
        public string TERRAIN_TYPE { get; set; }
        public string DATE_OF_AWARD_WORK { get; set; }
        public string DATE_OF_START_WORK { get; set; }
        public string STIPULATED_COMPLETION_DATE { get; set; }
        public Nullable<int> USER_ID { get; set; }
        public string IPADD { get; set; }
               
    }
}