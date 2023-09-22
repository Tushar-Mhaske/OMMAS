using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EmargDataPush
{
    public class RoadDetails
    {
        //public string STATE_NAME { get; set; }
        //public string DISTRICT_NAME { get; set; }

        //public string PACKAGE_ID { get; set; }

        //public string CONTRACTOR_ID { get; set; }

        //public string PAN_NUMBER { get; set; }
        //public string AGREEMENT_NUMBER { get; set; }
        //public string ROAD_CODE { get; set; }
        //public string WORK_NAME { get; set; }
        //public string COMPLETION_DATE { get; set; }

        //public string SANCTIONED_LENGTH { get; set; }

        //public string COMPLETED_LENGTH { get; set; }
        //public string CARRIAGEWAY_WIDTH { get; set; }

        //public string TRAFFIC_NAME { get; set; }



        //  --,EMARG_ID
        // -- ,PMGSY_SCHEME


      public string EID { get; set; }
 
      public string MAST_STATE_CODE { get; set; }
      public string MAST_STATE_NAME { get; set; }
   
      public string MAST_DISTRICT_CODE { get; set; }
      public string MAST_DISTRICT_NAME { get; set; }
      public string MAST_BLOCK_CODE { get; set; }
      public string MAST_BLOCK_NAME { get; set; }

      public string PIU_CODE { get; set; }
      public string PIU_NAME { get; set; }

      public string ROAD_CODE { get; set; }
      public string SANCTION_YEAR { get; set; }

      public string SANCTION_BATCH { get; set; }
      public string PACKAGE_NO { get; set; }
      public string ROAD_NAME { get; set; }

      public string SANCTION_LENGTH { get; set; }
      public string COMPLETED_LENGTH { get; set; }
      public string CC_LENGTH { get; set; }
      public string BT_LENGTH { get; set; }
      public string CDWorks { get; set; }
      public string CN_CODE { get; set; }
      public string TRAFFIC_CATEGORY { get; set; }
      public string CARRIAGE_WAY_WIDTH { get; set; }
    // -- ,STAGE
      public string COMPLETION_DATE { get; set; }
      public string WORK_ORDER_NO { get; set; }
      public string WORK_ORDER_DATE { get; set; }
      public string CONTRACTOR_NAME { get; set; }
      public string CONTRACTOR_PAN { get; set; }
      public string CONTRACTOR_ID { get; set; }

  

    }
}