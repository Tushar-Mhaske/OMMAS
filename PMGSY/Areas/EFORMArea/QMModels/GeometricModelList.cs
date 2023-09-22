using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class GeometricModelList
    {
        public GeometricModelList()
        {
            this.GeometricList = new List<GeometricModelDetails>();

            GeometricModelDetails GeometricModelDetailsR = new GeometricModelDetails();
            GeometricModelDetailsR.GeometricType = "R";
            GeometricModelDetailsR.RowCount = 3;
            this.GeometricList.Add(GeometricModelDetailsR);

            GeometricModelDetails GeometricModelDetailsS = new GeometricModelDetails();
            GeometricModelDetailsS.GeometricType = "S";
            GeometricModelDetailsS.RowCount = 3;
            this.GeometricList.Add(GeometricModelDetailsS);


            GeometricModelDetails GeometricModelDetailsL = new GeometricModelDetails();
            GeometricModelDetailsL.GeometricType = "L";
            GeometricModelDetailsL.RowCount = 3;
            this.GeometricList.Add(GeometricModelDetailsL);



            //R: Roadway, 
            //L: LONGITUDINAL GRADING, 
            //S : SUPER ELEVATION,
           
        }

        public List<GeometricModelDetails> GeometricList { get; set; }
    }

    public class GeometricModelDetails
    {
        public string GeometricType { get; set; }
        public int RowCount { get; set; }
    }


    //add on 29-07-2022
    public class GeometricModelList_Temp2_0
    {
        public GeometricModelList_Temp2_0()
        {
            this.GeometricList = new List<GeometricModelDetails_Temp2_0>();

            GeometricModelDetails_Temp2_0 GeometricModelDetailsR = new GeometricModelDetails_Temp2_0();
            GeometricModelDetailsR.GeometricType = "R";

            GeometricModelDetailsR.RowCount = 8;
            this.GeometricList.Add(GeometricModelDetailsR);

            GeometricModelDetails_Temp2_0 GeometricModelDetailsS = new GeometricModelDetails_Temp2_0();
            GeometricModelDetailsS.GeometricType = "S";

            GeometricModelDetailsS.RowCount = 6;
            this.GeometricList.Add(GeometricModelDetailsS);


            GeometricModelDetails_Temp2_0 GeometricModelDetailsL = new GeometricModelDetails_Temp2_0();
            GeometricModelDetailsL.GeometricType = "L";
            GeometricModelDetailsL.RowCount = 3;
            this.GeometricList.Add(GeometricModelDetailsL);



            //R: Roadway, 
            //L: LONGITUDINAL GRADING, 
            //S : SUPER ELEVATION,

        }

        public List<GeometricModelDetails_Temp2_0> GeometricList { get; set; }
    }

    public class GeometricModelDetails_Temp2_0
    {
        public string GeometricType { get; set; }
        public int RowCount { get; set; }
    }
}