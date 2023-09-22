using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.WebServices.eMarg.Model
{
    public class EmargRoadWiseBalanceWorks
    {


        // MODEL UPDATED ON 05-01-2022
        //public int recordId { get; set; }
        //public int stateCode { get; set; }
        //public string stateName { get; set; }
        //public int districtCode { get; set; }
        //public string districtName { get; set; }
        //public int blockCode { get; set; }
        //public string blockName { get; set; }
        //public int piuCode { get; set; }
        //public string piuName { get; set; }
        //public string packageNo { get; set; }
        //public string emargPackageNo { get; set; }
        //public int roadCode { get; set; }
        //public string roadName { get; set; }
        //public decimal ccLength { get; set; }
        //public decimal btLength { get; set; }
        //public decimal roadWidth { get; set; }
        //public string workOrderNo { get; set; }
        //public System.DateTime workOrderDate { get; set; }
        //public string agreementNo { get; set; }
        //public System.DateTime agreementDate { get; set; }
        //public string contractorName { get; set; }
        //public string contractorPan { get; set; }

        //Updated on 07-04-2022

        public int RECORD_ID { get; set; }
        public int STATE_CODE { get; set; }
        public int DISTRICT_CODE { get; set; }
        public int BLOCK_CODE { get; set; }
        public int ROAD_CODE { get; set; }
        public int PIU_CODE { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string E_MARG_PACKAGE_NUMBER { get; set; }
        public string CONTRACTOR_PAN { get; set; }
        public string CONTRACTOR_NAME { get; set; }
        public string AGREEMENT_NUMBER { get; set; }
        public System.DateTime AGREEMENT_DATE { get; set; }
        public System.DateTime MAINTENANCE_START_DATE { get; set; }
        public System.DateTime MAINTENANCE_END_DATE { get; set; }

    }
}