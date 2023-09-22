using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class MaintenancePolicyViewModel
    {
        public string EncFileId { get; set; }

        public int MAST_FILE_ID { get; set; }
        
        [Required(ErrorMessage="Please select State")]
        [Display(Name="State")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select State")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> lstStates { get; set; }

        [Required(ErrorMessage = "Please select Agency")]
        [Display(Name = "Agency")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Agency")]
        public int MAST_AGENCY_CODE { get; set; }
        public List<SelectListItem> lstAgencies { get; set; }

        public string IMS_FILE_NAME { get; set; }

        public string IMS_FILE_PATH { get; set; }

        [Required(ErrorMessage="Please select File Type")]
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = "Please select File Type")]
        [Display(Name="File Type")]
        public string IMS_FILE_TYPE { get; set; }
        public List<SelectListItem> lstFileTypes { get; set; }

        [Required(ErrorMessage="Please select Policy date")]
        [Display(Name="Date of Policy")]
        public string IMS_POLICY_DATE { get; set; }

        public string Operation { get; set; }
    }
}