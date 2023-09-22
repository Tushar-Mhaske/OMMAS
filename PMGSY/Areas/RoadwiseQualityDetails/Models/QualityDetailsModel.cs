using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RoadwiseQualityDetails.Models
{
    public class QualityDetailsModel
    {
        public QualityDetailsModel() {

            QualityDetails = new List<USP_CITIZEN_POST_PROPOSAL_DETAILS_Result>();
        }

        public string ErrorMessage { get; set; }
        public List<USP_CITIZEN_POST_PROPOSAL_DETAILS_Result> QualityDetails { get; set; }
    }

}