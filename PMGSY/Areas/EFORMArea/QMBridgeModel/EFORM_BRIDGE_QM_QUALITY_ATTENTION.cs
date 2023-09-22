using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_QUALITY_ATTENTION
    {

       

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- I. (a). Please select Based on executed quantities, whether all mandatory tests conducted ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- I. (a) Maximum one character is allowed in Based on executed quantities, whether all mandatory tests conducted ")]
      
        public string IS_ALL_TEST_CONDUCTED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- I. (b) Please select Whether QC Register Part I maintained as per provisions ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- I. (b) Maximum one character is allowed in Whether QC Register Part I maintained as per provisions ")]
      
        public string IS_QC_REG_P1_MAINTAINED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- I. (c) Please select Whether QC Register Part II maintained and test results monitored as per provisions ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- I. (c) Maximum one character is allowed in Whether QC Register Part II maintained and test results monitored as per provisions ")]
       
        public string IS_QC_REG_P2_MAINTAINED { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- II. (b) only Y/N is allowed in Negligence ")]

        public string IS_NEGLIGENCE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- II. (b) only Y/N is allowed in Lack of equipments in lab ")]

        public string IS_LOE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- II. (b) only Y/N is allowed in Lack of knowledge ")]

        public string IS_LOK { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]

        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-10: Item 3. ATTENTION TO QUALITY- II. (b) only Y/N is allowed in Any other, please specify ")]

        public string IS_OTHER { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- II. (b) Please enter in Any other, please specify comment field ")]

        [StringLength(250, ErrorMessage = "Page-11: Item 3. ATTENTION TO QUALITY- II. (b) The length must be 250 character or less for any other reason field")]
       
        public string OTHER_REASON { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- II. (d) Please select Whether non-conformities recorded in QCR-II by AE, have been rectified and recorded in QCR-I again as conformities, after conducting necessary tests ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 3. ATTENTION TO QUALITY- II. (d) Maximum one character is allowed in Whether non-conformities recorded in QCR-II by AE, have been rectified and recorded in QCR-I again as conformities, after conducting necessary tests: ")]
        
        public string IS_NON_CONFORMITIES_QCR2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- II. (e) Please select In order to assess the quality of concrete in case of doubt, if any Non DestructiveTest (NDT) such as Rebound Hammer test, Ultra Sonic Pulse Velocity (UPV) etc.has been conducted ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- II. (d) Maximum one character is allowed in In order to assess the quality of concrete in case of doubt, if any Non DestructiveTest (NDT) such as Rebound Hammer test, Ultra Sonic Pulse Velocity (UPV) etc.has been conducted ")]
        
        public string IS_NDT_CONDUCTED_12 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- Please select Item Grading-3 ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- Maximum three character is allowed in Item Grading-3 ")]
       
        public string ITEM_GRADING_3 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- Please enter suggestions for improvement ")]
        [StringLength(500, ErrorMessage = "Page-12: Item 3. ATTENTION TO QUALITY- The length must be 250 character or less for  suggestions for improvement")]
        
        public string IMPROVEMENT_REMARK_3 { get; set; }

 
    }
}