﻿using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2
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
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: Please enter Chainage for Chainage-II")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase:  Please Enter Valid number{cant be greater than 9999999.999} for Chainage-II")]
        public Nullable<decimal> CHAINAGE_ID_4_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Please enter Weight of Sample Taken (gm) for Chainage-II")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase:  Please Enter Valid number{cant be greater than 999999999999.999} in Weight of Sample Taken (gm) for Chainage-II")]
        public Nullable<decimal> WEIGHT_SAMPLE_TAKEN_4_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Please select Date of Testing for Chainage-II")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase:  Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing for Chainage-II")]
        public Nullable<System.DateTime> TESTING_DATE_4_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Please enter Material for Chainage-II")]
        [StringLength(50, ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase: The length must be 50 character or less for GSB Grading for Chainage-II")]
        public string GSB_GRADING_4_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Remark is required for Chainage-II. Please click on Calculate button.")]
        [StringLength(2000, ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase: The length must be 2000 character or less for Remark for Chainage-II")]
        public string REMARK_4_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Please enter Comment for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase: The length must be 2000 character or less for Comment for Chainage-II")]
        public string COMMENT_4_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Please enter Tested By: (Name of Head of PIU) for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase: The length must be 2000 character or less for Tested By: (Name of Head of PIU) for Chainage-II")]
        public string TESTED_BY_PIU { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Please enter Test conducted in my presence for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase: The length must be 2000 character or less for Test conducted in my presence for Chainage-II")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4: Item 2.2. SUBBASE-Cement Stabilised Subbase: -Confirms to the prescribed limit is required for Chainage-II. Please click on Calculate button.")]
        [StringLength(3, ErrorMessage = "Page-4: Item 2.2 SUBBASE-Cement Stabilised Subbase: The length must be 3 character or less for confirms to the prescribed limit for Chainage-II")]
        public string CON_PRESCRIBED_LIMIT_4_2 { get; set; }


    }
}