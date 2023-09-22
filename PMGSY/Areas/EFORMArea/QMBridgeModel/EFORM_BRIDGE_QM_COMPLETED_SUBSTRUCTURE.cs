using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_COMPLETED_SUBSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)- I. Please select Whether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE- I. Maximum three character is allowed inWhether Cement and Steel tests other than provided by supplier, got conducted byindependent laboratories")]
        public string IS_CONDUCTED_INDEP_LAB_16 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE (B)- II. Please select In order to assess the quality of concrete in case of doubt, if any Non Destructive Test (NDT)such as Rebound hammer test, Ultra Sonic Pulse Velocity (UPV) etc. has been conducted ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE- II. Maximum three character is allowed in In order to assess the quality of concrete in case of doubt, if any Non Destructive Test (NDT)such as Rebound hammer test, Ultra Sonic Pulse Velocity (UPV) etc. has been conducted ")]
        public string IS_NDT_COMDUCTED_16 { get; set; }


    }
}