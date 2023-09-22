using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.CoreNetwork
{
    public class HabitationDetailsViewModelPMGSY3
    {
        [UIHint("Hidden")]
        public string EncryptedRoadCode { get; set; }
        public string EncryptedHabCodes { get; set; }
        //public int PLAN_CN_ROAD_HAB_ID { get; set; }
        //public int PLAN_CN_ROAD_CODE { get; set; }
        //public int MAST_HAB_CODE { get; set; }


        public int MAST_HAB_CODE { get; set; }
        public int MAST_YEAR { get; set; }
        public int MAST_HAB_TOT_POP { get; set; }
        public int MAST_HAB_SCST_POP { get; set; }
        public string MAST_HAB_CONNECTED { get; set; }
        public string MAST_SCHEME { get; set; }
        public string MAST_PRIMARY_SCHOOL { get; set; }
        public string MAST_MIDDLE_SCHOOL { get; set; }
        public string MAST_HIGH_SCHOOL { get; set; }
        public string MAST_INTERMEDIATE_SCHOOL { get; set; }
        public string MAST_DEGREE_COLLEGE { get; set; }
        public string MAST_HEALTH_SERVICE { get; set; }
        public string MAST_DISPENSARY { get; set; }
        public string MAST_MCW_CENTERS { get; set; }
        public string MAST_PHCS { get; set; }
        public string MAST_VETNARY_HOSPITAL { get; set; }
        public string MAST_TELEGRAPH_OFFICE { get; set; }
        public string MAST_TELEPHONE_CONNECTION { get; set; }
        public string MAST_BUS_SERVICE { get; set; }
        public string MAST_RAILWAY_STATION { get; set; }
        public string MAST_ELECTRICTY { get; set; }
        public string MAST_PANCHAYAT_HQ { get; set; }
        public string MAST_TOURIST_PLACE { get; set; }

        public string UnlockFlag { get; set; }

        public string hdnERRoadCode { get; set; }

        public string stateCode { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid District")]
        public int districtCode { get; set; }
        public List<SelectListItem> lstDistrict { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select a valid Block")]
        public int blockCode { get; set; }
        public int blockName { get; set; }
        public List<SelectListItem> lstBlock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage="Please select a valid Road")]
        public int roadCode { get; set; }
        public List<SelectListItem> lstRoads { get; set; }

        [RegularExpression(@"^[N|Y]+$", ErrorMessage = "Please select valid value")]
        public string habDirect { get; set; }

        public List<SelectListItem> lstHabitations { get; set; }
    }
}