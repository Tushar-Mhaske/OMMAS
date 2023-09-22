/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterScourFoundationTypeViewModel.cs
 
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
    public class MasterScourFoundationTypeViewModel
    {
        [UIHint("hidden")]
        public string EncryptedScourCode { get; set; }

        public int IMS_SC_FD_CODE { get; set; }
        [Display(Name = "Scour / Foundation Name")]
        [Required(ErrorMessage = "Scour / Foundation Name is required.")]
        [StringLength(100, ErrorMessage = "Scour / Foundation Name is not greater than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._]+)$", ErrorMessage = "Scour / Foundation Name is not in valid format.")]
        public string IMS_SC_FD_NAME { get; set; }

        [Display(Name = "Scour / Foundation Type")]
        [Required(ErrorMessage = "Scour / Foundation Type is required.")]
        [StringLength(1, ErrorMessage = "Scour / Foundation Type is not greater than 1 character.")]           
        public string IMS_SC_FD_TYPE { get; set; }

        public virtual ICollection<IMS_LSB_BRIDGE_DETAIL> IMS_LSB_BRIDGE_DETAIL
        {
            get;
            set;

        }
    }
}