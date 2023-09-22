using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class ProposalUpdateBlockViewModel
    {
        public ProposalUpdateBlockViewModel()
        {
            //CommonFunctions objCommon = new CommonFunctions();
            //lstPIU = new List<SelectListItem>();
            //lstPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
        }


        [Required]
        [Display(Name = "Block")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Block")]
        public int BlockCode { get; set; }

        public int imsPrRoadCode { get; set; }

        public int OldBlockCode { get; set; }

        public string OldBlockName { get; set; }

        public string ProposalArray { get; set; }

        public List<SelectListItem> lstBlock { get; set; }

        public string DistrictName { get; set; }

    }
}