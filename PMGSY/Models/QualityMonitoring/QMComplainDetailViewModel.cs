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

namespace PMGSY.Models.QualityMonitoring
{
    public class QMComplainDetailViewModel
    {
        public int ComplainId { get; set; }

        [Display(Name = "Complainant")]
        public string ComplainantName { get; set; }
        
        [Display(Name = "Received Through")]
        public string RecievedThroughName { get; set; }
       
        [Display(Name = "Forwarded To")]
        public string ForwardedToName { get; set; }
       
        [Display(Name = "Nature of Complaint")]
        public string NatureComplaintName { get; set; }
        
        [Display(Name = "State")]
        public string StateName { get; set; }
       
        [Display(Name = "Complaint Received Date")]
        public string ComplainRecievedDate { get; set; }
       
        public List<QMComplainStage> StageList { get; set; }
        
    }
}