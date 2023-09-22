using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace PMGSY.Models.Proposal
{
    public class ShiftProposalModel
    {

        public int StateCode { get; set; }
        public int BlockCode { get; set; }
        public int DistrictCode { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string BlockName { get; set; }
    }
}