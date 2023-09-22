using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class PDFFileUploadViewModel
    {
        public int ADMIN_SCHEDULE_CODE { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }

        public string Enc_IMS_PR_ROAD_CODE { get; set; }

        public int? QM_FILE_ID { get; set; }

        public int IMS_FOREST_CLR_ID { get; set; }

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
    }
}