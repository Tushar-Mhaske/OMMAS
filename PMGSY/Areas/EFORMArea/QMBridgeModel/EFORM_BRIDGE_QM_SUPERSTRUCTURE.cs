using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE- Please select Structure Type ")]
        [StringLength(250, ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE- The length must be 10 character or less for Structure Type")]
        public string STRUCTURE_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE-RCC Please select Item Grading-6(A) ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE- Maximum three character is allowed in Item Grading-6(A) ")]
        public string ITEM_GRADING_6_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 6. SUPERSTRUCTURE-STEEL Please select Item Grading-6(B) ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-19: Item 6. SUPERSTRUCTURE- Maximum three character is allowed in Item Grading-6(B) ")]
        public string ITEM_GRADING_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 6. SUPERSTRUCTURE-BAILEY Please select Item Grading-6(C) ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-21: Item 6. SUPERSTRUCTURE- Maximum three character is allowed in Item Grading-6(C) ")]
        public string ITEM_GRADING_6_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE-RCC  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE- The length must be 250 character or less for suggestions for improvement")]
        public string IMPROVEMENT_REMARK_6_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 6. SUPERSTRUCTURE-STEEL  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-19: Item 6. SUPERSTRUCTURE- The length must be 250 character or less for suggestions for improvement")]
        public string IMPROVEMENT_REMARK_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 6. SUPERSTRUCTURE-BAILEY  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-21: Item 6. SUPERSTRUCTURE- The length must be 250 character or less for suggestions for improvement")]
        public string IMPROVEMENT_REMARK_6_3 { get; set; }


    }
}