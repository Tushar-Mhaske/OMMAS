using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.REAT.Models
{
    public class ContractorMappingREAT
    {
        public string cpsmsID { get; set; }
        public int contractorID { get; set; }
        //public string contractorName { get; set; }
        public int lgdStateCode { get; set; }
        public int lgdDistrictCode { get; set; }
        public string bankName { get; set; }
        public string branchName { get; set; }
        public string accountNumber { get; set; }

        public string batchId { get; set; }

        public string acceptStatus { get; set; }
        public List<string> lstRejectCode { get; set; }

        public string pfmsConName { get; set; }
        public string pfmsIFSC { get; set; }
        public string pfmsStatus { get; set; }

        public string pfmsStateCode { get; set; }

        public string pfmsResponseDate { get; set; }
    }
}