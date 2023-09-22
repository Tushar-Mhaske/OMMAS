using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_DEGREE_OF_COMPAQ
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DEGREE_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int COMPAQ_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter Location (RD) of row  ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-12 Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row  ")]
        public decimal LOCATION_RD_12 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter Dry density kN/m3 (As per QCR-I) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Dry density kN/m3 (As per QCR-I) of row ")]
        public decimal QCR1_DRY_DENSITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter % Compaction (As per QCR-I) of row ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum 999.99 is allowed and maximum two digits after decimal are allowed in % Compaction (As per QCR-I) of row ")]
        public decimal QCR1_PERCENT_COMPAQ { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter Date of test as per QCR-I of row ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-12:  Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please Enter Valid date{in dd/mm/yyyy format} in Date of test as per QCR-I ")]
        public System.DateTime QCR1_DATE_OF_TEST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter Field Moisture Content (%) for Degree of Compaction (Measured by QM) of row  ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum 999.99 is allowed and maximum two digits after decimal are allowed Field Moisture Content (%) for Degree of Compaction (Measured by QM) of row ")]
        public decimal MEAS_FMC { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter Dry Density kN/m3 for Degree of Compaction (Measured by QM) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Dry Density kN/m3 for Degree of Compaction (Measured by QM) of row ")]
        public decimal MEAS_DRY_DENSITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please enter % Compaction for Degree of Compaction (Measured by QM) of row ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum 999.99 is allowed and maximum two digits after decimal are allowed in % Compaction for Degree of Compaction (Measured by QM) of row ")]
        public decimal MEAS_PERCENT_COMPAQ { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Please select Grade (S / U) for Degree of Compaction (Measured by QM) of row ")]
        [StringLength(1, ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Compaction Details Table- Maximum 1 charater is allowed in Grade (S / U) for Degree of Compaction (Measured by QM) of row ")]
        public string MEAS_GRADE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}