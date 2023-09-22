using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_NEW_TECH_PIU
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAST_TECH_CODE1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string MAST_TECH_NAME1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> RD_FROM1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> RD_TO1 { get; set; }



        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAST_TECH_CODE2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string MAST_TECH_NAME2 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> RD_FROM2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> RD_TO2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAST_TECH_CODE3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string MAST_TECH_NAME3 { get; set; }




        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> RD_FROM3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> RD_TO3 { get; set; }
    }
}