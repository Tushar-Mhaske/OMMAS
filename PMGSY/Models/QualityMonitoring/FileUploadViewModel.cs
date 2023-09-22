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
    public class FileUploadViewModel
    {
        public int ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }
        public int? QM_FILE_ID { get; set; }
        public int QM_OBSERVATION_ID { get; set; }

        public string QM_INSPECTION_DATE { get; set; }  // added on 30-01-2023

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Image Description,Can only contains AlphaNumeric values")]
        public string Image_Description { get; set; }

        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid chainage,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public decimal? chainage { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Pdf Description,Can only contains AlphaNumeric values")]
        public string PdfDescription { get; set; }

        public int? NumberofFiles { get; set; }

        public int? NumberofImages { get; set; }
        public int? NumberofPdfs { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }

        public string ErrorMessage { get; set; }
        public int QM_ATR_ID { get; set; }


        public DateTime CurDate { get; set; }  // added on 05-09-2022
        public string workType { get; set; }
        public string workStatus { get; set; }

        public string isATRPage { get; set; }

        public bool allowRejectedAtrView { get; set; }

        public int selectedSgradeObsId { get; set; }

    }
}