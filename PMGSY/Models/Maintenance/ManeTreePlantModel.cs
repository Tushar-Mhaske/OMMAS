using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMGSY.Model.Maintenance
{
    public class ManeTreePlantModel
    {

        public int TREE_PLANT_ID { get; set; }
        public int IMS_PR_ROAD_CODE { get; set; }

        [Range(2000, Int32.MaxValue, ErrorMessage = "Please select Year")]
        public int TREE_PLANT_YEAR { get; set; }

        [Range(1, 12, ErrorMessage = "Please select Month")]
        public int TREE_PLANT_MONTH { get; set; }

        public string TREE_PLANT_MONTH_NAME { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Enter No of New Plant")]

        public int TREE_PLANT_NEW { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Please Enter No of Old Plant")]
        public int TREE_PLANT_OLD { get; set; }
        public long? SrNo { get; set; }

        public bool IsFirst { get; set; }

        public int verifyCount { get; set; }
        public string Verify { get; set; }
    }
}
