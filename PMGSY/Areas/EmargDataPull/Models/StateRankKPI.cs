using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EmargDataPull.Models
{
    public class StateRankKPI
    {
        public string stateName { get; set; }
        public int stateId { get; set; }
        public int packageCount { get; set; }
        public int workableCount { get; set; }
        public int verifiedCount { get; set; }
        public int freezedCount { get; set; }
        public int unfreezedCount { get; set; }
        public int lockedCount { get; set; }
        public int manualExpenditureCount { get; set; }
        public int completedCount { get; set; }
        public int invalidCount { get; set; }
        public int incorrectCount { get; set; }
        public int paymentStartedCount { get; set; }
        public int eligibleContractorCount { get; set; }
        public int registeredContractorCount { get; set; }
        public int districtCount { get; set; }
        public int dscEnrolledUserCount { get; set; }
    }
}