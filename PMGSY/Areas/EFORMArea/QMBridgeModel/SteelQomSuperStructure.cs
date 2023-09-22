using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class SteelQomSuperStructure
    {
        public SteelQomSuperStructure()
        {
            this.QomSuperStructureList = new List<QomModelList>();
            QomModelList qOMModelList = new QomModelList();
            qOMModelList.SuperStructureType = "S";
            qOMModelList.RowCount = 7;
            this.QomSuperStructureList.Add(qOMModelList);
        }
        public List<QomModelList> QomSuperStructureList { get; set; }
    }

    public class SteelCompletedMaterialuperstructure
    {
        public SteelCompletedMaterialuperstructure()
        {
            this.MaterialCompleteSuperStructureList = new List<QomModelList>();
            QomModelList qOMModelList = new QomModelList();
            qOMModelList.SuperStructureType = "S";
            qOMModelList.RowCount = 7;
            this.MaterialCompleteSuperStructureList.Add(qOMModelList);
        }
        public List<QomModelList> MaterialCompleteSuperStructureList { get; set; }
    }

    public class RCCSuperStructure
    {
        public RCCSuperStructure()
        {
            this.QomRCCStructureList = new List<QomModelList>();
            QomModelList qOMModelList = new QomModelList();
            qOMModelList.SuperStructureType = "R";
            qOMModelList.RowCount = 7;
            this.QomRCCStructureList.Add(qOMModelList);
        }
        public List<QomModelList> QomRCCStructureList { get; set; }
    }

    public class RCCCompletedSuperStructure
    {
        public RCCCompletedSuperStructure()
        {
            this.RCCCompletedStructureList = new List<QomModelList>();
            QomModelList qOMModelList = new QomModelList();
            qOMModelList.SuperStructureType = "R";
            qOMModelList.RowCount = 7;
            this.RCCCompletedStructureList.Add(qOMModelList);
        }
        public List<QomModelList> RCCCompletedStructureList { get; set; }
    }

    public class BaileySuperstructure
    {
        public BaileySuperstructure()
        {
            this.BaileyQOMStructureList = new List<QomModelList>();
            QomModelList qOMModelList = new QomModelList();
            qOMModelList.SuperStructureType = "B";
            qOMModelList.RowCount = 7;
            this.BaileyQOMStructureList.Add(qOMModelList);
        }
        public List<QomModelList> BaileyQOMStructureList { get; set; }
    }

    public class BaileyCompleteStructure
    {
        public BaileyCompleteStructure()
        {
            this.BaileyCompleteStructureList = new List<QomModelList>();
            QomModelList qOMModelList = new QomModelList();
            qOMModelList.SuperStructureType = "B";
            qOMModelList.RowCount = 7;
            this.BaileyCompleteStructureList.Add(qOMModelList);
        }
        public List<QomModelList> BaileyCompleteStructureList { get; set; }
    }
    public class QomModelList
    {
        public string SuperStructureType { get; set; }
        public int RowCount { get; set; }
    }
}