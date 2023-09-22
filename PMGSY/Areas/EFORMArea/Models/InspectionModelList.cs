using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class InspectionModelList
    {

        public InspectionModelList()
        {
            this.InspectionList = new List<InspectionModelDetails>();


            InspectionModelDetails InspectionModelDetails = new InspectionModelDetails();
            InspectionModelDetails.InspectionType = "E";
            InspectionModelDetails.RowCount = 10;
            this.InspectionList.Add(InspectionModelDetails);

        }
        public List<InspectionModelDetails> InspectionList { get; set; }
    }

    public class InspectionModelDetails
    {
        public string InspectionType { get; set; }
        public int RowCount { get; set; }
    }




    public class NewTechModelList
    {
        public NewTechModelList()
        {
            this.NewTechList = new List<NewTechModelDetails>();


            NewTechModelDetails newTechListDetails = new NewTechModelDetails();
            newTechListDetails.NewTechType = "N";
             newTechListDetails.RowCount = 3;
            this.NewTechList.Add(newTechListDetails);

        }

        public List<NewTechModelDetails> NewTechList { get; set; }
    }

    public class NewTechModelDetails
    {
        public string NewTechType { get; set; }
        public int RowCount { get; set; }
    }
}