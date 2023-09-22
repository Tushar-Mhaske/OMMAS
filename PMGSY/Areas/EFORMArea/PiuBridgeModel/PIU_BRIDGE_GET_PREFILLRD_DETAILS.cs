using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class PIU_BRIDGE_GET_PREFILLRD_DETAILS
    {

        [Range(1, 100)]
        public int RoadCode { get; set; }
        public int Eform_Id { get; set; }
        public string WORK_STATUS { get; set; }

        public string INSPECTION_DATE { get; set; }
        public string QM_TYPE { get; set; }
        public string QM_NAME { get; set; }

        public string State { get; set; }
        public string District { get; set; }
        public string Block { get; set; }
        public string RoadName { get; set; }
        public string PkgNumber { get; set; }
        public decimal? SancLength { get; set; }





        public decimal? ESTIMATED_COST { get; set; }
        public decimal TECHNICAL_SANC_COST { get; set; }
        public decimal? AWARDED_COST { get; set; }
        public decimal? EXPENDITURE_DONE { get; set; }
        public decimal BILLS_PENDING { get; set; }
        public decimal? COMPLETION_COST { get; set; }

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