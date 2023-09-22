using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class MaintenanceInspectionViewModel
    {

        public MaintenanceInspectionViewModel()
        {
            lstDistricts = new List<SelectListItem>();
            lstGrades = new List<SelectListItem>();
            lstProposals = new List<SelectListItem>();
            lstProposalType = new List<SelectListItem>();
            lstStates = new List<SelectListItem>();
            lstYears = new List<SelectListItem>();
            lstMonitors = new List<SelectListItem>();
            lstPackages = new List<SelectListItem>();
        }

        [Required]
        [Display(Name="State")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select State.")]
        public int State { get; set; }

        [Required]
        [Display(Name = "District")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select District.")]
        public int District { get; set; }

        [Required]
        [Display(Name = "NQM")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Monitor.")]
        public int MonitorCode { get; set; }

        [Required]
        [Display(Name = "Sanction Year")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please select Sanction Year.")]
        public int SanctionYear { get; set; }

        //[Required]
        //[Display(Name = "Proposal Type")]
        //[RegularExpression(@"[PL]",ErrorMessage="Please select Proposal Type.")]
        //public string ProposalType { get; set; }

        [Required]
        [Display(Name="Road/LSB")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Proposal.")]
        public int ProposalCode { get; set; }

        [Required(ErrorMessage="Please Enter Inspection Date")]
        [Display(Name="Inspection Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection Date must be in dd/mm/yyyy format.")]
        public string InspectionDate { get; set; }

        [Required]
        [Display(Name="From Chainage")]
        //[IsValidStartChainage("IMS_PAV_LENGTH", "ToChainage", "ProposalType", ErrorMessage = "From Chainage should be greater than or equal to 0 and less than Road Length. Maximum chainage difference must be 3.000.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid From Chainage, can only contains Numeric values and 3 digits after decimal place")]
        public decimal? FromChainage { get; set; }

        [Required]
        [Display(Name="To Chainage")]
        //[IsValidEndChainage("IMS_PAV_LENGTH", "FromChainage", "ProposalType", ErrorMessage = "To Chainage should be greater than 0 and less than or equal to Road Length. Maximum chainage difference must be 3.000")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid To Chainage, can only contains Numeric values and 3 digits after decimal place")]
        public decimal? ToChainage { get; set; }

        public DateTime? MaxInspectionDate { get; set; }

        [Required]
        [Display(Name="Overall Grading")]
        [Range(1,3,ErrorMessage="Please select Grade.")]
        public int OverallGrade { get; set; }

        [Required]
        [Display(Name="Package")]
        public string PackageId { get; set; }

        public List<SelectListItem> lstProposalType { get; set; }

        public List<SelectListItem> lstStates { get; set; }

        public List<SelectListItem> lstDistricts { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        public List<SelectListItem> lstProposals { get; set; }

        public List<SelectListItem> lstGrades { get; set; }

        public List<SelectListItem> lstMonitors { get; set; }

        public List<SelectListItem> lstPackages { get; set; }

        public decimal? IMS_PAV_LENGTH { get; set; }

    }
}