/*----------------------------------------------------------------------------------------
 * Project Id          :

 * Project Name        :OMMAS-II

 * File Name           :MasterContractorClassTypeViewModel.cs
 * 
 * Author              :Vikram Nandanwar

 * Creation Date       :01/May/2013

 * Desc                :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.Master
{
    public class MasterContractorClassTypeViewModel
    {
        [UIHint("hidden")]
        public string EncryptedContClassCode { get; set; }

        public int MAST_CON_CLASS { get; set; }

        [Display(Name = "Class Name")]
        [Required(ErrorMessage = "Class Name is required")]
        [RegularExpression("[a-zA-Z0-9- _,().]{0,25}", ErrorMessage = "Class Name is not in valid format.")]
        [StringLength(25, ErrorMessage = "Class Name is not greater than 25 characters.")] 
        public string MAST_CON_CLASS_TYPE_NAME { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "State Code is required.")]    
        [Range(1,Int32.MaxValue,ErrorMessage="Please select State")]
        public int MAST_STATE_CODE { get; set; }
        
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        public virtual ICollection<MASTER_CONTRACTOR_REGISTRATION> MASTER_CONTRACTOR_REGISTRATION { get; set; }  
    }
}