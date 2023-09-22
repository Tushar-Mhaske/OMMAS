#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   UploadQCRModel.cs
        * Description   :   This View Model is Used in Views UploadQCRLayoutModel.cshtml
        * Author        :   Vikky Ghate        
        * Creation Date :   12/01/2022
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
    public class UploadQCRModel
    {
      

        [Display(Name = "State Name")]
        public String StateName { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }


        [Display(Name = "Sanctioned Year")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select a valid year")]
        public int year { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "District")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }

    }

    public class UploadedQCRModel
    {


        [Display(Name = "State Name")]
        public String StateName { get; set; }

        [Display(Name = "State")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state")]
        public int stateCode { get; set; }
        public List<SelectListItem> lstStates { set; get; }


        [Display(Name = "Sanctioned Year")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select a valid year")]
        public int year { get; set; }
        public List<SelectListItem> Years { set; get; }

        [Display(Name = "District")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select a valid district")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistricts { set; get; }

    }
}