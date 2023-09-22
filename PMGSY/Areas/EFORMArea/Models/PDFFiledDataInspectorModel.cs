using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PDFFiledDataInspectorModel
    {
        public PdfLoadedForm LoadedForm { get; set; }
        public string FieldName { get; set; }
        public object Value { get; set; }
        public PDFFieldType FieldType { get; set; }
        public PdfField tooltip { get; set; }
    }
}