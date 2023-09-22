using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class BridgePrefilledModel
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
        public string NAME_BRIDGE_LOCATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string PACKAGE_NUMBER { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string QM_NAME_34 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string QM_NAME_35 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string IMS_PR_ROAD_CODE_35  { get; set; }
}
}