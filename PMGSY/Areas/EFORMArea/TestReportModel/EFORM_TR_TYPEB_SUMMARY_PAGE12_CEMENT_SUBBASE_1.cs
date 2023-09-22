﻿using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEB_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:  Please enter Chainage for Chainage-I")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase Please Enter Valid number{cant be greater than 9999999.999} for Chainage-I")]
        public Nullable<decimal> CHAINAGE_ID_12_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:  -Please enter Weight of Sample Taken (gm) for Chainage-I")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase Please Enter Valid number{cant be greater than 999999999999.999} in Weight of Sample Taken (gm) for Chainage-I")]
        public Nullable<decimal> WEIGHT_SAMPLE_TAKEN_12_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:  -Please select Date of Testing for Chainage-I")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing for Chainage-I")]
        public Nullable<System.DateTime> TESTING_DATE_12_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:  -Please enter Material for Chainage-I")]
        [StringLength(50, ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised SubbaseThe length must be 50 character or less for GSB Grading for Chainage-I")]
        public string GSB_GRADING_12_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:   -Remark is required for Chainage-I. Please click on Calculate button.")]
        [StringLength(2000, ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised SubbaseThe length must be 2000 character or less for Remark for Chainage-I")]
        public string REMARK_12_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:   -Please enter Comment for Chainage-I")]
        [StringLength(2000, ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised SubbaseThe length must be 2000 character or less for Comment for Chainage-I")]
        public string COMMENT_12_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:   -Please enter Tested By: (Name of Head of PIU) for Chainage-I")]
        [StringLength(2000, ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised SubbaseThe length must be 2000 character or less for Tested By: (Name of Head of PIU) for Chainage-I")]
        public string TESTED_BY_PIU { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:   -Please enter Test conducted in my presence for Chainage-I")]
        [StringLength(2000, ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised SubbaseThe length must be 2000 character or less for Test conducted in my presence for Chainage-I")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 13.1. BASE COURSE: 3rd Layer - Cement Stabilised Subbase:   -Confirms to the prescribed limit is required for Chainage-I. Please click on Calculate button.")]
        [StringLength(3, ErrorMessage = "Page-12: Item 13.1 BASE COURSE: 3rd Layer - Cement Stabilised SubbaseThe length must be 3 character or less for confirms to the prescribed limit for Chainage-I")]
        public string CON_PRESCRIBED_LIMIT_12_1 { get; set; }

    }
}