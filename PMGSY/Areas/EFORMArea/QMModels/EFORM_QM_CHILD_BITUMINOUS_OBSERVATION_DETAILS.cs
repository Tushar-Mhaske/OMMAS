using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int LAYER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]
        public decimal LOCATION_RD_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please select Grading of Coarse Aggregates (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-20: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum one character is allowed in Grading of Coarse Aggregates (S / U) of row ")]
        public string GRADING_COARSE_AGG_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please enter Density of BM/DBM achieved at site of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Density of BM/DBM achieved at site of row ")]
        public decimal DENSITY_ACHIEVED_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please select % Compaction of BM/DBM w.r.t designdensity (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-20: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum one character is allowed in % Compaction of BM/DBM w.r.t designdensity (S / U) of row ")]
        public string PERCENT_COMPAQ_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please enter Bitumen content % As per QCR-I of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in Bitumen content % As per QCR-I of row ")]
        public decimal PERCENT_BITCONT_ASPER_QCR1_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please enter Bitumen content % As measured by QM of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in Bitumen content % As measured by QM of row ")]
        public decimal PERCENT_BITCONT_MEAS_QM_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please select Bitumen content % S/U of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-20: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum one character is allowed in Bitumen content % S/U of row ")]
        public string GRADING_PERCENT_BITCONT_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please enter Observed thickness of layer (mm) As measured by QM of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Observed thickness of layer (mm) As measured by QM of row ")]
        public decimal OBSERV_TOL_MEAS_QM_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Please select Observed thickness of layer (mm) S/U of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-20: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- XVII. Table- Maximum one character is allowed in Observed thickness of layer (mm) S/U of row ")]
        public string GRADING_OBSERV_TOL_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


    }
}