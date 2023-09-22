/*----------------------------------------------------------------------------------------
 * Project Id              :

 * Project Name            :OMMAS-II

 * File Name               :FundingAgencyViewModel.cs
 
 * Author                  :Vikram Nandanwar

 * Creation Date           :01/May/2013

 * Desc                    : This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class FundingAgencyViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedFundCode { get; set; }

        [Display(Name="Funding Agency Code")]
        public int MAST_FUNDING_AGENCY_CODE { get; set; }

        [Required(ErrorMessage = "Funding Agency Name is required.")]
        [Display(Name="Funding Agency Name")]
        [RegularExpression(@"^([a-zA-Z ()-]+)$", ErrorMessage = "Funding Agency Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Funding Agency Name must be less than 50 characters.")]
        public string MAST_FUNDING_AGENCY_NAME { get; set; }
    
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
    }
}