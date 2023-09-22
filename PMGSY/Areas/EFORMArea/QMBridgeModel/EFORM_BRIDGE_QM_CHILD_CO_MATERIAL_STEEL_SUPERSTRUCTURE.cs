using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table:  Please enter Location/ Item of Work of row ")]
        [StringLength(20, ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: The length must be 20 character or less for Location/ Item of Work of row ")]
        public string RD_LOC_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please enter Grade of Steel of row ")]
        [StringLength(20, ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: The length must be 20 character or less for Grade of Steel of row ")]
        public string STEEL_GRADE_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please select Welding-Presence of Lamellar Tearing (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Maximum three character is allowed in Welding-Presence of Lamellar Tearing (Yes/ No) ")]
        public string IS_LAMELLAR_TRAINING_20 { get; set; } //


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please select Welding-Presence of Fatigue Cracking (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Maximum three character is allowed in Welding-Presence of Fatigue Cracking (Yes/ No) of row ")]
        public string IS_FATIGUE_CRACKING_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please select Material-Presence of Corrosion (Yes/ No)-If Yes, type of corrosion and specific location-whether Presence of Corrosion  of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Maximum three character is allowed in Material-If Yes, type of corrosion and specific location-Presence of Corrosion  of row ")]
        public string IS_PRESENCE_CORROSION_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please enter Material-If Yes, type of corrosion and specific location-Atmosphere Corrosion of row ")]
        [StringLength(20, ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: The length must be 20 character or less for Material-If Yes, type of corrosion and specific location-Atmosphere Corrosion of row ")]
        public string ATMOSPHERE_CORROSION_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please enter Material-If Yes, type of corrosion and specific location-Chemical Corrosion of row ")]
        [StringLength(20, ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: The length must be 20 character or less for Material-If Yes, type of corrosion and specific location-Chemical Corrosion of row ")]
        public string CHEMICAL_CORROSION_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please enter Material-If Yes, type of corrosion and specific location-Stress Corrosion of row ")]
        [StringLength(20, ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: The length must be 20 character or less for Material-If Yes, type of corrosion and specific location-Stress Corrosion of row ")]
        public string STRESS_CORROSION_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please select Material-Presence of Brittle Fracture (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Maximum three character is allowed in Material-Presence of Brittle Fracture (Yes/ No) of row ")]
        public string IS_PRESENCE_BRITTLE_FRACTURE_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please select Presence of Buckling (Yes/ No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Maximum three character is allowed in Presence of Buckling (Yes/ No) of row ")]
        public string IS_PRESENCE_BUCKLING_20 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Please select Any change in load classification (Yes / No) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-20: Item 6. (ii)Steel Superstructure-B)Completed Work-Table: Maximum three character is allowed in Any change in load classification (Yes / No) of row ")]
        public string IS_CHANGE_LOAD_CLASSIFICATION_20 { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SuperStructureType { get; set; }
    }
}