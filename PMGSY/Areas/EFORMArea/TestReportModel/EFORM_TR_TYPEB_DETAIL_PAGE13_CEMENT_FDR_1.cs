﻿using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_1
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


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.1.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table -Please enter Wt. of Sample Retained(g) for Chainage-I")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-13: Item 14.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table- Please Enter Valid number{cant be greater than 999999999999.999} in Wt. of Sample Retained(g) for chainage I")]
        public Nullable<decimal> SAMPLE_WEIGHT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.1.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table - Percent of Wt. Retained(g) is required for Chainage-I")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-13: Item 14.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table- Please Enter Valid number{cant be greater than 999.99} in Percent of Wt. Retained(g) for chainage I")]
        public Nullable<decimal> RETAINED_WEIGHT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.1.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table - Cumulative Percent of Wt. Retained(%) is required for Chainage-I")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-13: Item 14.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table- Please Enter Valid number{cant be greater than 999.99} in Cumulative Percent of Wt. Retained(%) for chainage I")]
        public Nullable<decimal> CUMULATIVE_WEIGHT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.1.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table - Percentage of Wt.Passing(%) is required for Chainage-I")]
        [RegularExpression(pattern: @"^-?(?:\d{0,3}\.\d{1,2})$|^-?\d{0,3}$", ErrorMessage = "Page-13: Item 14.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table- Please Enter Valid number{cant be greater than 999.99} in Percentage of Wt.Passing(%) for chainage I")]
        public Nullable<decimal> PASSING_WEIGHT { get; set; }


    }
}