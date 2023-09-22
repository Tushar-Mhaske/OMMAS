using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int LOC_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int FOND_ON_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- Please enter Location/ Item of work of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- The length must be 20 character or less for Location/ Item of work of row ")]
        public string WORK_ITEM_LOC_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- Please enter Grade of concrete of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- The length must be 20 character or less for Grade of concrete of row ")]
        public string CONCRETE_GRADE_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- Please enter Cement value of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- The length must be 20 character or less for Cement value of row ")]
        public string QM_CEMENT_QCR_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- (A)-Quality of material table: Please enter Quality of material is adequate as per quality control register-Coarse Aggregate  of row")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION (A)-Quality of material table: The length must be 20 character or less for Quality of material is adequate as per quality control register-Coarse Aggregate  of row ")]
        public string QM_COARSE_AGG_QCR_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:-(A)-Quality of material table: Please enter Quality of material is adequate as per quality control register-Fine Aggregate of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION (A)-Quality of material table: The length must be 20 character or less for Quality of material is adequate as per quality control register-Fine Aggregate of row ")]
        public string QM_FINE_AGG_QCR_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing - Quality of material Table- Please enter for Steel of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- The length must be 20 character or less for Steel of row ")]
        public string QM_STEEL_QCR_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing - Quality of material Table- Please enter for Testing of concrete cubes by NQM of row ")]
        [StringLength(20, ErrorMessage = "Page-13: Item 4. FOUNDATION- A) Ongoing - Quality of material Table- The length must be 20 character or less for Testing of concrete cubes by NQM of row ")]
        public string CONCRETE_TEST_NQM_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing - Quality of material Table- Please select Grading of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing - Quality of material Table- Maximum one character is allowed in Grading of row ")]
        public string GRADE_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int USER_ID { get; set; }
    }
}