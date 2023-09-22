using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_PRGS_DETAILS
    {

            [FieldType(PropertyType = PDFFieldType.Skip)]
            public int PROGRESS_ID { get; set; }

            [FieldType(PropertyType = PDFFieldType.Skip)]
            public int EFORM_ID { get; set; }

            [FieldType(PropertyType = PDFFieldType.Skip)]
            public int ADMIN_ND_CODE { get; set; }
            [FieldType(PropertyType = PDFFieldType.Skip)]
            public int USER_ID { get; set; }
            [FieldType(PropertyType = PDFFieldType.Skip)]
            public int PR_ROAD_CODE { get; set; }

            [FieldType(PropertyType = PDFFieldType.Skip)]
            public string IPADD { get; set; }


            // changed by rohit on 04-08-2022 
            [FieldType(PropertyType = PDFFieldType.Skip)]
            public int ITEM_ID { get; set; }


            [FieldType(PropertyType = PDFFieldType.TextBox)]
            [Required(ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please enter Unit(length /number) for ")]
            [StringLength(15, ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- The length must be 15 character or less for Unit(length /number) for ")]

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
            [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-2: Item 2. PHYSICAL PROGRESS- Please Enter Valid number{cant be greater than 999.99} in Completed percentage of item for ")]
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