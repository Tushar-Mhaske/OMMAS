using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Please enter Location / RD of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed Location / RD of row ")]

        public decimal LOCATION_RD_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please enter Structure type (Retaining Wall / Breast Wall / Parapets) of row ")]
        [StringLength(maximumLength: 50, ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Only 50 Characters Allowed in Structure type (Retaining Wall / Breast Wall / Parapets) of row ")]
        public string STRUCTURE_TYPE_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please enter Type of protection work (CC/ masonry/gabions) of row ")]
        [StringLength(maximumLength: 50, ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Only 50 Characters Allowed in Type of protection work (CC/ masonry/gabions) of row ")]
        public string PROTECTION_TYPE_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please select General quality of material conforms to specifications (Y / N) of row  ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum one character is allowed in General quality of material conforms to specifications (Y / N) of row  ")]
        public string GENERAL_QOM_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please enter Average width As per DPR of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Average width As per DPR of row ")]

        public decimal AVG_WIDTH_ASPER_DPR_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please enter Average height As per DPR of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Average height As per DPR of row ")]

        public decimal AVG_HEIGHT_ASPER_DPR_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please enter Average width As per records of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Average width As per records of row ")]

        public decimal AVG_WIDTH_ASPER_REC_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please enter Average height As per records of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Average height As per records of row ")]

        public decimal AVG_HEIGHT_ASPER_REC_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table-  Please select Whether compressive strength of material is as per design from QCR - I(Y / N) of row ")]

        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- IV. Quality of Materials Table- Maximum one character is allowed in Whether compressive strength of material is as per design from QCR - I(Y / N) of row ")]
        public string IS_COMPR_SOM_25 { get; set; }
    }
}