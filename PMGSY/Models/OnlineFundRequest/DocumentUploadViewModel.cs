using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.OnlineFundRequest
{
    public class DocumentUploadViewModel
    {
        public DocumentUploadViewModel()
        {
            lstDocuments = new List<SelectListItem>();
            lstFiles = new List<HttpPostedFileBase>();
        }

        public int REQUEST_DOCUMENT_ID { get; set; }
        
        public int REQUEST_ID { get; set; }

        public string EncryptedRequestId { get; set; }
        
        public int DOCUMENT_ID { get; set; }
        public List<SelectListItem> lstDocuments { get; set; }
        
        [Display(Name="File Name")]
        public string UPLOAD_FILE_NAME { get; set; }
        
        public System.DateTime DOCUMENT_GENERATION_DATE { get; set; }
        
        public Nullable<System.DateTime> DOCUMENT_FINALIZATION_DATE { get; set; }
        
        public string DOCUMENT_FINALIZE { get; set; }

        public HttpPostedFileBase fileInfo { get; set; }
        
        [Display(Name="Remarks")]
        public string REMARKS { get; set; }
        
        public Nullable<int> USERID { get; set; }
        
        public string IPADD { get; set; }

        public List<HttpPostedFileBase> lstFiles { get; set; }

        public string DocumentBefore { get; set; }

        
    }
}