using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.PFMS
{
    public class DSCPFMSModel
    {
        public bool IsAccountNumberAvailable { get; set; }
        public bool IsIFSCAvailable { get; set; }
        public bool IsInitPartyAvailable { get; set; }
        public bool IsEmailAvailable { get; set; }

        public Int64 billId { get; set; }

        public int mastConId { get; set; }
        public int conAccountId { get; set; }

        public bool IsValidContractor { get; set; }

        public string operation { get; set; }
        
        public int officerCode { get; set; }

        public string adminNdName { get; set; }

        public bool IsConAgency { get; set; }
    }
}