using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Areas.EFORMArea.Model;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_17
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEA_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:Please select WBM G-II Type(A/B)")]
        [RegularExpression(pattern: @"^([1][23]){0,2}$", ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:only number{11/12} allowed in WBM G-II Type(A/B) ")]
        public int Type_7 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:Please enter L")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Maximum seven digits before decimal and maximum three digits after decimal are allowed in L")]

        public Nullable<decimal> L_7_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:Please enter B")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Maximum seven digits before decimal and maximum three digits after decimal are allowed in B")]

        public Nullable<decimal> B_7_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:Please enter H")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Maximum seven digits before decimal and maximum three digits after decimal are allowed in H")]

        public Nullable<decimal> H_7_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:Volume is required")]
        [RegularExpression(pattern: @"^(?:\d{0,24}\.\d{1,3})$|^\d{0,24}$", ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Maximum twenty-four digits before decimal and maximum three digits after decimal are allowed in Volume")]

        public Nullable<decimal> VOL_7_2 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:Please select Date of Testing")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing ")]

        public Nullable<System.DateTime> TESTING_DATE_7_2 { get; set; }

      

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Remark is required.")]
        [StringLength(2000, ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: The length must be 2000 character or less for Remark")]
        public string REMARK_7_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: Please enter comment.")]
        [StringLength(2000, ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: The length must be 2000 character or less for Comment")]

        public string COMMENT_7_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:  Please enter Tested By: (Name of Head of PIU)")]
        [StringLength(2000, ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: The length must be 2000 character or less for Tested By: (Name of Head of PIU)")]

        public string TESTED_BY_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis:  Please enter Test conducted in my presence")]
        [StringLength(3000, ErrorMessage = "Page-7: BASE COURSE: 1st Layer: Volumetric Analysis: The length must be 2000 character or less for Test conducted in my presence")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
    }
}