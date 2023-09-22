#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   PaymentScheduleViewModel.cs
        * Description   :   This View Model is Used in CBR Views PaymentScheduleAddEdit.cshtml
        * Author        :   Vikram Nandanwar        
        * Creation Date :   19/June/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class PaymentScheduleViewModel
    {
        public string Operation { get; set; }

        [Display(Name="District")]
        public string District { get; set; }

        [Display(Name = "Sanctioned  Year")]
        public string SanctionedYear { get; set; }
        
        [Display(Name="Block")]
        public string Block { get; set; }

        [Display(Name = "Package Number")]
        public string PackageNumber { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        [Display(Name = "Sanction Cost (in Rs. lakh)")]
        public string SanctionedCost { get; set; }

        [Display(Name = "Agreement Amount (in Rs. lakh)")]
        public string AgreementAmount { get; set; }

        [Display(Name = "Sactioned Length (in Km)")]
        public string SactionedLength { get; set; }

        public string ProposalType { get; set; }

        //new
        public int IMS_PR_ROAD_CODE { get; set; }
        
        [Display(Name="Year")]
        [Range(2000,3000,ErrorMessage="Please select year")]
        [CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Year")]
        public int EXEC_MPS_YEAR { get; set; }

        [Display(Name="Month")]
        [Range(1,12,ErrorMessage="Please select Month")]
        [CompareAgrementMonth("EXEC_MPS_YEAR", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Month")]
        public int EXEC_MPS_MONTH { get; set; }

        [Required(ErrorMessage="Please enter Scheduled Payment")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Schedule Payment,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(0.01,9999999999999999.99,ErrorMessage="Payment value is invalid.")]
        [Display(Name="Scheduled Payment [Rs. Lakh] ")]
        public decimal EXEC_MPS_AMOUNT { get; set; }

        public int AgreementYear { get; set; }

        public int AgreementMonth { get; set; }

        public string AgreementDate { get; set; }


        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }

    }//end of model
}