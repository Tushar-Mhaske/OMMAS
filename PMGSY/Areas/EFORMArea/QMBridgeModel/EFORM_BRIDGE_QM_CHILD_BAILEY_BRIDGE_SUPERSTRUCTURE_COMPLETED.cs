using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_COMPLETED
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work: I. Please select Any bolt and/ or rakers and tie plates are missing or loose ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- I. Maximum three character is allowed in Any bolt and/ or rakers and tie plates are missing or loose ")]
        public string IS_BOLT_PLATES_MISSING { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- II. Please select Any sway braces and/ or transom clamps are missing or loose")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- II. Maximum three character is allowed in Any sway braces and/ or transom clamps are missing or loose ")]
        public string IS_SWAY_BRACES_MISSING { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- III. Please select Presence of any cracking in the Bailey bridge ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- III. Maximum three character is allowed in Presence of any cracking in the Bailey bridge ")]
        public string IS_ANY_CRACK_IN_BAILEY_BRIDGE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- IV. Please select Presence of any bends in bridge members ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-22: Item 6. (iii)Bailey Bridge Superstructure-B)Completed Work:- IV. Maximum three character is allowed in Bend In Presence of any bends in bridge members")]
        public string IS_BEND_IN_BRIDGE_MEMBER { get; set; }

    }
}