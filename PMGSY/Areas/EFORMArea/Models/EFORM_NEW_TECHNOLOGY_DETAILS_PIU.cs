using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_NEW_TECHNOLOGY_DETAILS_PIU
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]

        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string TECH_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        public short NEW_TECHNOLOGY_NAME { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> ROAD_FROM { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-1: Please Enter Valid number{cant be greater than 9999999.99} in  ")]
        public Nullable<decimal> ROAD_TO { get; set; }
    }
}