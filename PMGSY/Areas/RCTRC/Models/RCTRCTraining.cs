using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Areas.RCTRC.Models
{
    public class RCTRCTraining
    {
        [UIHint("Hidden")]
        public string EncryptedTrainingCode { get; set; }


        [Display(Name = "Training Start Date")]
        [Required(ErrorMessage = "Select Training Start Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string START_DATE { get; set; }


        [Display(Name = "Training End Date")]
        [Required(ErrorMessage = "Select Training End Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        public string END_DATE { get; set; }


        [Display(Name = "Training Topic")]
        [Required(ErrorMessage = "Please enter Training Topic.")]
        [RegularExpression(@"^([a-zA-Z .-]+)$", ErrorMessage = "Only Alphabets are allowed")]
        [StringLength(200, ErrorMessage = "Name must be less than 200 characters.")]
        public string TOPIC { get; set; }


        [Display(Name = "Training Duration")]
        [Required(ErrorMessage = "Please enter Training Duration.")]
        [RegularExpression("([0-9 ]{1,255})", ErrorMessage = "Invalid Training Duration is entered.")]
        //  [RegularExpression("([a-zA-Z 0-9 _(),#$-.]{1,255})", ErrorMessage = "Training Duration is not in valid format.")]
        [StringLength(4, ErrorMessage = "Invalid Training Duration.")]
        public string DURATION { get; set; }

     
        //[StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]

        [Display(Name = "Training Host")]
        [Required(ErrorMessage = "Please Training Host.")]
        [RegularExpression(@"^([a-zA-Z .-]+)$", ErrorMessage = "Only Alphabets are allowed")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string HOST { get; set; }


        [Display(Name = "Contact Person")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonID { get; set; }
        public List<SelectListItem> ContactPerson_List { get; set; }

        [Display(Name = "Contact Person")]
        [Range(0, int.MaxValue, ErrorMessage = "Select Contact Person.")]
        public int ContactPersonIDSearch { get; set; }




    }
}