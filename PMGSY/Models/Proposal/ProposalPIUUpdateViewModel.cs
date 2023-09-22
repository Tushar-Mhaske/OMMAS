#region File Header
/*
        * Project Name  :   OMMAS II
        * Name          :   ProposalPIUUpdateViewModel.cs
        * Description   :   This View Model is Used for updating the PIU of proposals
        * Author        :   Vikram Nandanwar        
        * Creation Date :   02/Sept/2014
 **/
#endregion

using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class ProposalPIUUpdateViewModel
    {
        public ProposalPIUUpdateViewModel()
        {
            CommonFunctions objCommon = new CommonFunctions();
            lstPIU = new List<SelectListItem>();
            //lstPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
        }
        
        
        [Required]
        [Display(Name="PIU Name")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select PIU")]
        public int AdminNdCode { get; set; }

        public int OldPIUCode { get; set; }

        public string OldPIUName { get; set; }

        public string ProposalArray { get; set; }

        public List<SelectListItem> lstPIU { get; set; }

    }
}