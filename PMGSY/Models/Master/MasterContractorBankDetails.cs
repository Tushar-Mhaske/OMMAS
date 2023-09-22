/*----------------------------------------------------------------------------------------
 * Project Id          :

 * Project Name        :OMMAS-II

 * File Name           :MasterContractorBankDetails.cs
 
 * Author              :Ashish Markande
 
 * Creation Date       :31/May/2013

 * Desc                :This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using PMGSY.DAL.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class MasterContractorBankDetails
    {
        public string EncryptedAccountId { get; set; }
        public string EncryptedContractorId { get; set; }
        
        public int MAST_CON_ID { get; set; }
       
        public int MAST_ACCOUNT_ID { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State.")]
        [Range(1, 2147483647, ErrorMessage = "Please select State.")]
        public int Mast_State_Code { get; set; }   

        [Display(Name="District")]
        [Required(ErrorMessage="Please select Distict.")]
        [Range(1, 2147483647, ErrorMessage = "Please select Distict.")]
        public int MAST_DISTRICT_CODE { get; set; }

        [Required(ErrorMessage="Account Number is required")]
        [Display(Name="Account Number")]
        // Changes made on 17-01-2022 by Srishti Tyagi
        //[RegularExpression("^([0-9]{5,17})$", ErrorMessage = "Account Number must be minimum 5 digits amd maximum 17 digits only.")]
        //[StringLength(17,ErrorMessage="Account Number must be 17 digits only.")]
        [RegularExpression("^[a-zA-Z0-9]{9,18}$", ErrorMessage = "Account Number must be minimum 9 digits amd maximum 18 digits only.")]
        [StringLength(18, ErrorMessage = "Account Number must be 18 digits only.")]
        public string MAST_ACCOUNT_NUMBER { get; set; }

        [Display(Name="Bank Name")]
        [Required(ErrorMessage = "Bank Name is required")]
        [RegularExpression(@"^([a-zA-Z ,_()&-;'.]+)$", ErrorMessage = "Bank Name is not in valid format.")]
        [StringLength(100, ErrorMessage = "Bank Name must be less than 100 characters.")]
        public string MAST_BANK_NAME { get; set; }
        public List<SelectListItem> lstBankNames { get; set; }

        [Required(ErrorMessage = "Please enter IFSC Code")]
        [Display(Name = "IFSC Code")]
        //[RegularExpression("^([a-zA-Z0-9]{11})", ErrorMessage = "IFSC Code is not in valid format.")]
        //[RegularExpression(@"^([a-zA-Z0-9]+)$", ErrorMessage = "IFSC Code is not in valid format.")]
        
        //[RegularExpression(@"^([A-Z|a-z]{4}[0][\d]{6})$", ErrorMessage = "IFSC Code is not in valid format.")]
        [RegularExpression(@"^([A-Z|a-z]{4}[0][A-Z|a-z|0-9]{6})$", ErrorMessage = "IFSC Code is not in valid format.")]
        [StringLength(11, ErrorMessage = "IFSC Code must be 11 characters only.")]
        public string MAST_IFSC_CODE { get; set; }
        public List<SelectListItem> lstIfscCodes { get; set; }

        public string pfmsErrorMessage { get; set; }

        public string MAST_ACCOUNT_STATUS { get; set; }
        public string MAST_LOCK_STATUS { get; set; }

        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }

        public string encrNodalOfficerCode { get; set; }
        public int NodalOfficerCode { get; set; }

    }
}