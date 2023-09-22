using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_EXPANSION
    {
        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in Buried Expansion joint checkbox ")]
        public string BURIED_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in Filler Joint with Copper Plate checkbox ")]
        public string FILLER_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint- only (Y/N) is allowed in	Asphaltic Plug Joint checkbox ")]
        public string ASPHALTIC_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in Compression Seal Joint checkbox ")]
        public string COMPRESSION_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in Single Strip / Box Seal Joint checkbox ")]
        public string SINGLE_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in reinforced Elastomeric Joint checkbox ")]
        public string REINFORCED_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in Modular Strip / Box Seal joint checkbox ")]
        public string MODULAR_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint- only (Y/N) is allowed in	Finger Joint checkbox ")]
        public string FINGER_OF_EXPANSION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  only (Y/N) is allowed in Reinforced Coupled Elastomeric Joint checkbox ")]
        public string REINFORCED_COUPLED_OF_EXPANSION { get; set; }




        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- i. Please select is there any crack in the wearing coat")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- i. The length must be 3 character or less for is there any crack in the wearing coat ")]
        public string IS_ANY_CRACK_IN_COAT { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- ii. Please select ii.	Whether existing gap is improper")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- ii. The length must be 3 character or less for ii. Whether existing gap is improper")]
        public string IS_EXISTING_GAP_IMPROPER { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- iii. Please select Any hardening / cracking observed in bitumen filler")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- iii. The length must be 3 character or less for Any hardening / cracking observed in bitumen filler")]
        public string IS_CRACKING_IN_BITUMEN { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- iv. Please select Any splitting/ oxidation/ creep/ bulging observed in neoprene sealing material")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- iv. The length must be 3 character or less for Any splitting/ oxidation/ creep/ bulging observed in neoprene sealing material ")]
        public string IS_SPLITTING_IN_NEOPRENE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- v. Please select Corrosion/ damage to welds is visible")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- v. The length must be 3 character or less for Corrosion/ damage to welds is visible ")]
        public string IS_CORROSION_IS_VISIBLE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- vi. Please select Presence of debris in the joint")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- vi. The length must be 3 character or less for Presence of debris in the joint ")]
        public string IS_DEBRIS_IN_JOINTS { get; set; }
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- vii. Please select Report - any other defect or observation found")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- vii. The length must be 3 character or less for Report - any other defect or observation found ")]
        public string OBSERVATION_FOUND { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- vii. Please enter Report - any other defect or observation found")]
        [StringLength(250, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- vii. The length must be 250 character or less for Report - any other defect or observation found ")]
        public string OTHER_OBSER_FOUND { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints-  Please select Item Grading-9:")]
        [StringLength(3, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints-   The length must be 3 character or less for Item Grading-9:")]
        public string ITEM_GRADING_9 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints-  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-25: Item 9. EXPANSION JOINTS- B) Condition of Expansion joints- i. The length must be 250 character or less for suggestions for improvement ")]
        public string IMPROVEMENT_REMARK_9 { get; set; }
        


    }
}