//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMGSY.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS_New
    {
        public int recordId { get; set; }
        public int stateCode { get; set; }
        public string stateName { get; set; }
        public int districtCode { get; set; }
        public string districtName { get; set; }
        public int blockCode { get; set; }
        public string blockName { get; set; }
        public int piuCode { get; set; }
        public string piuName { get; set; }
        public string packageNo { get; set; }
        public string emargPackageNo { get; set; }
        public int roadCode { get; set; }
        public string roadName { get; set; }
        public decimal ccLength { get; set; }
        public decimal btLength { get; set; }
        public decimal roadWidth { get; set; }
        public string workOrderNo { get; set; }
        public System.DateTime workOrderDate { get; set; }
        public string agreementNo { get; set; }
        public System.DateTime agreementDate { get; set; }
        public string contractorName { get; set; }
        public string contractorPan { get; set; }
    }
}
