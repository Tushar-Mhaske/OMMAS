using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_QC_OFFICIAL_DETAILS_PIU
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]

        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string OFFICIAL_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. QUALITY CONTROL-Please enter ")]
        [StringLength(120, ErrorMessage = "Page-3: 3. QUALITY CONTROL- The length must be 120 character or less for ")]
        public string OFFICIAL_NAME { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please enter ")]
        [StringLength(10, ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- The length must be 10 character or less for ")]
        [RegularExpression(pattern: @"([A-Z]){5}([0-9]){4}([A-Z]){1}$", ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please Enter Correct PAN No for ")]
        public string PAN { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3:  Item 3. QUALITY CONTROL-Please enter ")]
        [StringLength(15, ErrorMessage = "Page-3:  Item 3. QUALITY CONTROL-The length must be 15 character or less for ")]
        public string IDENTITY_NUMBER { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please enter ")]
        [StringLength(10, ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- The length must be 10 character or less for ")]
        [RegularExpression(pattern: @"^[6-9]\d{9}$", ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please Enter Correct Mobile number of ")]
        public string MOBILE_NO { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please enter ")]
        [StringLength(50, ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- The length must be 50 character or less for ")]
        [RegularExpression(pattern: @"^[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[a-zA-Z]{2,4}$", ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please Enter Correct E-mail ID. in ")]
        public string EMAIL_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. QUALITY CONTROL- Please enter ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-3: Item 3. QUALITY CONTROL-  Please Enter Valid date{in dd/mm/yyyy format} in ")]


        public string FROM_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. QUALITY CONTROL-  Please enter ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-3: Item 3. QUALITY CONTROL-Please Enter Valid date{in dd/mm/yyyy format} in  ")]

        public string TO_DATE { get; set; }
    }
}