/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterGradeTypeViewModel.cs
 
 * Author           : Abhishek Kamble.

 * Creation Date    :01/May/2013

 * Desc             : This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using PMGSY.DAL.Master;

namespace PMGSY.Models.Master
{
    public class MasterGradeTypeViewModel
    {
        public int MAST_GRADE_CODE { get; set; }

        [UIHint("hidden")]
        public string EncryptedGradeCode { get; set; }

        [Display(Name = "Grade Name")]
        [Required(ErrorMessage = "Grade Name is required.")]
        [RegularExpression(@"^([a-zA-Z ._]+)$", ErrorMessage = "Grade Name is not in valid format.")]
        [StringLength(100, ErrorMessage = "Grade Name is not greater than 100 characters.")]       
        public string MAST_GRADE_NAME { get; set; }

        [Display(Name = "Grade Short Name")]
        [Required(ErrorMessage = "Grade Short Name is required.")]
        //[RegularExpression(@"^([a-zA-Z0-9 ._]+)$", ErrorMessage = "Grade Short Name is not in valid format.")]
        //[StringLength(5, ErrorMessage = "Grade Short Name must be less than 5 characters.")]               
        [RegularExpression(@"^([IiSsUuNAna]+)$", ErrorMessage = "Please select Grade Short Name.")]
        public string MAST_GRADE_SHORT_NAME { get; set; }


        //Added By Abhishek kamble 21-Feb-2014

        public List<SelectListItem> PopulateGradeShortNames {

            get { 
                     IMasterDAL objDAL=new MasterDAL();
                     return objDAL.PopulateGradeShortNames();
            }                                                        
        
        }

    }
}