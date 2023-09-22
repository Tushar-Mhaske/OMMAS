﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PMIS.Models
{
    public class PMISDataCorrectionModel
    {
        [Display(Name = "State")]
        [Required(ErrorMessage = "Please Select State")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Display(Name = "District")]
        [Required(ErrorMessage = "Please Select District")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select District.")]
        public int DistrictCode { get; set; }
        public List<SelectListItem> DistrictList { get; set; }

        [Display(Name = "Block")]
        [Required(ErrorMessage = "Please Select Block")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select Block.")]
        public int BlockCode { get; set; }
        public List<SelectListItem> BlockList { get; set; }

        [Display(Name = "Batch")]
        [Range(0, 10, ErrorMessage = "Please select Batch.")]
        [Required(ErrorMessage = "Please select Batch.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Batch must be valid number.")]
        public int Batch { get; set; }
        public List<SelectListItem> BatchList { get; set; }

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Sanction_Year { get; set; }
        public List<SelectListItem> Sanction_Year_List { get; set; }

        [Display(Name = "List Type")]
        [Required(ErrorMessage = "Please select List Type")]
        public String ListType { get; set; }
        public List<SelectListItem> ListTypeList { get; set; }

    }

    public class PMISDataCorrectionRoadDAL
    {
        public string StateName;
        public string DistrictName;
        public string BlockName;
        public string BatchName;
        public string SanctionYear;
        public string SanctionDate;
        public string PackageName;
        public string SanctionLength;
        public string AgreementNo;
        public string AgreementCost;
        public string MordShare;
        public string StateShare;
        public string TotalSanctionedCost;
        public string AgreementStartDate;
        public string AgreementEndDate;
        public string RoadName;
        public string IMS_PR_RoadCode;

        // new added
        public string planCheckedRoadCode;
                
    }

    public class PMISDataCorrectionBridgeDAL
    {
        public string StateName;
        public string DistrictName;
        public string BlockName;
        public string BatchName;
        public string SanctionYear;
        public string SanctionDate;
        public string PackageName;
        public string SanctionLength;
        public string AgreementNo;
        public string AgreementCost;
        public string MordShare;
        public string StateShare;
        public string TotalSanctionedCost;
        public string AgreementStartDate;
        public string AgreementEndDate;
        public string LSBName;
        public string IMS_PR_RoadCode;

        // new added
        public string planCheckedRoadCode;
       
    }

    public class UpdateCompletionDateLengthModel
    {
        public string StateName;
        public string DistrictName;
        public string BlockName;
        public string SanctionYear;
        public string RoadName;
        public int PlanId;
        public int BaselineNo;

        [Required(ErrorMessage = "Completion Date is required.")]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Invalid date format.")]
        public string CompletedDate;
        
        [Required(ErrorMessage = "Length is required.")]
        [RegularExpression(@"^(10|\d)(\.\d{3})?$", ErrorMessage = "Only Numbers allowed.")]
        public decimal CompletedLength;
       
    }
    
}