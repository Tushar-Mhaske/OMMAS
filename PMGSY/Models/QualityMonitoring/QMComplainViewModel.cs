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
    public class QMComplainViewModel
    {

        public int ComplainId { get; set; }
        public int ComplainFileId { get; set; }
        
        public bool IsDelete { get; set; }
        
        [Display(Name = "Complainant")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Complainant.")]
        public int ComplainantCode { get; set; }
        public List<SelectListItem> ComplainantList { get; set; }
        public string ComplainantName { get; set; }

        [Display(Name = "Received Through")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Recieved Through.")]
        public int RecievedThroughCode { get; set; }
        public List<SelectListItem> RecievedThroughList { get; set; }
        public string RecievedThroughName { get; set; }

        [Display(Name = "Forwarded To")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Forwarded TO.")]
        public int ForwardedToCode { get; set; }
        public List<SelectListItem> ForwardedToList { get; set; }
        public string ForwardedToName { get; set; }

        [Display(Name = "Nature of Complaint")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Nature of Complaint.")]
        public int NatureComplaintCode { get; set; }
        public List<SelectListItem> NatureComplaintList { get; set; }
        public string NatureComplaintName { get; set; }

        [Display(Name = "State")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public string StateName { get; set; }

        [Display(Name = "Complaint Received Date")]
        [Required(ErrorMessage = "Please select a valid Complaint Received Date.")]
        public string ComplainRecievedDate { get; set; }

        public string Status { get; set; }
    }
}