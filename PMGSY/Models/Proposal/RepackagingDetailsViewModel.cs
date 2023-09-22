#region File Header
/*
        * Project Name  :   OMMAS II
        * Name          :   RepackagingDetailsViewModel.cs
        * Description   :   This View Model is Used for updating the packages of proposals
        * Author        :   Vikram Nandanwar        
        * Creation Date :   09/June/2014
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class RepackagingDetailsViewModel:IValidatableObject
    {

        public RepackagingDetailsViewModel()
        {
            lstPackages = new List<SelectListItem>();
        }

        [Range(2000, 2099, ErrorMessage = "Please select a valid Year")]
        public int Year { get; set; }
        public List<SelectListItem> lstYear { set; get; }

        public string EncProposalCode { get; set; }

        public string ShortStateCode { get; set; }

        [Display(Name = "Old Package")]
        public string OLD_PACKAGE_ID { get; set; }

        public string OLD_PACKAGE_ID_PREFIX { get; set; }

        public string NewOldPackage { get; set; }

        public string ExistingPackage { get; set; }

        public List<SelectListItem> lstPackages { get; set; }

        //[StringLength(10,ErrorMessage="Package Name must be less than or equal to 10 characters.")]
        [StringLength(11, ErrorMessage = "Package Name must be less than or equal to 11 characters.")]
        [Display(Name="New Package")]
        [RegularExpression(@"^[a-zA-Z0-9 -/]+$", ErrorMessage = "Invalid Package ID,Can only contains AlphaNumeric values")]
        public string NEW_PACKAGE_ID { get; set; }

        public decimal roadLength { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.NewOldPackage == "Y")
            {
                if (this.NEW_PACKAGE_ID == null || this.NEW_PACKAGE_ID == "0")
                {
                    yield return new ValidationResult("New Package is required.");
                }
                else
                {
                    yield return ValidationResult.Success;
                }
            }
            else if (this.NewOldPackage == "N")
            {
                if (this.ExistingPackage == null || this.ExistingPackage == "0")
                {
                    yield return new ValidationResult("New Package is required.");
                }
                else
                {
                    yield return ValidationResult.Success;
                }
            }



        }
    }
}