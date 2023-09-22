using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE19_SRM_44
    {
       

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEA_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }

       
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:Please enter Maximum Dy Density (MDD) as per lab record (gm/cc) for CH1")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-19: SHOULDER: Sand replacement method: Maximum seven digits before decimal and maximum three digits after decimal are allowed in Maximum Dy Density (MDD) as per lab record (gm/cc) for CH1")]

        public Nullable<decimal> MDD_CH1_19_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:Please enter Maximum Dy Density (MDD) as per lab record (gm/cc) for CH2")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-19: SHOULDER: Sand replacement method: Maximum seven digits before decimal and maximum three digits after decimal are allowed in Maximum Dy Density (MDD) as per lab record (gm/cc) for CH2")]

        public Nullable<decimal> MDD_CH2_19_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:Please enter Maximum Dy Density (MDD) as per lab record (gm/cc) for CH3")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-19: SHOULDER: Sand replacement method: Maximum seven digits before decimal and maximum three digits after decimal are allowed in Maximum Dy Density (MDD) as per lab record (gm/cc) for CH3")]

        public Nullable<decimal> MDD_CH3_19_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:Please select Date of Testing")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-19: SHOULDER: Sand replacement method: Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing ")]

        public Nullable<System.DateTime> TESTING_DATE_19_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:Table: b) Determination of soil density: vi) Please select Moisture Content method")]
        [RegularExpression(pattern: @"^([R][M][M]|[S][B][M]){0,3}$", ErrorMessage = "Page-19: SHOULDER: Sand replacement method:Table: b) Determination of soil density: vi) only {RMM/SBM} are allowed in Moisture Content method ")]

        public string MOISTURE_CONTENT_METHOD_19_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method: Remark is required.")]
        [StringLength(2000, ErrorMessage = "Page-19: SHOULDER: Sand replacement method: The length must be 2000 character or less for Remark")]
        public string REMARK_19_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method: Please enter comment.")]
        [StringLength(2000, ErrorMessage = "Page-19: SHOULDER: Sand replacement method: The length must be 2000 character or less for Comment")]

        public string COMMENT_19_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:  Please enter Tested By: (Name of Head of PIU)")]
        [StringLength(2000, ErrorMessage = "Page-19: SHOULDER: Sand replacement method: The length must be 2000 character or less for Tested By: (Name of Head of PIU)")]

        public string TESTED_BY_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: SHOULDER: Sand replacement method:  Please enter Test conducted in my presence")]
        [StringLength(3000, ErrorMessage = "Page-19: SHOULDER: Sand replacement method: The length must be 2000 character or less for Test conducted in my presence")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
    }
}