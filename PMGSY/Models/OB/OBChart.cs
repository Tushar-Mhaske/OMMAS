using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.OB
{
    public class OBChart
    {
        public Int16 Id { get; set; }
        public String TransDesc { get; set; }
        public Decimal? Amount { get; set; }
    }

    public class JsonCollection
    {
        public List<Dictionary<String, String>> assetList { get; set; }
        public List<Dictionary<String, String>> libList { get; set; }
    }
}