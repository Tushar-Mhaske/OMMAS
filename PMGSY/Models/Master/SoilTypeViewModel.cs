/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :SoilTypeViewModel.cs
 
 * Author           :Vikram Nandanwar

 * Creation Date    :01/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class SoilTypeViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedSoilCode { get; set; }

        [Display(Name="Soil Type Code")]
        public int MAST_SOIL_TYPE_CODE { get; set; }

        [Required(ErrorMessage = "Soil Type is required.")]
        [Display(Name="Soil Type")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "Soil Type is not in valid format.")]
        [StringLength(50, ErrorMessage = "Soil Type must be less than 50 characters.")]
        public string MAST_SOIL_TYPE_NAME { get; set; }
    
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }
    }
}