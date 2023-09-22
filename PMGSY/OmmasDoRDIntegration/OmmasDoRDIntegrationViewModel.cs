using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.OmmasDoRDIntegration
{
    public class OmmasDoRDIntegrationViewModel
    {
        //public int ommasStateCode { get; set; }
        public int lgdStateCode { get; set; }
        public string stateName { get; set; }

        //public int ommasDistrictCode { get; set; }
        public int lgdDistrictCode { get; set; }
        public string districtName { get; set; }

        //public int ommasBlockCode { get; set; }
        public int lgdBlockCode { get; set; }
        public string blockName { get; set; }

        public int habCode { get; set; }
        public string habName { get; set; }
        public int habTotPopulation { get; set; }
    }

    public class OmmasDoRDSQCDetailsViewModel
    {
        public string stateName { get; set; }
        public string sqcName { get; set; }
        public string designation { get; set; }
        public string address { get; set; }
        public string mobileNo { get; set; }
        public string email { get; set; }
    }

    public class OmmasDoRDHabStatewiseViewModel
    {
        public int lgdStateCode { get; set; }
        public string stateName { get; set; }

        public int lgdDistrictCode { get; set; }
        public string districtName { get; set; }

        public int lgdBlockCode { get; set; }
        public string blockName { get; set; }

        public int habCode { get; set; }
        public string habName { get; set; }

        public int habPopulation { get; set; }
        public int habSCSTPopulation { get; set; }
    }

    public class OmmasDoRDHabDetailsStateViewModel
    {
        public int roadId { get; set; }
        public int habCode { get; set; }
        public string scheme { get; set; }
        public string connectivity { get; set; }
        public int completionStage { get; set; }
        public string stage { get; set; }
        public string piu { get; set; }
        public string sanctionYear { get; set; }

        public int lgdStateCode { get; set; }
        public int lgdDistrictCode { get; set; }
        public int lgdBlockCode { get; set; }

        public string identification { get; set; }

        public decimal sanctionLength { get; set; }
        public decimal estimatedCost { get; set; }
        public decimal actualCost { get; set; }
        public decimal BTType { get; set; }
        public decimal CCType { get; set; }
        public decimal completedLength { get; set; }

        public string projectEndDate { get; set; }
    }
}