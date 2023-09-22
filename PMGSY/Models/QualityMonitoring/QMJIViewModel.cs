using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMJIViewModel
    {
        public string previousFileName { get; set; }

        public string dbOperation { get; set; }

        public int qmJICode { get; set; }

       // [Range(0, int.MaxValue, ErrorMessage = "Please select a valid State")]
        public int stateCode { get; set; }

        [Display(Name = "District")]
       // [Required(ErrorMessage = "Please select a District.")]
       // [Range(0, int.MaxValue, ErrorMessage = "Please select a valid District")]
        public int districtCode { get; set; }
        public List<SelectListItem> districtList { set; get; }

        

        [Display(Name = "Block")]
        //[Required(ErrorMessage = "Please select a Block.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select a valid Block")]
        public int blockCode { get; set; }
        public List<SelectListItem> blockList { set; get; }

        [Display(Name = "Work Type")]
       // [RegularExpression(@"^[P|L]+$", ErrorMessage = "Invalid work type selected, select either road or bridge")]
        public string workType { get; set; }
        public string _workType { get; set; }

        [Display(Name = "Work Name")]
        [Required(ErrorMessage = "Please select a Work Name.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Work Name")]
        public int roadCode { get; set; }
        public List<SelectListItem> roadList { set; get; }

        [Display(Name = "Connectivity Type")]
       // [RegularExpression(@"^[N|U]+$", ErrorMessage = "Invalid connectivity type selected, please select either new or upgradation")]
        public string connectivityType { get; set; }

        public string imsUpgradeConnect { get; set; }

        [Display(Name = "Public representative Type")]
       // [RegularExpression(@"^[MP|MLA|GP|O]+$", ErrorMessage = "Invalid representative type selected, please select either MP, MLA, GP or other")]
        public string publicRepresentativeType { get; set; }

        public bool isMP { get; set; }
        public bool isMLA { get; set; }
        public bool isGP { get; set; }
        public bool isOther { get; set; }

        [Display(Name = "Accompanying Govt. Officer Type")]
       //// [RegularExpression(@"^[SE|PIU|AE|DO]+$", ErrorMessage = "Invalid accompanying Govt Officer type selected, please select either SE, PIU, AE or District Officer")]
        public string accGovtOfficerType { get; set; }

        public bool isSE { get; set; }
        public bool isPIU { get; set; }
        public bool isAE { get; set; }
        public bool isDO { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "MP Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string mpName { get; set; }
        
        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "MLA Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string mlaName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "GP Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string gpName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "Other Representative Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string otherRepresentativeName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "SE Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string seName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "PIU Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string piuName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "AE Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string aeName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "District Officer Name contain Invalid characters, should contain only alphanumeric characters.")]
        public string districtOfficerName { get; set; }

        
        public string BlockName { get; set; }

        public string WorkName { get; set; }
        

        public string SanctionYear { get; set; }
        public string PackageId { get; set; }
        public string progressStatus { get; set; }
        public string imsProgress { get; set; }

       // [Required(ErrorMessage = "Please select contractor representative.")]
       // [RegularExpression(@"^[Y|N]+$", ErrorMessage = "Invalid contractor representative selected, please select yes or no")]
        public string contractorRepresentative { get; set; }

        public string contractorName { get; set; }

       // [Required(ErrorMessage = "Please select serve connectivity.")]
       // [RegularExpression(@"^[Y|N]+$", ErrorMessage = "Invalid serve connectivity selected, please select yes or no")]
        public string serveConnectivity { get; set; }

        //[RegularExpression(@"^[]+$", ErrorMessage = "Invalid contractor representative selected, please select yes or no")]
        public string inspectionFileName { get; set; }

       // [Required(ErrorMessage = "Please select work progress satisfactory.")]
       // [RegularExpression(@"^[Y|N]+$", ErrorMessage = "Invalid work progress satisfactory selected, please select yes or no")]
        public string workProgressSatisfactory { get; set; }

        //[RegularExpression(@"^[Y|N]+$", ErrorMessage = "Invalid contractor representative selected, please select yes or no")]
        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "Invalid characters in reason for variation in executed length, should contain only alphanumeric characters.")]
        public string variationExecLengthReason { get; set; }

      //  [Required(ErrorMessage = "Please select CD Works sufficient.")]
      //  [RegularExpression(@"^[Y|N]+$", ErrorMessage = "Invalid CD Works sufficient selected, please select yes or no")]
        public string cdWorkSufficient { get; set; }

        [Display(Name = "Inspection Date")]
        [Required(ErrorMessage = "Please select Inspection Date.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Inspection Date is not in valid format")]
        public string inspectionDate { get; set; }

      //  [Required(ErrorMessage = "Please select overall quality grading.")]
        [Display(Name = "Overall quality grading")]
     //   [RegularExpression(@"^[G|U|I]+$", ErrorMessage = "Invalid overall quality grading selected, please select either good, improvement required or unsatisfactory")]
        public string qualityGrading { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ,_().]+$", ErrorMessage = "Remarks contain Invalid characters, should contain only alphanumeric characters.")]
        public string remarks { get; set; }
    }
}