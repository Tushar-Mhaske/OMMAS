using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PIU_GET_PREFILLED_DETAILS
    {
        [Range(1, 100)]
        public int RoadCode { get; set; }
        public int Eform_Id { get; set; }
        public string WORK_STATUS { get; set; }
        public string CURRENT_STAGE { get; set; }
        public string INSPECTION_DATE { get; set; }
        public string QM_TYPE { get; set; }
        public string QM_NAME { get; set; }
        public string QM_CODE { get; set; }
        public string STAGE_PHASE { get; set; }




        public string State { get; set; }
        public string District { get; set; }
        public string Block { get; set; }
        public string RoadName { get; set; }
        public string PkgNumber { get; set; }
        public decimal SancLength { get; set; }
        public decimal SAN_FLEX_PAVEMENT { get; set; }
        public decimal SAN_RIGID_PAVEMENT { get; set; }
        public decimal EXEC_LENGTH { get; set; }
        public decimal EXEC_FLEX_PAVEMENT { get; set; }
        public decimal EXEC_RIGID_PAVEMENT { get; set; }
        [RegularExpression(" /^[0-9a-zA-Z\\s,:;?.'\"() -/]*$/", ErrorMessage = "Please Enter Valid deviation reason Data")]
        [StringLength(250, ErrorMessage = "Please enter valida data length")]
        public string DEVIATION_REASON { get; set; }
        [RegularExpression("/^[0-9a-zA-Z\\s,:;?.'\"() -/]*$/", ErrorMessage = "Please Enter Valid technology Data")]
        [StringLength(250, ErrorMessage = "Please enter valida data length")]
        public string NEW_TECHNOLOGY_NAME { get; set; }
        public decimal ROAD_FROM { get; set; }
        public decimal ROAD_TO { get; set; }
        public decimal ESTIMATED_COST { get; set; }
        public decimal TECHNICAL_SANC_COST { get; set; }
        public decimal AWARDED_COST { get; set; }
        public decimal EXPENDITURE_DONE { get; set; }
        public decimal BILLS_PENDING { get; set; }
        public decimal COMPLETION_COST { get; set; }
        public string WORK_TYPE { get; set; }
        public decimal TOTAL_LEN { get; set; }
        public string CARRIAGEWAY_WIDTH_NEW { get; set; }
        public string CARRIAGEWAY_WIDTH_TYPE_UPGRADE { get; set; }
        public string CARRIAGEWAY_WIDTH_EXISTING { get; set; }
        public string CARRIAGEWAY_WIDTH_PROPOSED { get; set; }
        public string CARRIAGEWAY_LENGTH { get; set; }
        public string TERRAIN_TYPE { get; set; }
        public string AWARD_OF_WORK_DATE { get; set; }
        public string START_OF_WORK_DATE { get; set; }
        public string STIPULATED_COMPLETION_DATE { get; set; }
        public string ACTUAL_COMPLETION_DATE { get; set; }
        public string PIU_HEAD_NAME { get; set; }
        public string PIU_HEAD_MOBILE_NO { get; set; }
        public string PIU_HEAD_EMAIL { get; set; }
        public string PIU_ADDR { get; set; }
        public string PIU_SIGN_DATE { get; set; }
    }
}