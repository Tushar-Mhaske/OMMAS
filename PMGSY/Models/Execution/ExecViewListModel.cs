using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Execution
{
    public class ExecViewListModel
    {
       
         public string  MAST_BLOCK_NAME;
        public string IMS_ROAD_NAME;
         public string IMS_PACKAGE_ID;
         public int  IMS_PR_ROAD_CODE;
       public int IMS_NO_OF_CDWORKS;
      public int IMS_NO_OF_BRIDGEWRKS;
       public string IMS_PROPOSAL_TYPE;
       public string IMS_LOCK_STATUS;
       public decimal ROAD_COST;
        public decimal ROAD_LENGTH;
        public decimal MAINTENANCE_COST;
         public int IMS_BATCH;
         public string IMS_BRIDGE_NAME;
         public string MAST_YEAR_TEXT;
         public string EXEC_ISCOMPLETED;
         public bool flag;
         public int status;

    }
}