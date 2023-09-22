using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PDFPreFilledDataMutatorModel
    {
        public PdfLoadedForm LoadedForm { get; set; }
        public string FieldName { get; set; }
        public string Value { get; set; }
        public PDFFieldType FieldType { get; set; }
        public bool IsReadOnly { get; set; }
    }
}