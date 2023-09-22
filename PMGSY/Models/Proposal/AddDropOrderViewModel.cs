using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Proposal
{
    public class AddDropOrderViewModel
    {
        public int imsPrRoadCode { get; set; }
        public string encryptedRoadCode { get; set; }

        public decimal expenditureIncurred { get; set; }

        [Display(Name = "Recoup Amount (Rs. in Lakhs)")]
        [Required(ErrorMessage="Please enter Recoup Amount")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid Recoup Amount, can only contains 8 Numeric digits and 2 digit after decimal place.")]
        public decimal recoupAmt { get; set; }
        
        [Display(Name="Drop Reason")]
        [Required(ErrorMessage = "Please select Reason for Dropping")]
        [Range(1, int.MaxValue, ErrorMessage="Please select a valid Dropping Reason")]
        public int dropReason { get; set; }
        public List<SelectListItem> lstDropReason { set; get; }

    }
}