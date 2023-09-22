using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEC_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: BITUMINOUS SURFACE COURSE: Please select Bituminous Surface Course.")]
        [RegularExpression(pattern: @"^([3][0]|[3][1]|[3][2]|[3][3]|[3][4]){0,2}$", ErrorMessage = "Page-17: BITUMINOUS SURFACE COURSE: only number{30/31/32/33/34} allowed in Bituminous Surface Course.")]
        public int Bituminous_Surface_Course { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE:  - Gradation Analysis of Aggregates  Please enter Chainage for Chainage-III")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates   Please Enter Valid number{cant be greater than 9999999.999} for Chainage-III")]
        public Nullable<decimal> CHAINAGE_ID_18_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  -Please enter Weight of Sample Taken (gm) for Chainage-III")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates   Please Enter Valid number{cant be greater than 999999999999.999} in Weight of Sample Taken (gm) for Chainage-III")]
        public Nullable<decimal> WEIGHT_SAMPLE_TAKEN_18_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  -Please select gradation type for Chainage-III")]
        [RegularExpression(pattern: @"^(1|2|3|4|4|5|6|7|8|9|10)$", ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates: only (1-10) numbers allowed in  gradation type for Chainage-III")]
        public Nullable<int> GRADATION_TYPE_ITEM_ID_18_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  -Please select Date of Testing for Chainage-III")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates   Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing for Chainage-III")]
        public Nullable<System.DateTime> TESTING_DATE_18_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates -Remark is required for Chainage-III. Please click on Calculate button.")]
        [StringLength(2000, ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates -The length must be 2000 character or less for Remark for Chainage-III")]
        public string REMARK_18_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates -Please enter Comment for Chainage-III")]
        [StringLength(2000, ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  The length must be 2000 character or less for Comment for Chainage-III")]
        public string COMMENT_18_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  -Please enter Tested By: (Name of Head of PIU) for Chainage-III")]
        [StringLength(2000, ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  The length must be 2000 character or less for Tested By: (Name of Head of PIU) for Chainage-III")]
        public string TESTED_BY_PIU { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  -Please enter Test conducted in my presence for Chainage-III")]
        [StringLength(2000, ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE: - Gradation Analysis of Aggregates  The length must be 2000 character or less for Test conducted in my presence for Chainage-III")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-18: Item 18.3. BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates -Confirms to the prescribed limit is required for Chainage-III. Please click on Calculate button.")]
        [StringLength(3, ErrorMessage = "Page-18: Item 18.3 BITUMINOUS SURFACE COURSE:- Gradation Analysis of Aggregates  The length must be 3 character or less for confirms to the prescribed limit for Chainage-III")]
        public string CON_PRESCRIBED_LIMIT_18_1 { get; set; }



    }
}