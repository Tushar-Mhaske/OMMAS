using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMJIFilterModel
    {
    
        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public string StateName { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid District")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public string DistrictName { get; set; }

        [Display(Name = "Block")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Block")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Proposal Type")]
        [RegularExpression(@"^[P|L|B|A]+$", ErrorMessage = "Invalid proposal type selected, select either road or bridge")]
        public string ProposalType { get; set; }
        public List<SelectListItem> ProposalTypeList { get; set; }


        [Display(Name = "Inspection Status")]
        [RegularExpression(@"^[A|Y|N]+$", ErrorMessage = "Invalid inspection status selected, select either yes or no or all")]
        public string InspectionStatus { get; set; }
        public List<SelectListItem> InspectionStatusList { get; set; }


    }

}