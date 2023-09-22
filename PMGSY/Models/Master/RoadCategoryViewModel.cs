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
    public class RoadCategoryViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }

        [Display(Name="Road Category Code")]
        public int MAST_ROAD_CAT_CODE { get; set; }
        
        [Required(ErrorMessage="Road Category Name is required.")]
        [Display(Name="Road Category Name")]
        [RegularExpression(@"^([a-zA-Z ()]+)$", ErrorMessage = "Road Category Name is not in valid format.")]
        [StringLength(50, ErrorMessage = "Road Category Name must be less than 50 characters.")]
        public string MAST_ROAD_CAT_NAME { get; set; }

        [Required(ErrorMessage = "Category Code is required.")]
        [Display(Name = "Category Code")]
        [RegularExpression(@"^([a-zA-Z ()._-]+)$", ErrorMessage = "Category Code is not in valid format.")]
        [StringLength(10, ErrorMessage = "Category Code must be less than 10 characters.")]
        public string MAST_ROAD_SHORT_DESC { get; set; }
    
        public virtual ICollection<MASTER_EXISTING_ROADS> MASTER_EXISTING_ROADS { get; set; }
    }
}