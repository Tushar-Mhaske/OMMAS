#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   PtaLSBSanctionViewModel.cs
        * Description   :   This View Model is Used in PTA LSB Details Views in PtaLSBSanctionViewModel.cshtml
        * Author        :   Shyam Yadav
        * Creation Date :   21-11-2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class PtaLSBSanctionViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string PTA_SANCTIONED { get; set; }
        public string IMS_SANCTIONED { get; set; }
        public int? PTA_SANCTIONED_BY { get; set; }

        [Display(Name="PTA Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid PTA,Can only contains Alphabets values")]
        public string NAME_OF_PTA { get; set; }

        [Display(Name = "Scrutiny Date")]
        public string PTA_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Please Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MS_PTA_REMARKS { get; set; }

        [Display(Name = "Un-Scrutiny Date")]
        [Required(ErrorMessage = "Please Enter Un-Scrutiny Date")]
        public string PTA_UNSCRUTINY_DATE { get; set; }

        [Display(Name = "Un-Scrutiny Remarks")]
        [Required(ErrorMessage = "Please Enter Un-Scrutiny Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string MS_PTA_UnScrutinised_REMARKS { get; set; }


        public string IMS_ISCOMPLETED { get; set; }
    }
}