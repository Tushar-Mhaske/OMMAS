using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PMGSY.Models;
using System.Web.Mvc;

namespace PMGSY.Models.QualityMonitoring
{
    public class ListForUploadedInspectionByNRIDA
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public string MAST_BLOCK_NAME { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string MAST_YEAR_TEXT { get; set; }
        public int MAST_PMGSY_SCHEME { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public string IMS_BRIDGE_NAME { get; set; }
        public string IMS_PROPOSAL_TYPE { get; set; }
        public decimal? ROAD_LENGTH { get; set; }
        public decimal? ROAD_COST { get; set; }
        public decimal? MAINTENANCE_COST { get; set; }
    }
}