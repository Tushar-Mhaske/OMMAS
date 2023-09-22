using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.OMMASIntegrations
{
    public class MoRDViewModel
    {
    }

    public class ShcemeOverAllStats
    {
        public int NoOfRoadWorksClearedScheme { get; set; }
        public decimal RoadLengthClearedScheme { get; set; }
        public int CompletedRoadWorksScheme { get; set; }
        public decimal TotalLengthCompletedScheme { get; set; }
        public decimal ExpenditureIncurred { get; set; }
    }

    public class SanctionWorksViewModel
    {
        public string StateName { get; set; }

        //public int SANCTION_NUMBER_ROAD { get; set; }
        //public decimal SANCTION_LENGTH_ROAD { get; set; }
        //public int COMPLETED_NUMBER_ROAD { get; set; }
        //public decimal COMPLETED_LENGTH_ROAD { get; set; }
        public int PROGRESS_NUMBER_ROAD { get; set; }
        public decimal PROGRESS_LENGTH_ROAD { get; set; }
        //public int SANCTION_NUMBER_LSB { get; set; }
        //public int COMPLETED_NUMBER_LSB { get; set; }
        public int PROGRESS_NUMBER_LSB { get; set; }

        public int PROGRESS_NUMBER_ROAD_LESS_THEN0 { get; set; }
        public decimal PROGRESS_LENGTH_ROAD_LESS_THEN0 { get; set; }
        public int PROGRESS_NUMBER_ROAD_MORE_THAN1 { get; set; }
        public decimal PROGRESS_LENGTH_ROAD_LESS_THEN1 { get; set; }
        public int PROGRESS_NUMBER_ROAD_MORE_THAN2 { get; set; }
        public decimal PROGRESS_LENGTH_ROAD_LESS_THEN2 { get; set; }
        public int PROGRESS_NUMBER_ROAD_MORE_THAN3 { get; set; }
        public decimal PROGRESS_LENGTH_ROAD_LESS_THEN3 { get; set; }
        public int PROGRESS_NUMBER_ROAD_MORE_THAN4 { get; set; }
        public decimal PROGRESS_LENGTH_ROAD_MORE_THAN4 { get; set; }
        public int PROGRESS_NUMBER_LSB_LESS_THEN0 { get; set; }
        public int PROGRESS_NUMBER_LSB_MORE_THAN1 { get; set; }
        public int PROGRESS_NUMBER_LSB_MORE_THAN2 { get; set; }
        public int PROGRESS_NUMBER_LSB_MORE_THAN3 { get; set; }
        public int PROGRESS_NUMBER_LSB_MORE_THAN4 { get; set; }

        public int LGD_State_Code { get; set; }
    }

    public class HabConnectivityStatusViewModel
    {
        public string StateName { get; set; }
        public int NetEligible { get; set; }
        public int TotalCleared { get; set; }
        public int TotalConnected { get; set; }
        //public int TotalStateConnected { get; set; }
        public int LGD_State_Code { get; set; }

    }

    public class TargetAchievementViewModel
    {
        //public string StateName { get; set; }
        public int Year { get; set; }
        public decimal TargetLength { get; set; }
        public decimal CompletedLength { get; set; }
        public int TargetHabitations { get; set; }
        public int CompletedHabitations { get; set; }
        public decimal Expenditure { get; set; }
        public int LGD_STATE_CODE { get; set; }
    }

    public class PhysicalFinancialProgressViewModel
    {
        //public string StateName { get; set; }
        //public int Year { get; set; }
        public int WorksSanctioned { get; set; }
        public decimal LengthSanctioned { get; set; }
        public int WorksCompleted { get; set; }
        public decimal LengthCompleted { get; set; }
        public decimal Expenditure { get; set; }
        public int StateCode { get; set; } 
    }
}