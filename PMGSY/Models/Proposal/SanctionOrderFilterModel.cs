using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class SanctionOrderFilterModel
    {
        public SanctionOrderFilterModel()
        {
            StateList = new List<SelectListItem>();
            YearList = new List<SelectListItem>();
            StreamList = new List<SelectListItem>();
            BatchList = new List<SelectListItem>();
            SchemeList = new List<SelectListItem>();
            SchemeList.Insert(0, new SelectListItem { Value = "0",Text = "Select PMGSY Scheme"});
            SchemeList.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY Scheme1" });
            SchemeList.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY Scheme2" });
            ///Changes for RCPLWE
            SchemeList.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
            SchemeList.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY Scheme3" });
            SchemeList.Insert(5, new SelectListItem { Value = "5", Text = "Vibrant Village" });
            TypeList = new List<SelectListItem>();
            TypeList.Insert(0, new SelectListItem { Value = "%" , Text = "All"});
            TypeList.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
            TypeList.Insert(2, new SelectListItem { Value = "L", Text = "LSB" });
            StatusList = new List<SelectListItem>();
            StatusList.Insert(0, new SelectListItem { Value = "%", Text = "All" });
            StatusList.Insert(1, new SelectListItem { Value = "Y", Text = "Approved" });
            StatusList.Insert(2, new SelectListItem { Value = "N", Text = "Pending" });
            
        }

        [Display(Name="State")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a valid State.")]
        public int State { get; set; }

        [Display(Name="Year")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a valid Year.")]
        public int Year { get; set; }

        [Display(Name = "Collaboration")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a valid Collaboration.")]
        public int Stream { get; set; }

        [Display(Name = "Agency")]
        [Required]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select a valid Agency.")]
        public int Agency { get; set; }

        [Display(Name = "Batch")]
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a valid Batch.")]
        public int Batch { get; set; }

        ///Changes for RCPLWE
        [Display(Name = "Scheme")]
        [Required]
        [Range(1, 5, ErrorMessage = "Please select a valid Scheme.")]
        public int PMGSYScheme { get; set; }

        [Display(Name = "Proposal Type")]
        public string ProposalType { get; set; }

        //pp
        [Display(Name = "Batch")]
        [Required]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select batch.")]
        public int BatchForDropProposal { get; set; }

        [Display(Name = "Collaboration")]
        [Required]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select collaboration.")]
        public int StreamForDropProposal { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public string BatchName { get; set; }

        public string CollaborationName { get; set; }

        public string StateName { get; set; }

        public string SanctionOrderNo { get; set; }

        public string SanctionOrderDate { get; set; }

        public string DropOrderNo { get; set; }

        public string DropOrderDate { get; set; }

        public List<SelectListItem> StateList { get; set; }

        public List<SelectListItem> YearList { get; set; }

        public List<SelectListItem> StreamList { get; set; }

        public List<SelectListItem> BatchList { get; set; }

        public List<SelectListItem> SchemeList { get; set; }

        public List<SelectListItem> TypeList { get; set; }

        public List<SelectListItem> StatusList { get; set; }

        public List<SelectListItem> AgencyList { get; set; }
    }
}