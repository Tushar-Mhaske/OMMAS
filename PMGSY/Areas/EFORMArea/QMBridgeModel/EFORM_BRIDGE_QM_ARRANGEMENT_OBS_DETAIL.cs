﻿using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_ARRANGEMENT_OBS_DETAIL
    {

       
 


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- I. Please select Whether field laboratory established")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- I. Maximum one character is allowed in Whether field laboratory established ")]
        
        public string IS_FIELD_LAB_ESTD { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- II. Please select Whether location of field laboratory is same as indicated by PIU in format part - I ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- II. Maximum one character is allowed in Whether location of field laboratory is same as indicated by PIU in format part - I ")]
         
        public string IS_LAB_LOC_SAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- III. Please select Whether necessary equipments as indicated in part-1 are actually available: ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- III. Maximum one character is allowed in Whether necessary equipments as indicated in part-1 are actually available ")]
       
        public string IS_EQUIP_AVAILABLE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- IV. Please select Whether equipment’s have been used ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- IV. Maximum one character is allowed in Whether equipment’s have been used ")]
        
        public string IS_EQUIP_USED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- V. Please select If all necessary equipments are not available, whether you have verified them with the list of deficient equipment’s provided by PIU in format Part-I")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- V. Maximum one character is allowed in If all necessary equipments are not available, whether you have verified them with the list of deficient equipment’s provided by PIU in format Part-I ")]
        
        public string IS_EQUIP_AVAIL_VERIFY { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- VI. Please select Calibration of equipment done or not ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- VI. Maximum one character is allowed in Calibration of equipment done or not ")]
       
        public string IS_CALIBRATION_OF_EQUIPMENT { get; set; }        

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- VII. Please select Whether contractor’s engineer as per Part-I of this format, is available at site ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- VII. Maximum one character is allowed in Whether contractor’s engineer as per Part-I of this format, is available at site ")]
        
        public string IS_ENGG_AVAILABLE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- VIII. Please select if contractor’s engineer as per Part-I of this report is not available, whether you are satisfied with alternative arrangement made to maintain QCR-I ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- VIII. Maximum one character is allowed in if contractor’s engineer as per Part-I of this report is not available, whether you are satisfied with alternative arrangement made to maintain QCR-I ")]
       
        public string IS_ALT_ENGG_ARR_SATISFIED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- IX. Please select Whether lab technician and other staff is available for testing in the laboratory ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- IX. Maximum one character is allowed in Whether lab technician and other staff is available for testing in the laboratory ")]
        
        public string IS_LAB_TECH_AVAILABLE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- Please select item Grading-2 ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS-  Maximum three character is allowed in item Grading-2 ")]
     
        public string ITEM_GRADING_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- Please enter suggestions for improvement")]
        [StringLength(250, ErrorMessage = "Page-10: Item 2. QUALITY ARRANGEMENTS-OBSERVATIONS- The length must be 250 character or less for suggestions for improvement ")]
       
        public string IMPROVEMENT_REMARK_2 { get; set; }


        
    }
}