/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterQualificationViewModel.cs
 
 * Author           :Abhishek Kamble.

 * Creation Date    :01/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class MasterQualificationViewModel
    {
        [UIHint("hidden")]
        public string EncryptedQualCode{get;set;}  
  
        public int MAST_QUALIFICATION_CODE { get; set; }

        [Required(ErrorMessage="Qualification is required.")]
        [Display(Name="Qualification")]
        [RegularExpression(@"[A-Za-z0-9 _().-]{1,100}", ErrorMessage = "Qualification is not in valid format.")]
        [StringLength(100,ErrorMessage="Qualification must be less than 100 characters.")]
        public string MAST_QUALIFICATION_NAME { get; set; }
    
        public virtual ICollection<TEND_EMPLOYMENT_DETAILS> TEND_EMPLOYMENT_DETAILS { get; set; }
    }

  }
