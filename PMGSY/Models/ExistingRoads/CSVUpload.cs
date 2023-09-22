using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ExistingRoads
{
    public class CSVUpload
    {
        public int NumberOfFiles { get; set; }

        public int PLAN_CN_ROAD_CODE { get; set; }
        public int PLAN_FILE_ID { get; set; }

        public string fileName { get; set; }
    }
}