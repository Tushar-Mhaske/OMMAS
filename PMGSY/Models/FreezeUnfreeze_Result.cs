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
    
    public partial class FreezeUnfreeze_Result
    {
        public string State { get; set; }
        public string District { get; set; }
        public string Block { get; set; }
        public string SANCTION_YEAR { get; set; }
        public string Package_ID { get; set; }
        public string Scheme { get; set; }
        public string Work_Type { get; set; }
        public string Road_Name { get; set; }
        public string Bridge_Name { get; set; }
        public decimal Sanctioned_Length { get; set; }
        public decimal Bridge_Length { get; set; }
        public Nullable<int> Is_Awarded { get; set; }
        public Nullable<System.DateTime> DATE_of_AGREEMENT { get; set; }
        public decimal Preparatory_Work { get; set; }
        public decimal Earthwork_Subgrade { get; set; }
        public decimal Base_Course { get; set; }
        public decimal Subbase_Preparation { get; set; }
        public decimal Surface_Course { get; set; }
        public decimal Completed_Length { get; set; }
        public Nullable<decimal> Sum_of_all_courses { get; set; }
        public string PHYSICAL_PROGRESS_STATUS { get; set; }
        public Nullable<decimal> Expenditure_till_date__Lakhs_ { get; set; }
        public Nullable<int> PHYSICAL_PROGRESS_ENTERED { get; set; }
    }
}
