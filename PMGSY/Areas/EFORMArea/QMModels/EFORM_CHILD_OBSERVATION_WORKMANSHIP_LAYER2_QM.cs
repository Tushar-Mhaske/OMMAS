using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string Work_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table-Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]

        public decimal LOCATION_RD_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table-Please select Grading of Aggregates (S / U) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table-Maximum one character is allowed in Grading of Aggregates (S / U) of row ")]

        public string GRADING_AGRI_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please select Plasticity of Filler material (S / U) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Maximum one character is allowed in Plasticity of Filler material (S / U) of row ")]

        public string GRADING_PLASTICITY_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please enter Volume of filler material percent of coarse Aggregate of row ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Maximum 999.99 is allowed and maximum two digits after decimal are allowed in Volume of filler material percent of coarse Aggregate of row ")]

        public decimal PERCENT_VOL_FILLER_MATERIAL_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please enter Compaction based on volumetric analysis/sand replacement method (S / U) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Maximum one character is allowed in Compaction based on volumetric analysis/sand replacement method (S / U) of row ")]

        public string GRADING_COMPACTION_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please enter Design thickness as per DPR (mm) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Design thickness as per DPR (mm) of row ")]

        public decimal DESIGN_THICKNESS_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please enter Thickness of each layer of WBM/ WMM (mm) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed Thickness of each layer of WBM/ WMM (mm) of row ")]

        public decimal WBM_THICKNESS_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Please enter Thickness adequate (S / U) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- VI. Quality of Material and Workmanship Table- Maximum one character is allowed in Thickness adequate (S / U) of row ")]

        public string ADEQUATE_THICKNESS_17 { get; set; }
    }
}