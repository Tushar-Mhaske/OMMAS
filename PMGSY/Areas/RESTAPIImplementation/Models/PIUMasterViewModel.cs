using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EmargDataPush.Models
{
    public class PIUMasterViewModel
    {
        public int MAST_STATE_CODE { get; set; }
        public string MAST_STATE_NAME { get; set; }
        public int MAST_DISTRICT_CODE { get; set; }
        public string MAST_DISTRICT_NAME { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int? MAST_PARENT_ND_CODE { get; set; }
        public string ADMIN_ND_NAME { get; set; }
    }
    public class STATEMasterViewModel
    {
        public int STATE_CODE { get; set; }
        public string STATE_NAME { get; set; }
    }
    public class DISTRICTMasterViewModel
    {
        public int STATE_CODE { get; set; }
        public int DISTRICT_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
    }
    public class BLOCKMasterViewModel
    {
        public int DISTRICT_CODE { get; set; }
        public int BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
    }
}