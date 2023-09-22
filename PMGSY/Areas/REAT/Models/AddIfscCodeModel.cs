using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.REAT.Models
{
    public class AddIfscCodeModel
    {
        public int Id { get; set; }

        [Display(Name = "Bank Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Bank Name")]
        [RegularExpression("^[a-zA-Z0-9-/.,()' ]{3,200}$", ErrorMessage = "Invalid Bank Name")]
        public string BankName { get; set; }
        public List<SelectListItem> listBanks { set; get; }

        [Display(Name = "Bank Branch")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Bank branch")]
        [RegularExpression("^[a-zA-Z. ]{3,50}$", ErrorMessage = "Invalid Bank Branch")]
        public string BranchName { get; set; }

        [Display(Name = "Bank Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter branch address")]
        [RegularExpression("^[A-Za-z0-9-,.&/ ]{3,300}$", ErrorMessage = "Invalid Bank Address")]
        public string BankAddress { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter city")]
        [RegularExpression("^[a-zA-Z ]{3,100}$", ErrorMessage = "Invalid city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select State")]
        [Range(1, 40, ErrorMessage = "Please select a valid State")]
        public int stateCode { get; set; }
        public string stateName { get; set; }
        public List<SelectListItem> lstState { set; get; }

        [Display(Name = "IFSC Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter IFSC code")]
        [RegularExpression("^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code")]
        public string IfscCode { get; set; }

        
    
    }
}