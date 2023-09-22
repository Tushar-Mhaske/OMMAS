using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Agreement
{
    public class AgreementFilterViewModel
    {

        public AgreementFilterViewModel()
        {
            List<SelectListItem> lstAllPackages = new List<SelectListItem>();
            lstAllPackages.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });
            lstBlocks = new List<SelectListItem>();
            lstPackages = new List<SelectListItem>();
            lstPackages = lstAllPackages;
            lstDistricts = new List<SelectListItem>();
            lstStates = new List<SelectListItem>();
            lstStates = new CommonFunctions().PopulateStates(true);
            lstYears = new List<SelectListItem>();
            List<SelectListItem> lstTypes = new List<SelectListItem>();
            lstTypes.Add(new SelectListItem { Value = "0", Text = "All" });
            lstTypes.Add(new SelectListItem { Value = "P", Text = "Road" });
            lstTypes.Add(new SelectListItem { Value = "L", Text = "Bridge" });
            lstTypes.Add(new SelectListItem { Value = "B", Text = "Building" });
            lstProposalType = lstTypes;
            List<SelectListItem> lstStatuses = new List<SelectListItem>();
            lstStatuses.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            lstStatuses.Insert(1, new SelectListItem { Value = "C", Text = "Completed" });
            lstStatuses.Insert(2, new SelectListItem { Value = "P", Text = "Inprogress" });
            lstStatuses.Insert(3, new SelectListItem { Value = "W", Text = "Terminated" });
            lstStatus = new List<SelectListItem>();
            lstStatus = lstStatuses;
            List<SelectListItem> lstFinalize = new List<SelectListItem>();
            lstFinalize.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            lstFinalize.Insert(1, new SelectListItem { Value = "Y",Text ="Finalized"});
            lstFinalize.Insert(2, new SelectListItem { Value = "N", Text = "Unfinalized" });
            lstFinalizeUnfinalize = new List<SelectListItem>();
            lstFinalizeUnfinalize = lstFinalize;
            lstAgreementTypes = new List<SelectListItem>();
            lstAgreementTypes.Insert(0, new SelectListItem { Value = "0" , Text = "Select Agreement Type"});
            lstAgreementTypes.Insert(1, new SelectListItem { Value = "C", Text = "Contractor" });
            lstAgreementTypes.Insert(2, new SelectListItem { Value = "O", Text = "Other Road" });
            lstAgreementTypes.Insert(3, new SelectListItem { Value = "R", Text = "Special Agreement" });
        }

        public int State { get; set; }

        public int District { get; set; }

        public int Year { get; set; }

        public int Block { get; set; }

        public string Package { get; set; }

        public string ProposalType { get; set; }

        [Display(Name="Agreement Status")]
        public string AgreementStatus { get; set; }

        [Display(Name="Finalize/Unfinalized")]
        public string FinalizeUnfinalize { get; set; }

        [Display(Name="Agreement Type")]
        [RegularExpression(@"^([COR]+)$", ErrorMessage = "Please select agreement type.")]
        public string AgreementType { get; set; }

        public List<SelectListItem> lstAgreementTypes { get; set; }

        public List<SelectListItem> lstYears { get; set; }

        public List<SelectListItem> lstBlocks { get; set; }

        public List<SelectListItem> lstPackages { get; set; }

        public List<SelectListItem> lstProposalType { get; set; }

        public List<SelectListItem> lstStatus { get; set; }

        public List<SelectListItem> lstFinalizeUnfinalize { get; set; }

        public List<SelectListItem> lstStates { get; set; }

        public List<SelectListItem> lstDistricts { get; set; }

    }
}