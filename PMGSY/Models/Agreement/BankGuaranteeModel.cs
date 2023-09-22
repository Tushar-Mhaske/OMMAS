using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Agreement
{
    public class BankGuaranteeModel
    {

        public BankGuaranteeModel()
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

            this.lstActiveStatus = new List<SelectListItem>();
            this.lstActiveStatus.Add(new SelectListItem { Value = "0", Text = "All" });
            this.lstActiveStatus.Add(new SelectListItem { Value = "E", Text = "Expiring", Selected=true });
            this.lstActiveStatus.Add(new SelectListItem { Value = "A", Text = "Active" });
            this.lstActiveStatus.Add(new SelectListItem { Value = "T", Text = "Expired" });

        }

        [Range(0,int.MaxValue,ErrorMessage="Please select state")]
        public int State { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select District")]
        public int District { get; set; }

        [Range(-1, int.MaxValue, ErrorMessage = "Please select year")]
        public int Year { get; set; }

        [Range(-1, int.MaxValue, ErrorMessage = "Please select block")]
        public int Block { get; set; }

        public string Package { get; set; }

        [Display(Name="Status")]
        [RegularExpression("^([0EAT]+)$", ErrorMessage = "Please select status")]
        public string ActiveStatus { get; set; }


        [RegularExpression(@"^([0PLB]+)$", ErrorMessage = "Please select valid proposal type.")]
        public string ProposalType { get; set; }

        [Display(Name="Agreement Status")]
        [RegularExpression(@"^([0CPW]+)$", ErrorMessage = "Please select valid agreement status.")]
        public string AgreementStatus { get; set; }
 
        //[Display(Name = "Agreement Type")]
        //[RegularExpression(@"^([COR]+)$", ErrorMessage = "Please select agreement type.")]
        //public string AgreementType { get; set; }
 
        public List<SelectListItem> lstYears { get; set; }

        public List<SelectListItem> lstBlocks { get; set; }

        public List<SelectListItem> lstPackages { get; set; }

        public List<SelectListItem> lstProposalType { get; set; }

        public List<SelectListItem> lstStatus { get; set; }

        public List<SelectListItem> lstStates { get; set; }

        public List<SelectListItem> lstDistricts { get; set; }

        public List<SelectListItem> lstActiveStatus { get; set; }

    }
}