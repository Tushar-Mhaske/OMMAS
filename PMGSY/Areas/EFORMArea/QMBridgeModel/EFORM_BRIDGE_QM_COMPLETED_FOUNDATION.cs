using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_COMPLETED_FOUNDATION
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int FOND_CO_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int FOUNDATION_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION:- B) Completed work - I. Whether Cement and Steel tests other than provided by supplier, got conducted by independentlaboratories:")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-14: Item 4. FOUNDATION:- B) Completed work - Maximum one character is allowed in I. Whether Cement and Steel tests other than provided by supplier, got conducted by independentlaboratories:")]
        public string IS_CONDUCTED_INDEP_LAB_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION:- B) Completed work - II. In order to assess the quality of concrete in case of doubt, if any, Non Destructive Test (NDT)such as Rebound hammer test, Ultra Sonic Pulse Velocity (UPV) etc. has been conducted")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-14: Item 4. FOUNDATION:- B) Completed work - II.In order to assess the quality of concrete in case of doubt, if any, Non Destructive Test (NDT)such as Rebound hammer test, Ultra Sonic Pulse Velocity (UPV) etc. has been conducted")]
        public string IS_NDT_CONDUCTED_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int USER_ID { get; set; }

    }
}