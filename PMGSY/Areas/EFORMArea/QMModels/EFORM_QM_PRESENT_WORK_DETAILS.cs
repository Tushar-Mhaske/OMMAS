using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_PRESENT_WORK_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int WORK_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-6: 1.GENERAL DETAILS- IX.Present status of work- Please Enter Valid number{cant be greater than 9999.99} in  ")]
        public Nullable<decimal> ROAD_FROM { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,4}\.\d{1,3})$|^\d{0,4}$", ErrorMessage = "Page-6: 1.GENERAL DETAILS- IX.Present status of work- Please Enter Valid number{cant be greater than 9999.99} in  ")]
        public Nullable<decimal> ROAD_TO { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


    }
}