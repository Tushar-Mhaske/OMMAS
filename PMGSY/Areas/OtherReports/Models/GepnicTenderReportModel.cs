using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.OtherReports.Models
{
    public class GepnicTenderReportModel
    {
        public int LevelCode { get; set; }
        public string StateName { get; set; }
        public string State_Name { get; set; }

        public string DistName { get; set; }
        public string BlockName { get; set; }
        public string CollabName { get; set; }
        public string AgencyName { get; set; }
        public string StatusName { get; set; }
        public string YearName { get; set; }
        public string BatchName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Scheme")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Scheme.")]
        public int SchemeCode { get; set; }
        public List<SelectListItem> SchemeList { get; set; }

        [Display(Name = "District")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        //[Display(Name = "Block")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Block.")]
        //public int BlockCode { get; set; }
        //public List<SelectListItem> BlockList { get; set; }

        //[Display(Name = "Collaboration")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Collaboration.")]
        //public int CollaborationCode { get; set; }
        //public List<SelectListItem> CollaborationList { get; set; }

        //[Display(Name = "Agency")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Agency.")]
        //public int AgencyCode { get; set; }
        //public List<SelectListItem> AgencyList { get; set; }

        //public string AgencyName { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }

        //[Display(Name = "Proposal")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Proposal.")]
        //public int Proposal { get; set; }
        //public List<SelectListItem> ProposalList { get; set; }

        //[Display(Name = "Type")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Type.")]
        //public int Type { get; set; }
        //public List<SelectListItem> TypeList { get; set; }

        //[Display(Name = "Status")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Status.")]
        //public int Status { get; set; }
        //public List<SelectListItem> StatusList { get; set; }

        //[Display(Name = "Batch")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Batch.")]
        //public int BatchCode { get; set; }
        //public List<SelectListItem> BatchList { get; set; }
        
    }
}