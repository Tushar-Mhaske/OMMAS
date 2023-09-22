using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EmargDataPull.Models
{
    public class EmargAck
    {
        public Int32 EID { get; set; }
        public string packageNo { get; set; }
        public string acknowledgementDate { get; set; }
        public string successStatus { get; set; }
        public string rejectStatus { get; set; }
        public string rejectCode { get; set; }
        public string rejectReason { get; set; }
       // public string generatedDate { get; set; }
      
    }
}