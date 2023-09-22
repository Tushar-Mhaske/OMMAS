using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_GENERAL_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 1. GENERAL DETAILS- I. Please select Date of inspection")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-6: Item 1. GENERAL DETAILS- I. Please Enter Valid date{in dd/mm/yyyy format} in  ")]

        public string INSPECTION_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 1. GENERAL DETAILS- VI. Please enter RD of inspection: From RD")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-6: Item 1. GENERAL DETAILS- VI. Please Enter Valid number{cant be greater than 9999.99} in RD of inspection: From RD ")]

        public decimal ROAD_FROM { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 1. GENERAL DETAILS- VI. Please enter RD of inspection: to RD")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-6: Item 1. GENERAL DETAILS- VI. Please Enter Valid number{cant be greater than 9999.99} in RD of inspection: to RD ")]

        public decimal ROAD_TO { get; set; }
    }
}