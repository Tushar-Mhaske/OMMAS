#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MasterReportsBAL.cs        
        * Description   :   Listing of Records for all Form Reports
        * Author        :   Pranav Nerkar 
        * Creation Date :   4/October/2013
 **/
#endregion

using PMGSY.DAL.MasterReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.MasterReports
{
    public class MasterReportsBAL : IMasterReportsBAL
    {
        IMasterReportsDAL masterReportsDAL = new MasterReportsDAL();
        public Array StateDetailsListingBAL(string stateOrUnion, string stateType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.StateDetailsListingDAL(stateOrUnion, stateType,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array DistrictDetailsListingBAL(int stateCode, string pmgsyIncluded, string iapDistrict, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.DistrictDetailsListingDAL(stateCode, pmgsyIncluded, iapDistrict,activeType,page, rows, sidx, sord, out totalRecords);
        }


        public Array BlockDetailsListingBAL(int districtCode, int stateCode, string isDesert, string isTribal, string pmgsyIncluded, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.BlockDetailsListingDAL(districtCode, stateCode, isDesert, isTribal, pmgsyIncluded, isSchedule5,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array VillageDetailsListingBAL(int censusYear, int blockCode, int districtCode, int stateCode, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.VillageDetailsListingDAL(censusYear, blockCode, districtCode, stateCode, isSchedule5,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array HabitationDetailsListingBAL(int censusYear, int villageCode, int blockCode, int districtCode, int stateCode, string habitationStatus, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.HabitationDetailsListingDAL(censusYear, villageCode, blockCode, districtCode, stateCode, habitationStatus, isSchedule5,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPConstituencyListingBAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.MPConstituencyListingDAL(stateCode,activeType, page, rows, sidx, sord,out totalRecords);
        }


        public Array MLAConstituencyListingBAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.MLAConstituencyListingDAL(stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array MPBlockListingBAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.MPBlockListingDAL(constCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array MLABlockListingBAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.MLABlockListingDAL(constCode,stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array PanchayatListingBAL(int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.PanchayatListingDAL(blockCode, districtCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array PanchayatHabitationListingBAL(int panchayatCode, int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.PanchayatHabitationListingDAL(panchayatCode, blockCode, districtCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array RegionListingBAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.RegionListingDAL(stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array RegionDistrictListingBAL(int regionCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.RegionDistrictListingDAL(regionCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array UnitListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.UnitListingDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array RoadCategoryListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.RoadCategoryListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array ScourFoundationListingBAL(string scourFoundationType,int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ScourFoundationListingDAL(scourFoundationType,page, rows, sidx, sord, out totalRecords);
        }


        public Array SoilListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.SoilListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array StreamListingBAL(string streamType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.StreamListingDAL(streamType, page, rows, sidx, sord, out totalRecords);
        }


        public Array TerrainListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TerrainListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array CDWorksLengthListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.CDWorksLengthListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array CDWorksTypeListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.CDWorksTypeListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array ComponentListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ComponentListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array GradeListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.GradeListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array DesignationListingBAL(string designationType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.DesignationListingDAL(designationType, page, rows, sidx, sord, out totalRecords);
        }


        public Array ReasonListingBAL(string reasonType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ReasonListingDAL(reasonType, page, rows, sidx, sord, out totalRecords);
        }


        public Array CheckListPointListingBAL(string checkListActive, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.CheckListPointListingDAL(checkListActive, page, rows, sidx, sord, out totalRecords);
        }


        public Array ExecutionItemListingBAL(string headType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ExecutionItemListingDAL(headType, page, rows, sidx, sord, out totalRecords);
        }


        public Array TaxesListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TaxesListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array AgencyListingBAL(string agencyType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.AgencyListingDAL(agencyType, page, rows, sidx, sord, out totalRecords);
        }


        public Array TrafficListingBAL(string trafficStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TrafficListingDAL(trafficStatus, page, rows, sidx, sord, out totalRecords);
        }


        public Array FundingAgencyListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.FundingAgencyListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array QualificationListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.QualificationListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array LokSabhaTermListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.LokSabhaTermListingDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array ContractorSupplierListingBAL(int stateCode, string contractorSupplierFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ContractorSupplierListingDAL(stateCode,contractorSupplierFlag, contractStatus,page, rows, sidx, sord, out totalRecords);
        }


        public Array AutonomousBodyListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.AutonomousBodyListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ContractorClassTypeListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ContractorClassTypeListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ContractorRegistrationListingBAL(string activeStatus, string registrationStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ContractorRegistrationListingDAL(activeStatus, registrationStatus, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array SQCListingBAL(string activeStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.SQCListingDAL(activeStatus,stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array SRRDAListingBAL(int stateCode, int agencyCode, string officeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.SRRDAListingDAL(stateCode,agencyCode,officeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array VidhanSabhaTermListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.VidhanSabhaTermListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array NodalOfficerListingBAL(int stateCode, int agencyCode, string officeType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.NodalOfficerListingDAL(stateCode,agencyCode,officeType,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array QualityMonitorListingBAL(int stateCode, string qmType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.QualityMonitorListingDAL(stateCode,qmType,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array TechnicalAgencyListingBAL(int stateCode, string taType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TechnicalAgencyListingDAL(stateCode,taType, page, rows, sidx, sord, out totalRecords);
        }

        public Array TechnicalAgencyStateMappingListingBAL(int stateCode, string taType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TechnicalAgencyStateMappingListingDAL(stateCode, taType,activeType, page, rows, sidx, sord, out totalRecords);
        }

        public Array MLAMemberListingBAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.MLAMemberListingDAL(constituency, term, stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array MPMemberListingBAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.MPMemberListingDAL(constituency, term, stateCode,activeType, page, rows, sidx, sord, out totalRecords);
        }


        public Array SurfaceListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.SurfaceListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array OfficerCategoryListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.OfficerCategoryListingDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array ContractorRegistrationBankListingBAL(int stateCode, string contractRegFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.ContractorRegistrationBankListingDAL(stateCode, contractRegFlag, contractStatus, page, rows, sidx, sord, out totalRecords);
        }
        public Array DepartmentDistrictListingBAL(int stateCode, int agencyCode, string officeType,int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.DepartmentDistrictListingDAL(stateCode, agencyCode, officeType,page, rows, sidx, sord, out totalRecords);
        }
        public Array CarriageListingBAL(string carriageStatus, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.CarriageListingDAL(carriageStatus,page, rows, sidx, sord, out totalRecords);
        }
        public Array QMItemsListingBAL(string qmType, string qmItemActive, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.QMItemsListingDAL(qmType,qmItemActive,page, rows, sidx, sord, out totalRecords);
        }
        public Array TechnologyListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TechnologyListingDAL(page, rows, sidx, sord, out totalRecords);
        }
        public Array TestListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.TestListingDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array FeedbackListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return masterReportsDAL.FeedbackListingDAL(page, rows, sidx, sord, out totalRecords);
        }
    }

    public interface IMasterReportsBAL
    {
        Array StateDetailsListingBAL(string stateOrUnion, string stateType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array DistrictDetailsListingBAL(int stateCode, string pmgsyIncluded, string iapDistrict, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array BlockDetailsListingBAL(int districtCode, int stateCode, string isDesert, string isTribal, string pmgsyIncluded, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array VillageDetailsListingBAL(int censusYear, int blockCode, int districtCode, int stateCode, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HabitationDetailsListingBAL(int censusYear, int villageCode, int blockCode, int districtCode, int stateCode, string habitationStatus, string isSchedule5, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPConstituencyListingBAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MLAConstituencyListingBAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPBlockListingBAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MLABlockListingBAL(int constCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PanchayatListingBAL(int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PanchayatHabitationListingBAL(int panchayatCode, int blockCode, int districtCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RegionListingBAL(int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RegionDistrictListingBAL(int regionCode, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array UnitListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array RoadCategoryListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ScourFoundationListingBAL(string scourFoundationType,int page, int rows, string sidx, string sord, out int totalRecords);
        Array SoilListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array StreamListingBAL(string streamType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TerrainListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array CDWorksLengthListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array CDWorksTypeListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ComponentListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array GradeListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array DesignationListingBAL(string designationType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ReasonListingBAL(string reasonType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CheckListPointListingBAL(string checkListActive,int page, int rows, string sidx, string sord, out int totalRecords);
        Array ExecutionItemListingBAL(string headType,int page, int rows, string sidx, string sord, out int totalRecords);
        Array TaxesListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array AgencyListingBAL(string agencyType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TrafficListingBAL(string trafficStatus, int page, int rows, string sidx, string sord, out int totalRecords);
        Array FundingAgencyListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array QualificationListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array LokSabhaTermListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ContractorSupplierListingBAL(int stateCode,string contractorSupplierFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords);

        Array AutonomousBodyListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ContractorClassTypeListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ContractorRegistrationListingBAL(string activeStatus, string registrationStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array SQCListingBAL(string activeStatus, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array SRRDAListingBAL(int stateCode,int agencyCode,string officeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array VidhanSabhaTermListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array NodalOfficerListingBAL(int stateCode, int agencyCode, string officeType,string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array QualityMonitorListingBAL(int stateCode, string qmType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array TechnicalAgencyListingBAL(int stateCode, string taType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TechnicalAgencyStateMappingListingBAL(int stateCode, string taType, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array MLAMemberListingBAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array MPMemberListingBAL(string constituency, string term, int stateCode, string activeType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array SurfaceListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array OfficerCategoryListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ContractorRegistrationBankListingBAL(int stateCode, string contractRegFlag, string contractStatus, int page, int rows, string sidx, string sord, out int totalRecords);
        Array DepartmentDistrictListingBAL(int stateCode, int agencyCode, string officeType,int page, int rows, string sidx, string sord, out int totalRecords);
        Array CarriageListingBAL(string carriageStatus, int page, int rows, string sidx, string sord, out int totalRecords);
        Array QMItemsListingBAL(string qmType, string qmItemActive, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TechnologyListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array TestListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array FeedbackListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
    }
}