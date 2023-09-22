/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterUnitsTypeViewModel.cs
 
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
    public class MasterUnitsTypeViewModel
    {

        [UIHint("hidden")]
        public string EncryptedUnitCode { get; set; }

        public int MAST_UNIT_CODE { get; set; }

        [Display(Name = "Unit Name")]
        [Required(ErrorMessage = "Unit Name is required.")]
        [RegularExpression(@"^([a-zA-Z %.]+)$", ErrorMessage = "Unit Name is not in valid format.")]
        [StringLength(30, ErrorMessage = "Unit Name must be less than 30 characters.")]      
        public string MAST_UNIT_NAME { get; set; }

        [Display(Name = "Unit Short Name")]
        [Required(ErrorMessage = "Unit Short Name is required.")]
        [RegularExpression(@"^([a-zA-Z %.]+)$", ErrorMessage = "Unit Short Name is not in valid format.")]
        [StringLength(20, ErrorMessage = "Unit Short Name must be less than 20 characters.")]         
        public string MAST_UNIT_SHORT_NAME { get; set; }

        [Display(Name = "Unit Dimension")]
        [RegularExpression("[0-9]{0,2}", ErrorMessage = "Unit Dimension is not in valid format.")]
        [StringLength(2, ErrorMessage = "Unit Dimension is not greater than 2 digits.")]         
        public Nullable<int> MAST_UNIT_DIMENSION { get; set; }
    }
}