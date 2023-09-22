using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.RESTAPIImplementation.Models
{
    public class MasterDRRPRoadDetailsModel
    {
        //public int DRRP_ID { get; set; }
        public int ER_ROAD_CODE { get; set; }
        //public int STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
        //public int DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        // public int BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public string ER_ROAD_NUMBER { get; set; }
        //public int ROAD_CAT_CODE { get; set; }
        public string ROAD_CAT_NAME { get; set; }
        public string ROAD_NAME { get; set; }
        public decimal ROAD_STR_CHAINAGE { get; set; }
        public decimal ROAD_END_CHAINAGE { get; set; }
        public Nullable<decimal> ROAD_C_WIDTH { get; set; }
        public Nullable<decimal> ROAD_F_WIDTH { get; set; }
        public Nullable<decimal> ROAD_L_WIDTH { get; set; }
        public string ROAD_TYPE { get; set; }
        //public int SOIL_TYPE { get; set; }
        public string SOIL_TYPE_NAME { get; set; }
        public int TERRAIN_TYPE { get; set; }
        //public string TERRAIN_TYPE { get; set; }
        public string TERRAIN_NAME { get; set; }
        public String DRRP_SCHEME { get; set; }
    }
}