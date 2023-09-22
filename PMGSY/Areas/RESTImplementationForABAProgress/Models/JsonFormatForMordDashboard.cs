using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RESTImplementationForABAProgress.Models
{
    public class JsonFormatForMordDashboard
    {
        public bool status { get; set; }
        public List<PMGSY.Models.USP_MORD_DASHBOAED_PHYSICAL_Result> Result { get; set; }

    }
    public class JsonFormatForMordDashboardPMGSY3
    {
        public bool status { get; set; }
        public List<PMGSY.Models.USP_MORD_DASHBOAED_PHYSICAL_PMGSY3_Result> Result { get; set; }

    }


    public class JsonFormatForStateRank
    {
        public bool status { get; set; }
        public List<PMGSY.Models.USP_STATE_RANK_OVERALLRANK_CALCULATION_Result> Result { get; set; }

    }



}