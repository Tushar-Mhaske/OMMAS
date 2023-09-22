using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_PRGS_DETAILS_PIU
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PROGRESS_ID { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DETAIL_ID { get; set; }
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ADMIN_ND_CODE { get; set; }
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PIU_USER_ID { get; set; }
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public short ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please enter Unit(length /number) for ")]
        public string IEM_UNIT { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS-Please enter Quantity As per DPR for ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-2:  Item 2. PHYSICAL PROGRESS-Please Enter Valid number{cant be greater than 999999.99} in Quantity As per DPR for  ")]
        public decimal DPR_QUANTITY { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS-Please enter Quantity Executed for ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid number{cant be greater than 999999.99} in Quantity Executed for ")]
        public decimal EXECUTED_QUANTITY { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please enter Completed percentage of item for ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid number{cant be greater than 99999.99} in Completed percentage of item for ")]
        public decimal COMPLETED_PERC { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please enter Due Start Date for ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid date{in dd/mm/yyyy format} in Due Start Date for ")]

        public string DUE_START_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please enter Due Completion Date for ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid date{in dd/mm/yyyy format} in Due Completion Date for ")]

        public string DUE_END_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-2: Please enter ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid date{in dd/mm/yyyy format} in Actual start Date for ")]

        public string ACTUAL_START_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-2: Please enter ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid date{in dd/mm/yyyy format} in Actual completion Date for ")]

        public string ACTUAL_END_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-2: Please enter ")]
        [RegularExpression(pattern: @"^[0-9]*(?:\.[0-9]*)?$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid input in Delay in months for ")]

       
        public string DELAY_MONTH { get; set; }
    }
}