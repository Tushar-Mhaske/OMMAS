using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Proposal
{
    public class PACKAGEINFO
    {
        //public PACKAGEINFO()
        //{
        //    this.PACKAGES.ROADINFO = new List<ROADDETAIL>();
        //}
        public List<PACKAGES> PACKAGES1 { get; set; }
    }

    public class PACKAGES
    {
        public string PACKAGE_REF_NO { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public int ACTUAL_ORGID { get; set; }
        public string STATE_NAME { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string DPIU_NAME { get; set; }
        public string CREATOR_NAME { get; set; }
        public string CREATOR_REFERENCE { get; set; }

        public List<ROADDETAIL> ROADINFO { get; set; }
    }

    

    public class ROADDETAIL
    {
        //public string PACKAGE_NUMBER { get; set; }
        public int ROAD_CODE  { get; set; }
        public string ROAD_NAME { get; set; }
        public decimal ROAD_LENGTH { get; set; }
        public int BLOCK_CODE { get; set; }
        public string BLOCK_NAME { get; set; }
        public decimal TOTAL_COST { get; set; }
    }
}