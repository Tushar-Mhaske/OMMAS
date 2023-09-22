using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.OverallStatus
{
    public class OverallStatusViewModel
    {

        [Display(Name = "No of road works cleared")]
        public int NoOfRoadWorksCleared { get; set; }

        [Display(Name = "New connectivity")]
        public int NewConnectivityWorks { get; set; }

        [Display(Name = "Upgradation")]
        public int UpgradationConnectivityWorks { get; set; }

        [Display(Name = "Completed road works")]
        public int CompletedRoadWorks { get; set; }

        [Display(Name = "In-progress road works")]
        public int InProgressRoadWorks { get; set; }

        [Display(Name = "Total length(in Kms)")]
        public decimal TotalLength { get; set; }

        public int NoOfComplaints { get; set; }
    }
}