
namespace PMGSY.Models
{
    using System;
    using System.Collections.Generic;

    public partial class AdditionalRoadDetailsViewModel
    {

        // public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_YEAR { get; set; }
        public int IMS_BATCH { get; set; }
        public string IMS_PACKAGE_ID { get; set; }
        public string IMS_ROAD_NAME { get; set; }
        public decimal IMS_PAV_LENGTH { get; set; }
        public decimal IMS_SANCTIONED_PAV_AMT { get; set; }
        public decimal IMS_SANCTIONED_CD_AMT { get; set; }
        public decimal IMS_SANCTIONED_OW_AMT { get; set; }
        public Nullable<decimal> IMS_SANCTIONED_HS_AMT { get; set; }
        public Nullable<decimal> IMS_SANCTIONED_FC_AMT { get; set; }
        public decimal IMS_SANCTIONED_BW_AMT { get; set; }
        public decimal IMS_SANCTIONED_RS_AMT { get; set; }
        public decimal IMS_SANCTIONED_BS_AMT { get; set; }
        public Nullable<System.DateTime> WORK_COMPLETION_DATE { get; set; }
    }
}
