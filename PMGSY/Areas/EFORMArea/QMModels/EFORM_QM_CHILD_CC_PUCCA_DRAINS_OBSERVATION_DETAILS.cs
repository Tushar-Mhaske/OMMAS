using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DRAIN_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table- Please Enter Location (RD) of CC/Pucca side drains From of row  ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of CC/Pucca side drains From of row ")]
        public decimal LOCATION_RD_FROM_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter Location (RD) of CC/Pucca side drains To of row  ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of CC/Pucca side drains To of row ")]
        public decimal LOCATION_RD_TO_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter RD at which observation made of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in RD at which observation made of row ")]
        public decimal LOCATION_RD_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter Cross-section size size as per DPR B of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Cross-section size size as per DPR B of row ")]
        public decimal CS_SIZE_B_ASPER_DPR_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter Cross-section size as per DPR D of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Cross-section size as per DPR D of row ")]
        public decimal CS_SIZE_D_ASPER_DPR_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter Cross-section size as measured B of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Cross-section size as measured B of row ")]
        public decimal CS_SIZE_B_MEAS_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter Cross-section size as measured D of row  ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Cross-section size as measured D of row ")]
        public decimal CS_SIZE_D_MEAS_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Select Cross-section size of drains is acceptable (Y/N) ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum 1 character is allowed in of drains is acceptable (Y/N)")]
        public string IS_SIZE_DRAINS_ACCEPTABLE_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Enter Strength of Concrete as per QCR –I MPa of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Strength of Concrete as per QCR –I MPa of row ")]
        public decimal SOC_ASPER_QCR1_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Please Select General quality of material and work-manship (S / U) of row ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- VI Table-  Maximum 1 character is allowed in General quality of material and work-manship (S / U) of row ")]
        public string GRADING_GEN_QOM_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}