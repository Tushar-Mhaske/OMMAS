using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Feedback
{
    public class FileUploadViewModel
    {
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }


        public string ATRId { get; set; }
        public string DocumentName { get; set; }
        public string IsFinalized { get; set; }


        public string FinalizedByPIU { get; set; }

        public string FinalizedBySQC { get; set; }

        public int REP_ID { get; set; }
        public int FEED_ID { get; set; }
        public int? FILE_ID { get; set; }
       

       // public int QM_OBSERVATION_ID { get; set; }


        [Required(ErrorMessage = "Description is mandatory")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Image Description,Can only contains AlphaNumeric values")]
        public string Image_Description { get; set; }

        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid chainage,Can only contains Numeric values and Total 7 Digits and 3 digits after decimal place")]
        public decimal? chainage { get; set; }

        [Required(ErrorMessage = "Description is mandatory")]
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
    }
}