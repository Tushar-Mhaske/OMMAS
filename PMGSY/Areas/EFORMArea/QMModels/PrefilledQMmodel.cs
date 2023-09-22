using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

 
using System.ComponentModel.DataAnnotations;
 
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class PrefilledQMmodel
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string checksum { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string RoadCode { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string WORK_STATUS { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string QM_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string QM_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string STATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string DISTRICT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string BLOCK { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string PACKAGE_NUMBER { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string CURRENT_STAGE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string NAME_OF_QM_38 { get; set; }
    }
}