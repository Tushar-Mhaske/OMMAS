using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_GENERAL_INFO_PIU
    {

        public EFORM_GENERAL_INFO_PIU(bool RoadStatusIsCompleted)
        {
            this.TemplateStatus = RoadStatusIsCompleted;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string checksum { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL VIII. Please Enter Valid number{cant be greater than 9999.99} in Flexible pavement ")]
        public Nullable<decimal> SAN_FLEX_PAVEMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL VIII. Please Enter Valid number{cant be greater than 9999.99} in Rigid/Semi-Rigid pavement ")]
        public Nullable<decimal> SAN_RIGID_PAVEMENT { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal EXEC_LENGTH { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL IX. Please Enter Valid number{cant be greater than 9999.99} in Flexible pavement  ")]
        public Nullable<decimal> EXEC_FLEX_PAVEMENT { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL IX. Please Enter Valid number{cant be greater than 9999.99} in Rigid/Semi-Rigid pavement  ")]
        public Nullable<decimal> EXEC_RIGID_PAVEMENT { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-1: Item 1. GENERAL X. The length must be 250 character or less for Reasons for deviation")]
        public string DEVIATION_REASON { get; set; }






        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal ESTIMATED_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XIII. Please Enter Valid number{cant be greater than 9999999999999999.99} in Technical sanction cost  ")]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XIII. Please enter Technical sanction cost   ")]
        public decimal TECHNICAL_SANC_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal AWARDED_COST { get; set; }





        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item Please enter ")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XV. a. Please Enter Valid number{cant be greater than 9999999999999999.99} in Expenditure done ")]
        [RoadStatusDependable]
        public decimal EXPENDITURE_DONE { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XV. b. Please enter Bills Pending")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XV. b. Please Enter Valid number{cant be greater than 9999999999999999.99} in  Bills Pending ")]
        [RoadStatusDependable]
        public decimal BILLS_PENDING { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XV. Please enter Total expenditure (a+b) ")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XV. Please Enter Valid number{cant be greater than 9999999999999999.99} in Total expenditure (a+b) ")]
        [RoadStatusDependable]
        public decimal TOTAL_EXPENDITURE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XVI. Please enter Completion cost")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XVI. Please Enter Valid number{cant be greater than 9999999999999999.99} in Completion cost ")]
       

        public decimal COMPLETION_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. Please select work type")]
        public string WORK_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. Please enter Total length for")]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1:  Item 1. GENERAL XVII.  Please Enter Valid number{cant be greater than 9999.99} in Total length ")]
        public decimal? TOTAL_LEN { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. A. Please select (i) Carriageway width (m)")]
        public string CARRIAGEWAY_WIDTH_NEW { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. B. Please select (i) Carriageway width and length type")]
        public string CARRIAGEWAY_WIDTH_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. B. Please enter Existing width for (i) Carriageway width and length type-With widening")]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL XVII. B) (i) Please Enter Valid number{cant be greater than 9999.99} in existing width ")]
        public string CARRIAGEWAY_WIDTH_EXISTING { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. B. Please enter Proposed width for (i) Carriageway width and length type-")]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL XVII. B) (i) Please Enter Valid number{cant be greater than 9999.99} in Proposed width ")]
        public string CARRIAGEWAY_WIDTH_PROPOSED { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2:  Item 1. GENERAL XVII. B. Please enter Length for (i) Carriageway width and length type-")]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-1: Item 1. GENERAL XVII. B) (i) Please Enter Valid number{cant be greater than 9999.99} in Length ")]
        public string CARRIAGEWAY_LENGTH { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string TERRAIN_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
     
        public string AWARD_OF_WORK_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
      
        public string START_OF_WORK_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
    

        public string STIPULATED_COMPLETION_DATE { get; set; }




        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 1. GENERAL XXII. Please enter Actual date of completion")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-2: Item 1. GENERAL XXII. Please Enter Valid date{in dd/mm/yyyy format} in Actual date of completion")]
        
        public string ACTUAL_COMPLETION_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4: Item 4. DETAILS OF MIX DESIGN -Please select 'Yes' or 'No' Checkbox")]
        public string MIX_DESIGN_APPLICABLE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Please enter Name and Designation of the Head of PIU")]
        public string PIU_HEAD_NAME { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Please enter Mobile Number of the Head of PIU")]
        public string PIU_HEAD_MOBILE_NO { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string PIU_HEAD_EMAIL { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
       [Required(ErrorMessage = "Page-5: Please enter Address of the PIU ")]
        public string PIU_ADDR { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Please enter Date")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-5: Please Enter Valid date{in dd/mm/yyyy format} in  ")]

        public string PIU_SIGN_DATE { get; set; }
    }
}