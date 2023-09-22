using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_PARTICULARS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PARTICULAR_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ADMIN_ND_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-2:  Item 3. PARTICULARS OF THE BRIDGE. 3.1 Please select Type of the bridge (according to HFL)")]
        [StringLength(20, ErrorMessage = "Page-2:  Item 3. PARTICULARS OF THE BRIDGE. 3.1 The length must be 20 character or less for Type of the bridge (according to HFL) ")]

        public string BRIDGE_TYPE_HFL { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-2:  Item 3. PARTICULARS OF THE BRIDGE. 3.1 Please select Type of the bridge (according to Material)")]
        [StringLength(20, ErrorMessage = "Page-2:  Item 3. PARTICULARS OF THE BRIDGE. 3.1 The length must be 20 character or less for Type of the bridge (according to Material) ")]

        public string BRIDGE_TYPE_MATERIAL { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-2:  Item 3. PARTICULARS OF THE BRIDGE. 3.2 Please select Type of foundation")]
        [StringLength(20, ErrorMessage = "Page-2:  Item 3. PARTICULARS OF THE BRIDGE. 3.2 The length must be 20 character or less for Type of foundation ")]
        public string FOUNDATION_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 3. PARTICULARS OF THE BRIDGE- 3.3 Please enter length of bridge ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-2: Item 3. PARTICULARS OF THE BRIDGE- 3.3 Please Enter Valid number{cant be greater than 999999.99} in Length of bridge")]
        public Nullable<decimal> BRIDGE_LENGTH { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 3. PARTICULARS OF THE BRIDGE- 3.4 Please enter span number ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-2: Item 3. PARTICULARS OF THE BRIDGE- 3.4 Please Enter Valid number{cant be greater than 999999.99} in Length of Span Number")]
        public Nullable<decimal> SPANS_NO { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-2: Item 3. PARTICULARS OF THE BRIDGE- 3.4 Please enter span length ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-2: Item 3. PARTICULARS OF THE BRIDGE- 3.4 Please Enter Valid number{cant be greater than 999999.99} in Length of Span length")]
        public Nullable<decimal> SPANS_LEN { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.5 Please enter width of bridge ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.5 Please Enter Valid number{cant be greater than 999999.99} in width of bridge")]
        public Nullable<decimal> BRIDGE_WIDTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.6 Please select Footpaths provided")]
        [StringLength(3, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.6 The length must be 3 character or less for Footpaths provided ")]
        public string IS_FOOTPATH_PROVIDED { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.6 Please enter width of bridge ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.6 Please Enter Valid number{cant be greater than 999999.99} in width of bridge ")]
        public Nullable<decimal> FOOTPATH_WIDTH { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.7 Please enter Formation Level (F.L.) ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in Formation Level (F.L.)")]
        public Nullable<decimal> FORMATION_LEVEL { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.8 Please enter Highest Flood Level (HFL) ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.8 Please Enter Valid number{cant be greater than 999999.99} in Highest Flood Level (HFL)")]
        public Nullable<decimal> HIGHEST_FLOOD_LEVEL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.8 Please enter Ordinary Flood Level (OFL) ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.8 Please Enter Valid number{cant be greater than 999999.99} in Ordinary Flood Level (OFL)")]
        public Nullable<decimal> ORDINARY_FLOOD_LEVEL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.9 Please enter bed level (Lowest) ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.9 Please Enter Valid number{cant be greater than 999999.99} in bed level (Lowest)")]
        public Nullable<decimal> BED_LEVEL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.10 Please enter Catchment area ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.10 Please Enter Valid number{cant be greater than 999999.99} in Catchment area")]
        public Nullable<decimal> CATCHMENT_AREA { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.11 Please enter Linear waterway at HFL ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.11 Please Enter Valid number{cant be greater than 999999.99} in Linear waterway at HFL ")]
        public Nullable<decimal> LINEAR_WATERWAY_HFL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.12 Please enter Design discharge   ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.12 Please Enter Valid number{cant be greater than 999999.99} in Design discharge ")]
        public Nullable<decimal> DESIGN_DISCHARGE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.13 Please enter Design velocity    ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.13 Please Enter Valid number{cant be greater than 999999.99} in Design velocity ")]
        public Nullable<decimal> DESIGN_VELOCITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers- Please enter foundation levels of Piers P1  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers- Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P1 ")]
        public Nullable<decimal> PIERS_P1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
         [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers- Please enter foundation levels of Piers P2  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a)- Piers Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P2 ")]
        public Nullable<decimal> PIERS_P2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
       [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers- Please enter foundation levels of Piers P3  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers- Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P3 ")]
        public Nullable<decimal> PIERS_P3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please enter foundation levels of Piers P4  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P4 ")]
        public Nullable<decimal> PIERS_P4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
         [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please enter foundation levels of Piers P5  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P5 ")]
        public Nullable<decimal> PIERS_P5 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please enter foundation levels of Piers P6  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P6 ")]
        public Nullable<decimal> PIERS_P6 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please enter foundation levels of Piers P7  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P7 ")]
        public Nullable<decimal> PIERS_P7 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please enter foundation levels of Piers P8  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P8 ")]
        public Nullable<decimal> PIERS_P8 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       //[Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 Please enter foundation levels of Piers P9  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (a) Piers-Please Enter Valid number{cant be greater than 999999.99} in foundation levels of Piers P9 ")]
        public Nullable<decimal> PIERS_P9 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (b) Abutments: A1 Please enter foundation levels of Abutments A1")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in foundation levels of  Abutments A1")]
        public Nullable<decimal> ABUTMENTS_A1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (b) Abutments: A2 Please enter foundation levels of Abutments A2")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in foundation levels of  Abutments A2")]
        public Nullable<decimal> ABUTMENTS_A2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (c) Returns: R1 Please enter foundation levels of Returns R1")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in foundation levels of  Returns R1")]
        public Nullable<decimal> RETURNS_R1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
      //  [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.14 (c) Returns: R2 Please enter foundation levels of Returns R2")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in foundation levels of  Returns R2")]
        public Nullable<decimal> RETURNS_R2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.15 Please enter Foundation Strata ")]
        [StringLength(50, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.15 The length must be 50 character or less for Foundation Strata ")]
        public string FOUNDATION_STRATA { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.16 Please enter Safe Bearing Capacity (SBC) ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.16Please Enter Valid number{cant be greater than 999999.99} in Safe Bearing Capacity (SBC) ")]
        public decimal SAFE_BEARING_CAP { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.17 Please select River protection works provided or not")]
        [StringLength(3, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.17 The length must be 3 character or less for River protection works provided or not ")]
        public string IS_RIVER_PROT_PROVIDED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.17 Please select if provided  ")]
        [StringLength(25, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.17 The length must be 25 character or less for if provided ")]
        public string RIVER_PROT_PROVIDED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (a) Piers-  Please select Type of Substructure Piers")]
        [StringLength(25, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (a) Piers- The length must be 25 character or less for Type of Substructure Piers ")]
        public string SUBSTRUCTURE_PIERS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.18 (a) Piers- Please enter Type of Substructure Piers other ")]
        [StringLength(50, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (a)Piers- The length must be 50 character or less for Type of Substructure Piers other ")]
        public string OTHER_SUBSTRUCTURE_PIERS { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (b) Abutments- Please select Type of Substructure Abutments")]
        [StringLength(25, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (b) Abutments- The length must be 25 character or less for Type of Substructure Abutments ")]
        public string SUBSTRUCTURE_ABUTMENTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-3: Item 3. PARTICULARS OF THE BRIDGE- 3.18 (b) Abutments- Please enter Type of Substructure Abutments other ")]
        [StringLength(50, ErrorMessage = "Page-3:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (b)Abutments- The length must be 50 character or less for Type of Substructure Abutments other ")]
        public string OTHER_SUBSTRUCTURE_ABUTMENTS { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (c) Returns- Please select Type of Substructure Returns")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (c) Returns-The length must be 25 character or less for Type of Substructure Returns ")]
        public string SUBSTRUCTURE_RETURNS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 3. PARTICULARS OF THE BRIDGE- 3.18 (c) Returns- Please enter Type of Substructure Returns other")]
        [StringLength(50, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.18 (c)Returns- The length must be 50 character or less for Type of Substructure Returns other ")]
        public string OTHER_SUBSTRUCTURE_RETURNS { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.19 Please select Type of bearings ")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.19 The length must be 25 character or less for Type of bearings  ")]
        public string BEARING_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 3. PARTICULARS OF THE BRIDGE- 3.19 Please enter Type of bearings  other")]
        [StringLength(50, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.19 The length must be 50 character or less for Type of bearings  other ")]
        public string OTHER_BEARING_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.20 Please select Test report of bearings available or not")]
        [StringLength(3, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.20 The length must be 3 character or less for Type of bearings available or not")]
        public string IS_BEARING_REPORT_AVBL { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 Please select Type of superstructure ")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 The length must be 25 character or less for Type of superstructure ")]
        public string SUPERSTRUCTURE_MAIN_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 Please select Type of superstructure RCC")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 The length must be 25 character or less for Type of superstructure RCC ")]
        public string RCC_SUB_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 3. PARTICULARS OF THE BRIDGE- 3.21 Please enter Type of superstructure RCC other")]
        [StringLength(50, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 The length must be 50 character or less for Type of superstructure RCC other ")]

        public string OTHER_RCC_SUB_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 Please select Type of superstructure Steel")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 The length must be 25 character or less for Type of superstructure Steel ")]
        public string STEEL_SUB_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 3. PARTICULARS OF THE BRIDGE- 3.21 Please enter Type of superstructure Steel other")]
        [StringLength(50, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.21 The length must be 50 character or less for Type of superstructure Steel other")]
        public string OTHER_STEEL_SUB_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.22 Please select Type of expansion joints")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.22 The length must be 25 character or less for Type of expansion joints")]
        public string EXPANSION_JNT_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 3. PARTICULARS OF THE BRIDGE- 3.22 Please enter Type of expansion joints other")]
        [StringLength(50, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.22 The length must be 50 character or less for Type of expansion joints other")]
        public string OHTER_EXPANSION_JNT_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.23 Please select Type of wearing coat")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.23 The length must be 25 character or less for Type of wearing coat")]
        public string WEARING_COAT_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.24 Please select Type of drainage spouts")]
        [StringLength(25, ErrorMessage = "Page-4:  Item 3. PARTICULARS OF THE BRIDGE. 3.24 The length must be 25 character or less for Type of drainage spouts")]
        public string DRAINAGES_SPOUTS_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.25 Please select Design loading")]
        [StringLength(250, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.25 The length must be 250 character or less for Design loading")]
        public string DESIGN_LOADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.25 Please enter Design loading other")]
        [StringLength(250, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.25 The length must be 250 character or less for Design loading other")]
        public string OTHER_DESIGN_LOADING { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.26 Please select Whether approaches are in cutting or in filling ")]
        [StringLength(25, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.26 The length must be 25 character or less for Whether approaches are in cutting or in filling")]
        public string APPROCH_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.27 Please select Type of pitching in approaches ")]
        [StringLength(25, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.27 The length must be 25 character or less for Type of pitching in approaches")]
        public string PITCHING_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.28 Please select Whether bench marks and alignment are established ")]
        [StringLength(3, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.28 The length must be 3 character or less for Whether bench marks and alignment are established ")]
        public string IS_BENCHMARKS_ESTB { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter location of row 1 ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in location of row 1")]
        public Nullable<decimal> BENCHMARK_LOC1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter  Whether bench marks and alignment are established near the bridge site location 2  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in  location of row 2 ")]
        public Nullable<decimal> BENCHMARK_LOC2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
      //  [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter  Whether bench marks and alignment are established near the bridge site location 3  ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in location of row 3 ")]
        public Nullable<decimal> BENCHMARK_LOC3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
      //  [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter  Whether bench marks and alignment are established near the bridge site location 4   ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in location of row 4 ")]
        public Nullable<decimal> BENCHMARK_LOC4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter RL of row 1 ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in RL of row 1 ")]
        public Nullable<decimal> BENCHMARK_RL1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
      //  [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter  Whether bench marks and alignment are established near the bridge site RL 2   ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in RL of row 2 ")]
        public Nullable<decimal> BENCHMARK_RL2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter  Whether bench marks and alignment are established near the bridge site RL 3   ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in RL of row 3 ")]
        public Nullable<decimal> BENCHMARK_RL3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.28 Please enter  Whether bench marks and alignment are established near the bridge site RL 4   ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-5: Item 3. 3.28 PARTICULARS OF THE BRIDGE- Please Enter Valid number{cant be greater than 999999.99} in RL of row 4 ")]
        public Nullable<decimal> BENCHMARK_RL4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE.  3.29 Please select Type of Railings ")]
        [StringLength(30, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.29 The length must be 30 character or less for Type of Railings")]
        public string RAILING_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Item 3. PARTICULARS OF THE BRIDGE- 3.29 Please enter Type of Railings other")]
        [StringLength(50, ErrorMessage = "Page-5:  Item 3. PARTICULARS OF THE BRIDGE. 3.29 The length must be 50 character or less for Type of Railings other")]
        public string OTHER_RAILING_TYPE { get; set; }

    }
}