
/*----------------------------------------------------------------------------------------
 * Project Id             :

 * Project Name           : OMMAS-II

 * File Name              : TenderCostInformationViewModel.cs
 
 * Author                 : Abhishek Kamble

 * Creation Date          : 20/Nov/2013

 * Desc                   : This class is used as model to apply server side validation for Tender Cost Information.
 
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.FortyPointCheckList
{
    public class TenderCostInformationViewModel
    {   
        [UIHint("hidden")]                                  
        public string EncryptedTenderPriceId { get; set; }                
                
        public int MAST_STATE_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int MAST_CHECKLIST_POINTID { get; set; }
        public int TEND_PRICE_ID { get; set; }

        [Display(Name = "Works Costing From ( In Rs. )")]
        [Required(ErrorMessage = "Works Costing From is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Works Costing From,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(0.01, 9999999.99, ErrorMessage = "Invalid Works Costing From")]
        public decimal TEND_WORKS_COSTING_FROM { get; set; }

        [Display(Name = "Works Costing To ( In Rs. )")]
        [Required(ErrorMessage = "Works Costing To is required")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Works Costing To,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(0.01, 9999999.99, ErrorMessage = "Invalid Works Costing To")]
        public decimal TEND_WORKS_COSTING_TO { get; set; }

        [Display(Name = "Tender Price ( In Rs. )")]
        [Required(ErrorMessage = "Tender Price is required")]
        [RegularExpression("[A-Za-z0-9- ./()]{1,100}", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string TEND_SALE_PRICE { get; set; }

        
        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CHECKLIST_POINTS MASTER_CHECKLIST_POINTS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}
