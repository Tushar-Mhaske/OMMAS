#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MordSanctionViewModel.cs
        * Description   :   This View Model is Used in Sanctioning Road Proposal View - MordSanctionRoadProposal.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   30/May/2013
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
    public class MordSanctionViewModel
    {

        public MordSanctionViewModel()
        {
            REASONS = new List<SelectListItem>();
            PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
        }

        public byte PMGSYScheme { get; set; }
        public string OperationType { get; set; }

        public string IMS_SANCTIONED_BY_TEXT { get; set; }
        public string IMS_PR_ROAD_CODES { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }
        [Required]
        public string IMS_SANCTIONED { get; set; }        
        
        [Display(Name="Sanctioned By ")]
        [RegularExpression(@"^[a-zA-Z .]+$", ErrorMessage = "Invalid Sanctioned by Name,Can only contains Alphabets values and [.]")]
        [Required]
        public string IMS_SANCTIONED_BY { get; set; }
        
        public string IMS_ISCOMPLETED { get; set; }

        [Display(Name = "Date of Clearance")]
        [Required]
        public string IMS_SANCTIONED_DATE { get; set; }


        [Display(Name = "Pavement Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Pavement Cost,Can only contains Numeric values and Total 8 Digits and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Invalid Pavement Cost (Range should be 0-99999)")]
        public decimal IMS_SANCTIONED_PAV_AMT { get; set; }

        [Display(Name = "CD Works Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid CD Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Invalid CD Works Sanctioned Cost (Range should be 0-99999)")]
        public decimal IMS_SANCTIONED_CD_AMT { get; set; }

        [Display(Name = "Protection Works Cost(Rs Lakhs)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Protection Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        [Range(0, 99999, ErrorMessage = "Invalid Protection Works Cost (Range should be 0-99999)")]
        public decimal IMS_SANCTIONED_PW_AMT { get; set; }

        [Display(Name = "Other Works Cost (if Any)(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Other Works Cost(Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Other Works Cost,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_OW_AMT { get; set; }

        [Display(Name = "State Share(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid State Share Amount(Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "State Share Cost,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_RS_AMT { get; set; }

        [Display(Name = "Year 1(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 1 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 1 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT1 { get; set; }

        [Display(Name = "Year 2(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 2 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 2 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT2 { get; set; }

        [Display(Name = "Year 3(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 3 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 3 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT3 { get; set; }

        [Display(Name = "Year 4(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 4 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 4 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT4 { get; set; }

        [Display(Name = "Year 5(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Maintenance Year 5 Cost (Range should be 0-99999)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Maintenance Year 5 ,Can only contains Numeric values and 4 digits after decimal place")]
        public decimal IMS_SANCTIONED_MAN_AMT5 { get; set; }

        /// <summary>
        /// Total Maintenance Cost
        /// Only for the Display Purpose
        /// </summary>
        [Display(Name = "Total Maintenance Cost(Rs Lakhs)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Maintenance Cost Should not be Zero, Please Enter Atleast One Maintenance Cost.")]
        public decimal TotalMaintenanceCost { get; set; }

        [Display(Name="Remarks")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ,.()-]+$", ErrorMessage = "Invalid Remarks,Can only contains AlphaNumeric values and [,.()-]")]
        public string IMS_PROG_REMARKS { get; set; }


        [Display(Name = "Reason")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Reason")]
        public Nullable<int> IMS_REASON { get; set; }

        public List<SelectListItem> REASONS { get; set; }

        #region PMGSY Scheme2
        [Display(Name = "Whether any higher specification beyond PMGSY2 Guidelines have been proposed(including utility shifting)?")]
        public string IMS_IS_HIGHER_SPECIFICATION { get; set; }

        [Display(Name = "Higher Specification Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Higher Specification Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_SANCTIONED_HS_AMT { get; set; }

        [Display(Name = "Furniture Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Furniture Cost (Range should be 0-99999)")]
        public Nullable<decimal> IMS_SANCTIONED_FC_AMT { get; set; }

        [Display(Name = "Renewal Cost(Rs Lakhs)")]
        [Range(0, 99999, ErrorMessage = "Invalid Renewal Cost (Range should be 0-99999)")]
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