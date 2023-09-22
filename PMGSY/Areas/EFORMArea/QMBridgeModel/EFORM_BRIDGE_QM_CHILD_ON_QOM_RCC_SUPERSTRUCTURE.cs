using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Location/ Item of work of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Location/ Item of work of row ")]
        public string RD_LOC_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Mix design/ Grade of Concrete of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Mix design/ Grade of Concrete of row ")]
        public string CONCRETE_GRADE_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Quality of material-Cement  of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of material-Cement of row ")]
        public string CEMENT_QUALITY_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Quality of material-Coarse Aggregate of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of material-Coarse Aggregate  of row ")]
        public string COARSE_AGGR_QUALITY_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Quality of material-Fine Aggregate of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of material-Fine Aggregate of row ")]
        public string FINE_AGGR_QUALITY_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Quality of material-Steel Reinforcement Verification of properties of lot with standard norms of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Quality of material-Steel Reinforcement Verification of properties of lot with standard norms of row ")]
        public string STEEL_REIN_QUALITY_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Workmanship-Staging and Shuttering arrangements of span checked by competent authority as per the approved staging arrangements of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Workmanship-Staging and Shuttering arrangements of span checked by competent authority as per the approved staging arrangements of row ")]
        public string STAG_WORKMEN_CHECK_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Workmanship-Whether reinforcement checked by competent authority for spacing, bond length, cover etc. of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Workmanship-Whether reinforcement checked by competent authority for spacing, bond length, cover etc. of row ")]
        public string REINFORCEMENT_CHECK_AUTH_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Workmanship-Workability of Cement concrete of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Workmanship-Workability of Cement concrete of row ")]
        public string CONCRETE_WORKABILITY_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Workmanship-Compaction Arrangement of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Workmanship-Compaction Arrangement of row ")]
        public string COMPACTION_ARRANGEMENT_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Camber of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Camber of row ")]
        public string CAMBER_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please enter Drainage spouts provided as per the standards of row ")]
        [StringLength(20, ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: The length must be 20 character or less for Drainage spouts provided as per the standards of row ")]
        public string DRAINAGE_SPOUTS_STD_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Please select Grade(S/ U) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-17: Item 6. (i)RCC Superstructure- A)Ongoing (Work in progress)-Table: Maximum three character is allowed in Grade(S/ U) of row ")]
        public string GRADE_17 { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string SuperStructureType { get; set; }


    }
}