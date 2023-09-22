using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.MABProgressAndLab
{
    public class FacilityOfflineDetailModel
    {
        //FACILITY CATEGORY
        public string MAST_FACILITY_CATEGORY_ID { get; set; }
        public string MAST_FACILITY_CATEGORY_NAME { get; set; }


        //FACILITY TYPE
        public string MAST_FACILITY_CATEGORY_ID1 { get; set; }
        public string MAST_FACILITY_CATEGORY_NAME1 { get; set; }
        public string MAST_FACILITY_PARENT_ID { get; set; }
        public string MAST_FACILITY_TYPE_ID { get; set; }
        public string MAST_FACILITY_TYPE_NAME { get; set; }


        //DISTRICT
        public string MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }

        //BLOCK
        public string MAST_DISTRICT_CODE1 { get; set; }
        public string MAST_BLOCK_CODE { get; set; }
        public string MAST_BLOCK_NAME { get; set; }

        //HABITATION
        public string MAST_HAB_CODE { get; set; }
        public string MAST_HAB_NAME { get; set; }
    }
}