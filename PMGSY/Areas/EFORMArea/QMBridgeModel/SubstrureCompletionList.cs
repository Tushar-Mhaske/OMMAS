using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class SubstrureCompletionList
    {

        public SubstrureCompletionList()
        {
            this.SubstruCtureCompletedList = new List<ComplSubstruModelList>();
            ComplSubstruModelList qOMModelList = new ComplSubstruModelList();
            qOMModelList.SubstructureType = "M";
            qOMModelList.RowCount = 14;
            this.SubstruCtureCompletedList.Add(qOMModelList);
        }
        public List<ComplSubstruModelList> SubstruCtureCompletedList { get; set; }
    }

    public class ComplSubstruModelList
    {
        public string SubstructureType { get; set; }
        public int RowCount { get; set; }
    }
}