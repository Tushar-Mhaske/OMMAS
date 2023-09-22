using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class FieldTypeAttribute : Attribute
    {
        public PDFFieldType PropertyType { get; set; }
    }
}