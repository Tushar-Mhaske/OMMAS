using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_SUMMARY_PAGE21_SHOULDER_2
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
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)  Please enter Chainage for Chainage-II")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)   Please Enter Valid number{cant be greater than 9999999.999} for Chainage-II")]
        public Nullable<decimal> CHAINAGE_ID_21_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)  -Please enter Weight of Sample Taken (gm) for Chainage-II")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)   Please Enter Valid number{cant be greater than 999999999999.999} in Weight of Sample Taken (gm) for Chainage-II")]
        public Nullable<decimal> WEIGHT_SAMPLE_TAKEN_21_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)  -Please select Date of Testing for Chainage-II")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)   Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing for Chainage-II")]
        public Nullable<System.DateTime> TESTING_DATE_21_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)  -Please select GSB Grading for Chainage-II")]
        [RegularExpression(pattern: @"^([1]|[2]){0,1}$", ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel):-Please select GSB Grading for Chainage-II.")]
        [StringLength(50, ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)  The length must be 50 character or less for GSB Grading for Chainage-II")]
        public string GSB_GRADING_21_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)   -Remark is required for Chainage-II. Please click on Calculate button.")]
        [StringLength(2000, ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)  The length must be 2000 character or less for Remark for Chainage-II")]
        public string REMARK_21_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)   -Please enter Comment for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)  The length must be 2000 character or less for Comment for Chainage-II")]
        public string COMMENT_21_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)   -Please enter Tested By: (Name of Head of PIU) for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)  The length must be 2000 character or less for Tested By: (Name of Head of PIU) for Chainage-II")]
        public string TESTED_BY_PIU { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)   -Please enter Test conducted in my presence for Chainage-II")]
        [StringLength(2000, ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)  The length must be 2000 character or less for Test conducted in my presence for Chainage-II")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 21.2. SHOULDER: - GSB (Gravel)   -Confirms to the prescribed limit is required for Chainage-II. Please click on Calculate button.")]
        [StringLength(3, ErrorMessage = "Page-21: Item 21.2 SHOULDER: - GSB (Gravel)  The length must be 3 character or less for confirms to the prescribed limit for Chainage-II")]
        public string CON_PRESCRIBED_LIMIT_21_2 { get; set; }



    }
}