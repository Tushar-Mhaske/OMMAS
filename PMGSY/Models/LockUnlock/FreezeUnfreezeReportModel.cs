using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.LockUnlock
{
    public class FreezeUnfreezeReportModel
    {
        public FreezeUnfreezeReportModel()
        {  
            Years = new List<SelectListItem>();
            BATCHS = new List<SelectListItem>();
            STATES = new List<SelectListItem>();
            lstScheme = new List<SelectListItem>();
            lstScheme.Insert(0, new SelectListItem { Value = "0",Text = "Select Scheme"});
            lstScheme.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY Scheme 1" });
            lstScheme.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY Scheme 2" });
            ///Changes for RCPLWE
            lstScheme.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
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
        [Range(0, 2147483647, ErrorMessage = "Please select Batch")]
        public int IMS_BATCH { get; set; }
        public List<SelectListItem> BATCHS { set; get; }

        public string IMS_PR_ROAD_CODES { get; set; }

        public string FreezeStatus { get; set; }

        public string StateCode{ get; set; }
        public string YearCode{ get; set; }
        public string BatchCode{ get; set; }

        //Change done by Vikram on 21 March 2014 
        [Display(Name="PMGSY Scheme")]
        ///Changes for RCPLWE
        [Range(1,3,ErrorMessage="Please Select PMGSY Scheme.")]
        public int PMGSYScheme { get; set; }

        public List<SelectListItem> lstScheme { get; set; }
    }
}