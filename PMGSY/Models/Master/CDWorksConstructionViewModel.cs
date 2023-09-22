/*----------------------------------------------------------------------------------------
 * Project Id         :

 * Project Name       :OMMAS-II

 * File Name          :CDWorksConstructionViewModel.cs
 
 * Author             : Vikram Nandanwar

 * Creation Date      :01/May/2013

 * Desc               : This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class CDWorksConstructionViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedConstructionCode { get; set;}
        
        [Display(Name="Construction Code")]
        public int MAST_CDWORKS_CODE { get; set; }

        [Required(ErrorMessage = "CD Works Type is required.")]
        [Display(Name="CD Works Type")]
        [RegularExpression(@"^([a-zA-Z ()/]+)$", ErrorMessage = "CD Works Type is not in valid format.")]
        [StringLength(50, ErrorMessage = "CD Works Type  must be less than 50 characters.")]
        public string MAST_CDWORKS_NAME { get; set; }
    
        public virtual ICollection<MASTER_ER_CDWORKS_ROAD> MASTER_ER_CDWORKS_ROAD { get; set; }
    }
}