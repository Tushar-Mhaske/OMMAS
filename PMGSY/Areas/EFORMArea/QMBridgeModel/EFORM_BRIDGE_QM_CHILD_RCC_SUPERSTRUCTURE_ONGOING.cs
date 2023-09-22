using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_ONGOING
    {


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE-RCC- I. Please select Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-17: Item 6. SUPERSTRUCTURE-RCC- I. Maximum three character is allowed in Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories ")]
        public string IS_CEMENT_TEST_CONDUCTED { get; set; }



    
    }
}