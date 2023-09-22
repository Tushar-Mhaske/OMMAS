using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEC_DETAIL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Detail item id is required for Chainage-I.Please contact to ommas team")]
        [RegularExpression(pattern: @"^[0-9]{0,4}$", ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates: Table-only number allowed in Detail item id for Chainage-I.Please contact to ommas team")]
        public int DETAIL_ITEM_ID_17_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Please enter Sieve Designation for Chainage-I")]
        [StringLength(150, ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates: Table-  The length must be 150 character or less for Sieve Designation for Chainage-I")]
        public string SIEVE_DESIGNATION_17_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for Chainage-I")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999999999999.999} in Wt. of Sample Retained(g) for Chainage I")]
        public Nullable<decimal> SAMPLE_WEIGHT_17_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table - Percent of Wt. Retained(g) is required for Chainage-I")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999.99} in Percent of Wt. Retained(g) for Chainage I")]
        public Nullable<decimal> RETAINED_WEIGHT_17_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table - Cumulative Percent of Wt. Retained(%) is required for Chainage-I")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999.99} in Cumulative Percent of Wt. Retained(%) for Chainage I")]
        public Nullable<decimal> CUMULATIVE_WEIGHT_17_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table - Percentage of Wt.Passing(%) is required for Chainage-I")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999.99} in Percentage of Wt.Passing(%) for Chainage I")]
        public Nullable<decimal> PASSING_WEIGHT_17_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Please enter Permissible Range for Chainage-I")]
        [StringLength(150, ErrorMessage = "Page-17: Item 18.1 BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates: Table  The length must be 150 character or less for Permissible range for Chainage-I")]
        public string PERMISSIBLE_RANGE_17_1 { get; set; }


    }
}