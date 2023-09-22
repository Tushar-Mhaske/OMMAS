#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBOthDetailsModel.cs
        * Description   :   This View Model is Used in MoRD LSB Details Views in MordLSBSanctionViewModel.cshtml        
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh
        * Creation Date :   20-05-2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;
using PMGSY.Extensions;
namespace PMGSY.Models.Proposal
{
    public class MordLSBSanctionViewModel
    {
        public MordLSBSanctionViewModel()
        {
            REASONS = new List<SelectListItem>();
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public byte PMGSYScheme { get; set; }

        public string IMS_PR_ROAD_CODES { get; set; }        

        public int IMS_PR_ROAD_CODE { get; set; }

        public string IMS_SANCTIONED_BY_TEXT { get; set; } //change by Deepak 20 Sept 2014

        [Required]
        public string IMS_SANCTIONED { get; set; }
        public string IMS_ISCOMPLETED { get; set; }

        [Display(Name = "MoRD Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid MORD, Can only contains Alphabets values")]
        public string IMS_SANCTIONED_BY { get; set; }

        [Display(Name = "Date of Clearance")]
        [Required]
        public string IMS_SANCTIONED_DATE { get; set; }

        [Display(Name = "MoRD Cost")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid MoRD Cost, Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0.0001, 99999, ErrorMessage = "Invalid MoRD Cost.")]
        [Required]
        public decimal IMS_SANCTIONED_BW_AMT { get; set; }

        [Display(Name = "State Share")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid State Share, Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0.0001, 99999, ErrorMessage = "Invalid State Share.")]
        [Required]
        public decimal IMS_SANCTIONED_BS_AMT { get; set; }

        [Display(Name = "Total Bridge Cost (Rs Lakhs)")]
        public decimal TotalEstimatedCost { get; set; }

        [Display(Name="Remarks")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values")]
        public string IMS_PROG_REMARKS { get; set; }

        //[Display(Name="Sanction Date")]
        //public string IMS_SANCTIONED_DATE { get; set; }


        [Display(Name = "Reason")]
        public Nullable<int> IMS_REASON { get; set; }
        public List<SelectListItem> REASONS { get; set; }
        public string Reason { get; set; }


        #region PMGSY Scheme2
        [Display(Name = "Whether any higher specification beyond PMGSY2 Guidelines have been proposed(including utility shifting)?")]
        public string IMS_IS_HIGHER_SPECIFICATION { get; set; }

        [Display(Name = "Higher Specification Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Higher Specification Cost too small or too large.")]
        public Nullable<decimal> IMS_SANCTIONED_HS_AMT { get; set; }

        [Display(Name = "Furniture Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Furniture Cost too small or too large.")]
        public Nullable<decimal> IMS_SANCTIONED_FC_AMT { get; set; }

        [Display(Name = "Renewal Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Renewal Cost too small or too large.")]
        public Nullable<decimal> IMS_SANCTIONED_RENEWAL_AMT { get; set; }
        #endregion

        public bool IS_SANCTIONABLE { get; set; }
        public bool IS_UNSANCTIONABLE { get; set; }
        public bool IS_RECONSIDERABLE { get; set; }
        public bool IS_DROPPABLE { get; set; }
        public string IS_AGREEMENT_FINALIZED { get; set; }
        public string IS_EXECUTION_STARTED { get; set; }
    }
}