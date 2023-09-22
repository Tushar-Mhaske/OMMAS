using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.OmmasIntegrations
{
    public class WorksInformationModel
    {
        public int censusStateCode { get; set; }
        //public int censusDistrictCode { get; set; }
        //public int censusBlockCode { get; set; }

        //[Display(Name = "No of road works cleared")]
        public int NoOfRoadWorksClearedScheme { get; set; }

        //[Display(Name = "New connectivity")]
        public int NewConnectivityScheme { get; set; }

        //[Display(Name = "Upgradation")]
        public int UpgradationConnectivityScheme { get; set; }

        //[Display(Name = "Completed road works")]
        public int CompletedRoadWorksScheme { get; set; }

        public int CompletedNewConnectivityScheme { get; set; }

        public int CompletedUpgradationScheme { get; set; }

        //[Display(Name = "In-progress road works")]
        //public int InProgressRoadWorksScheme { get; set; }

        //[Display(Name = "Total length(in Kms)")]
        public decimal TotalLengthScheme { get; set; }

        public decimal TotalSanctionedCost { get; set; }

        public decimal TotalExpenditure { get; set; }
    }
}