using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class OffcialModelList
    {
 

        public OffcialModelList(bool RoadStatusIsCompleted)
        {
            this.OfficialList = new List<OfficialModelDetails>();

            if (RoadStatusIsCompleted != true)
            {

                OfficialModelDetails officialModelDetailsC = new OfficialModelDetails();
                officialModelDetailsC.OfficialType = "C";
                officialModelDetailsC.RowCount = 2;
                this.OfficialList.Add(officialModelDetailsC);

                OfficialModelDetails officialModelDetailsL = new OfficialModelDetails();
                officialModelDetailsL.OfficialType = "L";
                officialModelDetailsL.RowCount = 2;
                this.OfficialList.Add(officialModelDetailsL);

                OfficialModelDetails officialModelDetailsE = new OfficialModelDetails();
                officialModelDetailsE.OfficialType = "E";
                officialModelDetailsE.RowCount = 3;
                this.OfficialList.Add(officialModelDetailsE);
            }
            
                OfficialModelDetails officialModelDetailsJ = new OfficialModelDetails();
                officialModelDetailsJ.OfficialType = "J";
                officialModelDetailsJ.RowCount = 3;
                this.OfficialList.Add(officialModelDetailsJ);

                OfficialModelDetails officialModelDetailsA = new OfficialModelDetails();
                officialModelDetailsA.OfficialType = "A";
                officialModelDetailsA.RowCount = 3;
                this.OfficialList.Add(officialModelDetailsA);

                OfficialModelDetails officialModelDetailsS = new OfficialModelDetails();
                officialModelDetailsS.OfficialType = "S";
                officialModelDetailsS.RowCount = 3;
                this.OfficialList.Add(officialModelDetailsS);
            

           

          

            //C: CONTRACTOR ,
            //L: LAB TECHNICIAN, 
            //J : jUNIOR ENGG,
            //A:ASSISTANT ENGG, 
            //S: SUPERVISOR,
            //E: Contractor Engg,
        }
        public List<OfficialModelDetails> OfficialList { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }
    }

    public class OfficialModelDetails
    {
        public string OfficialType { get; set; }
        public int RowCount { get; set; }
    }

    //change for bridge
    public class OffcialModelListBridge
    {


        public OffcialModelListBridge(bool RoadStatusIsCompleted)
        {
            this.OfficialList = new List<OfficialModelDetails>();

            if (RoadStatusIsCompleted != true)
            {

                OfficialModelDetails officialModelDetailsC = new OfficialModelDetails();
                officialModelDetailsC.OfficialType = "C";
                officialModelDetailsC.RowCount = 1;
                this.OfficialList.Add(officialModelDetailsC);


                OfficialModelDetails officialModelDetailsE = new OfficialModelDetails();
                officialModelDetailsE.OfficialType = "E";
                officialModelDetailsE.RowCount = 3;
                this.OfficialList.Add(officialModelDetailsE);
            }

            OfficialModelDetails officialModelDetailsJ = new OfficialModelDetails();
            officialModelDetailsJ.OfficialType = "J";
            officialModelDetailsJ.RowCount = 3;
            this.OfficialList.Add(officialModelDetailsJ);

            OfficialModelDetails officialModelDetailsA = new OfficialModelDetails();
            officialModelDetailsA.OfficialType = "A";
            officialModelDetailsA.RowCount = 3;
            this.OfficialList.Add(officialModelDetailsA);

            OfficialModelDetails officialModelDetailsS = new OfficialModelDetails();
            officialModelDetailsS.OfficialType = "S";
            officialModelDetailsS.RowCount = 3;
            this.OfficialList.Add(officialModelDetailsS);






            //C: CONTRACTOR ,
            //L: LAB TECHNICIAN, 
            //J : jUNIOR ENGG,
            //A:ASSISTANT ENGG, 
            //S: SUPERVISOR,
            //E: Contractor Engg,
        }
        public List<OfficialModelDetails> OfficialList { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TemplateStatus { get; set; }
    }

    public class OfficialModelDetailsBridge
    {
        public string OfficialType { get; set; }
        public int RowCount { get; set; }
    }

    //end here change for bridge




}