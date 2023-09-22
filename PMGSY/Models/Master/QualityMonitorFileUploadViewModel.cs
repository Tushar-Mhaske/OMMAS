/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :QualityMonitorFileUploadViewModel.cs
 
 * Author           :Abhishek Kamble.

 * Creation Date    :27/June/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class QualityMonitorFileUploadViewModel
    {
        public int ADMIN_QM_CODE { get; set; }

        public int ADMIN_QualityMonitor_CODE { get; set; }     

        public int? NumberofFiles { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }


        public string type { get; set; } //n
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; } //n


        public string file_type { get; set; }

        public string QMName { get; set; }
        public string QMDesignation { get; set; }
        public string QMType { get; set; }
        public string QMState { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }

    }
}