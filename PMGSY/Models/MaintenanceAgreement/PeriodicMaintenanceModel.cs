using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MaintenanceAgreement
{
    public class PeriodicMaintenanceModel
    {

        public PeriodicMaintenanceModel()
        {
            BlockList = new List<SelectListItem>();
            YearList = new List<SelectListItem>();
            PackageList = new List<SelectListItem>();
            BatchList = new List<SelectListItem>();
            FundingAgencyList = new List<SelectListItem>();
            NewUpgradationList = new List<SelectListItem>();
        }

        [Display(Name="Block")]
        [Required(ErrorMessage="Please Select Block")]
        [Range(0,int.MaxValue,ErrorMessage="Please Select Block")]
        public int Block { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name="Financial Year")]
        [Required(ErrorMessage = "Please Select Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Year")]
        public int FinancialYear { get; set; }
        public List<SelectListItem> YearList { get; set; }

        [Display(Name="Package")]
        [Required(ErrorMessage = "Please Select Package")]
        public string Package { get; set; }
        public List<SelectListItem> PackageList { get; set; }

        [Display(Name="Batch")]
        [Required(ErrorMessage = "Please Select Batch")]
        [Range(0, int.MaxValue, ErrorMessage = "Please Select Batch")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name="Funding Agency")]
        [Required(ErrorMessage = "Please Select Funding Agency")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please Select Funding Agency")]
        public int FundingAgency { get; set; }
        public List<SelectListItem> FundingAgencyList { get; set; }

        [Display(Name="New/Upgradation")]
        [Required(ErrorMessage = "Please Select New/upgradation")]
        [RegularExpression(@"^([0NU]+)$", ErrorMessage = "Please Select New/upgradation")]
        public int NewUpgradation { get; set; }
        public List<SelectListItem> NewUpgradationList { get; set; }

    }
}