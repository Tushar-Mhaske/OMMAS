/*----------------------------------------------------------------------------------------
 * Project Id         :

 * Project Name       :OMMAS-II

 * File Name          :MasterExecutionItemViewModel.cs
 
 * Author             :Ashish Markande

 * Creation Date      :03/May/2013

 * Desc               :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PMGSY.Models.Master
{
       public class MasterExecutionItemViewModel
    {
        public string EncryptedHeadCode { get; set; }
        
        public int MAST_HEAD_CODE { get; set; }

        [Required]
        [Display(Name = "Item Description")]
        [RegularExpression("[A-Za-z _(),/.\r\n&-]{1,255}", ErrorMessage = "Item Description is not in valid format.")]
        [StringLength(255, ErrorMessage = "Item Descriptiom must be less than 255 characters.")]
       
        public string MAST_HEAD_DESC { get; set; }

        [Required]
        [Display(Name="Item Short Description")]
        [RegularExpression("[A-Za-z _()&,/.-]{1,50}", ErrorMessage = "Item Short Description is not in valid format.")]
        [StringLength(50, ErrorMessage = "Item Short Descriptiom must be less than 50 characters.")]
        public string MAST_HEAD_SH_DESC { get; set; }

        [Required]
        [Display(Name="Item Type")]
        [RegularExpression("[LR]", ErrorMessage = "Please select Item Type.")]
        public string MAST_HEAD_TYPE { get; set; }

    }
}
