using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please enter Location/ Item of Work of row ")]
        [StringLength(20, ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Location/ Item of Work of row ")]
        public string RD_LOC_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please enter Type of Bailey Bridge (Single - Single, Single - Double, Double - Double etc.) of row ")]
        [StringLength(20, ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Type of Bailey Bridge (Single - Single, Single - Double, Double - Double etc.)  of row ")]
        public string TYPES_OF_BAILY_BRIDGE_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please select Quality of Material-All components are available (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Quality of Material-All components are available (Yes/ No) of row ")]
        public string IS_ALL_COMPONENT_AVAILABLE_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please select Quality of Material-All component are corrosion free (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Quality of Material-All component are corrosion free (Yes / No) of row ")]
        public string IS_COMPONENT_CORROSION_FREE_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please select Quality of Material-Launching Roller are available (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Quality of Material-Launching Roller are available (Yes / No) of row ")]
        public string IS_LAUNCHING_ROLLER_AVAILABLE_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please select All panel PINs are placed in male-female joints properly with safety pin (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in All panel PINs are placed in male-female joints properly with safety pin (Yes/ No) of row ")]
        public string IS_PANEL_PINS_PLACED_PROPERLY_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please select Workmanship-Painting is proper and no sign of rust stain (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Workmanship-Painting is proper and no sign of rust stain (Yes / No) of row ")]
        public string IS_PAINTING_PROPER_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Please select Clearance between the lowest point of the bridge and HFL is adequate (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. (iii)Bailey Bridge Superstructure-A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Clearance between the lowest point of the bridge and HFL is adequate (Yes / No) of row ")]
        public string IS_HFL_CLEARANCE_ADEQUATE_21 { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SuperStructureType { get; set; }
    }
}