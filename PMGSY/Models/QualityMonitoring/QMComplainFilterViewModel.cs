#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   FileUploadViewModel.cs        
        * Description   :   Properties for File Upload in quality module
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMComplainFilterViewModel
    {
        public int RoleCode { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Stage")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Status.")]
        public int status { get; set; }
        public List<SelectListItem> statusList { get; set; }

        [Display(Name = "Received Through")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Recieved Through.")]
        public int RecievedThroughCode { get; set; }
        public List<SelectListItem> RecievedThroughList { get; set; }
        
        [Display(Name = "From Month")]
        public int FROM_MONTH { get; set; }
        public List<SelectListItem> FROM_MONTHS_LIST { set; get; }

        [Display(Name = "To Month")]
        public int TO_MONTH { get; set; }
        public List<SelectListItem> TO_MONTHS_LIST { set; get; }

        [Display(Name = "From Year")]
        public int FROM_YEAR { get; set; }
        public List<SelectListItem> FROM_YEARS_LIST { set; get; }

        [Display(Name = "To Year")]
        public int TO_YEAR { get; set; }
        public List<SelectListItem> TO_YEARS_LIST { set; get; }

        

        
    }
}