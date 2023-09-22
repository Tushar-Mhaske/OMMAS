using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_SIDE_SLOPE_DETAIL
    {
        public EFORM_QM_CHILD_SIDE_SLOPE_DETAIL()
        {
            this.RowID = 3;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SS_DETAIL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SIDE_SLOP_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Please enter Location (RD) of row ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Please Enter Valid number{cant be greater than 9999999.99} in Location (RD) or row ")]
        public decimal? LOCATION_RD_13_1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Please enter Side Slopes Observed by QM- H:V of row  ")]
        [StringLength(10, ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- The length must be 10 character or less for Side Slopes Observed by QM- H:V of row ")]
        public string SS_OBSERVED_BY_QM { get; set; }




        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Please select Whether Side Slopes Satisfactory (Y / N) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Maximum one character is allowed in Whether Side Slopes Satisfactory (Y / N) of row ")]
        public string IS_SS_SATISFACTORY { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Please select  Whether profile is Satisfactory (Y / N) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Maximum one character is allowed in Whether profile is Satisfactory (Y / N) of row ")]
        public string IS_PROFILE_SATISFACTORY { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Please select Grading (S / U) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Side slopes Table- Maximum one character is allowed in Grading (S / U) of row  ")]
        public string GRADING { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}