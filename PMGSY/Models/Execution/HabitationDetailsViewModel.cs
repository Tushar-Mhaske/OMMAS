using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Execution
{
    public class HabitationDetailsViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }

        public string EncryptedPhysicalRoadCode { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public double MaintananceCost { get; set; }

        public decimal Sanction_length { get; set; }

        public string CompleteStatus { get; set; }

        public string AgreementDate { get; set; }

        public int Year { get; set; }

        public decimal? AgreementCost { get; set; }

        public string SanctionYear { get; set; }

        public decimal changedLength { get; set; }

        public string EncryptedHabCodes { get; set; }
    }
}