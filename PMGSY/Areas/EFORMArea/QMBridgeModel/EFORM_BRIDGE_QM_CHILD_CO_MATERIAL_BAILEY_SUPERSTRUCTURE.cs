using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please enter Location/ Item of Work of row ")]
        [StringLength(20, ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: The length must be 20 character or less for Location/ Item of Work of row ")]
        public string RD_LOC_22 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please enter Type of Bailey Bridge (Single - Single, Single - Double, Double - Double etc.) of row ")]
        [StringLength(20, ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: The length must be 20 character or less for Type of Bailey Bridge (Single - Single, Single - Double, Double - Double etc.) of row ")]
        public string TYPES_OF_BAILY_BRIDGE_22 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please select Welding-Presence of Lamellar Tearing (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Maximum three character is allowed in Welding-Presence of Lamellar Tearing (Yes/ No) of row ")]
        public string IS_LAMELLAR_TRAINING_22 { get; set; }  // 


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please select Welding-Presence of Fatigue Cracking (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Maximum three character is allowed in Welding-Presence of Fatigue Cracking (Yes/ No) of row ")]
        public string IS_FATIGUE_CRACKING_22 { get; set; } // 

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please select Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location- of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Maximum three character is allowed in Material-Presence of Corrosion of row ")]
        public string IS_PRESENCE_CORROSION_22 { get; set; } 


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please enter Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location-Atmospheric Corrosion of row ")]
        [StringLength(20, ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: The length must be 20 character or less for Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location-Atmospheric Corrosion of row ")]
        public string ATMOSPHERE_CORROSION_22 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please enter Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location-Chemical Corrosion of row ")]
        [StringLength(20, ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: The length must be 20 character or less for Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location-Chemical Corrosion of row ")]
        public string CHEMICAL_CORROSION_22 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please enter Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location-Stress Corrosion of row ")]
        [StringLength(20, ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: The length must be 20 character or less for Material-Presence of Corrosion(Yes / No)If Yes, type of corrosion and specific location-Stress Corrosion of row ")]
        public string STRESS_CORROSION_22 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please select Material-All panel PINs are placed in male-female joints properly with safety pin (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Maximum three character is allowed in Material-All panel PINs are placed in male-female joints properly with safety pin (Yes/ No) of row ")]
        public string IS_PANEL_PINS_PLACED_PROPERLY_22 { get; set; } // IS_PANEL_PINS_PLACED_PROPERLY_22


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Please select Any change in load classification (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work-Table: Maximum three character is allowed in Any change in load classification (Yes / No) of row ")]
        public string IS_CHANGE_IN_LOAD_CLASSIFICATION_22 { get; set; }  // IS_CHANGE_IN_LOAD_CLASSIFICATION_22



        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SuperStructureType { get; set; }

    }
}