using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PAVE_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table- Please Enter Reference RD of CC / SR pavements (m) From of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Reference RD of CC / SR pavements (m) From of row ")]
        public Nullable<decimal> REF_RD_FROM_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Enter Reference RD of CC / SR pavements (m) To of row  ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed Reference RD of CC / SR pavements (m) To of row  ")]
        public Nullable<decimal> REF_RD_TO_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Enter RD at which observation made (m) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in RD at which observation made (m) of row ")]
        public Nullable<decimal> LOCATION_RD_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Select Quality of material concrete/ stone/ CC blocks pavements etc. (visual inspection) (S / U) of row ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum 1 charater is allowed Quality of material concrete/ stone/ CC blocks pavements etc. (visual inspection) (S / U) of row ")]
        public string GRADING_QOM_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Enter 28 days Strength of concrete as per QCR-I MPa of row ")]
        [StringLength(20, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum 20 charaters are allowed in 28 days Strength of concrete as per QCR-I MPa of row ")]
        public string CONCRETE_STRENGTH_ASPER_QCR1_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Select Quality of workmanship wearing surface, joints, edges etc. (S / U) of row ")]
        [StringLength(3, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum 1 charater is allowed in 28 days Strength of concrete as per QCR-I MPa of row ")]

        public string GRADING_QOW_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Enter Thickness As per DPR (mm) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Thickness As per DPR (mm) of row ")]
        public Nullable<decimal> THICKNESS_ASPER_DPR_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Enter Thickness As Measured by QM (mm) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Thickness As Measured by QM (mm) of row ")]
        public Nullable<decimal> THICKNESS_MEAS_QM_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Please Select Thickness Acceptable (Y/N) of row ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- V.Table-  Maximum 1 charater is allowed in Thickness Acceptable (Y/N) of row ")]
        public string IS_THICKNESS_ACCEPTABLE_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}