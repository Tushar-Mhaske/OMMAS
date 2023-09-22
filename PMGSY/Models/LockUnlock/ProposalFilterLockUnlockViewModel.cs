using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;


namespace PMGSY.Models.LockUnlock
{
    public class ProposalFilterLockUnlockViewModel
    {
        public ProposalFilterLockUnlockViewModel()
        {  
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
            lstScheme = new List<SelectListItem>();
            lstScheme.Insert(0, new SelectListItem { Value = "0",Text = "Select Scheme"});
            lstScheme.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY Scheme 1" });
            lstScheme.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY Scheme 2" });
            ///Changes for RCPLWE,PMGSY3
            lstScheme.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
            lstScheme.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY Scheme 3" });
        }

        [Display(Name = "Year")]
        [Range(1, 2147483647, ErrorMessage = "Please select Year")]
        public int IMS_YEAR { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "State Name")]
        [Range(1, 2147483647, ErrorMessage = "Please select State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> STATES { get; set; }

        [Display(Name = "Batch")]
        [Range(1, 2147483647, ErrorMessage = "Please select Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        public string IMS_PR_ROAD_CODES { get; set; }

        public string FreezeStatus { get; set; }

        public string StateCode{ get; set; }
        public string YearCode{ get; set; }
        public string BatchCode{ get; set; }

        //Change done by Vikram on 21 March 2014 
        ///Changes for RCPLWE,PMGSY3
        [Display(Name = "PMGSY Scheme")]
        //[Range(1, 4, ErrorMessage = "Please Select valid PMGSY Scheme.")]
        public int PMGSYScheme { get; set; }

        public List<SelectListItem> lstScheme { get; set; }
    }
}