using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PMGSY.Areas.EFORMArea.Model;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DETAIL_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: EARTHWORK / SUBGRADE: Core cutter method:Table: CH 1: Please enter ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: EARTHWORK / SUBGRADE: Core cutter method:Table: CH 1:  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in  ")]

        public Nullable<decimal> CH1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: EARTHWORK / SUBGRADE: Core cutter method:Table: CH 2: Please enter ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: EARTHWORK / SUBGRADE: Core cutter method:Table: CH 2:  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in ")]

        public Nullable<decimal> CH2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-1: EARTHWORK / SUBGRADE: Core cutter method:Table: CH 3: Please enter ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: EARTHWORK / SUBGRADE: Core cutter method:Table: CH 3:  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in ")]

        public Nullable<decimal> CH3 { get; set; }
    }
}