using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class DropProposalModel
    {
        public string State { get; set; }
        public int StateCode { get; set; }
        public int Year { get; set; }
        public int Batch { get; set; }
        public string Collabortion { get; set; }
        public int Stream { get; set; }
        public byte Scheme { get; set; }
        public int Count { get; set; }
        public DateTime ReqDate { get; set; }
        public int ReqCode { get; set; }
        public string Isdropped { get; set; }
        public DateTime? ApproveDate { get; set; }
        public int ImsRoadCode { get; set; }
        public string ProposalType { get; set; }
         
    }

    public class DropOrderDetailModel
    {
        public int Count { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate  { get; set; }
        public int ReqCode { get; set; }
        public int pdfCode { get; set; }
        public int scheme { get; set; }

    }

}