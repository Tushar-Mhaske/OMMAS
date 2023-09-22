using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Common
{
    public class GridParams
    {
        public bool Search { get; set; }
        public long Nd { get; set; }
        public int? Page { get; set; }
        public int? Rows { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }

        public GridParams(int? page, int? rows, string sidx, string sord, bool search, long nd)
        {
            this.Search = search;
            this.Nd = nd;
            this.Page = page;
            this.Rows = rows;
            this.Sidx = sidx;
            this.Sord = sord;
        }
    }

}