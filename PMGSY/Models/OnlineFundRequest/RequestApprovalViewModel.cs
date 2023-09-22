using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.OnlineFundRequest
{
    public class RequestApprovalViewModel
    {
        public RequestApprovalViewModel()
        {
            lstRequestTo = new List<SelectListItem>();
            lstConditions = new List<SelectListItem>();
        }

        public int REQUEST_APPROVAL_ID { get; set; }
        
        public string REQUEST_ID { get; set; }

        [Display(Name="File No.")]
        public string FILE_NO { get; set; }
        
        public Nullable<int> REQUEST_FORWADED_FROM { get; set; }
        
        [Display(Name="Forward Request To")]
        [Required(ErrorMessage="Please select Forward Request To")]
        public Nullable<int> REQUEST_FORWADED_TO { get; set; }
        public List<SelectListItem> lstRequestTo { get; set; }

        public string RejectLetterName { get; set; }

        [Display(Name="Approval Status")]
        public string APPROVAL_STATUS { get; set; }

        [Display(Name = "Recommendation")]
        public string RECOMMENDATION { get; set; }
        
        [Required(ErrorMessage="Please select Condition Imposed")]
        [RegularExpression(@"^[YN]",ErrorMessage="Please select Condition Imposed")]
        [Display(Name="Condition Imposed")]
        public string CONDITION_IMPOSED { get; set; }

        [Display(Name="Condition")]
        [Range(0,Int32.MaxValue,ErrorMessage="Please select Condition")]
        public int ConditionCode { get; set; }
        public List<SelectListItem> lstConditions { get; set; }
        
        [Required(ErrorMessage="Please enter Remarks")]
        [Display(Name="Remarks")]
        public string REMARKS { get; set; }
        
    }
}