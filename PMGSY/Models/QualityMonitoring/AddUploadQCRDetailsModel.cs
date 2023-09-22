#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   AddUploadQCRModel.cs
        * Description   :   This View Model is Used in Views AddQCRUploadDetails.cshtml
        * Author        :   Vikky Ghate        
        * Creation Date :   14/01/2022
 **/
#endregion
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class AddUploadQCRDetailsModel
    {
        public String IMS_PR_ROAD_CODE { get; set; }

        public String QCRfileName { get; set; }

        public String filePath { get; set; }

        public Char IS_LATEST { get; set; }

        [Display(Name = "State")]
        public List<SelectListItem> lstStates { set; get; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "District")]
        public List<SelectListItem> lstDistricts { set; get; }

        [Display(Name = "PDF of QCR-I")]
        [Required(ErrorMessage = "Pdf file is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase QCRFile { get; set; }

      
        [Required]
        [Display(Name = "Uploaded Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Issue Date must be in dd/mm/yyyy format.")]
        public string UploadedDate { get; set; }
        
         [Display(Name = "Remark")]
        [StringLength(250, ErrorMessage = "Maximum 250 Characters Allowed")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public String uploadRemark { set; get; }

         [Display(Name = "QCR Part-I Grading by SE")]
         [Required(AllowEmptyStrings = false, ErrorMessage = "Grade by SE is required")]
         public string awardGradeSE { set; get; }
         public List<SelectListItem> gradeSE { set; get; }

         [Display(Name = "QCR Part-I Grading by SQC")]
         [Required(AllowEmptyStrings = false, ErrorMessage = "Grade by SQC is required")]
         public string awardGradeSQC { set; get; }
         public List<SelectListItem> gradeSQC { set; get; }

    }
}