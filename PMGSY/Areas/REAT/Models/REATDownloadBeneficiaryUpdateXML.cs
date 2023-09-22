using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.REAT.Models
{
    public class REATDownloadBeneficiaryUpdateXML
    {
        public int StateCode { get; set; }
        public int AgencyCode { get; set; }
        public int ContractorID { set; get; }
        public int AccountID { get; set; }
       
        public string reatIFSC { get; set; }
        public string reatContractorName { set; get; }

        public int UserID { get; set; }
        public string IPAddress { get; set; }

        public int DETAIL_ID { get; set; }

        public string BankName { get; set; }

    }
}