using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RoadwiseQualityDetails.Models
{
    public class PropsalDetailModel
    {
        public string ErrorMessage { get; set; }

        public List<USP_CS_PROPOSAL_DETAILS_Result> lstIMSProposalDetails { get; set; }
    }
}