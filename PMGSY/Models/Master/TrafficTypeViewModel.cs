/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :RoadCategoryViewModel.cs
 
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
    public class TrafficTypeViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedStateCode { get; set; }

        [Display(Name="Traffic Code")]
        public int MAST_TRAFFIC_CODE { get; set; }

        [Required(ErrorMessage = "Traffic Type is required.")]
        [Display(Name="Traffic Type")]
        [StringLength(50,ErrorMessage="Traffic Type must be less than 50 characters.")]
//      [RegularExpression(@"^([a-zA-Z0-9()_. ]+)$", ErrorMessage = "Traffic Type is not in valid format.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9()_. ]+$", ErrorMessage = "Traffic Type is not in valid format.")]
        public string MAST_TRAFFIC_NAME { get; set; }
        
        public string MAST_TRAFFIC_STATUS { get; set; }
    
        public virtual ICollection<IMS_SANCTIONED_PROJECTS> IMS_SANCTIONED_PROJECTS { get; set; }
    }
}