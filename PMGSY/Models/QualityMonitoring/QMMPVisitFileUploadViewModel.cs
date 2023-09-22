using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace PMGSY.Models.QualityMonitoring
{
    public class QMMPVisitFileUploadViewModel
    {
        public int FILE_ID { get; set; }
        public int MP_VISIT_ID { get; set; }

        [Display(Name = "File Name")]
        public string FILE_NAME { get; set; }

        public char IS_PDF { get; set; }

        [Display(Name = "Date of Upload.")]
        public string FILE_UPLOAD_DATE { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid Image Description,Can only contains AlphaNumeric values")]
        public string Image_Description { get; set; }

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

        [Display(Name = "MP Name")]        
        public string MPName { get; set; }

        [Display(Name = "MP House")]        
        public string MPHouse { get; set; }
        
        [Display(Name = "PIU Name")]        
        public string PIUName { get; set; }

        [Display(Name = "Date Of Visit")]        
        public string DateOfVisit { get; set; }


    }
}