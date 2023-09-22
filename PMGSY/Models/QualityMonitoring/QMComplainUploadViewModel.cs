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
    public class QMComplainUploadViewModel
    {
        public QMComplainUploadViewModel()
        {
            DetailId=0;
            ComplainId =0;
            StageId =0;
            Remarks=string.Empty;
            FileName =string.Empty;
            NRRDAAction ="N";
            RemarkLabel =string.Empty;
            HeaderLabel =string.Empty;
            Type = string.Empty; 
      
        }
        
        public int DetailId { get; set; }
        
        public int ComplainId { get; set; }
        
        public int StageId { get; set; }

        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Please enter remarks")]
        [RegularExpression(@"^([a-zA-Z0-9 ]+)$", ErrorMessage = "Remarks contain invalid characters.")]
        [MaxLength(500, ErrorMessage="Maximum 500 characters allowed.")]
        public string Remarks { get; set; }
        
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        
        [Display(Name = "NRRDA Approvment")]
        public string NRRDAAction { get; set; }
        
        public string RemarkLabel { get; set; }
        public string HeaderLabel { get; set; }
        public string Type{get;set;}
      
    }
}