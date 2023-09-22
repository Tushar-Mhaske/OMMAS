using PMGSY.DAL.ExistingRoads;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.ExistingRoads
{
    public class HabitationDetailsViewModel
    {
        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }
        public string EncryptedHabCodes { get; set; }

        public string RoadName { get; set; }
        public string RoadNumber { get; set; }

        [Range(1, 2147483647, ErrorMessage = " Please select Reason.")] 
        public int MAST_NOHABS_REASON { get; set; }

        public int MAST_ER_ROAD_CODE { get; set; }

        public string IsBenifitedHabitation { get; set; }
       
       
    }
}