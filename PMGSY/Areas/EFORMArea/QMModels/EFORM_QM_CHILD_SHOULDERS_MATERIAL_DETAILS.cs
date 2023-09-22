using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_SHOULDERS_MATERIAL_DETAILS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SH_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]
        public decimal LOCATION_RD_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please select Quality of the material from hand feel test (S/U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one character is allowed in Quality of the material from hand feel test (S/U) of row ")]
        public string GRADING_QOM_HAND_FEEL_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please enter Degree of Compaction As per QCR-I of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Degree of Compaction As per QCR-I of row ")]
        public decimal COMPAQ_DEGREE_ASPER_QCR1_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please enter Degree of Compaction As measured by QM (%) of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum three digits before decimal and maximum two digits after decimal are allowed in Degree of Compaction As measured by QM (%) of row ")]
        public decimal COMPAQ_DEGREE_MEAS_QM_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please select Degree of Compaction Grading (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one character is allowed in Degree of Compaction Grading (S / U) of row ")]
        public string GRADING_COMPAQ_DEGREE_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please enter Camber As per DPR (%) of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum three digits before decimal and maximum two digits after decimal are allowed in Camber As per DPR (%) of row ")]
        public decimal CAMBER_ASPER_DPR_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please enter Camber As measured by QM (%) of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum three digits before decimal and maximum two digits after decimal are allowed in Camber As measured by QM (%) of row ")]
        public decimal CAMBER_MEAS_QM_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please select Camber Grading (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one character is allowed in Camber Grading (S / U) of row ")]
        public string GRADING_CAMBER_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please enter Sectional Parameters Width (measured by QM) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Sectional Parameters Width (measured by QM) of row ")]
        public decimal SECTIONAL_WIDTH_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please enter Sectional Parameters Thickness (measured by QM) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Sectional Parameters (measured by QM) of row ")]
        public decimal SECTIONAL_THICKNESS_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Please select Sectional Parameters Grading of Width & Thickness (S / U) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-22: Item 12. SHOULDERS- IV. Quality of Shoulders Table-  Maximum one character is allowed in Sectional Parameters Grading of Width & Thickness (S / U) of row ")]
        public string GRADING_SECTIONAL_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}