using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_GENERAL_INFO
    {



        public EFORM_BRIDGE_PIU_GENERAL_INFO(bool RoadStatusIsCompleted)
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
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Item 1. GENERAL VII. Please Enter Valid number{cant be greater than 9999999.999} in Deviation in length of the bridge ")]
       
        public Nullable<decimal> DEVIATION_LENGTH { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-1: Item 1. GENERAL VIII. The length must be 250 character or less for Reasons for deviation")]
        public string DEVIATION_REASON { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public decimal ESTIMATED_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL X. Please Enter Valid number{cant be greater than 9999999999999999.99} in Technical sanction cost  ")]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL X. Please enter Technical sanction cost   ")]
        public decimal TECHNICAL_SANC_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public decimal AWARDED_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XII. a. Please enter Expenditure done")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XII. a. Please Enter Valid number{cant be greater than 9999999999999999.99} in Expenditure done ")]
        [RoadStatusDependable]
        public decimal EXPENDITURE_DONE { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XII. b. Please enter Bills Pending")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XII. b. Please Enter Valid number{cant be greater than 9999999999999999.99} in  Bills Pending ")]
        [RoadStatusDependable]
        public decimal BILLS_PENDING { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XII. Please enter Total expenditure (a+b) ")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XII. Please Enter Valid number{cant be greater than 9999999999999999.99} in Total expenditure (a+b) ")]
        [RoadStatusDependable]
        public decimal TOTAL_EXPENDITURE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: Item 1. GENERAL XIII. Please enter Completion cost")]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-1: Item 1. GENERAL XIII. Please Enter Valid number{cant be greater than 9999999999999999.99} in Completion cost ")]
        public decimal COMPLETION_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string AWARD_OF_WORK_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string START_OF_WORK_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string STIPULATED_COMPLETION_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 1. GENERAL XVII. Please enter Actual date of completion")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-2: Item 1. GENERAL XVII. Please Enter Valid date{in dd/mm/yyyy format} in Actual date of completion")]
        public string ACTUAL_COMPLETION_DATE { get; set; }

      

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Please enter Name and Designation of the Head of PIU")]
        [StringLength(150, ErrorMessage = "Page-8: The length must be 150 character or less for Name and Designation of the Head of PIU")]

        public string PIU_HEAD_NAME { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Please enter Mobile Number of the Head of PIU")]
        public string PIU_HEAD_MOBILE_NO { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string PIU_HEAD_EMAIL { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Please enter Address of the PIU ")]
        [StringLength(250, ErrorMessage = "Page-8: The length must be 250 character or less for Address of the PIU")]

        public string PIU_ADDR { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Please enter Date")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-8: Please Enter Valid date{in dd/mm/yyyy format} in  ")]
        public string PIU_SIGN_DATE { get; set; }
    }
}