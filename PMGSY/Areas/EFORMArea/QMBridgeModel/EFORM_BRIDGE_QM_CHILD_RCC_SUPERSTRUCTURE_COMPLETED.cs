using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_COMPLETED
    {


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-18: Item 6. SUPERSTRUCTURE-RCC- I. Please select Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-18: Item 6. SUPERSTRUCTURE-RCC- I. Maximum three character is allowed in Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories ")]
        public string IS_CEMENT_TEST_CONDUCTED_18 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-18: Item 6. SUPERSTRUCTURE-RCC- II. Please select In order to assess the quality of concrete in case of doubt, Non Destructive Test (NDT) suchas Rebound hammer test, Ultra Sonic Pulse Velocity (UPV) etc. has been conducted if any")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-18: Item 6. SUPERSTRUCTURE-RCC- II. Maximum three character is allowed in In order to assess the quality of concrete in case of doubt, Non Destructive Test (NDT) suchas Rebound hammer test, Ultra Sonic Pulse Velocity (UPV) etc. has been conducted if any ")]
        public string IS_NDT_CONDUCTED_18 { get; set; }


    }
}