#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CoreNetworkUploadFileViewModel.cs
        * Description   :   This View Model is Used in Core Network Views FileUpload.cshtml,CoreNetworkFileUpload.cshtml
        * Author        :   Vikram Nandanwar       
        * Creation Date :   30/May/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.CoreNetwork
{
    public class CoreNetworkUploadFileViewModel
    {

        [UIHint("hidden")]
        public string EncryptedUploadCode { get;set;}

        public int NumberOfFiles {get;set; }

        public int PLAN_CN_ROAD_CODE { get; set; }
        public int PLAN_FILE_ID { get; set; }
        public string PLAN_FILE_NAME { get; set; }
        public Nullable<int> PLAN_FILE_SIZE { get; set; }
        
        public System.DateTime PLAN_UPLOAD_DATE { get; set; }
        
        public decimal PLAN_START_CHAINAGE { get; set; }
        public decimal PLAN_END_CHAINAGE { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }   
    }
}