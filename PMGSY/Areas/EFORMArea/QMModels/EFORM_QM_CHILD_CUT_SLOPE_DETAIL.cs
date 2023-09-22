using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_CUT_SLOPE_DETAIL
    {

        public EFORM_QM_CHILD_CUT_SLOPE_DETAIL()
        {
            this.RowID = 3;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int CUT_DETAIL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SIDE_SLOP_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13:  Item 5.EARTHWORK & SUB GRADE- IV. (b) Cut slope Table- Please enter Location (RD) of row ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (b) Cut slope Table- Please Enter Valid number{cant be greater than 9999999.99} in Location (RD) of row ")]
        public decimal? LOCATION_RD_13_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (b) Cut slope Table- Please select Whether cut slopes & profile appears to be stable (S/U) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (b) Cut slope Table- Maximum one character is allowed Whether cut slopes & profile appears to be stable (S/U) of row  ")]
        public string IS_STABLE { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}