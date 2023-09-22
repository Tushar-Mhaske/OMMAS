using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_QUALITY_GRADING
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS-The length must be 3 character or less for Item No. 2 ")]
        public string I2_QUALITY_ARRANGEMENTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 3 ")]
        public string I3_ATTENTION_QUALITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 4 ")]
        public string I4_FOUNDATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for  Item No. 5")]
        public string I5_SUBSTRUCTURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 6 ")]
        public string I6_SUPERSTRUCTURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 7 ")]
        public string I7_LOAD_TEST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for  Item No. 8B")]
        public string I8_CONDITION_BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 9 ")]
        public string I9_EXPANSION_JOINT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 10 ")]
        public string I10_APPROACHES { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS- The length must be 3 character or less for Item No. 11 ")]
        public string I11_BRIDGE_FURNITURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-35: - 14. QUALITY GRADING OF ITEMS OF BRIDGE WORKS-   Please Enter Valid date{in dd/mm/yyyy format} in upload date")]
        public Nullable<System.DateTime> UPLOAD_DATE_35 { get; set; }

      

    }
}