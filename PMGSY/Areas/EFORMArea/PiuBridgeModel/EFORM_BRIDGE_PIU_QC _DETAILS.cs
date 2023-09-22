using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_QC_DETAILS
    {

        public EFORM_BRIDGE_PIU_QC_DETAILS(bool RoadStatusIsCompleted)
        {
            this.TemplateStatus = RoadStatusIsCompleted;
        }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ITEM_ID { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Item 4. QUALITY CONTROL I. Please enter Location of field laboratory")]
        [StringLength(40, ErrorMessage = "Page-5: Item 4. QUALITY CONTROL I. The length must be 40 character or less for Location of field laboratory")]
        [RoadStatusDependable]
        public string LAB_LOCATION { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-5: Item 4. QUALITY CONTROL II. Please enter Geo-tagged photograph of laboratory uploaded on: (date/ month/ year)")]
        [RoadStatusDependable]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-5: Item 4. QUALITY CONTROL II. Please Enter Valid date{in dd/mm/yyyy format} in Geo-tagged photograph of laboratory uploaded on: (date/ month/ year) ")]

        public string PHOTO_UPLOAD_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-5: Item 4. QUALITY CONTROL III. The length must be 250 character or less for Reason for delay in establishment of field laboratory")]

        public string ESTB_DELAY_REASON { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4. QUALITY CONTROL IX. Please enter List of equipments available in field lab")]
        [StringLength(250, ErrorMessage = "Page-4: Item 4. QUALITY CONTROL IX. The length must be 250 character or less for List of equipments available in field lab")]
        [RoadStatusDependable]
        public string LAB_EQUIP_AVBL { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4. QUALITY CONTROL - Please enter Available equipments that are in working condition ")]
        [StringLength(250, ErrorMessage = "Page-4: Item 4. QUALITY CONTROL X- The length must be 250 character or less for Available equipments that are in working condition")]
        [RoadStatusDependable]
        public string EQUIP_WORKING { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4. QUALITY CONTROL - Please enter Available equipments that are not in working condition ")]
        [StringLength(250, ErrorMessage = "Page-4: Item 4. QUALITY CONTROL XI- The length must be 250 character or less for Available equipments that are not in working condition")]
        [RoadStatusDependable]
        public string EQUIP_NOT_WORKING { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-6: Item 4. QUALITY CONTROL- X. Please enter List of equipments not available in field lab")]
        [StringLength(250, ErrorMessage = "Page-4: Item 4. QUALITY CONTROL- X. The length must be 250 character or less for List of equipments not available in field lab")]
        [RoadStatusDependable]
        public string LAB_EQUIP_NOT_AVBL { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-7: Item 4. QUALITY CONTROL XI. The length must be 250 character or less for Reasons put forth by PIU for non availability of equipments in field lab")]
        public string REASON_LAB_EQUIP_NOT_AVBL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-7: Item 4. QUALITY CONTROL XII. The length must be 250 character or less for Reasons put forth by PIU for non availability of equipments in field lab")]
        public string CALIBRATION_DETAILS { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: Item 4. QUALITY CONTROL XIII. Please enter Equipments and documents ready to be made available to QM before or during the inspection")]
        [StringLength(250, ErrorMessage = "Page-7: Item 4. QUALITY CONTROL XIII. The length must be 250 character or less for Equipments and documents ready to be made available to QM before or during the inspection")]
        public string DOCUMENT_FOR_QM { get; set; }
    }
}