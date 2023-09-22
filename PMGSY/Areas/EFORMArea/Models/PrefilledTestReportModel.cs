using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.Models
{
    public class PrefilledTestReportModel
    {
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string checksum { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string RoadCode { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string Road_Name { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string TESTED_BY_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


    }
}