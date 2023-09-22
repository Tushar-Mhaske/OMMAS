using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int LOC_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int FD_CO_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }
        
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Location/ Item of work")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Location/ Item of work ")]
        public string WORK_ITEM_LOC_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Grade of concrete")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Grade of concrete ")]
        public string CONCRETE_GRADE_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Compressive strength of Concrete cubes as per Quality Control Test Register")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Compressive strength of Concrete cubes as per Quality Control Test Register")]
        public string COMP_STRENGTH_CONCRETE_QCR_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Testing of concrete cubes by NQM (if any)")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Testing of concrete cubes by NQM (if any)")]
        public string CONCRETE_TEST_NQM_14 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Surface acceptability and workmanship")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Surface acceptability and workmanship")]
        public string SURFACE_ACCEPTABLITY_WK_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Size and shape as per design")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Size and shape as per design")]
        public string DESIGN_SIZE_SHAPE_14 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- Please enter Quality assessment by personal judgment")]
        [StringLength(20, ErrorMessage = "Page-14: Item 4. FOUNDATION- B) Completed work - Quality of material Table- The length must be 20 character or less for Quality assessment by personal judgment")]
        public string QUALITY_ASS_PERSONAL_JUDG_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION:- B) Completed work - Quality of material Table- Please select Grading")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-14: Item 4. FOUNDATION:- B) Completed work - Quality of material Table- Maximum one character is allowed in Grading")]
        public string GRADE_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int USER_ID { get; set; }

    }
}