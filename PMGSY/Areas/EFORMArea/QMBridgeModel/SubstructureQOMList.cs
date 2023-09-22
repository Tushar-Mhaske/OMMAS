using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class SubstructureQOMList
    {
        public SubstructureQOMList()
        {
            this.QOMSubstruCtureList = new List<QOMModelList>();
            QOMModelList qOMModelList = new QOMModelList();
            qOMModelList.SubstructureType = "Q";
            qOMModelList.RowCount = 5;
            this.QOMSubstruCtureList.Add(qOMModelList);
        }

        public List<QOMModelList> QOMSubstruCtureList { get; set; }


    }

    public class QOMModelList
    {
        public string SubstructureType { get; set; }
        public int RowCount { get; set; }
    }
}