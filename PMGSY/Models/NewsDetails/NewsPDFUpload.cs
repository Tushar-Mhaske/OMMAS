﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.NewsDetails
{
    public class NewsPDFUpload
    {
        [UIHint("hidden")]
        public string EncryptedUploadCode { get; set; }

        public int NumberOfFiles { get; set; }

        public int News_Id { get; set; }
        public int NEWS_FILE_ID { get; set; }
        public string NEWS_FILE_NAME { get; set; }
        public Nullable<int> NEWS_FILE_SIZE { get; set; }

        //public int IMS_PR_ROAD_CODE { get; set; }
        //public int? IMS_FILE_ID { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Image Description,Can only contains AlphaNumeric values")]
        public string Image_Description { get; set; }

        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid chainage,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public decimal? chainage { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Pdf Description,Can only contains AlphaNumeric values")]
        public string PdfDescription { get; set; }

        //public int? NumberofFiles { get; set; }

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


        //[Display(Name = "Year")]
        //public int IMS_YEAR { get; set; }
        //[Display(Name = "Batch")]
        //public int IMS_BATCH { get; set; }
        //[Display(Name = "Package No.")]
        //public string IMS_PACKAGE_ID { get; set; }
        //[Display(Name = "Road Name")]
        //public string IMS_ROAD_NAME { get; set; }
        //[Display(Name = "Pavement Length")]
        //public decimal IMS_PAV_LENGTH { get; set; }
    }
}