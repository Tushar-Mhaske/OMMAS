using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MARK_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public short ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table- Please Enter Number of furniture to be provided of row  ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Number of furniture to be provided of row ")]
        public Nullable<decimal> FURNITURE_QTY_TOBE_PROVIDED_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Please Enter Furniture provided at site of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Furniture provided at site of row ")]
        public Nullable<decimal> FURNITURE_QTY_PROVIDED_AT_SIDE_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Please Select Number of furniture provided is adequate (Y/N) of row  ")]
        [StringLength(1, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Maximum 1 character is allowed in Number of furniture provided is adequate (Y/N) of row ")]
        public string IS_PROVIDED_FURNITURE_ADEQUATE_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Please Select Quality of furniture (S / SRI / U) of row ")]
        [StringLength(3, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- II.Observations Table-  Maximum 3 character are allowed Quality of furniture (S / SRI / U) of row ")]
        public string GRADING_FURNITURE_QUALITY_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}