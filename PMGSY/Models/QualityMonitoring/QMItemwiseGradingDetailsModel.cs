using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMItemwiseGradingDetailsModel
    {
        [Display(Name = "Grading")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Integers are allowed")]
        [Required(ErrorMessage = "Grade selection is required")]
        public int Grade_Code { get; set; }

        [Display(Name = "From Year")]
        [Required(ErrorMessage = "From Year is  required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Integers are allowed")]
        public int fromYear { get; set; }

        [Display(Name = "From month")]
        [Required(ErrorMessage = "From month is  required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Integers are allowed")]
        public int fromMonth { get; set; }

        [Display(Name = "To Year")]
        [Required(ErrorMessage = "To Year is  required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Integers are allowed")]
        public int toYear { get; set; }

        [Display(Name = "To month")]
        [Required(ErrorMessage = "To month is  required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Integers are allowed")]
        public int toMonth { get; set; }

        [Display(Name = "Work Type")]
        [Required(ErrorMessage = "Work Type is  required.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Work Type selection is invalid")]
        public char workType { get; set; }

        [Display(Name = "Work Status")]
        [Required(ErrorMessage = "Work Status is  required.")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Work Status selection is invalid")]
        public char workStatus { get; set; }


        public List<SelectListItem> gradeList { get; set; }
        public List<SelectListItem> yearList { get; set; }
        public List<SelectListItem> monthList { get; set; }
        public List<SelectListItem> workTypeList { get; set; }
        public List<SelectListItem> workStatusList { get; set; }
    }
}