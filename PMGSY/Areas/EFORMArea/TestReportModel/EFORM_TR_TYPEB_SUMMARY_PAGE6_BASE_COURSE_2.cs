using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_SUMMARY_PAGE6_BASE_COURSE_2
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEB_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-6: BASE COURSE: 1st Layer: Please select Base Course 1st Layer.")]
        [RegularExpression(pattern: @"^([8]|[9]|[1][0]){0,2}$", ErrorMessage = "Page-6: BASE COURSE: 1st Layer: only number{8/9/10} allowed in Base Course 1st Layer.")]
        public int Base_Course_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Please enter Chainage for Chainage-II")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates - Please Enter Valid number{cant be greater than 9999999.999} in Chainage-II")]
        public Nullable<decimal> CHAINAGE_ID_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: -Please enter Weight of Sample Taken (gm) for Chainage-II")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates - Please Enter Valid number{cant be greater than 999999999999.999} in Weight of Sample Taken (gm) in Chainage-II")]
        public Nullable<decimal> WEIGHT_SAMPLE_TAKEN_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: -Please select Date of Testing for Chainage-II")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates - Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing in Chainage-II")]
        public Nullable<System.DateTime> TESTING_DATE_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: -Please enter Grading Type for Chainage-II")]
        [StringLength(50, ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates -The length must be 50 character or less for GSB Grading in Chainage-II")]
        public string GSB_GRADING_6 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates:- Remark is required for Chainage-II. Please click on Calculate button.")]
        [StringLength(2000, ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates -The length must be 2000 character or less for Remark in Chainage-II")]
        public string REMARK_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates:  -Please enter Comment for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates -The length must be 2000 character or less for Comment in Chainage-II")]
        public string COMMENT_6_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates:  -Please enter Tested By: (Name of Head of PIU) for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates -The length must be 2000 character or less for Tested By: (Name of Head of PIU) in Chainage-II")]
        public string TESTED_BY_PIU { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates:  -Please enter Test conducted in my presence for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates -The length must be 2000 character or less for Test conducted in my presence in Chainage-II")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-6: Item 4.2. BASE COURSE: 1st Layer - Gradation Analysis of Aggregates:- Confirms to the prescribed limit is required for Chainage-II. Please click on Calculate button.")]
        [StringLength(3, ErrorMessage = "Page-6: Item 4.2 BASE COURSE: 1st Layer - Gradation Analysis of Aggregates -The length must be 3 character or less for confirms to the prescribed limit in Chainage-II")]
        public string CON_PRESCRIBED_LIMIT_6_2 { get; set; }
    }
}