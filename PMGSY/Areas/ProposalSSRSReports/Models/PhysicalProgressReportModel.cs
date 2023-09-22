using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProposalSSRSReports.Models
{
    public class PhysicalProgressReportModel
    {
        public PhysicalProgressReportModel()
        {
            StateList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            BlockList = new List<SelectListItem>();
            FundingAgencyList = new List<SelectListItem>();
            PhaseYearList = new List<SelectListItem>();
            BatchList = new List<SelectListItem>();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            //lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "Both" });
            lstTypes.Insert(0, new SelectListItem { Value = "P", Text = "Road" });
            lstTypes.Insert(1, new SelectListItem { Value = "L", Text = "Long Span Bridge" });
            TypeList = lstTypes;
        }


        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public string StateName { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "District")]
        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please select Block.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public string BlockName { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Collaboration")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select Collaboration.")]
        [Required(ErrorMessage = "Please select Collaboration.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Collaboration must be valid number.")]
        public int FundingAgency { get; set; }
        public string CollaborationName { get; set; }
        public List<SelectListItem> FundingAgencyList { get; set; }

        [Display(Name = "Year")]
        [Range(0, 2090, ErrorMessage = "Please select Phase.")]
        [Required(ErrorMessage = "Please select Phase.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public string PhaseName { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public string BatchName { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Proposal Type")]
        [Required(ErrorMessage = "Please select Type.")]
        public string ProposalType { get; set; }
        public string ProposalTypeName { get; set; }
        public List<SelectListItem> TypeList { get; set; }
    }
}