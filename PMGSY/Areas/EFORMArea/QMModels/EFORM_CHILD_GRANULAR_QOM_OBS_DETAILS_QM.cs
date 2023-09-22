using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_GRANULAR_QOM_OBS_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string QOM_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]

        public decimal LOCATION_RD_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please select Conforms to Grading (Y/N) of row  ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum one character is allowed in Conforms to Grading (Y/N) of row ")]
        public string IS_CONFORM_TO_GRADING_14 { get; set; }

       [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please select Material Suitable from plasticity angle (Y / N) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum one character is allowed in Material Suitable from plasticity angle (Y / N) of row ")]

        public string IS_MAT_SUITABLE_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please enter Dry density kN / m3 of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed Dry density kN / m3 of row ")]

        public decimal DRY_DENSITY_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please enter % Compaction of row ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum 999.99 is allowed and maximum two digits after decimal are allowed in % Compaction of row ")]

        public decimal PERCENT_COMPAQ_14 { get; set; }

       [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please select Whether compaction is adequate (Y / N) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum one character is allowed Whether compaction is adequate (Y / N) of row  ")]

        public string IS_COMPAQ_ADIQUATE_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please enter Thickness as per DPR (in mm) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Thickness as per DPR (in mm) of row ")]

        public decimal THICKNESS_ASPER_DPR_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please enter Measured Thickness (in mm) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Measured Thickness (in mm) of row ")]

        public decimal THICKNESS_MEAS_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Please select Prescribed Thickness provided (Y/N) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VII. Quality of Material and workmanship Table- Maximum one character is allowed in Prescribed Thickness provided (Y/N) of row ")]

        public string IS_PRISCRIBED_14 { get; set; }
    

    }
}