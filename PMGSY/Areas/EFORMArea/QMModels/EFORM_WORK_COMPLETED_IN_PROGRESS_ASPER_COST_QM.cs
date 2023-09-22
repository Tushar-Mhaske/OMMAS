using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM
    {

        public EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM(string STATUScheck)
        {

            this.WORK_STATUS_CHECK = STATUScheck;


        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_STATUS_CHECK { get; set; }



        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int STATUS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }





        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C. Please select Whether the work is completed within the sanctioned cost ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  Maximum one character is allowed in Whether the work is completed within the sanctioned cost ")]
        public string WORK_STATUS_32 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [DeficiencyStatusDependable]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  Please enter  Sanctioned cost (in lakh )")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  Please Enter Valid number{cant be greater than 9999999999999999.99} in Sanctioned cost (in lakh ) ")]
        public Nullable<decimal> SANCTION_COST_32 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [DeficiencyStatusDependable]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  Please enter Completion cost (in lakh ) ")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  Please Enter Valid number{cant be greater than 9999999999999999.99} in Completion cost (in lakh ) ")]
        public Nullable<decimal> COMPLETION_COST_32 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [DeficiencyStatusDependable]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  Please enter Reason for extra cost ")]
        [StringLength(250, ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  The length must be 250 character or less for Reason for extra cost")]
        public string REASON_FOR_EXTRA_32 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C. Maximum one character is allowed in The revised DPR has been prepared ")]

        [DeficiencyStatusDependable]
        public string IS_REVISED_DPR_PREPARED_32 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C. Maximum one character is allowed in Change in scope of work approved by competent authority ")]

        [DeficiencyStatusDependable]
        public string IS_CHANGED_SCOPEOFWORK_32 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C. Maximum one character is allowed in Variation in quantites approved by competent authority ")]

        [DeficiencyStatusDependable]
        public string IS_VARIATION_IN_QTY_32 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C. Maximum one character is allowed in Expenditure for additional cost approved by competent authority ")]

        [DeficiencyStatusDependable]
        public string IS_COST_APPROVED_32 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C. Maximum one character is allowed in Other: (Please describe) ")]

        [DeficiencyStatusDependable]
        public string IS_OTHER_32 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [DeficiencyStatusDependable]
        [StringLength(250, ErrorMessage = "Page-32: ITEM 21. General Observations of QM- C.  The length must be 250 character or less for other action")]
        public string OTHER_DESCRIBTION_32 { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }



    }
}