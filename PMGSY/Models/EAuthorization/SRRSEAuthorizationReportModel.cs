using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.EAuthorization
{
    public class SRRSEAuthorizationReportModel
    {

        public string UserName { get; set; }

        public string Password { get; set; }
        public string FolderNameAndReportName { get; set; }
        public string ReportServerUrl { get; set; }
        public Object ReportParameter { get; set; }

        public string EncryptedID { get; set; }
        public string QueryString { get; set; }

        public byte[] FileBytes { get; set; }


        
    }
}