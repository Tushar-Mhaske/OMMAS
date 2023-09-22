using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EformGenInfoViewModel
    {
        public int INFO_ID { get; set; }
        public int DETAIL_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int PIU_USER_ID { get; set; }
        public int PR_ROAD_CODE { get; set; }
        public string WORK_STATUS { get; set; }
        public string CURRENT_STAGE { get; set; }
        public System.DateTime INSPECTION_DATE { get; set; }
        public string QM_TYPE { get; set; }
        public string QM_NAME { get; set; }
        public string QM_CODE { get; set; }
        public Nullable<decimal> SAN_FLEX_PAVEMENT { get; set; }
        public Nullable<decimal> SAN_RIGID_PAVEMENT { get; set; }
        public Nullable<decimal> EXEC_LENGTH { get; set; }
        public Nullable<decimal> EXEC_FLEX_PAVEMENT { get; set; }
        public Nullable<decimal> EXEC_RIGID_PAVEMENT { get; set; }
        public string DEVIATION_REASON { get; set; }
        public string NEW_TECHNOLOGY_NAME { get; set; }
        public decimal ROAD_FROM { get; set; }
        public decimal ROAD_TO { get; set; }
        public decimal ESTIMATED_COST { get; set; }
        public decimal TECHNICAL_SANC_COST { get; set; }
        public decimal AWARDED_COST { get; set; }
        public decimal EXPENDITURE_DONE { get; set; }
        public decimal BILLS_PENDING { get; set; }
        public Nullable<decimal> COMPLETION_COST { get; set; }
        public string WORK_TYPE { get; set; }
        public Nullable<decimal> TOTAL_LEN { get; set; }
        public string CARRIAGEWAY_WIDTH_NEW { get; set; }
        public string CARRIAGEWAY_WIDTH_TYPE { get; set; }
        public Nullable<decimal> CARRIAGEWAY_WIDTH_EXISTING { get; set; }
        public Nullable<decimal> CARRIAGEWAY_WIDTH_PROPOSED { get; set; }
        public Nullable<decimal> CARRIAGEWAY_LENGTH { get; set; }
        public string TERRAIN_TYPE { get; set; }
        public Nullable<System.DateTime> AWARD_OF_WORK_DATE { get; set; }
        public Nullable<System.DateTime> START_OF_WORK_DATE { get; set; }
        public Nullable<System.DateTime> STIPULATED_COMPLETION_DATE { get; set; }
        public Nullable<System.DateTime> ACTUAL_COMPLETION_DATE { get; set; }
        public string PIU_HEAD_NAME { get; set; }
        public string PIU_HEAD_MOBILE_NO { get; set; }
        public string PIU_HEAD_EMAIL { get; set; }
        public string PIU_ADDR { get; set; }
        public System.DateTime PIU_SIGN_DATE { get; set; }

        public string LAB_LOCATION { get; set; }
        public Nullable<System.DateTime> PHOTO_UPLOAD_DATE { get; set; }
        public string ESTB_DELAY_REASON { get; set; }
        public string LAB_EQUIP_AVBL { get; set; }
        public string EQUIP_WORKING { get; set; }
        public string EQUIP_NOT_WORKING { get; set; }
        public string LAB_EQUIP_NOT_AVBL { get; set; }
        public string REASON_LAB_EQUIP_NOT_AVBL { get; set; }
        public string DOCUMENT_FOR_QM { get; set; }
    }
}