/*----------------------------------------------------------------------------------------
 * Project Id        :

 * Project Name      :OMMAS-II

 * File Name         :MasterChecklistPointsViewModel.cs
 
 * Author            :Rohit Jadhav 
 
 * Creation Date     :03/May/2013

 * Desc              : This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class MasterChecklistPointsViewModel
    {

            [UIHint("hidden")]
            public string EncryptedChecklistCode { get; set; }
            public int MAST_CHECKLIST_POINTID    { get; set; }

            [Display(Name = "Checklist Point")]            
            [StringLength(4000, ErrorMessage = " Checklist Point must be less then 4000 characters.")]
            [Required(ErrorMessage = "Checklist Point is required.")]
            [RegularExpression(@"^([a-zA-Z0-9 ._,:/?'\r\n&()-]+)$", ErrorMessage = "Checklist Point is not in valid format.")]

            public string MAST_CHECKLIST_ISSUES { get; set; }
       
            public string MAST_CHECKLIST_ACTIVE { get; set; }
    
            public virtual ICollection<MASTER_TEND_EQUIPMENT> MASTER_TEND_EQUIPMENT { get; set; }
     }
}