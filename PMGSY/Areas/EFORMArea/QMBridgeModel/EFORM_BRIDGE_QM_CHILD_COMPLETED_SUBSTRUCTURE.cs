using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Location/ Item of work of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Location/ Item of work of row ")]
        public string WORK_ITEM_LOC_16 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Grade of concrete of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Grade of concrete of row ")]
        public string CONCRETE_GRADE_16 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Compressive strength of Concrete cubes as per Quality Control Test Register  of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Compressive strength of Concrete cubes as per Quality Control Test Register of row ")]
        public string COMP_STRENGTH_CONCRETE_QCR_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
     //   [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Concrete Test by NQM ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Testing of concrete cubes by NQM (if any)  of row ")]
        public string CONCRETE_TEST_NQM_16 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Surface acceptability and workmanship of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Surface acceptability and workmanship of row ")]
        public string SURFACE_ACCEPTABLITY_WK_16 { get; set; } // SURFACE_ACCEPTABILITY_WK_16

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Size & Shape as per design of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Size & Shape as per design of row ")]
        public string DESIGN_SIZE_SHAPE_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Quality assessment by personal judgment of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Quality assessment by personal judgment of row ")]
        public string QUALITY_ASS_PERSONAL_JUDG_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Weep holes Adequacy (For abutment and return) of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Weep holes Adequacy (For abutment and return)  of row ")]
        public string WEEP_HOLES_ADEQUACY_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Riding Return  of row ")]
        [StringLength(20, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 20 character or less for Riding Return  of row ")]
        public string RIDING_RETURNS_16 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: Please enter Grade(S/U)  of row ")]
        [StringLength(3, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)-Table: The length must be 3 character or less for Grade(S/U) of row ")]
        public string GRADE_16 { get; set; }





        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SubstructureType { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

    }
}