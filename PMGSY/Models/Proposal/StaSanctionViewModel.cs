#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   StaSanctionViewModel.cs
        * Description   :   This View Model is Used in Scrutinizing Proposals By STA - StaSactionProposal.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   30/May/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class StaSanctionViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string STA_SANCTIONED { get; set; }
        public string IMS_ISCOMPLETED { get; set; }

        [Display(Name="STA Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z .]+$", ErrorMessage = "Invalid STA Name,Can only contains Alphabet values and [.]")]
        public string STA_SANCTIONED_BY { get; set; }

        [Display(Name = "Scrutiny Date")]
        [Required(ErrorMessage = "Please Enter Scrutiny Date")]
        public string STA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Please Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MS_STA_REMARKS { get; set; }

        [Display(Name = "Un-Scrutiny Date")]
        [Required(ErrorMessage = "Please Enter Un-Scrutiny Date")]
        public string STA_UNSCRUTINY_DATE { get; set; }

        [Display(Name = "Un-Scrutiny Remarks")]
        [Required(ErrorMessage = "Please Enter Un-Scrutiny Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MS_STA_UnScrutinised_REMARKS { get; set; }

    }
}