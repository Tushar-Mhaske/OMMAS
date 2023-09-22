using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Location/ Item of work of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Location/ Item of work of row ")]
        public string RD_LOC_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Mix design / Grade of Concrete of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Mix design / Grade of Concrete of row ")]
        public string CONCRETE_GRADE_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Compressive strength of concrete cubes as per Quality Control Test of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Compressive strength of concrete cubes as per Quality Control Test of row ")]
        public string CONCRETE_STRENGTH_PER_QUALITY_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Testing of concrete cubes by NQM  of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Testingof concrete cubes by NQM  of row ")]
        public string TEST_CONCRETE_CUBE_QM_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Surface Acceptability of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Surface Acceptability of row ")]
        public string SURFACE_ACCEPTABLITY_18 { get; set; }  // SURFACE_ACCEPTABLITY_18


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Size & Shape as per design of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Size & Shape as per design of row ")]
        public string DESIGN_SIZE_SHAPE_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Camber of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Camber of row ")]
        public string CAMBER_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Bearing of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Bearing of row ")]
        public string BEARING_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Kerbs of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Kerbs of row ")]
        public string KERBS_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Crash Barrier of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Crash Barrier of row ")]
        public string CRASH_BARRIER_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Wearing Coat of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Wearing Coat of row ")]
        public string WEARING_COAT_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Railing of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Railing of row ")]
        public string RAILING_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Expansion Joints of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Expansion Joints of row ")]
        public string EXP_JOINTS_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Drainage Spout of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Drainage Spout of row ")]
        public string DRAINAGE_SPOUTS_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Approach Slab of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Approach Slab of row ")]
        public string APPROACH_SLAB_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Stopper of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Stopper of row ")]
        public string STOPPER_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please enter Protection Work of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Protection Work of row ")]
        public string PROTECTION_WORK_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: Please select Awarded Grade of row ")]
        [StringLength(20, ErrorMessage = "Page-18: Item 6. (i)RCC Superstructure-B)Completed work-Table: The length must be 20 character or less for Awarded Grade of row ")]
        public string AWARDED_GRADE_18 { get; set; }





        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SuperStructureType { get; set; }


    }
}