using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_QC_DETAILS_PIU_View
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter Location of field laboratory ")]
        public string LAB_LOCATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter Geo-tagged photograph of laboratory uploaded on ")]
        public string PHOTO_UPLOAD_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter Reason for delay in establishment of field laboratory(if so) ")]
        public string ESTB_DELAY_REASON { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter  List of equipments available in field lab")]
        public string LAB_EQUIP_AVBL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter Available equipments that are in working condition  ")]
        public string EQUIP_WORKING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter Available equipments that are not in working ")]
        public string EQUIP_NOT_WORKING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter List of equipments not available in field lab ")]
        public string LAB_EQUIP_NOT_AVBL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter Reasons put forth by PIU for non availability of equipments in field lab")]
        public string REASON_LAB_EQUIP_NOT_AVBL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter  Equipments and documents ready to be made available to QM before or during the inspection ")]
        public string DOCUMENT_FOR_QM { get; set; }

    }
}