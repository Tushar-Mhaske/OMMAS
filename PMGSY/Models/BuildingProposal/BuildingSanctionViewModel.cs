
#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MordSanctionViewModel.cs
        * Description   :   This View Model is Used in Sanctioning Building Proposal View - MordSanctionBuildingProposal.cshtml
        * Author        :   Anand Singh
        * Creation Date :   August 14, 2015
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.BuildingProposal
{
    public class BuildingSanctionViewModel
    {


        public BuildingSanctionViewModel()
        {
           
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
            
        }

        public byte PMGSYScheme { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }
       
        public string IMS_SANCTIONED { get; set; }        
        
        [Display(Name="Sanctioned By ")]
        [RegularExpression(@"^[a-zA-Z .]+$", ErrorMessage = "Invalid Sanctioned by Name,Can only contains Alphabets values and [.]")]
        [Required]
        public string IMS_SANCTIONED_BY { get; set; }
        
        public string IMS_ISCOMPLETED { get; set; }

        [Display(Name = "Date of Clearance")]
        [Required]
        public string IMS_SANCTIONED_DATE { get; set; }

        [Display(Name = "Remarks")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string IMS_PROG_REMARKS { get; set; }

       
 
    }
}