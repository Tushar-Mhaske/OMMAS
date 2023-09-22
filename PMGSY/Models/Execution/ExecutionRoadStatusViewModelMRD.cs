using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Execution
{
    public class ExecutionRoadStatusViewModelMRD 
    {

        public int PLAN_CN_ROAD_CODE { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select Cluster.")]
        public List<SelectListItem> clusterList { get; set; }
        public int clusterCode { get; set; }

        [RegularExpression("[CH]", ErrorMessage = "Please Select Cluster or Habitation")]
        public string Cluster_Habitation { get; set; }

        //For Multiselect
        public List<SelectListItem> HABITATIONS { get; set; }
        public int HABITATION_LIST { get; set; }
        public string ASSIGNED_HABITATION_LIST { get; set; }

        public string EncryptedHabCodes { get; set; }

        public string EncryptedRoadCode { get; set; }

        public int IMS_PR_ROAD_CODE { get; set; }

        public string EncryptedPhysicalRoadCode { get; set; }

        [Required(ErrorMessage = "Please select year.")]
        [Range(2000, 2099, ErrorMessage = "Please select year.")]
        [Display(Name = "Year")]
        //[CompareAgrementYear("AgreementYear", "AgreementDate", ErrorMessage = "Year must be greater than or equal to Agreement Year")]
        public int EXEC_PROG_YEAR { get; set; }

        [Required(ErrorMessage = "Please select month.")]
        [Range(1, 12, ErrorMessage = "Please select month.")]
        [Display(Name = "Month")]
        //[CompareAgrementMonth("EXEC_PROG_YEAR", "AgreementDate", ErrorMessage = "Month must be greater than or equal to Agreement Month")]
        public int EXEC_PROG_MONTH { get; set; }

        [Display(Name = "Preparatory Work/Setting out and Earth Work Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Cutoff/Raft/Individual footing,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Preparatory Work,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Preparatory Work,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousPreparatoryWork", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Preparatory work length must be greater than or equal to previous preparatory work length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED", "Operation", ErrorMessage = "Preparatory Work Length should not be zero.")]
        public Nullable<decimal> EXEC_PREPARATORY_WORK { get; set; }

        [Display(Name = "Earthwork Subgrade Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Subgrade Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Subgrade Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Subgrade Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousEarthWork", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Earthwork subgrade length must be greater than or equal to previous Earthwork subgrade length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Subgrade Stage Length should not be zero.")]
        public Nullable<decimal> EXEC_EARTHWORK_SUBGRADE { get; set; }

        [Display(Name = "Subbase/GSB Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Subbase/GSB Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Subbase/GSB Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Subbase/GSB Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousSubbase", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Subbase stage length must be greater than or equal to previous Subbase stage length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Subbase Stage Length should not be zero.")]
        public Nullable<decimal> EXEC_SUBBASE_PREPRATION { get; set; }

        [Display(Name = "Base Course /G2-G3 Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Basecourse Length,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Basecourse Length,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Basecourse Length,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousBaseCourse", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Base Course length must be greater than or equal to previous Base Course work length and less than Sanction Length.")]
        //[CompareLengthValues("EXEC_ISCOMPLETED", "Operation",ErrorMessage = "Base Course Stage Length should not be zero.")]
        public Nullable<decimal> EXEC_BASE_COURSE { get; set; }

        [Display(Name = "Completed(Length in Km.)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Completed Length,Can only contains Numeric values and 3 digit after decimal place")]
        //[CompareIsStage("EXEC_EARTHWORK_SUBGRADE", "IsStage", ErrorMessage = "Road is a Stage construction so value should be equal to value of Earthwork Subgrade Stage.")]
        //[CompareNoStage("EXEC_SURFACE_COURSE", "IsStage", ErrorMessage = "Value should be equal to value of Surface Course.")]
        ////[CompareRoadLength("changed_SanctionedLength", "EXEC_ISCOMPLETED", ErrorMessage = "On complete status road length completed must be between (sanction length + 10 %) and (sanction length - 10%)")]
        //[CompareRoadLength("changed_SanctionedLength", "EXEC_ISCOMPLETED", ErrorMessage = "On complete status road length completed must be upto 10 % of sanction length")]
        ////[ComparePreviousCompletedLength("EXEC_PROG_MONTH", "EXEC_PROG_YEAR", "IMS_PR_ROAD_CODE", ErrorMessage = "Completed Length must be greater than previous completed length.")]
        //[ComparePreviousLength("PreviousCompletedLength", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Completed length must be greater than or equal to previous completed length and less than Sanction Length.")]
        public Nullable<decimal> EXEC_COMPLETED { get; set; }

        [Display(Name = "Surface Course/BT Stage (Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Surface Course/BT Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[Range(0, 9999.999, ErrorMessage = "Invalid Surface Course/BT Stage,Can only contains Numeric values and 3 digit after decimal place")]
        [Range(0, 90.000, ErrorMessage = "Invalid Surface Course/BT Stage,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousSurfaceCourse", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Surface Course length must be greater than or equal to previous Surface Course work length and less than Sanction Length.")]
        ////[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Surface Course Length should not be zero.")]
        public Nullable<decimal> EXEC_SURFACE_COURSE { get; set; }

        [Display(Name = "Road Signs Stones (Nos.)")]
        [Range(0, 9999, ErrorMessage = "Invalid Value.")]
        //[ComparePreviousValue("PreviousRoadSigns","Operation", ErrorMessage = "Road Signs Stones must be greater than or equal to previous Road Signs Stones values.")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        public Nullable<decimal> EXEC_SIGNS_STONES { get; set; }

        [Display(Name = "Cross Drainage Works(Nos.)")]
        [Range(0, 9999, ErrorMessage = "Invalid Value.")]
        //[ComparePreviousValue("PreviousCDWorks","Operation", ErrorMessage = "Cross Drainage Works must be greater than or equal to previous Cross Drainage Works values.")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        public Nullable<decimal> EXEC_CD_WORKS { get; set; }

        [Display(Name = "Long Span Bridges(Nos.)")]
        [Range(0, 9999, ErrorMessage = "Invalid Value.")]
        //[ComparePreviousValue("PreviousLSB","Operation", ErrorMessage = "Long Span Bridges must be greater than or equal to previous Long Span Bridges values.")]
        [RegularExpression(@"\d+$", ErrorMessage = "Please enter valid value.")]
        public Nullable<decimal> EXEC_LSB_WORKS { get; set; }

        [Display(Name = "Miscellaneous(Length in Km)")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,3})?\s*$", ErrorMessage = "Invalid Miscellaneous length,Can only contains Numeric values and 3 digit after decimal place")]
        //[ComparePreviousLength("PreviousMiscellaneous", "EXEC_ISCOMPLETED", "changed_SanctionedLength", ErrorMessage = "Miscellaneous length must be greater than or equal to previous Miscellaneous length and less than Sanction Length.")]
        //[CompareMiscellaneousLength("changed_SanctionedLength", ErrorMessage = "Miscellaneous length should not exceed sanction length.")]
        ////[CompareLengthValues("EXEC_ISCOMPLETED","Operation", ErrorMessage = "Miscellaneous Length should not be zero.")]
        [Range(0, 90.000, ErrorMessage = "Invalid Miscelaneous Length,Can only contains Numeric values and 3 digit after decimal place")]
        public Nullable<decimal> EXEC_MISCELANEOUS { get; set; }

        [Required(ErrorMessage = "Please select work status.")]
        [Display(Name = "Work Status")]
        //[RegularExpression(@"^([CPFAL]+)$", ErrorMessage = "Please select work status.")]
        //[ComparePreviousStatus("CompleteStatus", "Operation", ErrorMessage = "Work status is already completed.")]
        ////[CompareStatus("EXEC_ISCOMPLETED",ErrorMessage="Progress can not be updated for this status.")]
        [IsSplitWork("IMS_PR_ROAD_CODE", ErrorMessage = "The agreement or work is incomplete against this road or The agreement has not been made against this road, You can not select the status Complete.")]
        public string EXEC_ISCOMPLETED { get; set; }

        public string Operation { get; set; }

        public Nullable<decimal> OldCompleted { get; set; }

        public string BlockName { get; set; }

        public string Package { get; set; }

        public int RoadNo { get; set; }

        public string RoadName { get; set; }

        public double Sanction_Cost { get; set; }

        public double MaintananceCost { get; set; }

        //[CompareLengthRoad("EXEC_COMPLETED", "EXEC_ISCOMPLETED", ErrorMessage = "Sanctioned Length is less than Completed Length.")]
        public decimal Sanction_length { get; set; }

        public string CompleteStatus { get; set; }

        //[CompareMonth("EXEC_PROG_MONTH", "EXEC_PROG_YEAR", "PreviousYear", "Operation", ErrorMessage = "Month and Year must be greater than previous entered month and year")]
        public int PreviousMonth { get; set; }

        public int PreviousYear { get; set; }

        public decimal? PreviousPreparatoryWork { get; set; }

        public decimal? PreviousEarthWork { get; set; }

        public decimal? PreviousSubbase { get; set; }

        public decimal? PreviousBaseCourse { get; set; }

        public decimal? PreviousSurfaceCourse { get; set; }

        public decimal? PreviousMiscellaneous { get; set; }

        public int PreviousCDWorks { get; set; }

        public int PreviousLSB { get; set; }

        public int PreviousRoadSigns { get; set; }

        public decimal? PreviousCompletedLength { get; set; }

        public string IsStage { get; set; }

        public int AgreementYear { get; set; }

        public int AgreementMonth { get; set; }

        public string AgreementDate { get; set; }

        public int Year { get; set; }

        public decimal? AgreementCost { get; set; }

        [Display(Name = "Completion Date")]
        //[CompareCompletionDate("EXEC_PROG_YEAR", "EXEC_PROG_MONTH", "EXEC_ISCOMPLETED", ErrorMessage = "The completion date must match the selected Month and Year.")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Completion Date must be in dd/mm/yyyy format.")]
        public string ExecutionCompleteDate { get; set; }

        public decimal changedLength { get; set; }

        public decimal changed_SanctionedLength { get; set; }

        public int crYear { get; set; }

        public string currmonthName { get; set; }
        public string prevmonthName { get; set; }

        public string SanctionYear { get; set; }

        //added by pradip on 9/03/2017

        public string mappedHabitaionDate { get; set; }
    }

}