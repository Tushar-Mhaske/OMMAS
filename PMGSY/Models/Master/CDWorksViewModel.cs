/*----------------------------------------------------------------------------------------
 * Project Id            :

 * Project Name          :OMMAS-II

 * File Name             :CDWorksViewModel.cs
 
 * Author                : Vikram Nandanwar

 * Creation Date         :01/May/2013

 * Desc                  : This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class CDWorksViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedCdWorksCode { get; set; }

        [Display(Name="CdWorks Code")]
        public int MAST_CDWORKS_CODE { get; set; }

        [Required(ErrorMessage="CD Works Length is required.")]
        [RegularExpression(@"^([a-zA-Z0-9 =]+)$", ErrorMessage = "CD Works Length is not in valid format.")]
        [StringLength(50, ErrorMessage = "CD Works Length must be less than 50 characters.")]
        [Display(Name="CD Works Length")]
        public string MAST_CDWORKS_NAME { get; set; }
    }
}