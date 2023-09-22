using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SC_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]
        public decimal LOCATION_RD_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please select Grading of coarse aggregate (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum one character is allowed in Grading of coarse aggregate (S / U) of row ")]
        public string GRADING_COARSE_AGG_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please enter Laying temperature of the mix as per QCR-I of row")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Laying temperature of the mix as per QCR-I of row ")]
        public decimal LAYING_TEMP_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please enter Bitumen content % As per QCR-I of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in Bitumen content % As per QCR-I of row ")]
        public decimal PERCENT_BITCONT_ASPER_QCR1_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please enter Bitumen content % As measured by NQM/SQM of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in Bitumen content % As measured by NQM/SQM of row ")]
        public decimal PERCENT_BITCONT_MEAS_QM_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please select Bitumen content % S/U of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum one character is allowed in Bitumen content % S/U of row ")]
        public string GRADING_PERCENT_BITCONT_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please enter Observed thickness of layer As measured by NQM/SQM of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Observed thickness of layer As measured by NQM/SQM of row ")]
        public decimal OBSERV_TOL_MEAS_QM_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please select Observed thickness of layer S/U of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum one character is allowed in Observed thickness of layer S/U of row ")]
        public string GRADING_OBSERV_TOL_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Please select Surface un evenness (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XV. Table- Maximum one character is allowed in Surface un evenness (S / U) of row ")]
        public string GRADING_SURFACE_UE_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }



    }
}