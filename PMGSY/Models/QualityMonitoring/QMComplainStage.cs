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
    public class QMComplainStage
    {
        public int StageId { get; set; }
        public string StageTitle { get; set; }
        public string Remark { get; set; }
        public string NRRDAAction { get; set; }
        public string Download { get; set; }
        public string Delete { get; set; }

        
    }
}