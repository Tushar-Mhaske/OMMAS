using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class PDFSaveFormModel
    {
        public string Enc_IMS_PR_ROAD_CODE { get; set; }



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