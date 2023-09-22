/*----------------------------------------------------------------------------------------
 * Project Id           :

 * Project Name         :OMMAS-II

 * File Name            :MasterComponentTypeViewModel.cs
 
 * Author               :Abhishek Kamble.

 * Creation Date        :01/May/2013

 * Desc                 :This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.Master
{
    public class MasterComponentTypeViewModel
    {

        [UIHint("hidden")]
        public string EncryptedStateCode { get; set; }

        public int MAST_COMPONENT_CODE { get; set; }
        
        [Display(Name = "Component Name")]        
        [Required(ErrorMessage = "Component Name is required")]
        [StringLength(100, ErrorMessage = "Component Name must be less than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._()]+)$", ErrorMessage = "Component Name is not in valid format.")]
        public string MAST_COMPONENT_NAME { get; set; }

        public virtual ICollection<IMS_LSB_BRIDGE_COMPONENT_DETAIL> IMS_LSB_BRIDGE_COMPONENT_DETAIL
        {
            get;
            set;
        }
    }
}