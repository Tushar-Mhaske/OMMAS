using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.OnlineFundRequest
{
    public class OnlineFundRequestViewModel
    {

        public OnlineFundRequestViewModel()
        {
            lstYears = new List<SelectListItem>();
            lstBatches = new List<SelectListItem>();
            lstSchemes = new List<SelectListItem>();
            lstAgencies = new List<SelectListItem>();
            lstCollaborations = new List<SelectListItem>();
            lstSchemes.Insert(0, new SelectListItem { Value = "0", Text = "Select Scheme" });
            lstSchemes.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY I" });
            lstSchemes.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY II" });
        }

        public string EncRequestId { get; set; }
        public int REQUEST_ID { get; set; }
        [Display(Name="State")]
        public int MAST_STATE_CODE { get; set; }
        public string StateName { get; set; }
        public string BatchName { get; set; }
        public string YearName { get; set; }
        public string CollaborationName { get; set; }
        public string AgencyName { get; set; }
        
        [Required(ErrorMessage="Please select Agency")]
        [Display(Name="Agency")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Agency")]
        public int ADMIN_ND_CODE { get; set; }
        public List<SelectListItem> lstAgencies { get; set; }
        
        [Display(Name="Year")]
        [Required(ErrorMessage="Please select Year")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Year")]
        public int YEAR { get; set; }
        public List<SelectListItem> lstYears { get; set; }

        [Display(Name="Batch")]
        [Required(ErrorMessage="Batch")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Batch")]
        public int BATCH { get; set; }
        public List<SelectListItem> lstBatches { get; set; }

        [Display(Name="Collaboration")]
        [Required(ErrorMessage="Collaboration")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Collaboration")]
        public int COLLABORATION { get; set; }
        public List<SelectListItem> lstCollaborations { get; set; }

        [Display(Name="PMGSY Scheme")]
        [Required(ErrorMessage="Please select Scheme")]
        [Range(1, 2, ErrorMessage = "Please select Scheme")]
        public byte PMGSY_SCHEME { get; set; }
        public List<SelectListItem> lstSchemes { get; set; }

        [Display(Name="Fund Type")]
        public string FUND_TYPE { get; set; }
        
        [Display(Name="Installment No.")]
        public int INSTALLMENT { get; set; }

        [Display(Name="Request Amount (in Cr.)")]
        [Required(ErrorMessage="Please enter Request Amount.")]
        [Range(1,99999999.99,ErrorMessage="Please enter valid Request Amount")]
        public decimal AMOUNT { get; set; }

        [RegularExpression(@"^[YN]",ErrorMessage="Finalize Flag is invalid")]
        public string FINALIZE { get; set; }
        
        [Display(Name="Finalize Date")]
        public string FINALIZE_DATE { get; set; }
        
        [Display(Name="File No.")]
        public string FILE_NO { get; set; }

        [Display(Name="File Date")]
        public string FILE_DATE { get; set; }
        
        [Display(Name="Concurred Amount (in Cr.)")]
        [Range(1, 99999999.99, ErrorMessage = "Please enter valid Concurred Amount")]
        public Nullable<decimal> RELEASE_AMOUNT { get; set; }

        [Display(Name = "Sanction Amount (in Cr.)")]
        [Range(1, 99999999.99, ErrorMessage = "Please enter valid Sanction Amount")]
        public Nullable<decimal> SANCTION_AMOUNT { get; set; }

        [Display(Name="UO No.")]
        public string RELEASE_UO_NO { get; set; }

        [Display(Name="Release Date")]
        public string RELEASE_DATE { get; set; }
        
        public string ELIGIBLE_FOR_NEXT_REQUEST { get; set; }
        
        public string PREVIOUS_CONDITION_IMPOSSED { get; set; }
        
        public string APPROVAL_CONDITION_IMPOSSED { get; set; }
        
        [Display(Name="Remarks")]
        public string REMARKS { get; set; }
        
        public Nullable<int> USERID { get; set; }

        [RegularExpression(@"^[AE]",ErrorMessage="Invalid Operation")]
        public string Operation { get; set; }
        
        public string IPADD { get; set; }
    }
}