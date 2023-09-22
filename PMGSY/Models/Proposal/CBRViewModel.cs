#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CBRViewModel.cs
        * Description   :   This View Model is Used in CBR Views CBRValue.cshtml
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   30/Apr/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;

namespace PMGSY.Models.Proposal
{
    public class CBRViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string Operation { get; set;  }
        
        [Display(Name="District Name")]
        public string DistrictName { get; set; }

        [Display(Name="Block Name")]
        public string BlockName { get; set; }

        [Display(Name="Package ID")]
        public string PackageID { get; set; }

        [Display(Name="Road ID ")]
        public string RoadID { get; set; }

        [Display(Name="Road Name")]
        public string RoadName { get; set; }

        [Display(Name="Segment Number")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public int IMS_SEGMENT_NO { get; set; }
        
        [Display(Name="Start Chainage(in Kms.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Start Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0, 999999.999, ErrorMessage = "Invalid Start Chainage")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal? IMS_STR_CHAIN { get; set; }

        [Display(Name="End Chainage(in Kms.)")]
        [Required]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid End Chainage ,Can only contains Numeric values and 3 digits after decimal place")]
        [Range(0.001, 999999.999, ErrorMessage = "Invalid End Chainage")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal? IMS_END_CHAIN { get; set; }

        [Display(Name = "Segment Length")]
        //[Required]
        //[Range(0.001,999999.999,ErrorMessage="Invalid Segment Length, Please Enter Correct Start,End Chainage")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal? Segment_Length { get; set; }

        [Display(Name="CBR Value")]        
        [Required(ErrorMessage="Please Enter CBR Value")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid CBR Value ,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(1,30,ErrorMessage="Invalid CBR, Enter CBR Value between 1 to 30.")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal? IMS_CBR_VALUE1 { get; set; }

        public decimal RoadLength { get; set; }
           
        [Display(Name = "Pavement Length")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal IMS_PAV_LENGTH { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal Remaining_Length { get; set; }
        public decimal CBRLenghEntered { get; set; }

        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }

        [Display(Name = "Year")]
        public int IMS_YEAR { get; set; }
        [Display(Name = "Batch")]
        public int IMS_BATCH { get; set; }
        [Display(Name = "Package No.")]
        public string IMS_PACKAGE_ID { get; set; }
        [Display(Name = "Road Name")]
        public string IMS_ROAD_NAME { get; set; }

    }
}