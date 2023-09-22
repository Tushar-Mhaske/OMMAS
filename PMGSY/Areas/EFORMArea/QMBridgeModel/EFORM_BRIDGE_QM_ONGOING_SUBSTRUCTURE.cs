using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_ONGOING_SUBSTRUCTURE
    {
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE- I. Please select Whether granular/ sandy filling behind the abutments and returns done properly ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE- I. Maximum three character is allowed in Whether granular/ sandy filling behind the abutments and returns done properly ")]
        public string IS_GRANULAR_SANDY_DONE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE- II. Please select Whether marking with paint is done on bridge components with date of casting for concrete ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE- II. Maximum three character is allowed in Whether marking with paint is done on bridge components with date of casting for concrete ")]
        public string IS_PAINT_MARKING_DONE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE- III. Please select Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-15: Item 5. SUBSTRUCTURE- III. Maximum three character is allowed in Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories ")]
        public string IS_CONDUCTED_INDEP_LAB { get; set; }




    }
}