using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.ProgressReport.Models
{
    public class ExecutionMonitoringDetails
    {
        public int ProposalCode { get; set; }

        public bool IsLatLongAvailable { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string State { set; get; }
        public string District { set; get; }
        public string Package { set; get; }
        public string SanctionYear { set; get; }
        public string RoadName { set; get; }
        public string Type { set; get; }
        public string RoadStatus { set; get; }
        public string RoadLength { set; get; }
        public string StartChainage { set; get; }
        public string EndChainage { set; get; }
        public string BridgeName { set; get; }
        public string BridgeLength { set; get; }

        public int LabCode { get; set; }
        public string labEstablishDate { set; get; }
    }
}