/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterSurfaceViewModel.cs
 
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
    public class MasterSurfaceViewModel
    {
        [UIHint("hidden")]
        public string EncryptedSurfaceCode { get; set; }

        public int MAST_SURFACE_CODE { get; set; }
        [Display(Name="Surface Name")]
        [Required(ErrorMessage="Surface Name is required.")]
        [StringLength(50, ErrorMessage = "Surface Name must be less than 50 characters.")]
        [RegularExpression("[A-Za-z0-9 ._()]{1,50}", ErrorMessage = "Surface Name is not in valid format.")]
        public string MAST_SURFACE_NAME { get; set; }
        
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
        public virtual ICollection<MASTER_ER_SURFACE_TYPES> MASTER_ER_SURFACE_TYPES { get; set; }
    }
}