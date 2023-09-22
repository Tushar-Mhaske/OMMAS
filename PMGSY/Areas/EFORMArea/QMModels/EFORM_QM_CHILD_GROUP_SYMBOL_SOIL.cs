using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_GROUP_SYMBOL_SOIL
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SYMBOL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QOM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]
        public decimal LOCATION_RD_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Please enter Group Symbol of soil as per DPR of row  ")]
        [StringLength(4, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Maximum 4 characters are allowed in Group Symbol of soil as per DPR of row ")]
        public string GSS_ASPER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Please enter Group Symbol of soil as observed of row  ")]
        [StringLength(4, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Maximum 4 characters are allowed in Group Symbol of soil as observed of row ")]
        public string GSS_OBSERVED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Please select Suitability from Plasticity angle (Y/N) of row ")]
        [StringLength(1, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Maximum 1 charater is allowed in Suitability from Plasticity angle (Y/N) of row ")]
        public string SUITABILITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Please select Quality of material used (S / U) of row ")]
        [StringLength(1, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- II. Group symbol of soil Table- Maximum 1 charater is allowed in Quality of material used (S / U) of row ")]
        public string MATERIAL_QLTY { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}