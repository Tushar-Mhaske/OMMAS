using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MaintainanceInspection
{
    public class FileUploadViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }

        public int MAINTENANCE_FILE_ID { get; set; }

        //public int? IMS_FILE_ID { get; set; }

        public string Urlparameter { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Image Description,Can only contains AlphaNumeric values")]
        public string Image_Description { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Stage")]
        [Display(Name = "Stage")]
        public int HeadItem { get; set; }
        public List<SelectListItem> lstHeadItems { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public int? NumberofFiles { get; set; }

        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }

        public string upload_date { get; set; }
        public string status { get; set; }
        public int file_type { get; set; }

        //Added By Abhishek kamble 26-Apr-2014
        public string ErrorMessage { get; set; }
    }
}