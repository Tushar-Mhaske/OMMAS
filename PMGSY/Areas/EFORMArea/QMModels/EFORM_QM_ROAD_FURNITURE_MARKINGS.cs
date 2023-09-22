using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_ROAD_FURNITURE_MARKINGS
    {
        public EFORM_QM_ROAD_FURNITURE_MARKINGS(bool RoadStatusIsCompleted)
        {
            this.TemplateStatus = RoadStatusIsCompleted;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MARK_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- I. a) Please Select Main Informatory Board Fixed ")]
        [StringLength(1, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  I. a) Maximum 1 character is allowed in Main Informatory Board Fixed")]
        public string IS_MAIN_INFO_BOARD_FIXED_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  I. b) Please Select Citizen Information Board Fixed ")]
        [StringLength(1, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  I. b) Maximum 1 character is allowed in Citizen Information Board Fixed")]
        public string IS_CITIZEN_INFO_BOARD_FIXED_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  I. c) Please Select Maintenance Board Fixed ")]
        [StringLength(1, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  I. c) Maximum 1 character is allowed in Maintenance Board Fixed")]
        [RoadStatusDependable]
        public string IS_MAINTANANCE_BOARD_FIXED_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS- III. Please Select Whether the information on boards is given in local language ")]
        [StringLength(1, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  Maximum 1 character is allowed in Whether the information on boards is given in local language")]
        public string IS_BOARD_INFO_IN_LOCAL_LANG_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  Please Select Item Grading-20 ")]
        [StringLength(3, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  Maximum 3 character are allowed in Item Grading-20")]
        public string ITEM_GRADING_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  Please enter suggestions for improvement  ")]
        [StringLength(250, ErrorMessage = "Page-30: Item 20. ROAD FURNITURE AND MARKINGS-  Maximum 250 character are allowed in suggestions for improvement ")]
        public string IMPROVE_SUGGESTIONS_31 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}