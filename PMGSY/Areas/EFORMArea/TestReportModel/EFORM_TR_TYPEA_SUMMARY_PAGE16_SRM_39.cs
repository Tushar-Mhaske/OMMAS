using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Areas.EFORMArea.Model;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE16_SRM_39
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
        [Required(ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content :Please enter Bitumen content as per job mix (%)")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,5}$", ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : Maximum 3 digits before decimal and maximum two digits after decimal are allowed in Bitumen content as per job mix (%) ")]

        public Nullable<decimal> B_CONTENT_16_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content :Please select Date of Testing")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing ")]

        public Nullable<System.DateTime> TESTING_DATE_16_1 { get; set; }

        
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : Remark is required.")]
        [StringLength(2000, ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : The length must be 2000 character or less for Remark")]
        public string REMARK_16_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : Please enter comment.")]
        [StringLength(2000, ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : The length must be 2000 character or less for Comment")]

        public string COMMENT_16_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content :  Please enter Tested By: (Name of Head of PIU)")]
        [StringLength(2000, ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : The length must be 2000 character or less for Tested By: (Name of Head of PIU)")]

        public string TESTED_BY_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content :  Please enter Test conducted in my presence")]
        [StringLength(3000, ErrorMessage = "Page-16: BITUMINOUS BASE COURSE: Binder Content : The length must be 2000 character or less for Test conducted in my presence")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
    }
}