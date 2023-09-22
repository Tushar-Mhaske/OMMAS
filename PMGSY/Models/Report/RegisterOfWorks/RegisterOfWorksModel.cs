using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Report.RegisterOfWorks
{
    public class RegisterOfWorksModel
    {
        // -------Filter Specific Info
        
        [Display(Name = "DPIU")]
        [Range(0,Int64.MaxValue,ErrorMessage="Please select DPIU")]
        public Int32 ADMIN_ND_CODE { get; set; }
        public List<SelectListItem> DEPARTMENT_LIST { get; set; }

        [Display(Name = "Contractor")]
        [Range(1,Int64.MaxValue,ErrorMessage="Please select contractor.")]
        public Int32 MAST_CON_ID { get; set; }
        public List<SelectListItem> CONTRACTOR_LIST { get; set; }

        [Display(Name = "Agreement")]
        [Range(1,Int64.MaxValue,ErrorMessage="Please select agreement.")]
        public Int32 TEND_AGREEMENT_CODE { get; set; }
        public List<SelectListItem> AGREEEMENT_LIST { get; set; }

        //Added By Ashish Markande on 8/10/2013
        public Int32 ParentAdminNdCode { get; set; } //To get nodal agency

        // -----------------------------------------

        // ------Header specific Info

        [Display(Name = "Report Header")]
        public String ReportAnnex { get; set; }

        [Display(Name = "Name Of PIU")]
        public String StateDepartment { get; set; }

        [Display(Name = "Name Of PIU")]
        public String DistrictDepartment { get; set; }

        [Display(Name = "Agreement Date")]
        public string AGREEMENT_DATE { get; set; }

        [Display(Name = "Agreement Amount (in Lacs)")]
        public decimal AGREEMENT_AMOUNT { get; set; }

        [Display(Name = "Funded by")]
        public string FUNDING_AGENCY { get; set; }

        [Display(Name = "Agreement No.")]
        public string AGREEMENT_NUMBER { get; set; }


        // ---------------------------------------

        List<SP_ACC_RPT_PF_WORK_REGISTER_Result> SP_ACC_RPT_PF_WORK_REGISTER_LIST { get; set; }



        //Report Header parameter
        public string FundTypeName { get; set; }
        public string ReportName { get; set; }
        public string ReportParagraphName { get; set; }
        public string ReportFormNumber { get; set; }
    
    }
}