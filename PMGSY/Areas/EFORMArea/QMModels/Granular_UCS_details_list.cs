using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class Granular_UCS_details_list
    {
        public Granular_UCS_details_list()
        {
            this.Granular_UCS_List = new List<Granular_UCS_Model_Details>();


            Granular_UCS_Model_Details  ModelDetails = new Granular_UCS_Model_Details();
            ModelDetails.UCSType = "GSB";
            ModelDetails.RowCount = 3;
            this.Granular_UCS_List.Add(ModelDetails);

        }
        public List<Granular_UCS_Model_Details> Granular_UCS_List { get; set; }

    }
    public class Granular_UCS_Model_Details
    {
        public string UCSType { get; set; }
        public int RowCount { get; set; }
    }



    public class Granular_QOM_OBS_list
    {
        public Granular_QOM_OBS_list()
        {
            this.Granular_QOM_OBS_List = new List<Granular_QOM_OBS_Model_Details>();


            Granular_QOM_OBS_Model_Details ModelDetails = new Granular_QOM_OBS_Model_Details();
            ModelDetails.QOMType = "G-QOM";
            ModelDetails.RowCount = 3;
            this.Granular_QOM_OBS_List.Add(ModelDetails);

        }
        public List<Granular_QOM_OBS_Model_Details> Granular_QOM_OBS_List { get; set; }

    }
    public class Granular_QOM_OBS_Model_Details
    {
        public string QOMType { get; set; }
        public int RowCount { get; set; }
    }

    //----layer1-----
    public class Base_coarse_layer1_list
    {
        public Base_coarse_layer1_list()
        {
            this.Base_coarse_layer1 = new List<Base_coarse_layer1_Model_Details>();


            Base_coarse_layer1_Model_Details ModelDetails = new Base_coarse_layer1_Model_Details();
            ModelDetails.UCSType = "L1-UCS";
            ModelDetails.RowCount = 3;
            this.Base_coarse_layer1.Add(ModelDetails);

        }
        public List<Base_coarse_layer1_Model_Details> Base_coarse_layer1 { get; set; }

    }
    public class Base_coarse_layer1_Model_Details
    {
        public string UCSType { get; set; }
        public int RowCount { get; set; }
    }


    public class Base_coarse_layer1_workmanship_list
    {
        public Base_coarse_layer1_workmanship_list()
        {
            this.Base_coarse_workmanship_layer1 = new List<Base_coarse_layer1_workmanship_Model_Details>();


            Base_coarse_layer1_workmanship_Model_Details ModelDetails = new Base_coarse_layer1_workmanship_Model_Details();
            ModelDetails.WorkType = "L1-WS";
            ModelDetails.RowCount = 3;
            this.Base_coarse_workmanship_layer1.Add(ModelDetails);

        }
        public List<Base_coarse_layer1_workmanship_Model_Details> Base_coarse_workmanship_layer1 { get; set; }

    }
    public class Base_coarse_layer1_workmanship_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }


    //----layer2-----

    public class Base_coarse_layer2_list
    {
        public Base_coarse_layer2_list()
        {
            this.Base_coarse_layer2 = new List<Base_coarse_layer2_Model_Details>();


            Base_coarse_layer2_Model_Details ModelDetails = new Base_coarse_layer2_Model_Details();
            ModelDetails.UCSType = "L2-UCS";
            ModelDetails.RowCount = 3;
            this.Base_coarse_layer2.Add(ModelDetails);

        }
        public List<Base_coarse_layer2_Model_Details> Base_coarse_layer2 { get; set; }

    }
    public class Base_coarse_layer2_Model_Details
    {
        public string UCSType { get; set; }
        public int RowCount { get; set; }
    }


    public class Base_coarse_layer2_workmanship_list
    {
        public Base_coarse_layer2_workmanship_list()
        {
            this.Base_coarse_workmanship_layer2 = new List<Base_coarse_layer2_workmanship_Model_Details>();


            Base_coarse_layer2_workmanship_Model_Details ModelDetails = new Base_coarse_layer2_workmanship_Model_Details();
            ModelDetails.WorkType = "L2-WS";
            ModelDetails.RowCount = 3;
            this.Base_coarse_workmanship_layer2.Add(ModelDetails);

        }
        public List<Base_coarse_layer2_workmanship_Model_Details> Base_coarse_workmanship_layer2 { get; set; }

    }
    public class Base_coarse_layer2_workmanship_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }

    //----layer3-----
    public class Base_coarse_layer3_list
    {
        public Base_coarse_layer3_list()
        {
            this.Base_coarse_layer3 = new List<Base_coarse_layer3_Model_Details>();


            Base_coarse_layer3_Model_Details ModelDetails = new Base_coarse_layer3_Model_Details();
            ModelDetails.UCSType = "L3-UCS";
            ModelDetails.RowCount = 3;
            this.Base_coarse_layer3.Add(ModelDetails);

        }
        public List<Base_coarse_layer3_Model_Details> Base_coarse_layer3 { get; set; }

    }
    public class Base_coarse_layer3_Model_Details
    {
        public string UCSType { get; set; }
        public int RowCount { get; set; }
    }


    public class Base_coarse_layer3_workmanship_list
    {
        public Base_coarse_layer3_workmanship_list()
        {
            this.Base_coarse_workmanship_layer3 = new List<Base_coarse_layer3_workmanship_Model_Details>();


            Base_coarse_layer3_workmanship_Model_Details ModelDetails = new Base_coarse_layer3_workmanship_Model_Details();
            ModelDetails.WorkType = "L3-WS";
            ModelDetails.RowCount = 3;
            this.Base_coarse_workmanship_layer3.Add(ModelDetails);

        }
        public List<Base_coarse_layer3_workmanship_Model_Details> Base_coarse_workmanship_layer3 { get; set; }

    }
    public class Base_coarse_layer3_workmanship_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }




    //-----Page 23-27  ------

    public class EFORM_CHILD_CDWORKS_PIPE_CULVERTS_List
    {
        public EFORM_CHILD_CDWORKS_PIPE_CULVERTS_List()
        {
            this.CHILD_CDWORKS_PIPE_CULVERTS = new List<CHILD_CDWORKS_PIPE_CULVERTS_Model_Details>();


            CHILD_CDWORKS_PIPE_CULVERTS_Model_Details ModelDetails = new CHILD_CDWORKS_PIPE_CULVERTS_Model_Details();
            ModelDetails.WorkType = "CD-PC";
            ModelDetails.RowCount = 4;
            this.CHILD_CDWORKS_PIPE_CULVERTS.Add(ModelDetails);

        }
        public List<CHILD_CDWORKS_PIPE_CULVERTS_Model_Details> CHILD_CDWORKS_PIPE_CULVERTS { get; set; }

    }
    public class CHILD_CDWORKS_PIPE_CULVERTS_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }



    public class EFORM_CHILD_CDWORKS_SLAB_CULVERTS_List
    {

        public EFORM_CHILD_CDWORKS_SLAB_CULVERTS_List()
        {
            this.CHILD_CDWORKS_SLAB_CULVERTS = new List<CHILD_CDWORKS_SLAB_CULVERTS_Model_Details>();


            CHILD_CDWORKS_SLAB_CULVERTS_Model_Details ModelDetails = new CHILD_CDWORKS_SLAB_CULVERTS_Model_Details();
            ModelDetails.WorkType = "CD-SC";
            ModelDetails.RowCount = 4;
            this.CHILD_CDWORKS_SLAB_CULVERTS.Add(ModelDetails);

        }
        public List<CHILD_CDWORKS_SLAB_CULVERTS_Model_Details> CHILD_CDWORKS_SLAB_CULVERTS { get; set; }

    }
    public class CHILD_CDWORKS_SLAB_CULVERTS_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }



    public class EFORM_CHILD_PROT_WORKS_QOM_List
    {
       

        public EFORM_CHILD_PROT_WORKS_QOM_List()
        {
            this.CHILD_PROT_WORKS_QOM = new List<CHILD_PROT_WORKS_QOM_Model_Details>();


            CHILD_PROT_WORKS_QOM_Model_Details ModelDetails = new CHILD_PROT_WORKS_QOM_Model_Details();
            ModelDetails.WorkType = "PROT-QOM";
            ModelDetails.RowCount = 4;
            this.CHILD_PROT_WORKS_QOM.Add(ModelDetails);

        }
        public List<CHILD_PROT_WORKS_QOM_Model_Details> CHILD_PROT_WORKS_QOM { get; set; }

    }
    public class CHILD_PROT_WORKS_QOM_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }






    public class EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_List
    {
       

        public EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_List()
        {
            this.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS = new List<CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_Model_Details>();


            CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_Model_Details ModelDetails = new CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_Model_Details();
            ModelDetails.WorkType = "PROT-WORK";
            ModelDetails.RowCount = 4;
            this.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS.Add(ModelDetails);

        }
        public List<CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_Model_Details> CHILD_PROT_WORKS_WORKMANSHIP_OF_RS { get; set; }

    }
    public class CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }


    public class EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_List
    {


        public EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_List()
    {
        
        this.CHILD_CRASH_BARRIERS_OBSERVATION = new List<CHILD_CRASH_BARRIERS_OBSERVATION_Model_Details>();


            CHILD_CRASH_BARRIERS_OBSERVATION_Model_Details ModelDetails = new CHILD_CRASH_BARRIERS_OBSERVATION_Model_Details();
        ModelDetails.WorkType = "CCBO";
        ModelDetails.RowCount = 4;
        this.CHILD_CRASH_BARRIERS_OBSERVATION.Add(ModelDetails);

    }
    public List<CHILD_CRASH_BARRIERS_OBSERVATION_Model_Details> CHILD_CRASH_BARRIERS_OBSERVATION { get; set; }

}
public class CHILD_CRASH_BARRIERS_OBSERVATION_Model_Details
    {
    public string WorkType { get; set; }
    public int RowCount { get; set; }
}






    public class EFORM_CHILD_SD_AND_CW_DRAINS_List
    {
      

        public EFORM_CHILD_SD_AND_CW_DRAINS_List()
        {

            this.CHILD_SD_AND_CW_DRAINS = new List<CHILD_SD_AND_CW_DRAINS_Model_Details>();


            CHILD_SD_AND_CW_DRAINS_Model_Details ModelDetails = new CHILD_SD_AND_CW_DRAINS_Model_Details();
            ModelDetails.WorkType = "CSCW";
            ModelDetails.RowCount = 4;
            this.CHILD_SD_AND_CW_DRAINS.Add(ModelDetails);

        }
        public List<CHILD_SD_AND_CW_DRAINS_Model_Details> CHILD_SD_AND_CW_DRAINS { get; set; }

    }
    public class CHILD_SD_AND_CW_DRAINS_Model_Details
    {
        public string WorkType { get; set; }
        public int RowCount { get; set; }
    }


}