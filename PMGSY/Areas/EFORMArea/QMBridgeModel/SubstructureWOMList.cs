using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class SubstructureWOMList
    {
        public SubstructureWOMList()
        {
            this.WOMSubstruCtureList = new List<WOMModelList>();
            WOMModelList qOMModelList = new WOMModelList();
            qOMModelList.SubstructureType = "W";
            qOMModelList.RowCount = 5;
            this.WOMSubstruCtureList.Add(qOMModelList);
        }
        public List<WOMModelList> WOMSubstruCtureList { get; set; }
    }

    public class WOMModelList
    {
        public string SubstructureType { get; set; }
        public int RowCount { get; set; }
    }
}