using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_OVERALL_GRADING
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK-The length must be 3 character or less for Item No. 2 ")]
        public string QUALITY_ARRANGEMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 3 ")]
        public string ATT_TO_QUALITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 4 ")]
        public string FOUNDATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 5 ")]
        public string SUBSTRUCTURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 6 ")]
        public string SUPERSTRUCTURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 7 ")]
        public string LOAD_TEST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 8 ")]
        public string BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 250 character or less for Item No. 8 A. ")]
        public string BEARING_A { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 8 B. ")]
        public string BEARING_B { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 9 ")]
        public string EXPANSION_JOINTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(500, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 500 character or less for Item No. 9 A. ")]
        public string EXPANSION_JOINTS_A { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 9 B. ")]
        public string EXPANSION_JOINTS_B { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 10 ")]
        public string APPROACHES { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 10->(i) ")]
        public string APPROACH_EMBANKMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 10->(ii) ")]
        public string APPROACH_SUBBASE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 10->(iii)")]
        public string APPROACH_BASECOURSE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 10->(iv) ")]
        public string APPROACH_WEARINGCOURSE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-33: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 10->(v) ")]
        public string APPROACH_PROTECTIONWORK { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 11 ")]
        public string BRIDGE_FURNITURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 11->(i) ")]
        public string BRIDGE_FURNITURE_MAIN_INFO { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 11->(ii) ")]
        public string BRIDGE_FURN_CITIZEN_BOARD { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 11->(iii) ")]
        public string BRIDGE_FURN_GUARD_STONE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(3, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Item No. 11->(iv) ")]
        public string BRIDGE_FURN_CAUTION_SIGNAGE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- Please Select Overall Grading ")]
        [StringLength(3, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 3 character or less for Overall Grading ")]
        public string OVERALL_GRADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(50, ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK- The length must be 50 character or less for Name of QM  ")]
        public string QM_NAME_34 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-34: - 13.OVERALL GRADING OF BRIDGE WORK-   Please Enter Valid date{in dd/mm/yyyy format} in upload date")]

        public Nullable<System.DateTime> UPLOAD_DATE_34 { get; set; }
       
    }
}