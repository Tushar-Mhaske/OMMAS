using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class ProposalPIUUpdateViewModel_New
    {
        public ProposalPIUUpdateViewModel_New()
        {
            CommonFunctions objCommon = new CommonFunctions();
            lstPIU = new List<SelectListItem>();
            //lstPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
        }


        [Required]
        [Display(Name = "PIU Name")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select PIU")]
        public int AdminNdCode { get; set; }

        public int OldPIUCode { get; set; }

        public int DistrictCode { get; set; }     // change

        public string OldPIUName { get; set; }

        public string PackageId { get; set; }

        public List<SelectListItem> lstPIU { get; set; }
    }
}