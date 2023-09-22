using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEB_DETAIL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DETAIL_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.3.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for Chainage-III")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-6: Item 4.3 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999999999999.999} in Wt. of Sample Retained(g) in Chainage III")]
        public Nullable<decimal> SAMPLE_WEIGHT_6_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.3.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table - Percent of Wt. Retained(g) is required for Chainage-III")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-6: Item 4.3 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999.99} in Percent of Wt. Retained(g) in Chainage III")]
        public Nullable<decimal> RETAINED_WEIGHT_6_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.3.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table - Cumulative Percent of Wt. Retained(%) is required for Chainage-III")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-6: Item 4.3 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999.99} in Cumulative Percent of Wt. Retained(%) in Chainage III")]
        public Nullable<decimal> CUMULATIVE_WEIGHT_6_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.3.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table - Percentage of Wt.Passing(%) is required for Chainage-III")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-6: Item 4.3 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table- Please Enter Valid number{cant be greater than 999.99} in Percentage of Wt.Passing(%) in Chainage III")]
        public Nullable<decimal> PASSING_WEIGHT_6_3 { get; set; }


    }
}