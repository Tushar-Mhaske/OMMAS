using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Master
{
    public class FinancialYearTargetViewModel
    {
        
        public int state { get; set; }
        public string stateName { get; set; }
        public int year { get; set; }
        public string Month { get; set; }
        public decimal PMGSYILength { get; set; }
        public decimal PMGSYIILength { get; set; }
        public decimal RCPLWELength { get; set; }
        public decimal PMGSYIIILength { get; set; }

        



        private static readonly IEnumerable<SelectListItem> months = new SelectList(

       new List<int> { 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3 }
            //.OrderBy(month => month)
       .Select(month => new SelectListItem
       {
           Value = CultureInfo.CurrentCulture.
           DateTimeFormat.GetMonthName
           (month),//month.ToString(CultureInfo.InvariantCulture),
           Text = month < 10 ? string.Format("0{0}", month) : month.ToString(CultureInfo.InvariantCulture)


       }),

       "Value",
       "Text");

        public IEnumerable<SelectListItem> Months
        {
            get
            {
                return months;
            }
        }

    }
}