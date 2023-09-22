using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please enter  Location/ Item of Work of row ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for  Location/ Item of Work of row ")]
        public string RD_LOC_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please enter  Grade of Steel of row ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for  Grade of Steel of row ")]
        public string STEEL_GRADE_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please enter  Tensile strength as per QC of row ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Tensile strength as per QC of row ")]
        public string TENSILE_STRENGTH_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please enter Quality of Material-Yield stress as per QC of row ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of Material-Yield stress as per QC of row ")]
        public string YIELD_STRESS_PER_QC_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please enter Quality of Material-Elongation percentage as per QC of row ")]
        //  [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,2})$|^\d{0,7}$", ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: . Please Enter Valid number{cant be greater than 9999.99} in Elongation Per QC ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of Material-Elongation percentage as per QC of row ")]

        public string ELONGATION_PER_QC_19 { get; set; }  // (5,2)


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please select Quality of Material-Chemical Analysis Done (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Quality of Material-Chemical Analysis Done (Yes / No) of row ")]
        public string IS_CHEMICAL_ANALYSIS_DONE_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please enter Quality of Material-If Chemical Analysis is done, is it adequate of row ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of Material-If Chemical Analysis is done, is it adequate of row ")]
        public string IS_CHEM_ANALYSIS_ADEQUATE_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please select Quality of Material-NDT of welding is done (Yes / No / NA) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Quality of Material-NDT of welding is done (Yes / No / NA) of row ")]
        public string IS_NDT_WELDING_DONE_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please select Quality of Material-Testing of steel bolts or rivets done (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Quality of Material-Testing of steel bolts or rivets done (Yes / No) of row ")]
        public string IS_TESTING_OF_STEEL_BOLT_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please select Workmanship-Painting is proper and no sign of rust stain (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Workmanship-Painting is proper and no sign of rust stain (Yes / No) of row ")]
        public string IS_PAINTING_PROPER_19 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Please select Clearance between the lowest point of the bridge and HFL is adequate (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-19: Item 6. (ii)Steel Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Clearance between the lowest point of the bridge and HFL is adequate (Yes / No) of row ")]
        public string IS_HFL_CLEARANCE_ADEQUATE_19 { get; set; }





        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SuperStructureType { get; set; }
    }
}