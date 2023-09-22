using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.OmmasDoRDIntegration
{
    public class OmmasDoRDHabitationDetailsViewModel
    {
        public int roadCode { get; set; }
        public int habCode { get; set; }
        public int pmgsyScheme { get; set; }
        public string connectivity { get; set; }
        public int completionStage { get; set; }
        public string stage { get; set; }
        public string piu { get; set; }
        public string sanctionYear { get; set; }

        public int lgdStateCode { get; set; }
        public string stateName { get; set; }

        public int lgdDistrictCode { get; set; }
        public string districtName { get; set; }

        public int lgdBlockCode { get; set; }
        public string blockName { get; set; }

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