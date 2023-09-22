using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class GrowthScoreViewModel
    {

        //[UIHint("hidden")]
        //public string EncryptedScourCode { get; set; }

        [Required(ErrorMessage = "Score Id is required.")]
        public int ScoreId { get; set; }

        [Required(ErrorMessage = "Parent Id is required.")]
        public int ParentId { get; set; }

        [Required(ErrorMessage = "Score Level is required.")]
        public int ScoreLevel { get; set; }

        [Display(Name = "Score Value")]
        [Required(ErrorMessage = "Score Value is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Score Value is exceeding the max value.")]
        public int ScoreValue { get; set; }

        [Display(Name = "Item Description")]
        [Required(ErrorMessage = "Item Description is required.")]
        [StringLength(100, ErrorMessage = "Item Description must not be greater than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "Item Description is not in valid format.")]
        public string ScoreName { get; set; }

        [Display(Name = "Score Type")]
        [Required(ErrorMessage = "Score Type is required.")]
        [StringLength(1, ErrorMessage = "Score Type must not be greater than 1 character.")]
        public string ScoreType { get; set; }

        public List<SelectListItem> ScoreTypeList = new List<SelectListItem>();


        [UIHint("hidden")]
        public string EncryptedScoreCode { get; set; }

        public string hdnScoreCode { get; set; }
    }

    public class GrowthScoreSubItemViewModel
    {

        [Required(ErrorMessage = "Score Id is required.")]
        public int ScoreId { get; set; }

        [Required(ErrorMessage = "Parent Id is required.")]
        public int ParentId { get; set; }

        [Required(ErrorMessage = "Score Level is required.")]
        public int ScoreLevel { get; set; }

        [Display(Name = "Score Value")]
        [Required(ErrorMessage = "Score Value is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Score Value is exceeding the max value.")]
        public int ScoreValue { get; set; }

        [Display(Name = "Item Description")]
        [Required(ErrorMessage = "Item Description is required.")]
        [StringLength(100, ErrorMessage = "Item Description must not be greater than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "Item Description is not in valid format.")]
        public string ScoreName { get; set; }

        [Display(Name = "Score Type")]
        //[Required(ErrorMessage = "Score Type is required.")]
        [StringLength(1, ErrorMessage = "Score Type must not be greater than 1 character.")]
        public string ScoreType { get; set; }

        //public List<SelectListItem> ScoreTypeList = new List<SelectListItem>();


        [UIHint("hidden")]
        public string EncryptedScoreCode { get; set; }

        public string hdnScoreCode { get; set; }

        public string Description { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

    }
}