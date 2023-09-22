/*----------------------------------------------------------------------------------------
 * Project Id    :

 * Project Name  :OMMAS-II

 * File Name     : MasterBAL.cs

 * Author        :Vikram Nandanwar, Rohit Jadhav , Ashish Markande, Abhishek Kamble.
 
 * Creation Date :01/May/2013

 * Desc          : This class is used to call methods from data access layer class.  
 
 * ---------------------------------------------------------------------------------------*/
using PMGSY.DAL.Master;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Master;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.BAL.Master
{
    public class MasterBAL : IMasterBAL
    {
        // IMasterDAL objDAL = new MasterDAL();
        IMasterDAL objDAL = null;

        public List<SelectListItem> GetDepartListBAL(int id)
        {
            MasterDAL objDAL = new MasterDAL();
            return objDAL.GetDepartmentListDAL(id);
        }

        public List<SelectListItem> GetDeptOfStates(int id)
        {
            MasterDAL objDAL = new MasterDAL();
            return objDAL.GetDeptListForStates(id);

        }

        #region Master CD Works Types Construction

        //--------------Master CD Works Types Construction-------------------//

        public bool AddConstructionType(CDWorksConstructionViewModel constructionModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddConstructionType(constructionModel, ref message);
        }

        public Array ListConstructionType(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListConstructionType(page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteConstructionType(int constructionCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteConstructionType(constructionCode);
        }

        public bool EditConstructionType(CDWorksConstructionViewModel constructionModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditConstructionType(constructionModel, ref message);
        }

        public CDWorksConstructionViewModel GetConstructionTypeDetails(int constructionCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetConstructionTypeDetails(constructionCode);
        }

        #endregion

        #region Master CD Works Types

        //------------------Master CD Works Types------------------//

        public bool AddCdWorks(CDWorksViewModel cdworksViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddCdWorks(cdworksViewModel, ref message);
        }
        public Array ListCdWorks(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListCdWorks(page, rows, sidx, sord, out totalRecords);

        }
        public bool DeleteCdWorks(int cdWorksType)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteCdWorks(cdWorksType);
        }
        public bool EditCdWorks(CDWorksViewModel cdworksViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditCdWorks(cdworksViewModel, ref message);
        }
        public CDWorksViewModel GetCdWorksDetails(int cdWorksCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetCdWorksDetails(cdWorksCode);
        }

        #endregion

        #region Master Funding Agency

        //------------------Master Funding Agency-----------------//

        public bool AddFundingAgency(FundingAgencyViewModel fundingModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddFundingAgency(fundingModel, ref message);
        }

        public Array ListFundingAgency(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListFundingAgency(page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteFundingAgency(int fundCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteFundingAgency(fundCode);
        }

        public bool EditFundingAgency(FundingAgencyViewModel fundingModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditFundingAgency(fundingModel, ref message);
        }

        public FundingAgencyViewModel GetFundingAgencyDetails(int fundCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetFundingAgencyDetails(fundCode);
        }

        #endregion

        #region Master Road Category

        //---------------- Master Road Category------------------//

        public bool AddRoadCategory(RoadCategoryViewModel cdworksViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddRoadCategory(cdworksViewModel, ref message);
        }
        public Array ListRoadCategory(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListRoadCategory(page, rows, sidx, sord, out totalRecords);
        }
        public bool DeleteRoadCategory(int cdWorksType)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteRoadCategory(cdWorksType);
        }
        public bool EditRoadCategory(RoadCategoryViewModel cdworksViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditRoadCategory(cdworksViewModel, ref message);
        }

        public RoadCategoryViewModel GetRoadDetails(int trafficCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetRoadDetails(trafficCode);
        }

        #endregion

        #region Master Soil Type

        //----------------Master Soil Type--------------------//


        public bool AddSoilType(SoilTypeViewModel soilTypeModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddSoilType(soilTypeModel, ref message);
        }

        public Array ListSoilType(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListSoilType(page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteSoilType(int soilCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteSoilType(soilCode);
        }

        public bool EditSoilType(SoilTypeViewModel soilTypeModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditSoilType(soilTypeModel, ref message);
        }

        public SoilTypeViewModel GetSoilDetails(int soilCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetSoilDetails(soilCode);
        }

        #endregion

        #region Master Traffic Type

        //--------------------Master Traffic Type-------------------------//

        public bool AddTrafficType(TrafficTypeViewModel trafficModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddTrafficType(trafficModel, ref message);
        }

        public Array ListTrafficType(string statusType,int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListTrafficType(statusType,page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteTrafficType(int agencyCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteTrafficType(agencyCode);
        }

        public bool EditTrafficType(TrafficTypeViewModel trafficModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditTrafficType(trafficModel, ref message);
        }

        public TrafficTypeViewModel EditTrafficType(string id)
        {
            objDAL = new MasterDAL();
            return objDAL.EditTrafficType(id);
        }

        public TrafficTypeViewModel GetTrafficDetails(int trafficCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetTrafficDetails(trafficCode);
        }

        public bool ChangeTrafficType(int trafficCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangeTrafficType(trafficCode);
        }

        #endregion

        #region Master Contractor

        //----------------Master Contractor---------------------//

        public MasterContractorViewModel EditContractor(int id)
        {
            objDAL = new MasterDAL();
            return objDAL.EditContractor(id);
        }

        public bool AddContractor(MasterContractorViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddContractor(model, ref message);
        }

        public bool EditContractor(MasterContractorViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditContractor(model, ref message);
        }

        public Array ListContractor(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListContractor(page, rows, sidx, sord, out totalRecords);
        }

        public Array GetContractorList(string state, string district, string contractorName, string status, string contrsuppType, string panno, int page, int rows, string sidx, string sord, out long totalRecords, string filters)
        {
            objDAL = new MasterDAL();
            return objDAL.GetContractorList(state, district, contractorName, status, contrsuppType, panno, page, rows, sidx, sord, out totalRecords, filters);
        }

        public bool DeleteContractor(int id)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteContractor(id);
        }

        public Array GetContractorRegistrationList(int id, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetContractorRegistrationList(id, page, rows, sidx, sord, out totalRecords);
        }

        public bool PanNumberSearchExistBAL(string panNumber, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.PanNumberSearchExistDAL(panNumber, ref message);
        }

        #endregion

        #region Master Designation

        //--------------- Master Designation---------------------//

        public Array GetDesignationList(string desigCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetDesignationList(desigCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool AddDesignation(MasterDesignationViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddDesignation(model, ref message);
        }

        public bool EditDesignation(MasterDesignationViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditDesignation(model, ref message);
        }

        public bool DeleteDesignation(int desigCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteDesignation(desigCode);
        }

        public MasterDesignationViewModel GetDesignationDetails(string desigCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetDesignationDetails(desigCode);
        }


        #endregion

        #region Master Lok Sabha Term

        //--------------- Master Lok Sabha Term---------------------//

        public Array GetLokSabhaTermList(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetLokSabhaTermList(page, rows, sidx, sord, out totalRecords);
        }

        public bool AddLokSabhaTerm(MasterLokSabhaTermViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddLokSabhaTerm(model, ref message);
        }

        public bool EditLokSabhaTerm(MasterLokSabhaTermViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditLokSabhaTerm(model, ref message);
        }

        public bool DeleteLokSabhaTerm(int loksabhaTerm)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteLokSabhaTerm(loksabhaTerm);
        }

        public MasterLokSabhaTermViewModel GetLokSabhaTermDetails(int loksabhaTerm)
        {
            objDAL = new MasterDAL();
            return objDAL.GetLokSabhaTermDetails(loksabhaTerm);
        }

        #endregion

        #region Master Qualification
        //--------------- Master Qualification---------------------//
        public bool AddMasterQualification(MasterQualificationViewModel masterQualViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterQualification(masterQualViewModel, ref message);
        }
        public bool EditMasterQualification(MasterQualificationViewModel masterQualViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterQualification(masterQualViewModel, ref message);
        }
        public Boolean DeleteMasterQualification(int qualId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterQualification(qualId);
        }
        public Array ListMasterQualification(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterQualification(page, rows, sidx, sord, out totalRecords);
        }
        public MasterQualificationViewModel GetQualificationDetails_ByQualCode(int QualCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetQualificationDetails_ByQualCode(QualCode);
        }



        #endregion master qualification


        #region Master Streams
        //---------------Master Streams---------------------//
        public bool AddMasterStreams(MasterStreamsViewModel masterStreamsViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterStreams(masterStreamsViewModel, ref message);
        }


        public bool EditMasterStreams(MasterStreamsViewModel masterStreamsViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterStreams(masterStreamsViewModel, ref message);
        }

        public Boolean DeleteMasterStreams(int streamsId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterStreams(streamsId);
        }

        public Array ListMasterStreams(string streamType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterStreams(streamType, page, rows, sidx, sord, out totalRecords);
        }

        public MasterStreamsViewModel GetStreamsDetails_ByStream(int StreamCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetStreamsDetails_ByStream(StreamCode);
        }

        public List<SelectListItem> GetStreamsCode()
        {
            objDAL = new MasterDAL();
            return objDAL.GetStreamsCode();
        }



        #endregion MasterStreams

        #region Master Unit
        //---------------Master Unit---------------------//
        public bool AddMasterUnit(MasterUnitsTypeViewModel masterUnitViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterUnit(masterUnitViewModel, ref message);
        }

        public bool EditMasterUnit(MasterUnitsTypeViewModel masterUnitsViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterUnit(masterUnitsViewModel, ref message);
        }

        public Boolean DeleteMasterUnit(int masterUnitId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterUnit(masterUnitId, ref message);
        }

        public Array ListMasterUnit(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterUnit(page, rows, sidx, sord, out totalRecords);
        }

        public MasterUnitsTypeViewModel GetUnitDetails_ByUnitCode(int unitCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetUnitDetails_ByUnitCode(unitCode);
        }
        #endregion Unit BAL defination

        #region Master Terrain
        //--------------Master Terrain-------------------//
        public bool AddMasterTerrainType(MasterTerrainTypeViewModel masterTerrainTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterTerrainType(masterTerrainTypeViewModel, ref message);
        }
        public bool EditMasterTerrainType(MasterTerrainTypeViewModel masterTerrainTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterTerrainType(masterTerrainTypeViewModel, ref message);
        }
        public Boolean DeleteMasterTerrainType(int masterTerrainTypeId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterTerrainType(masterTerrainTypeId, ref message);
        }
        public Array ListMasterTerrainType(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterTerrainType(page, rows, sidx, sord, out totalRecords);
        }
        public MasterTerrainTypeViewModel GetTerrainTypeDetails_ByTerrainCode(int terrainCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetTerrainTypeDetails_ByTerrainCode(terrainCode);
        }
        #endregion Terrain BAL defination

        #region Master Surface
        //-------------Master Surface------------------//
        public bool AddMasterSurface(MasterSurfaceViewModel masterSurfaceViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterSurface(masterSurfaceViewModel, ref message);
        }
        public bool EditMasterSurface(MasterSurfaceViewModel masterSurfaceViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterSurface(masterSurfaceViewModel, ref message);
        }
        public Boolean DeleteMasterSurface(int masterSurfaceId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterSurface(masterSurfaceId, ref message);
        }
        public Array ListMasterSurface(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterSurface(page, rows, sidx, sord, out totalRecords);
        }
        public MasterSurfaceViewModel GetSurfaceDetails_BySurfaceCode(int surfaceCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetSurfaceDetails_BySurfaceCode(surfaceCode);
        }
        #endregion Surface BAL defination

        #region Master Scour Foundation
        //-------------Master Scour Foundation ------------------//
        public bool AddMasterScourFoundationType(MasterScourFoundationTypeViewModel masterScourFoundationTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterScourFoundationType(masterScourFoundationTypeViewModel, ref message);
        }
        public bool EditMasterScourFoundationType(MasterScourFoundationTypeViewModel masterScourFoundationTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterScourFoundationType(masterScourFoundationTypeViewModel, ref message);
        }
        public Boolean DeleteMasterScourFoundationType(int masterScourFoundationTypeId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterScourFoundationType(masterScourFoundationTypeId, ref message);
        }
        public Array ListMasterScourFoundationType(string SfType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterScourFoundationType(SfType, page, rows, sidx, sord, out totalRecords);
        }
        public MasterScourFoundationTypeViewModel GetScourFoundationDetails_ByScourFoundationCode(int ScourFoundationCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetScourFoundationDetails_ByScourFoundationCode(ScourFoundationCode);
        }
        #endregion Scour Foundation Type BAL declaration

        #region Growth Master Score
        public Boolean DeleteScoreSubItem(int masterScoreId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteScoreSubItem(masterScoreId, ref message);
        }
        public bool EditGrowthScoreSubItemDetails(GrowthScoreSubItemViewModel growthScoreViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditGrowthScoreSubItemDetails(growthScoreViewModel, ref message);
        }

        public bool AddScoreSubItemDetails(GrowthScoreSubItemViewModel objScore, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddScoreSubItemDetails(objScore, ref message);
        }

        public Boolean DeleteMasterGrowthScore(int masterScoreId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterGrowthScore(masterScoreId, ref message);
        }

        public bool EditMasterGrowthScore(GrowthScoreViewModel growthScoreViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterGrowthScore(growthScoreViewModel, ref message);
        }

        public bool AddMasterGrowthScore(GrowthScoreViewModel objScore, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterGrowthScore(objScore, ref message);
        }

        public Array ListGrowthMasterScoreType(string SfType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListGrowthMasterScoreType(SfType, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListScoreSubItems(string SfType, int? page, int? rows, string sidx, string sord, out long totalRecords, int scoreId)
        {
            objDAL = new MasterDAL();
            return objDAL.ListScoreSubItems(SfType, page, rows, sidx, sord, out totalRecords, scoreId);
        }
        #endregion

        #region Master Grade
        //------------Master Grade------------------//
        public bool AddMasterGradeType(MasterGradeTypeViewModel masterGradeTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterGradeType(masterGradeTypeViewModel, ref message);
        }
        public bool EditMasterGradeType(MasterGradeTypeViewModel masterGradeTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterGradeType(masterGradeTypeViewModel, ref message);
        }
        public Boolean DeleteMasterGradeType(int masterGradeTypeId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterGradeType(masterGradeTypeId, ref message);
        }
        public Array ListMasterGradeType(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterGradeType(page, rows, sidx, sord, out totalRecords);
        }
        public MasterGradeTypeViewModel GetGradeTypeDetails_ByGradeCode(int componentCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetGradeTypeDetails_ByGradeCode(componentCode);
        }
        #endregion Grade BAL defination

        #region Master Component
        //-------------Master Component-----------------//
        public bool AddMasterComponentType(MasterComponentTypeViewModel masterComponentTypeVIewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterComponentType(masterComponentTypeVIewModel, ref message);
        }
        public bool EditMasterComponentType(MasterComponentTypeViewModel masterComponentTypeViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterComponentType(masterComponentTypeViewModel, ref message);
        }
        public Boolean DeleteMasterComponentType(int masterComponentTypeId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterComponentType(masterComponentTypeId, ref message);
        }
        public Array ListMasterComponentType(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterComponentType(page, rows, sidx, sord, out totalRecords);
        }
        public MasterComponentTypeViewModel GetComponentDetails_ByComponentCode(int componentCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetComponentDetails_ByComponentCode(componentCode);
        }
        #endregion Component Type BAL defination

        #region Master Contractor Class
        //-------------Master Contractor Class------------------//
        public bool AddMasterContractorClassType(MasterContractorClassTypeViewModel masterContractorClassTypeViewModel, ref string message)
        {
            IMasterDAL objDAL = new MasterDAL();

            return objDAL.AddMasterContractorClassType(masterContractorClassTypeViewModel, ref message);
        }


        public bool EditMasterContractorClassType(MasterContractorClassTypeViewModel masterContractorClassTypeViewModel, ref string message)
        {
            IMasterDAL objDAL = new MasterDAL();
            return objDAL.EditMasterContractorClassType(masterContractorClassTypeViewModel, ref message);
        }

        public Boolean DeleteMasterContractorClassType(int masterContractorClassId, ref string message)
        {
            IMasterDAL objDAL = new MasterDAL();
            return objDAL.DeleteMasterContractorClassType(masterContractorClassId, ref message);
        }

        public Array ListMasterContractorClassType(int statecode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            IMasterDAL objDAL = new MasterDAL();
            return objDAL.ListMasterContractorClassType(statecode, page, rows, sidx, sord, out totalRecords);


        }

        public MasterContractorClassTypeViewModel GetContractorClassDetails_ByClassCode(int masterContractorClassId)
        {
            IMasterDAL objDAL = new MasterDAL();
            return objDAL.GetContractorClassDetails_ByClassCode(masterContractorClassId);
        }

        #endregion Contractor Class Type  BAL defination

        #region Master Contractor Registration
        //------------Master Contractor Registration-----------------//
        public bool AddMasterContractorReg(MasterContractorRegistrationViewModel masterContractorRegViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterContractorReg(masterContractorRegViewModel, ref message);
        }

        public bool EditMasterContractorReg(MasterContractorRegistrationViewModel masterContractorRegViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterContractorReg(masterContractorRegViewModel, ref message);
        }

        // Added on 25-01-2022 by Srishti Tyagi
        public bool EditMasterContractorRegFundType(MasterContractorRegistrationViewModel masterContractorRegViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterContractorRegFundType(masterContractorRegViewModel, ref message);
        }

        public Boolean DeleteMasterContractorReg(int ContRegId, int ContRegCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterContractorReg(ContRegId, ContRegCode, ref message);
        }

        public Array ListMasterContractorReg(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contrctorId)
        {
            objDAL = new MasterDAL();

            return objDAL.GetContractorRegList(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, contrctorId);
        }

        public Array GetViewContractorRegistrationListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int ContRegId, int ContRegCode)
        {
            objDAL = new MasterDAL();

            return objDAL.GetViewContractorRegistrationList(page, rows, sidx, sord, out totalRecords, ContRegId, ContRegCode);
        }
        public MasterContractorRegistrationViewModel GetContRegDetails_ByConId_RegCode(int ContRegId, int ContRegCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetContRegDetails_ByConId_RegCode(ContRegId, ContRegCode);
        }

        #endregion Contractor Registration BAL defination
        #region Contractor Detail
        public Array GetContractorAgreementListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contrctorId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetContractorAgreementListDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, contrctorId);
        }
        public Array GetContractorIMSMaintenanceListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contrctorId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetContractorIMSMaintenanceListDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, contrctorId);
        }
        public Array GetContractorPaymentListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contrctorId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetContractorPaymentListDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, status, contractorName, conStatus, panNumber, classType, regNo, companyName, contrctorId);
        }
        #endregion
        #region Master Vidhan Sabha Term
        //-------------Master Vidhan Sabha Term------------------//
        public bool AddMasterVidhanSabhaTerm(MasterVidhanSabhaTermViewModel masterVidhanSabhaTermViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterVidhanSabhaTerm(masterVidhanSabhaTermViewModel, ref message);
        }

        public bool EditMasterVidhanSabhaTerm(MasterVidhanSabhaTermViewModel masterVidhanSabhaTermViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterVidhanSabhaTerm(masterVidhanSabhaTermViewModel, ref message);
        }

        public Boolean DeleteVidhanSabhaTerm(int stateCode, int VSTermId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteVidhanSabhaTerm(stateCode, VSTermId, ref message);
        }

        public Array ListVidhanSabhaTerm(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListVidhanSabhaTerm(stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public MasterVidhanSabhaTermViewModel GetVidhanSabhaTerm_ByStateCode_TermId(int stateCode, int TermId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetVidhanSabhaTerm_ByStateCode_TermId(stateCode, TermId);
        }


        #endregion Vidhan sabha term BAL defination

        #region Region BAL defination
        //-------------Master Region------------------//
        public bool AddMasterRegion(MasterRegionViewModel masterRegionViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterRegion(masterRegionViewModel, ref message);
        }

        public bool EditMasterRegion(MasterRegionViewModel masterRegionViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterRegion(masterRegionViewModel, ref message);
        }

        //changes by Koustubh Nakate on 10-05-2013
        public Boolean DeleteMasterRegion(int regionCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterRegion(regionCode, ref message);
        }
        //changes by Koustubh Nakate on 10-05-2013
        public Array ListMasterRegion(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterRegion(stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public MasterRegionViewModel GetMasterRegion_ByRegionCode(int regionCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMasterRegion_ByRegionCode(regionCode);
        }

        //changes by Abhishek kamble 24-Feb-2014
        public Boolean DeleteMappedDistrictRegionDetails(int regionId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMappedDistrictRegionDetails(regionId, ref message);
        }


        #endregion Region  BAL defination

        #region Master Admin Autonomous Body
        //-------------Master Admin Autonomous Body----------------//
        public bool AddMasterAdminAutonomousBody(MasterAdminAutonomousBodyViewModel masterAdminAutonomousBodyViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterAdminAutonomousBody(masterAdminAutonomousBodyViewModel, ref message);
        }

        public bool EditMasterAdminAutonomousBody(MasterAdminAutonomousBodyViewModel masterAdminAutonomousBodyViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterAdminAutonomousBody(masterAdminAutonomousBodyViewModel, ref message);
        }

        public Boolean DeleteMasterAdminAutonomousBody(int stateCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterAdminAutonomousBody(stateCode, ref message);
        }

        public Array ListMasterAdminAutonomousBody(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterAdminAutonomousBody(page, rows, sidx, sord, out totalRecords, stateCode);
        }

        public MasterAdminAutonomousBodyViewModel GetMasterAdminAutonomousBodyViewModel_ByStateCode(int stateCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMasterAdminAutonomousBodyViewModel_ByStateCode(stateCode);
        }


        #endregion Admin Autonomous body BAL defination

        #region Master MP Member
        //------------Master MP Member-----------------//
        public bool AddMasterMpMember(MasterMpMembersViewModel masterMpMemberViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterMpMember(masterMpMemberViewModel, ref message);
        }

        public bool EditMasterMpMember(MasterMpMembersViewModel masterMpMemberViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterMpMember(masterMpMemberViewModel, ref message);
        }

        public Boolean DeleteMpMember(int Term, int ConstCode, int MemberId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMpMember(Term, ConstCode, MemberId, ref message);
        }

        public Array ListMpMember(int termCode, int stateCode, int constituencyCode, string memberName, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMpMember(termCode, stateCode, constituencyCode, memberName, page, rows, sidx, sord, out totalRecords);
        }

        public MasterMpMembersViewModel GetMpMember_ByTerm_ConstCode_MemberId(int Term, int ConstCode, int MemberId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMpMember_ByTerm_ConstCode_MemberId(Term, ConstCode, MemberId);
        }

        #endregion  MP Member  BAL defination


        #region Allow SQC to Edit SQM Details
        //Changed by deendayal on 28/7/2017
        public bool CheckIdentityInformation(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, int QualityMonitorCode)
        {
            objDAL = new MasterDAL();
            return objDAL.CheckIdentityInformation(masterQualityMonitorViewModel, QualityMonitorCode);
        }
        #endregion


        #region Master Quality Monitor
        //-------------Master Quality Monitor------------------//
        //public bool AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message)
        //{
        //    objDAL = new MasterDAL();
        //    return objDAL.AddMasterQualityMonitor(masterQualityMonitorViewModel, ref message);
        //}

        public bool AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message, ref int AdminQMCode)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterQualityMonitor(masterQualityMonitorViewModel, ref message, ref AdminQMCode);
        }
        //public bool EditMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message)
        //{
        //    objDAL = new MasterDAL();
        //    return objDAL.EditMasterQualityMonitor(masterQualityMonitorViewModel, ref message);
        //}
        public bool EditMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterQualityMonitor(masterQualityMonitorViewModel, ref message);
        }
        public Boolean DeleteQualityMonitor(int qualityMonitorCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteQualityMonitor(qualityMonitorCode, ref message);
        }

        //public Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords)
        //{
        //    objDAL = new MasterDAL();
        //    return objDAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, filters, page, rows, sidx, sord, out totalRecords);
        //}

        public Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, filters, page, rows, sidx, sord, out totalRecords);
        }


        //public MasterAdminQualityMonitorViewModel GetQualityMonitor_ByQualityMonitorCode(int qualityMonitorCode)
        //{
        //    objDAL = new MasterDAL();
        //    return objDAL.GetQualityMonitor_ByQualityMonitorCode(qualityMonitorCode);
        //}

        public MasterAdminQualityMonitorViewModel GetQualityMonitor_ByQualityMonitorCode(int qualityMonitorCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetQualityMonitor_ByQualityMonitorCode(qualityMonitorCode);
        }

        #region Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC
        //Added by Hrishikesh to provide Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC--start
        public MasterAdminQualityMonitorViewModel GetQMProfileInformationBAL(int userId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetQMProfileInformationDAL(userId);
        }
        #endregion

        public bool AddSQMUserLoginQualityMonitorBAL(int qualityMonitorCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddSQMUserLoginQualityMonitorDAL(qualityMonitorCode, ref message);

        }

        #region Upload File Details

        public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            objDAL = new MasterDAL();
            return objDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        public Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int ADMIN_QM_CODE)
        {
            objDAL = new MasterDAL();
            return objDAL.GetPDFFilesListDAL(page, rows, sidx, sord, out totalRecords, ADMIN_QM_CODE);
        }

        public string AddFileUploadDetailsBAL(QualityMonitorFileUploadViewModel fileUploadViewModel)
        {
            //Image Upload
            if (fileUploadViewModel.file_type.ToUpper() == "I")
            {
                objDAL = new MasterDAL();
                return objDAL.AddFileUploadDetailsDAL(fileUploadViewModel);
            }
            else if (fileUploadViewModel.file_type.ToUpper() == "D")
            {
                objDAL = new MasterDAL();
                return objDAL.AddFileUploadDetailsDAL(fileUploadViewModel);
            }
            return ("An Error Occurred While Your Processing Request.");
        }

        public string DeleteFileDetails(int ADMIN_QM_CODE, string FILE_NAME)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteFileDetailsDAL(ADMIN_QM_CODE, FILE_NAME);
        }
        public string DeletePdfFileDetails(int ADMIN_QM_CODE, string FILE_NAME)
        {

            objDAL = new MasterDAL();
            return objDAL.DeletePdfFileDetailsDAL(ADMIN_QM_CODE, FILE_NAME);
        }

        public void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            // For Thumbnail Image    

            ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                new ImageResizer.ResizeSettings("width=100;height=100;format=jpg;mode=max"));

            ThumbnailJob.Build();

            // For Original Image
            ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

            job.Build();
        }

        #endregion


        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPANFileListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int qmCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetPANFileListDAL(page, rows, sidx, sord, out totalRecords, qmCode);
        }


        /// <summary>
        /// Validates the PDF File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidatePDFFile(int FileSize, string FileExtension)
        {
            if (FileSize == 0)
            {
                return "Upload PDF file";
            }
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PAN_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }


        public string AddPANUploadDetailsBAL(List<QualityMonitorFileUploadViewModel> lstFileUploadViewModel)
        {
            objDAL = new MasterDAL();
            return objDAL.AddPANUploadDetailsDAL(lstFileUploadViewModel);
        }

        
        public string DeletePANFileDetailsBAL(int qmCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeletePANFileDetailsDAL(qmCode);
        }

        #endregion Quality Monitor BAL defination

        #region Master Execution
        //-------------Master Execution------------------//
        public bool AddMasterExecution(MasterExecutionItemViewModel masterExecutionView, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterExecution(masterExecutionView, ref message);
        }

        public bool EditMasterExecution(MasterExecutionItemViewModel masterExecutionView, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterExecution(masterExecutionView, ref message);
        }

        public Boolean DeleteMasterExecution(int ExecutionId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterExecution(ExecutionId);
        }

        public Array ListMasterExecution(string ItemType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterExecution(ItemType, page, rows, sidx, sord, out totalRecords);
        }

        public MasterExecutionItemViewModel GetExecutionDetails_ByExecutionCode(int ExecutionId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetExecutionDetails_ByExecutionCode(ExecutionId);
        }


        public MASTER_EXECUTION_ITEM CloneExecutionModel(MasterExecutionItemViewModel masterExecutionView, bool flagAddEdit)
        {
            objDAL = new MasterDAL();
            return objDAL.CloneExecutionModel(masterExecutionView, flagAddEdit);
        }
        public MasterExecutionItemViewModel CloneExecutionObject(MASTER_EXECUTION_ITEM masterExecutionModel)
        {
            objDAL = new MasterDAL();
            return objDAL.CloneExecutionObject(masterExecutionModel);
        }
        #endregion Master_Execution

        #region Master Sqc
        //-------------Matser Sqc------------------//
        public bool AddAdminSqc(AdminSqcViewModel adminSqcViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddAdminSqc(adminSqcViewModel, ref message);
        }

        public bool EditAdminSqc(AdminSqcViewModel adminSqcViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditAdminSqc(adminSqcViewModel, ref message);
        }

        public Boolean DeleteAdminSqc(int AdminQcId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteAdminSqc(AdminQcId);
        }
        public Array ListadminQc(int stateCode, int adminNdCode, string status, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListadminQc(stateCode, adminNdCode, status, page, rows, sidx, sord, out totalRecords);
        }

        public AdminSqcViewModel GetAdminQc_ByQcCode(int AdminQcCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAdminQc_ByQcCode(AdminQcCode);
        }

        public List<MASTER_DESIGNATION> GetDesignation()
        {
            objDAL = new MasterDAL();
            return objDAL.GetDesignation();
        }

        public List<MASTER_STATE> GetStates()
        {
            objDAL = new MasterDAL();
            return objDAL.GetStates();
        }

        public List<MASTER_DISTRICT> GetDistrictName(int stateId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetDistrictName(stateId);
        }
        #endregion master_sqc

        #region Master MLA Members
        //-------------Master MLA Members------------------//
        public bool AddMLAMembers(MasterMLAMembersViewModel memberModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMLAMembers(memberModel, ref message);
        }

        public Array ListMLAMembers(int stateCode, int term, int constCode, string memberName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMLAMembers(stateCode, term, constCode, memberName, page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteMLAMembers(int memberCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMLAMembers(memberCode);
        }

        public bool EditMLAMembers(MasterMLAMembersViewModel memberModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMLAMembers(memberModel, ref message);
        }

        public MasterMLAMembersViewModel GetMemberDetails(int memberCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMemberDetails(memberCode);
        }


        #endregion

        #region Master Admin Department
        //-------------Master Admin Department------------------//
        public bool AddAdminDepartment(AdminDepartmentViewModel adminModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddAdminDepartment(adminModel, ref message);
        }

        public Array ListAdminDepartmentList(int stateCode, int agencyCode, int page, int rows, string sidx, string sord, out long totalRecords) //changes by Koustubh Nakate on 16-05-2013
        {
            objDAL = new MasterDAL();
            return objDAL.ListAdminDepartmentList(stateCode, agencyCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteAdminDepartment(int adminCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteAdminDepartment(adminCode, ref message);
        }

        public bool EditAdminDepartment(AdminDepartmentViewModel adminModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditAdminDepartment(adminModel, ref message);
        }

        public AdminDepartmentViewModel GetAdminDetails(int adminCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAdminDetails(adminCode);
        }

        public AdminDepartmentViewModel AddStateAdmin()
        {
            objDAL = new MasterDAL();
            return objDAL.AddStateAdmin();
        }

        #endregion
        #region PIU Department
        public Array GetDPIUListBAL(int stateCode, int agencyCode, int adminNDCode, string activeflag, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetDPIUListDAL(stateCode, agencyCode, adminNDCode, activeflag, page, rows, sidx, sord, out totalRecords);
        }

        #endregion

        #region Master Checklist Points
        //-------------Master Checklist Point ------------------//
        public bool AddMasterChecklist(MasterChecklistPointsViewModel masterChecklistViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterChecklist(masterChecklistViewModel, ref message);
        }
        public bool EditMasterChecklist(MasterChecklistPointsViewModel masterChecklistViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterChecklist(masterChecklistViewModel, ref message);
        }
        public Boolean DeleteMasterChecklist(int masterChecklistId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterChecklist(masterChecklistId);
        }
        public Array ListMasterChecklist(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterChecklist(page, rows, sidx, sord, out totalRecords);
        }
        public MasterChecklistPointsViewModel GetChecklistDetails_ByChecklistCode(int ChecklistCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetChecklistDetails_ByChecklistCode(ChecklistCode);
        }

        #endregion  Checklist Points Defination

        #region Master Reason
        //-------------Master Reason------------------//
        public bool AddMasterReason(MasterReasonViewModel masterReasonViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterReason(masterReasonViewModel, ref message);
        }
        public bool EditMasterReason(MasterReasonViewModel masterReasonViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterReason(masterReasonViewModel, ref message);
        }

        public Boolean DeleteMasterReason(int masterReasonId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterReason(masterReasonId);
        }

        public Array ListMasterReason(string reasonType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterReason(reasonType, page, rows, sidx, sord, out totalRecords);
        }

        public MasterReasonViewModel GetReasonDetails_ByReasonCode(int ReasonCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetReasonDetails_ByReasonCode(ReasonCode);
        }

        public List<SelectListItem> GetReasonCode()
        {
            objDAL = new MasterDAL();
            return objDAL.GetReasonCode();
        }

        #endregion  Reason Defination


        #region Master Agency
        //-------------Master Agency-----------------//
        public bool AddMasterAgency(MasterAgencyViewModel masterAgencyViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMasterAgency(masterAgencyViewModel, ref message);
        }

        public bool EditMasterAgency(MasterAgencyViewModel masterAgencyViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMasterAgency(masterAgencyViewModel, ref message);
        }

        public Boolean DeleteMasterAgency(int masterAgencyId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMasterAgency(masterAgencyId);
        }

        public Array ListMasterAgency(string agencyType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMasterAgency(agencyType, page, rows, sidx, sord, out totalRecords);

        }

        public MasterAgencyViewModel GetAgencyDetails_ByAgencyCode(int AgencyCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAgencyDetails_ByAgencyCode(AgencyCode);
        }

        public List<SelectListItem> GetAgencyCode()
        {
            objDAL = new MasterDAL();
            return objDAL.GetAgencyCode();
        }

        #endregion Master Agency

        #region Master Technical Agency
        //-------------Master Technical Agency------------------//
        public bool AddAdminTechnicalAgency(AdminTechnicalAgencyViewModel adminTechnicalAgencyViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddAdminTechnicalAgency(adminTechnicalAgencyViewModel, ref message);
        }

        public bool EditAdminTechnicalAgency(AdminTechnicalAgencyViewModel adminTechnicalAgencyViewModel, ref string message)
        {

            objDAL = new MasterDAL();
            return objDAL.EditAdminTechnicalAgency(adminTechnicalAgencyViewModel, ref message);
        }

        public Boolean DeleteAdminTechnicalAgency(int AdminTechnicalAgencyId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteAdminTechnicalAgency(AdminTechnicalAgencyId);
        }

        public Array ListadminTechnicalAgency(string taName, string taType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListadminTechnicalAgency(taName, taType, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListdistrictTechnicalAgency(int? page, int? rows, string sidx, string sord, out long totalRecords, string agencyType, int stateCode, int districtCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ListdistrictTechnicalAgency(page, rows, sidx, sord, out totalRecords, agencyType, stateCode, districtCode);
        }

        public AdminTechnicalAgencyViewModel GetAdminTA_ByTACode(int AdminTechnicalAgencyCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAdminTA_ByTACode(AdminTechnicalAgencyCode);
        }


        public AdminTechnicalAgencyViewModel CloneAdminTechnicalAgencyObject(ADMIN_TECHNICAL_AGENCY AdminTechnicalAgencyCode)
        {
            objDAL = new MasterDAL();
            return objDAL.CloneAdminTechnicalAgencyObject(AdminTechnicalAgencyCode);
        }

        public List<MASTER_DESIGNATION> GetDesignationTA()
        {
            objDAL = new MasterDAL();
            return objDAL.GetDesignationTA();
        }

        public List<MASTER_STATE> GetStatesTA()
        {
            objDAL = new MasterDAL();
            return objDAL.GetStatesTA();
        }

        public List<MASTER_DISTRICT> GetDistrictNameTA(int? stateId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetDistrictNameTA(stateId);
        }
        #endregion TA

        #region Region-District-Mapping
        //-------------Mapping of Region and District------------------//
        public bool MapRegionDistrictsBAL(string encryptedRegionCode, string encryptedDistrictCodes)
        {
            objDAL = new MasterDAL();
            return objDAL.MapRegionDistrictsDAL(encryptedRegionCode, encryptedDistrictCodes);
        }

        public Array GetMappedDistrictDetailsListBAL_Region(int regionCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMappedDistrictDetailsListDAL_Region(regionCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteMappedRegionDistrictBAL(int regionId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMappedRegionDistrictDAL(regionId);
        }


        #endregion Region-District-Mapping

        //added by Koustubh Nakate on 16/05/2013 for admin department 
        #region DPIU LIST BY SRDA
        //-------------List DPIU------------------//
        public Array GetDPIUListBAL_ByAdminNDCode(int adminNDCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetDPIUListDAL_ByAdminNDCode(adminNDCode, page, rows, sidx, sord, out totalRecords);
        }
        #endregion DPIU LIST BY SRDA


        #region Bank Details
        //-------------Bank Details------------------//
        public bool AddContractorBankDetails(MasterContractorBankDetails contractorBankDetails, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddContractorBankDetails(contractorBankDetails, ref message);
        }
        public bool EditContractorBankDetails(MasterContractorBankDetails contractorBankDetails, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditContractorBankDetails(contractorBankDetails, ref message);
        }
        public bool DeleteContractorBankDetails(int accountId, int coustomerId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteContractorBankDetails(accountId, coustomerId, ref message);
        }
        public Array ListContractorBankDetails(int ContractorCode, int regState, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListContractorBankDetails(ContractorCode, regState, page, rows, sidx, sord, out totalRecords);
        }

        public List<MASTER_DISTRICT> getContractorDistricts(int contractorId)
        {
            objDAL = new MasterDAL();
            return objDAL.getContractorDistricts(contractorId);
        }
        public MasterContractorBankDetails getContractorBankDetails_ByBankCode(int accountId, int ContractorId)
        {
            objDAL = new MasterDAL();
            return objDAL.getContractorBankDetails_ByBankCode(accountId, ContractorId);
        }
        //added by pp 01-05-2018
        public Boolean FinalizeBankDetails(int AccountCode, int ContractorCode)
        {
            objDAL = new MasterDAL();
            return objDAL.FinalizeBankDetails(AccountCode, ContractorCode);
        }
        #endregion BankDetails

        #region Master Nodal Officer
        //------------Master Nodel Officer------------------//
        public bool EditBankDetailsNO(MasterContractorBankDetails contractorBankDetails, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditBankDetailsNO(contractorBankDetails, ref message);
        }

        public bool DeleteBankDetailsNO(int accountId, int customerId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteBankDetailsNO(accountId, customerId, ref message);
        }

        public MasterContractorBankDetails getBankDetailsNO_ByBankCode(int accountId, int ContractorId)
        {
            objDAL = new MasterDAL();
            return objDAL.getBankDetailsNO_ByBankCode(accountId, ContractorId);
        }

        public bool AddBankDetailsNO(MasterContractorBankDetails contractorBankDetails, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddBankDetailsNO(contractorBankDetails, ref message);
        }

        public Array ListBankDetailsNO(int NodalOfficerCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListBankDetailsNO(NodalOfficerCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool AddNodalOfficer(AdminNodalOfficerViewModel nodalOfficerView, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddNodalOfficer(nodalOfficerView, ref message);
        }
        public bool EditNodalOfficer(AdminNodalOfficerViewModel nodalOfficerView, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditNodalOfficer(nodalOfficerView, ref message);
        }
        public bool DeleteNodalOfficer(int OfficerCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteNodalOfficer(OfficerCode);
        }
        public Array ListNodalOfficer(int stateCode, int officeCode, int designationCode, int NoTypeCode, string moduleType, string active, int? page, int? rows, string sidx, string sord, out long totalRecord)
        {
            objDAL = new MasterDAL();
            return objDAL.ListNodalOfficer(stateCode, officeCode, designationCode, NoTypeCode, moduleType, active, page, rows, sidx, sord, out totalRecord);
        }

        public AdminNodalOfficerViewModel GetAdminNodalOfficer_ByOfficerCode(int OfficerCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAdminNodalOfficer_ByOfficerCode(OfficerCode);
        }

        public List<ADMIN_DEPARTMENT> GetAdminNdCode()
        {
            objDAL = new MasterDAL();
            return objDAL.GetAdminNdCode();
        }
        public List<MASTER_DESIGNATION> GetNodalDesignation()
        {
            objDAL = new MasterDAL();
            return objDAL.GetNodalDesignation();
        }
        public List<SelectListItem> PopulateDistrict(string adminNdCode)
        {
            objDAL = new MasterDAL();
            return objDAL.PopulateDistrict(adminNdCode);
        }

        #endregion NodalOfficer

        #region Agency-State-District-Mapping
        //-------------Mapping of the Agency-State-District----------------//

        public bool MapAgencyStatesBAL(string encryptedAgencyCode, string encryptedStateCodes, string startDate)
        {
            objDAL = new MasterDAL();
            return objDAL.MapAgencyStatesDAL(encryptedAgencyCode, encryptedStateCodes, startDate);
        }

        public bool MapAgencyDistrictsBAL(string encryptedAgencyCode, string encryptedDistrictCodes, string startDate)
        {
            objDAL = new MasterDAL();
            return objDAL.MapAgencyDistrictsDAL(encryptedAgencyCode, encryptedDistrictCodes, startDate);
        }

        public Array GetMappedStateDetailsListBAL_Agency(int stateCode, int agencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMappedStateDetailsListDAL_Agency(stateCode, agencyCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetMappedDistrictDetailsListBAL_Agency(int agencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMappedDistrictDetailsListDAL_Agency(agencyCode, page, rows, sidx, sord, out totalRecords);
        }
        #endregion Agency-State-District-Mapping

        public bool MapSRRDADistrictsBAL(string encryptedAdminCode, string encryptedDistrictCodes)
        {
            objDAL = new MasterDAL();
            return objDAL.MapSRRDADistrictsDAL(encryptedAdminCode, encryptedDistrictCodes);
        }


        public Array GetMappedDistrictDetailsListBAL_SRRDA(int adminCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMappedDistrictDetailsListDAL_SRRDA(adminCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool ContractorRegChangeStatusBAL(int conID, int ConRegCode, bool IsActive)
        {
            objDAL = new MasterDAL();
            return objDAL.ContractorRegChangeStatusDAL(conID, ConRegCode, IsActive);
        }

        public bool DeleteMappedStateAgencyBAL(int adminId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMappedStateAgencyDAL(adminId);
        }

        public bool FinalizeMappedStateAgencyBAL(int adminId)
        {
            objDAL = new MasterDAL();
            return objDAL.FinalizeMappedStateAgencyDAL(adminId);
        }

        public bool UpdateStateEndDatePTA_BAL(string endDate, string encryptedadminId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.UpdateStateEndDatePTA_DAL(endDate, encryptedadminId, ref message);
        }

        public EndDateDistrictViewModel AddEndDateDistrictBAL(EndDateDistrictViewModel endDateDistrictViewModel, int adminTaId)
        {
            objDAL = new MasterDAL();
            return objDAL.AddEndDateDistrictDAL(endDateDistrictViewModel, adminTaId);
        }

        public EndDateStateViewModel AddEndDateStateBAL(EndDateStateViewModel endDateStateViewModel, int adminTaId)
        {
            objDAL = new MasterDAL();
            return objDAL.AddEndDateStateDAL(endDateStateViewModel, adminTaId);
        }

        public bool UpdateDistrictEndDateSTA_BAL(string endDate, string encryptedadminId, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.UpdateDistrictEndDateSTA_DAL(endDate, encryptedadminId, ref message);
        }


        public bool FinalizeMappedDistrictAgencyBAL(int adminId)
        {
            objDAL = new MasterDAL();
            return objDAL.FinalizeMappedDistrictAgencyDAL(adminId);
        }


        public bool DeleteMappedDistrictAgencyBAL(int adminId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMappedDistrictAgencyDAL(adminId);
        }

        public bool DeleteMappedSRRDADistrictBAL(int adminId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMappedSRRDADistrictDAL(adminId);
        }

        #region MASTER_TAX

        public Array GetTaxDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetTaxDetailsListDAL(page, rows, sidx, sord, out totalRecords);
        }

        public MasterTaxViewModel GetTaxDetails(int taxCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetTaxDetails(taxCode);
        }

        public bool DeleteTaxDetailsBAL(int taxCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteTaxDetailsDAL(taxCode);
        }

        public bool AddTaxDetailsBAL(MasterTaxViewModel taxModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddTaxDetailsDAL(taxModel, ref message);
        }

        public bool EditTaxDetailsBAL(MasterTaxViewModel taxModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditTaxDetailsDAL(taxModel, ref message);
        }

        #endregion

        #region Technology
        public Array ListTechnologyDetails(string statusType,int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListTechnologyDetails(statusType,page, rows, sidx, sord, out totalRecords);
        }

        public bool AddTechnologyDetails(MasterTechnologyViewModel techViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddTechnologyDetails(techViewModel, ref message);

        }

        public bool EditTechnologyDetails(MasterTechnologyViewModel techViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditTechnologyDetails(techViewModel, ref message);

        }

        public MasterTechnologyViewModel GetTechnologyDetails(int techCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetTechnologyDetails(techCode);
        }

        public bool DeleteTechnologyDetails(int technologyCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteTechnologyDetails(technologyCode);
        }

        public bool ChangeTchnologyStatus(int technologyCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangeTchnologyStatus(technologyCode);
        }
        #endregion Technology

        #region Test

        public Array ListTestDetails(string statusType,int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListTestDetails(statusType,page, rows, sidx, sord, out totalRecords);
        }
        public bool AddTestDetails(MasterTestViewModel testViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddTestDetails(testViewModel, ref message);

        }
        public bool EditTestDetails(MasterTestViewModel testViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditTestDetails(testViewModel, ref message);
        }
        public MasterTestViewModel GetTestDetails(int testCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetTestDetails(testCode);
        }

        public bool DeleteTestDetails(int testCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteTestDetails(testCode);
        }

        public bool ChangeTestStatus(int testCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangeTestStatus(testCode);
        }

        #endregion Test

        #region Alerts
        public Array ListAlertsDetails(string status, int? page, int? rows, String sidx, String sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListAlertsDetails(status, page, rows, sidx, sord, out totalRecords);
        }
        public bool AddAlertDetails(AdminAlertsViewModel AlertViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddAlertDetails(AlertViewModel, ref message);
        }

        public bool EditAlertDetails(AdminAlertsViewModel AlertViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditAlertDetails(AlertViewModel, ref message);
        }

        public AdminAlertsViewModel ViewAlertDetails(int AlertId)
        {
            objDAL = new MasterDAL();
            return objDAL.ViewAlertDetails(AlertId);
        }

        public bool DeleteAlertDetails(int AlertId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteAlertDetails(AlertId);
        }
        public bool ChangeAlertStatus(int AlertId)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangeAlertStatus(AlertId);
        }
        #endregion Alerts

        #region PMGSY2

        public Array ListPMGSYIIDetails(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListPMGSYIIDetails(page, rows, sidx, sord, out totalRecords);
        }

        public bool ChangePMGSY2Status(int stateCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangePMGSY2Status(stateCode);
        }

        public bool IsPMGSY2Active(int stateCode)
        {
            objDAL = new MasterDAL();
            return objDAL.IsPMGSY2Active(stateCode);
        }

        #endregion PMGSY2

        #region Feedback Category

        public Array ListFeedbackCategoryDetails(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListFeedbackCategoryDetails(page, rows, sidx, sord, out totalRecords);
        }
        public bool AddFeedbackDetails(FeedbackCategoryViewModel feedbackCategoryViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddFeedbackDetails(feedbackCategoryViewModel, ref message);
        }

        public bool EditFeedbackDetails(FeedbackCategoryViewModel feedbackCategoryViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditFeedbackDetails(feedbackCategoryViewModel, ref message);
        }
        public FeedbackCategoryViewModel GetFeedbackDetails(int feebackId)
        {
            objDAL = new MasterDAL();
            return objDAL.GetFeedbackDetails(feebackId);
        }
        public bool DeleteFeedbackDetails(int feedBackId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteFeedbackDetails(feedBackId);
        }

        #endregion Feedback Category

        #region Carriage

        public Array ListCarriageDetails(string statusType,int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListCarriageDetails(statusType,page, rows, sidx, sord, out totalRecords);
        }

        public bool AddCarriageDetails(MasterCarriageViewModel carriageViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddCarriageDetails(carriageViewModel, ref message);
        }

        public bool EditCarriageDetails(MasterCarriageViewModel carriageViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditCarriageDetails(carriageViewModel, ref message);
        }
        public MasterCarriageViewModel GetCarriageDetails(int carriageCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetCarriageDetails(carriageCode);
        }

        public bool DeleteCarriageDetails(int carriageCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteCarriageDetails(carriageCode);
        }

        public bool ChangeCarriageStatus(int carriageCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangeCarriageStatus(carriageCode);
        }

        #endregion Carriage


        #region Info

        /// <summary>
        /// get Information details list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListInfoDetails(string infoType, int infoStateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListInfoDetails(infoType, infoStateCode, page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// call add information method to save information details
        /// </summary>
        /// <param name="infoViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddInfoDetails(MasterInfoViewModel infoViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddInfoDetails(infoViewModel, ref message);
        }

        /// <summary>
        /// call update method for update information details.
        /// </summary>
        /// <param name="infoViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool EditInfoDetails(MasterInfoViewModel infoViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditInfoDetails(infoViewModel, ref message);
        }

        /// <summary>
        /// call get information method to dispaly information for edit.
        /// </summary>
        /// <param name="infoCode"></param>
        /// <returns></returns>
        public MasterInfoViewModel GetInfoDetails(int infoCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetInfoDetails(infoCode);
        }

        /// <summary>
        /// call delete information method.
        /// </summary>
        /// <param name="infoCode"></param>
        /// <returns></returns>
        public bool DeleteInfoDetails(int infoCode)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteInfoDetails(infoCode);
        }

        /// <summary>
        /// call change information status method.
        /// </summary>
        /// <param name="infoCode"></param>
        /// <returns></returns>
        public bool ChangeInfoStatus(int infoCode)
        {
            objDAL = new MasterDAL();
            return objDAL.ChangeInfoStatus(infoCode);
        }
        #endregion Info

        #region Cluster Master
        //public Array ListClusterDetailBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        //{
        //    objDAL = new MasterDAL();
        //    return objDAL.ListClusterDetailDAL(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        //}
        public Array GetHabitationListClusterBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetHabitationListClusterDAL(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }
        public bool AddClusterHabitationBAL(string encryptedHabCode, string habitationName, int blockCode)
        {
            objDAL = new MasterDAL();
            return objDAL.AddClusterHabitationDAL(encryptedHabCode, habitationName, blockCode);
        }
        public Array ListClusterBAL(int stateCode, int districtCode, int blockCode, string activeStatus, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListClusterDAL(stateCode, districtCode, blockCode, activeStatus, page, rows, sidx, sord, out totalRecords);

        }
        public bool DeleteClusterBAL(int clusterCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteClusterDAL(clusterCode, ref message);

        }
        public bool EditClusterNameHabiationBAL(MasterClusterEditViewModel clusterModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditClusterNameHabiationDAL(clusterModel, ref message);
        }
        public MasterClusterEditViewModel GetClusterDetailsBAL(int clusterCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetClusterDetailsDAL(clusterCode);
        }
        public MasterClusterViewEditHabiationModel GetClusterHabsDetailsBAL(int clusterCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetClusterHabsDetailsDAL(clusterCode);
        }
        public bool DeleteClusterHabitationBAL(int clusterCode, int habCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteClusterHabitationDAL(clusterCode, habCode, ref message);
        }
        public Array GetClusterHabitationListByClusterCodeBAL(int clusterCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetClusterHabitationListByClusterCodeDAL(clusterCode, page, rows, sidx, sord, out totalRecords);

        }
        public Array GetAddHabitationListIntoClusterBAL(int clusterCode, int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAddHabitationListIntoClusterDAL(clusterCode, stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }
        public bool UpdateClusterHabitationBAL(string encryptedHabCodeSendbyCheckBoxCheck, string habitationName, int clusterCode)
        {
            objDAL = new MasterDAL();
            return objDAL.UpdateClusterHabitationDAL(encryptedHabCodeSendbyCheckBoxCheck, habitationName, clusterCode);

        }
        public bool FinalizeClusterHabitationBAL(MasterClusterViewEditHabiationModel clusterModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.FinalizeClusterHabitationDAL(clusterModel, ref message);
        }
        #endregion

        #region Cluster Core Network
        public Array ListClusterCNBAL(int stateCode, int districtCode, int blockCode, string activeStatus, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListClusterCNDAL(stateCode, districtCode, blockCode, activeStatus, page, rows, sidx, sord, out totalRecords);
        }
        public bool DeleteClusterCNBAL(int clusterCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteClusterCNDAL(clusterCode, ref message);
        }
        public bool DeleteClusterCNHabitationBAL(int clusterCode, int habCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteClusterCNHabitationDAL(clusterCode, habCode, ref message);
        }
        public bool EditClusterCNNameHabiationBAL(MasterClusterEditViewModel clusterModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditClusterCNNameHabiationDAL(clusterModel, ref message);
        }
        public MasterClusterEditViewModel GetClusterCNDetailsBAL(int clusterCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetClusterCNDetailsDAL(clusterCode);
        }
        public MasterClusterViewEditHabiationModel GetClusterCNHabsDetailsBAL(int clusterCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetClusterCNHabsDetailsDAL(clusterCode);

        }
        public Array GetCoreNetworkListClusterCNBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetCoreNetworkListClusterCNDAL(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);

        }
        public Array GetHabitationListClusterCNBAL(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetHabitationListClusterCNDAL(roadCode, blockCode, page, rows, sidx, sord, out totalRecords);

        }
        public Array GetAddCoreNetworkListByClusterCodeBAL(int clusterCode, int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAddCoreNetworkListByClusterCodeDAL(clusterCode, stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);

        }
        public Array GetAddHabitationListIntoClusterCNBAL(int clusterCode, int roadCode, int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetAddHabitationListIntoClusterCNDAL(clusterCode, roadCode, stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);

        }
        public Array GetClusterCNHabitationListByClusterCodeBAL(int clusterCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.GetClusterCNHabitationListByClusterCodeDAL(clusterCode, page, rows, sidx, sord, out totalRecords);

        }
        public bool AddClusterCNHabitationBAL(string encryptedHabCodeSendbyCheckBoxCheck, string habitationName, int blockCode)
        {
            objDAL = new MasterDAL();
            return objDAL.AddClusterCNHabitationDAL(encryptedHabCodeSendbyCheckBoxCheck, habitationName, blockCode);

        }
        public bool UpdateClusterCNHabitationBAL(string encryptedHabCodeSendbyCheckBoxCheck, string habitationName, int clusterCode)
        {
            objDAL = new MasterDAL();
            return objDAL.UpdateClusterCNHabitationDAL(encryptedHabCodeSendbyCheckBoxCheck, habitationName, clusterCode);

        }
        public bool FinalizeClusterCNHabitationBAL(MasterClusterViewEditHabiationModel clusterModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.FinalizeClusterCNHabitationDAL(clusterModel, ref message);

        }
        #endregion

        #region  EC Check List
        public bool AddImsEcCheckListBAL(IMSECCheckListViewModel imsEcCheckModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddImsEcCheckListDAL(imsEcCheckModel, ref message);
        }
        public Array ListImsEcCheckListBAL(int stateCode, int year, int batch, int agencyCode, string TypeEc, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListImsEcCheckListDAL(stateCode, year, batch, agencyCode, TypeEc, page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteImsEcCheckBAL(int EcCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteImsEcCheckDAL(EcCode, ref message);
        }
        public bool EditImsEcCheckBAL(IMSECCheckListViewModel imsEcCheckModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditImsEcCheckDAL(imsEcCheckModel, ref message);
        }
        public IMSECCheckListViewModel GetImsEcCheckDetailsBAL(int ecCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetImsEcCheckDetailsDAL(ecCode);
        }
        public bool FinalizeECCheckListBAL(int ecId, ref string meesage)
        {
            objDAL = new MasterDAL();
            return objDAL.FinalizeECCheckListDAL(ecId,ref meesage);
        }
        public string DeFinalizeECBAL(int ecId)
        { 
            objDAL = new MasterDAL();
            return objDAL.DeFinalizeECDAL(ecId);
        }
        #endregion

        #region EC File Upload
        public bool AddImsEcFileUploadBAL(IMSEcFileUploadViewModel imsEcFileUpload, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddImsEcFileUploadDAL(imsEcFileUpload, ref message);
        }
        public Array ListImsFileUploadBAL(int stateCode, int year, int batch, int agencyCode, string fileType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListImsFileUploadDAL(stateCode, year, batch, agencyCode, fileType, page, rows, sidx, sord, out totalRecords);
        }
        public bool DeleteImsFileUploadBAL(int FileCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteImsFileUploadDAL(FileCode, ref message);
        }
        public bool EditImsFileUploadBAL(IMSEcFileUploadViewModel imsEcFileUploadModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditImsFileUploadDAL(imsEcFileUploadModel, ref message);
        }
        public IMSEcFileUploadViewModel GetImsEcFileUploadDetailsBAL(int fileCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetImsEcFileUploadDetailsDAL(fileCode);
        }
        #endregion

        #region IMs EC Training
        public bool AddImsEcTrainingBAL(IMSEcTrainingViewModel imsEcTraining, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddImsEcTrainingDAL(imsEcTraining, ref message);
        }
        public Array ListImsEcTrainingBAL(int stateCode, int year, int desigCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListImsEcTrainingDAL(stateCode, year, desigCode, page, rows, sidx, sord, out totalRecords);
        }
        public bool DeleteImsTrainingBAL(int TrainingCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteImsTrainingDAL(TrainingCode, ref message);

        }
        public bool EditImsTrainingBAL(IMSEcTrainingViewModel imsEcTrainingModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditImsTrainingDAL(imsEcTrainingModel, ref message);

        }
        public IMSEcTrainingViewModel GetImsEcTrainingDetailsBAL(int trainingCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetImsEcTrainingDetailsDAL(trainingCode);
        }
        #endregion

        #region Quality Inspection & ATR Deletions

        public Array QMViewInspectionDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string qmType)
        {
            objDAL = new MasterDAL();
            return objDAL.QMViewInspectionDetailsDAL(page, rows, sidx, sord, out totalRecords, stateCode, monitorCode, fromMonth, fromYear, toMonth, toYear, qmType);
        }


        public List<qm_inspection_list_atr_Result> ATRDetailsBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string atrStatus, string rdStatus)
        {
            objDAL = new MasterDAL();
            return objDAL.ATRDetailsDAL(stateCode, monitorCode, fromMonth, fromYear,
                                                toMonth, toYear, atrStatus, rdStatus);
        }

        #endregion

        #region MAINTENANCE_POLICY_UPLOAD

        public Array ListMaintenancePolicyBAL(int stateCode, int agencyCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMaintenancePolicyDAL(stateCode, agencyCode,  page, rows, sidx, sord, out totalRecords);
        }

        public bool AddMaintenancePolicyBAL(MaintenancePolicyViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMaintenancePolicyDAL(model, ref message);
        }

        public bool DeleteMaintenancePolicyBAL(int FileCode, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMaintenancePolicyDAL(FileCode, ref message);
        }

        public bool EditMaintenancePolicyBAL(MaintenancePolicyViewModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMaintenancePolicyDAL(model, ref message);
        }

        public MaintenancePolicyViewModel GetMaintenancePolicyDetailsBAL(int fileCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMaintenancePolicyDetailsDAL(fileCode);
        }


        #endregion

        #region BLOCKING AND LISTING FOR QUALITY MONITOR
        /// <summary>
        /// added by pradip
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean BlockeQualityMonitor(string PAN)
        {
            objDAL = new MasterDAL();
            bool status = objDAL.BlockQualityMonitor(PAN);
            return status;
        }

        //by Pradip 29-12
        public Array ListBlockedQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListBlockedQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, filters, page, rows, sidx, sord, out totalRecords);

        }
        #endregion

        #region MATRIX PARAMETERS

        public Array ListMatrixDetails(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListMatrixDetails(page, rows, sidx, sord, out totalRecords);
        }

        public bool AddMatrixParamDetails(MatrixParamModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddMatrixParamDetails(model, ref message);
        }

        public bool DeleteMatrixParamDetails(int matrixId)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteMatrixParamDetails(matrixId);
        }

        public MatrixParamModel GetMatrixParamDetails(int matrixCode)
        {
            objDAL = new MasterDAL();
            return objDAL.GetMatrixParamDetails(matrixCode);
        }

        public bool EditMatrixParamDetails(MatrixParamModel model, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.EditMatrixParamDetails(model, ref message);
        }


        #endregion

        #region PMGSY State

        public Array ListPmgsyStates(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListPmgsyStatesDAL(page, rows, sidx, sord, out totalRecords);
        }

        public bool AddStateBAL(PMGSYStatesViewModel masterAgencyViewModel, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.AddPmgsyStateDAL(masterAgencyViewModel, ref message);
        }
        #endregion


        #region ListFinancialYearTarget
        public Array ListFinancialYearTarget(int stateCode, int year, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MasterDAL();
            return objDAL.ListFinancialYearTargetDAL(stateCode, year, page, rows, sidx, sord, out totalRecords);
        }
        public Boolean DeleteFinancialYearTarget(int pmgsyID, ref string message)
        {
            objDAL = new MasterDAL();
            return objDAL.DeleteFinancialYearTargetDAL(pmgsyID, ref message);
        }
        #endregion
    }

    public interface IMasterBAL
    {
        bool AddConstructionType(CDWorksConstructionViewModel constructionModel, ref string message);
        Array ListConstructionType(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteConstructionType(int constructionCode);
        bool EditConstructionType(CDWorksConstructionViewModel constructionModel, ref string message);
        CDWorksConstructionViewModel GetConstructionTypeDetails(int constructionCode);


        bool AddCdWorks(CDWorksViewModel cdworksViewModel, ref string message);
        Array ListCdWorks(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteCdWorks(int cdWorksCode);
        bool EditCdWorks(CDWorksViewModel cdworksViewModel, ref string message);
        CDWorksViewModel GetCdWorksDetails(int cdWorksCode);

        bool AddFundingAgency(FundingAgencyViewModel fundingModel, ref string message);
        Array ListFundingAgency(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteFundingAgency(int fundCode);
        bool EditFundingAgency(FundingAgencyViewModel fundingModel, ref string message);
        FundingAgencyViewModel GetFundingAgencyDetails(int fundCode);

        bool AddRoadCategory(RoadCategoryViewModel cdworksViewModel, ref string message);
        Array ListRoadCategory(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteRoadCategory(int cdWorksType);
        bool EditRoadCategory(RoadCategoryViewModel cdworksViewModel, ref string message);
        RoadCategoryViewModel GetRoadDetails(int trafficCode);


        bool AddSoilType(SoilTypeViewModel soilTypeModel, ref string message);
        Array ListSoilType(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteSoilType(int soilCode);
        bool EditSoilType(SoilTypeViewModel soilTypeModel, ref string message);
        SoilTypeViewModel GetSoilDetails(int soilCode);

        bool AddTrafficType(TrafficTypeViewModel trafficModel, ref string message);
        Array ListTrafficType(string statusType,int page, int rows, string sidx, string sord, out long totalRecords);
        Boolean DeleteTrafficType(int agencyCode);
        bool EditTrafficType(TrafficTypeViewModel trafficModel, ref string message);
        TrafficTypeViewModel EditTrafficType(string id);
        TrafficTypeViewModel GetTrafficDetails(int trafficCode);
        bool ChangeTrafficType(int trafficCode);

        MasterContractorViewModel EditContractor(int id);
        bool AddContractor(MasterContractorViewModel model, ref string message);
        bool EditContractor(MasterContractorViewModel model, ref string message);
        Array ListContractor(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteContractor(int id);
        Array GetContractorList(string state, string district, string contractorName, string status, string contrsuppType, string panno, int page, int rows, string sidx, string sord, out long totalRecords, string filters);
        Array GetContractorRegistrationList(int id, int page, int rows, string sidx, string sord, out long totalRecords);

        //------------------------------------------------------------//

        #region Master Designation

        Array GetDesignationList(string desigCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddDesignation(MasterDesignationViewModel model, ref string message);
        bool EditDesignation(MasterDesignationViewModel model, ref string message);
        bool DeleteDesignation(int desigCode);
        MasterDesignationViewModel GetDesignationDetails(string desigCode);

        #endregion

        #region Master Lok Sabha Term

        Array GetLokSabhaTermList(int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddLokSabhaTerm(MasterLokSabhaTermViewModel model, ref string message);
        bool EditLokSabhaTerm(MasterLokSabhaTermViewModel model, ref string message);
        bool DeleteLokSabhaTerm(int loksabhaTerm);
        MasterLokSabhaTermViewModel GetLokSabhaTermDetails(int loksabhaTerm);

        #endregion

        #region Matser Unit
        bool AddMasterUnit(MasterUnitsTypeViewModel masterUnitViewModel, ref string message);
        bool EditMasterUnit(MasterUnitsTypeViewModel masterUnitsViewModel, ref string message);
        Boolean DeleteMasterUnit(int masterUnitId, ref string message);
        Array ListMasterUnit(int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterUnitsTypeViewModel GetUnitDetails_ByUnitCode(int unitCode);

        #endregion Unit BAL declaration

        #region Master Terrain

        bool AddMasterTerrainType(MasterTerrainTypeViewModel masterTerrainTypeViewModel, ref string message);
        bool EditMasterTerrainType(MasterTerrainTypeViewModel masterTerrainTypeViewModel, ref string message);
        Boolean DeleteMasterTerrainType(int masterTerrainTypeId, ref string message);
        Array ListMasterTerrainType(int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterTerrainTypeViewModel GetTerrainTypeDetails_ByTerrainCode(int terrainCode);

        #endregion Terrain BAL declaration

        #region Master Surface

        bool AddMasterSurface(MasterSurfaceViewModel masterSurfaceViewModel, ref string message);
        bool EditMasterSurface(MasterSurfaceViewModel masterSurfaceViewModel, ref string message);
        Boolean DeleteMasterSurface(int masterSurfaceId, ref string message);
        Array ListMasterSurface(int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterSurfaceViewModel GetSurfaceDetails_BySurfaceCode(int surfaceCode);

        #endregion Surface BAL declaration

        #region Master Scour Foundation

        bool AddMasterScourFoundationType(MasterScourFoundationTypeViewModel masterScourFoundationTypeViewModel, ref string message);
        bool EditMasterScourFoundationType(MasterScourFoundationTypeViewModel masterScourFoundationTypeViewModel, ref string message);
        Boolean DeleteMasterScourFoundationType(int masterScourFoundationTypeId, ref string message);
        Array ListMasterScourFoundationType(string SfType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterScourFoundationTypeViewModel GetScourFoundationDetails_ByScourFoundationCode(int ScourFoundationCode);

        #endregion Scour Foundation Type BAL declaration

        #region Growth Score

        Boolean DeleteScoreSubItem(int masterScoreId, ref string message);
        bool EditGrowthScoreSubItemDetails(GrowthScoreSubItemViewModel objScore, ref string message);
        bool AddScoreSubItemDetails(GrowthScoreSubItemViewModel objScore, ref string message);

        //int EditGrowthScore(int id);
        Boolean DeleteMasterGrowthScore(int masterScoreId, ref string message);
        bool AddMasterGrowthScore(GrowthScoreViewModel objScore, ref string message);
        bool EditMasterGrowthScore(GrowthScoreViewModel objScore, ref string message);
        Array ListGrowthMasterScoreType(string SfType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Array ListScoreSubItems(string SfType, int? page, int? rows, string sidx, string sord, out long totalRecords, int scoreId);
        #endregion

        #region Matster Grade

        bool AddMasterGradeType(MasterGradeTypeViewModel masterGradeTypeViewModel, ref string message);
        bool EditMasterGradeType(MasterGradeTypeViewModel masterGradeTypeViewModel, ref string message);
        Boolean DeleteMasterGradeType(int masterGradeTypeId, ref string message);
        Array ListMasterGradeType(int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterGradeTypeViewModel GetGradeTypeDetails_ByGradeCode(int componentCode);

        #endregion Grade BAL declaration

        #region Matster Component

        bool AddMasterComponentType(MasterComponentTypeViewModel masterComponentTypeViewModel, ref string message);
        bool EditMasterComponentType(MasterComponentTypeViewModel masterComponentTypeViewModel, ref string message);
        Boolean DeleteMasterComponentType(int masterComponentTypeId, ref string message);
        Array ListMasterComponentType(int page, int rows, string sidx, string sord, out long totalRecords);
        MasterComponentTypeViewModel GetComponentDetails_ByComponentCode(int componentCode);

        #endregion Component Type BAL declaration

        #region Master Contractor Class

        bool AddMasterContractorClassType(MasterContractorClassTypeViewModel masterContractorClassTypeViewModel, ref string message);
        bool EditMasterContractorClassType(MasterContractorClassTypeViewModel masterContractorClassTypeViewModel, ref string message);
        Boolean DeleteMasterContractorClassType(int masterContractorClassId, ref string message);
        Array ListMasterContractorClassType(int statecode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterContractorClassTypeViewModel GetContractorClassDetails_ByClassCode(int masterContractorClassId);
        bool PanNumberSearchExistBAL(string panNumber, ref string message);
        #endregion Contractor Class Type  BAL declaration

        #region Master Contractor Registration

        bool AddMasterContractorReg(MasterContractorRegistrationViewModel masterContractorRegViewModel, ref string message);
        bool EditMasterContractorReg(MasterContractorRegistrationViewModel masterContractorRegViewModel, ref string message);
        // Added on 25-01-2022 by Srishti Tyagi
        bool EditMasterContractorRegFundType(MasterContractorRegistrationViewModel masterContractorRegViewModel, ref string message);       
        Boolean DeleteMasterContractorReg(int ContRegId, int ContRegCode, ref string message);
        Array ListMasterContractorReg(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contractorId);
        MasterContractorRegistrationViewModel GetContRegDetails_ByConId_RegCode(int ContRegId, int ContRegCode);
        Array GetViewContractorRegistrationListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int ContRegId, int ContRegCode);
        #endregion Contractor Registration BAL declaration

        #region Matster Qualification

        bool AddMasterQualification(MasterQualificationViewModel masterQualViewModel, ref string message);
        bool EditMasterQualification(MasterQualificationViewModel masterQualViewModel, ref string message);
        Boolean DeleteMasterQualification(int qualId);
        Array ListMasterQualification(int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterQualificationViewModel GetQualificationDetails_ByQualCode(int QualCode);

        #endregion Qualification

        #region Master Vidhan Sabha Term

        bool AddMasterVidhanSabhaTerm(MasterVidhanSabhaTermViewModel masterVidhanSabhaTermViewModel, ref string message);
        bool EditMasterVidhanSabhaTerm(MasterVidhanSabhaTermViewModel masterVidhanSabhaTermViewModel, ref string message);
        Boolean DeleteVidhanSabhaTerm(int stateCode, int VSTermId, ref string message);
        Array ListVidhanSabhaTerm(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterVidhanSabhaTermViewModel GetVidhanSabhaTerm_ByStateCode_TermId(int stateCode, int TermId);

        #endregion Vidhan sabha term BAL declaration

        #region Master Region

        bool AddMasterRegion(MasterRegionViewModel masterRegionViewModel, ref string message);
        bool EditMasterRegion(MasterRegionViewModel masterRegionViewModel, ref string message);
        Boolean DeleteMasterRegion(int regionCode, ref string message);//changes by Koustubh Nakate on 10-05-2013
        Array ListMasterRegion(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords); //changes by koustubh Nakate on 10-05-2013
        MasterRegionViewModel GetMasterRegion_ByRegionCode(int regionCode);

        #endregion Master region  BAL declaration

        #region Master Admin Autonomous Body

        bool AddMasterAdminAutonomousBody(MasterAdminAutonomousBodyViewModel masterAdminAutonomousBodyViewModel, ref string message);
        bool EditMasterAdminAutonomousBody(MasterAdminAutonomousBodyViewModel masterAdminAutonomousBodyViewModel, ref string message);
        Boolean DeleteMasterAdminAutonomousBody(int stateCode, ref string message);
        Array ListMasterAdminAutonomousBody(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode);
        MasterAdminAutonomousBodyViewModel GetMasterAdminAutonomousBodyViewModel_ByStateCode(int stateCode);

        #endregion Admin Autonomous body BAL declaration

        #region Master MP Member

        bool AddMasterMpMember(MasterMpMembersViewModel masterMpMemberViewModel, ref string message);
        bool EditMasterMpMember(MasterMpMembersViewModel masterMpMemberViewModel, ref string message);
        Boolean DeleteMpMember(int Term, int ConstCode, int MemberId, ref string message);
        Array ListMpMember(int termCode, int stateCode, int constituencyCode, string memberName, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterMpMembersViewModel GetMpMember_ByTerm_ConstCode_MemberId(int Term, int ConstCode, int MemberId);

        #endregion Mp Memmber BAL declaration


        #region Master Quality Monitor

        //Changed by deendayal on 28/7/2017
        #region Allow SQC to edit SQM Details
        bool CheckIdentityInformation(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, int QualityMonitorCode);
        #endregion


        bool AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message, ref int AdminQMCode);
        //  bool AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message);
        bool EditMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message);
        //bool EditMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message);
        Boolean DeleteQualityMonitor(int qualityMonitorCode, ref string message);

        Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords);
       // Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords);

        MasterAdminQualityMonitorViewModel GetQualityMonitor_ByQualityMonitorCode(int qualityMonitorCode);
        //MasterAdminQualityMonitorViewModel GetQualityMonitor_ByQualityMonitorCode(int qualityMonitorCode);
        bool AddSQMUserLoginQualityMonitorBAL(int qualityMonitorCode, ref string message);

        #region Upload File Details

        Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int ADMIN_QM_CODE);
        string AddFileUploadDetailsBAL(QualityMonitorFileUploadViewModel fileUploadViewModel);
        string DeleteFileDetails(int ADMIN_QM_CODE, string FILE_NAME);
        string DeletePdfFileDetails(int ADMIN_QM_CODE, string FILE_NAME);
        Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int ADMIN_QM_CODE);
        //string UpdateImageDetailsBAL(QualityMonitorFileUploadViewModel fileuploadViewModel);
        //string UpdatePDFDetailsBAL(QualityMonitorFileUploadViewModel fileuploadViewModel);
        void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath);

        #endregion


        #region PAN Upload For Monitor
        
        Array GetPANFileListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int qmCode);
        string ValidatePDFFile(int FileSize, string FileExtension);
        string AddPANUploadDetailsBAL(List<QualityMonitorFileUploadViewModel> lstFileUploadViewModel);
        string DeletePANFileDetailsBAL(int qmCode);

        #endregion

        #endregion Quality Monitor BAL declaration


        #region Master Streams

        bool AddMasterStreams(MasterStreamsViewModel masterStreamsViewModel, ref string message);
        bool EditMasterStreams(MasterStreamsViewModel masterStreamsViewModel, ref string message);
        Boolean DeleteMasterStreams(int streamsId);
        Array ListMasterStreams(string streamType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterStreamsViewModel GetStreamsDetails_ByStream(int StreamCode);

        List<SelectListItem> GetStreamsCode();

        #endregion master streams

        #region Master MLA Members

        bool AddMLAMembers(MasterMLAMembersViewModel memberModel, ref string message);

        Array ListMLAMembers(int stateCode, int term, int constCode, string memberName, int page, int rows, string sidx, string sord, out long totalRecords);

        bool DeleteMLAMembers(int memberCode);

        bool EditMLAMembers(MasterMLAMembersViewModel memberModel, ref string message);

        MasterMLAMembersViewModel GetMemberDetails(int memberCode);

        #endregion

        #region Master Admin Department

        bool AddAdminDepartment(AdminDepartmentViewModel adminModel, ref string message);

        Array ListAdminDepartmentList(int stateCode, int agencyCode, int page, int rows, string sidx, string sord, out long totalRecords);//int districtCode, string departmentName, changes by Koustubh Nakate on 16-05-2013

        bool DeleteAdminDepartment(int adminCode, ref string message);

        bool EditAdminDepartment(AdminDepartmentViewModel adminModel, ref string message);

        AdminDepartmentViewModel GetAdminDetails(int adminCode);

        AdminDepartmentViewModel AddStateAdmin();

        #endregion
        #region PIU Department
        Array GetDPIUListBAL(int stateCode, int agencyCode, int adminNDCode, string activeflag, int? page, int? rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Master Execution

        bool AddMasterExecution(MasterExecutionItemViewModel masterExecutionView, ref string message);
        bool EditMasterExecution(MasterExecutionItemViewModel masterExecutionView, ref string message);
        Boolean DeleteMasterExecution(int ExecutionId);
        Array ListMasterExecution(string ItemType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterExecutionItemViewModel GetExecutionDetails_ByExecutionCode(int ExecutionId);

        MASTER_EXECUTION_ITEM CloneExecutionModel(MasterExecutionItemViewModel masterExecutionView, bool flagAddEdit);
        MasterExecutionItemViewModel CloneExecutionObject(MASTER_EXECUTION_ITEM masterExecutionModel);
        #endregion master_execution

        #region Master Admin Sqc

        bool AddAdminSqc(AdminSqcViewModel adminSqcViewModel, ref string message);
        bool EditAdminSqc(AdminSqcViewModel adminSqcViewModel, ref string message);
        Boolean DeleteAdminSqc(int AdminQcId);
        Array ListadminQc(int stateCode, int adminNdCode, string status, int? page, int? rows, string sidx, string sord, out long totalRecords);
        AdminSqcViewModel GetAdminQc_ByQcCode(int AdminQcCode);

        List<MASTER_DESIGNATION> GetDesignation();

        List<MASTER_STATE> GetStates();

        List<MASTER_DISTRICT> GetDistrictName(int stateId);

        List<SelectListItem> GetDepartListBAL(int id);
        List<SelectListItem> GetDeptOfStates(int id);

        #endregion admin_sqc

        #region Master Checklist Points


        bool AddMasterChecklist(MasterChecklistPointsViewModel masterChecklistViewModel, ref string message);
        bool EditMasterChecklist(MasterChecklistPointsViewModel masterChecklistViewModel, ref string message);
        Boolean DeleteMasterChecklist(int masterChecklistId);
        Array ListMasterChecklist(int? page, int? rows, string sidx, string sord, out long totalRecords);

        MasterChecklistPointsViewModel GetChecklistDetails_ByChecklistCode(int ChecklistCode);

        #endregion Checklist Points Declaration

        #region Master Agency

        bool AddMasterAgency(MasterAgencyViewModel masterAgencyViewModel, ref string message);
        bool EditMasterAgency(MasterAgencyViewModel masterAgencyViewModel, ref string message);
        Boolean DeleteMasterAgency(int masterAgencyId);
        Array ListMasterAgency(string agencyType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterAgencyViewModel GetAgencyDetails_ByAgencyCode(int AgencyCode);

        List<SelectListItem> GetAgencyCode();
        #endregion Master Agency

        #region  Master Reason


        bool AddMasterReason(MasterReasonViewModel masterReasonViewModel, ref string message);
        bool EditMasterReason(MasterReasonViewModel masterReasonViewModel, ref string message);
        Boolean DeleteMasterReason(int masterReasonId);
        Array ListMasterReason(string reasonType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        MasterReasonViewModel GetReasonDetails_ByReasonCode(int ReasonCode);

        List<SelectListItem> GetReasonCode();

        #endregion  Reason

        #region Master Technical Agency


        bool AddAdminTechnicalAgency(AdminTechnicalAgencyViewModel adminTechnicalAgencyViewModel, ref string message);

        bool EditAdminTechnicalAgency(AdminTechnicalAgencyViewModel adminTechnicalAgencyViewModel, ref string message);

        Boolean DeleteAdminTechnicalAgency(int AdminTechnicalAgencyId);

        Array ListadminTechnicalAgency(string taName, string taType, int? page, int? rows, string sidx, string sord, out long totalRecords);

        Array ListdistrictTechnicalAgency(int? page, int? rows, string sidx, string sord, out long totalRecords, string agencyType, int stateCode, int districtCode);

        AdminTechnicalAgencyViewModel GetAdminTA_ByTACode(int AdminTechnicalAgencyCode);

        AdminTechnicalAgencyViewModel CloneAdminTechnicalAgencyObject(ADMIN_TECHNICAL_AGENCY AdminTechnicalAgencyCode);

        List<MASTER_DESIGNATION> GetDesignationTA();

        List<MASTER_STATE> GetStatesTA();

        List<MASTER_DISTRICT> GetDistrictNameTA(int? stateId);

        #endregion TA

        #region MapRegion-Districts
        bool MapRegionDistrictsBAL(string encryptedRegionCode, string encryptedDistrictCodes);

        Array GetMappedDistrictDetailsListBAL_Region(int regionCode, int? page, int? rows, string sidx, string sord, out long totalRecords);//added by koustubh Nakate on 14-05-2013

        bool DeleteMappedRegionDistrictBAL(int regionId);

        #endregion MapRegion-Districts

        //added by Koustubh Nakate on 16/05/2013 for admin department 
        #region DPIU LIST BY SRDA

        Array GetDPIUListBAL_ByAdminNDCode(int adminNDCode, int? page, int? rows, string sidx, string sord, out long totalRecords);


        #endregion DPIU LIST BY SRDA

        #region Bank Details

        bool AddContractorBankDetails(MasterContractorBankDetails contractorBankDetails, ref string message);
        bool EditContractorBankDetails(MasterContractorBankDetails contractorBankDetails, ref string message);
        bool DeleteContractorBankDetails(int accountId, int coustomerId, ref string message);
        Array ListContractorBankDetails(int ContractorCode, int regState, int? page, int? rows, string sidx, string sord, out long totalRecords);
        List<MASTER_DISTRICT> getContractorDistricts(int contractorId);
        MasterContractorBankDetails getContractorBankDetails_ByBankCode(int accountId, int ContractorId);
        //added by pp 01-05-2018
        Boolean FinalizeBankDetails(int AccountCode, int ContractorCode);
        #endregion BankDetails

        #region Master Nodal Officer
        bool DeleteBankDetailsNO(int accountId, int customerId, ref string message);
        bool EditBankDetailsNO(MasterContractorBankDetails contractorBankDetails, ref string message);
        MasterContractorBankDetails getBankDetailsNO_ByBankCode(int accountId, int ContractorId);
        bool AddBankDetailsNO(MasterContractorBankDetails contractorBankDetails, ref string message);
        Array ListBankDetailsNO(int ContractorCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool AddNodalOfficer(AdminNodalOfficerViewModel nodalOfficerView, ref string message);
        bool EditNodalOfficer(AdminNodalOfficerViewModel nodalOfficerView, ref string message);
        bool DeleteNodalOfficer(int OfficerCode);
        Array ListNodalOfficer(int stateCode, int officeCode, int designationCode, int NoTypeCode, string moduleType, string active, int? page, int? rows, string sidx, string sord, out long totalRecord);
        AdminNodalOfficerViewModel GetAdminNodalOfficer_ByOfficerCode(int OfficerCode);
        List<ADMIN_DEPARTMENT> GetAdminNdCode();
        List<MASTER_DESIGNATION> GetNodalDesignation();
        List<SelectListItem> PopulateDistrict(string adminNdCode);

        #endregion NodalOfficer

        #region Agency-State-District-Mapping
        bool MapAgencyStatesBAL(string encryptedAgencyCode, string encryptedStateCodes, string startDate);

        bool MapAgencyDistrictsBAL(string encryptedAgencyCode, string encryptedDistrictCodes, string startDate);

        Array GetMappedStateDetailsListBAL_Agency(int stateCode, int agencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        Array GetMappedDistrictDetailsListBAL_Agency(int agencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool DeleteMappedStateAgencyBAL(int adminId);

        bool DeleteMappedDistrictAgencyBAL(int adminId);

        bool FinalizeMappedStateAgencyBAL(int adminId);

        bool FinalizeMappedDistrictAgencyBAL(int adminId);

        bool UpdateStateEndDatePTA_BAL(string endDate, string encryptedadminId, ref string message);

        bool UpdateDistrictEndDateSTA_BAL(string endDate, string encryptedadminId, ref string message);

        EndDateDistrictViewModel AddEndDateDistrictBAL(EndDateDistrictViewModel endDateDistrictViewModel, Int32 adminTaId);

        EndDateStateViewModel AddEndDateStateBAL(EndDateStateViewModel endDateStateViewModel, Int32 adminTaId);

        #endregion Agency-State-District-Mapping


        bool MapSRRDADistrictsBAL(string encryptedAdminCode, string encryptedDistrictCodes);

        Array GetMappedDistrictDetailsListBAL_SRRDA(int adminCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool DeleteMappedSRRDADistrictBAL(int adminId);

        bool ContractorRegChangeStatusBAL(int conID, int ConRegCode, bool IsActive);

        #region MASTER_TAX

        Array GetTaxDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);

        MasterTaxViewModel GetTaxDetails(int taxCode);

        bool DeleteTaxDetailsBAL(int taxCode);

        bool AddTaxDetailsBAL(MasterTaxViewModel taxModel, ref string message);

        bool EditTaxDetailsBAL(MasterTaxViewModel taxModel, ref string message);

        #endregion


        #region Technology
        Array ListTechnologyDetails(string statusType,int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddTechnologyDetails(MasterTechnologyViewModel techViewModel, ref string message);
        bool EditTechnologyDetails(MasterTechnologyViewModel techViewModel, ref string message);
        MasterTechnologyViewModel GetTechnologyDetails(int techCode);
        bool DeleteTechnologyDetails(int technologyCode);
        bool ChangeTchnologyStatus(int technologyCode);

        #endregion Technology

        #region Test
        Array ListTestDetails(string statusType,int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddTestDetails(MasterTestViewModel testViewModel, ref string message);
        bool EditTestDetails(MasterTestViewModel testViewModel, ref string message);
        MasterTestViewModel GetTestDetails(int testCode);
        bool DeleteTestDetails(int testCode);
        bool ChangeTestStatus(int testCode);
        #endregion Test

        #region Alerts
        Array ListAlertsDetails(string status, int? page, int? rows, String sidx, String sord, out long totalRecords);
        bool AddAlertDetails(AdminAlertsViewModel AlertViewModel, ref string message);
        bool EditAlertDetails(AdminAlertsViewModel AlertViewModel, ref string message);
        AdminAlertsViewModel ViewAlertDetails(int AlertId);
        bool DeleteAlertDetails(int AlertId);
        bool ChangeAlertStatus(int AlertId);
        #endregion Alerts

        #region PMGSY2

        Array ListPMGSYIIDetails(int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool ChangePMGSY2Status(int stateCode);
        bool IsPMGSY2Active(int stateCode);
        #endregion PMGSY2

        #region Feedback Category

        Array ListFeedbackCategoryDetails(int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddFeedbackDetails(FeedbackCategoryViewModel feedbackCategoryViewModel, ref string message);
        bool EditFeedbackDetails(FeedbackCategoryViewModel feedbackCategoryViewModel, ref string message);
        FeedbackCategoryViewModel GetFeedbackDetails(int feebackId);
        bool DeleteFeedbackDetails(int feedBackId);

        #endregion Feedback Category

        #region Carriage

        Array ListCarriageDetails(string statusType,int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddCarriageDetails(MasterCarriageViewModel carriageViewModel, ref string message);
        bool EditCarriageDetails(MasterCarriageViewModel carriageViewModel, ref string message);
        MasterCarriageViewModel GetCarriageDetails(int carriageCode);
        bool DeleteCarriageDetails(int carriageCode);
        bool ChangeCarriageStatus(int carriageCode);

        #endregion Carriage


        #region Info
        Array ListInfoDetails(string infoType, int infoStateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddInfoDetails(MasterInfoViewModel infoViewModel, ref string message);
        bool EditInfoDetails(MasterInfoViewModel infoViewModel, ref string message);
        MasterInfoViewModel GetInfoDetails(int infoCode);
        bool DeleteInfoDetails(int infoCode);
        bool ChangeInfoStatus(int infoCode);
        #endregion Info

        #region Contractor Detail
        Array GetContractorAgreementListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contrctorId);
        Array GetContractorIMSMaintenanceListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contractorNo);
        Array GetContractorPaymentListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, string status, string contractorName, string conStatus, string panNumber, int classType, string regNo, string companyName, int contractorNo);
        #endregion

        #region Cluster Master
        // Array ListClusterDetailBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationListClusterBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddClusterHabitationBAL(string encryptedHabCode, string habitationName, int blockCode);
        Array ListClusterBAL(int stateCode, int districtCode, int blockCode, string activeStatus, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteClusterBAL(int clusterCode, ref string message);
        MasterClusterEditViewModel GetClusterDetailsBAL(int clusterCode);
        bool EditClusterNameHabiationBAL(MasterClusterEditViewModel clusterModel, ref string message);

        MasterClusterViewEditHabiationModel GetClusterHabsDetailsBAL(int clusterCode);
        bool DeleteClusterHabitationBAL(int clusterCode, int habCode, ref string message);
        Array GetClusterHabitationListByClusterCodeBAL(int clusterCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetAddHabitationListIntoClusterBAL(int clusterCode, int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateClusterHabitationBAL(string encryptedHabCodeSendbyCheckBoxCheck, string habitationName, int ClusterCode);
        bool FinalizeClusterHabitationBAL(MasterClusterViewEditHabiationModel clusterModel, ref string message);
        #endregion

        #region Cluster Core Network
        Array ListClusterCNBAL(int stateCode, int districtCode, int blockCode, string activeStatus, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteClusterCNBAL(int clusterCode, ref string message);
        bool DeleteClusterCNHabitationBAL(int clusterCode, int habCode, ref string message);
        bool EditClusterCNNameHabiationBAL(MasterClusterEditViewModel clusterModel, ref string message);
        MasterClusterEditViewModel GetClusterCNDetailsBAL(int clusterCode);
        MasterClusterViewEditHabiationModel GetClusterCNHabsDetailsBAL(int clusterCode);
        Array GetCoreNetworkListClusterCNBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationListClusterCNBAL(int roadCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetAddHabitationListIntoClusterCNBAL(int clusterCode, int roadCode, int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetClusterCNHabitationListByClusterCodeBAL(int clusterCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddClusterCNHabitationBAL(string encryptedHabCodeSendbyCheckBoxCheck, string habitationName, int blockCode);
        Array GetAddCoreNetworkListByClusterCodeBAL(int clusterCode, int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool UpdateClusterCNHabitationBAL(string encryptedHabCodeSendbyCheckBoxCheck, string habitationName, int clusterCode);
        bool FinalizeClusterCNHabitationBAL(MasterClusterViewEditHabiationModel clusterModel, ref string message);
        #endregion

        #region EC Check List
        bool AddImsEcCheckListBAL(IMSECCheckListViewModel imsEcCheckModel, ref string message);
        Array ListImsEcCheckListBAL(int stateCode, int year, int batch, int agencyCode, string typeEc, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteImsEcCheckBAL(int EcCode, ref string message);
        bool EditImsEcCheckBAL(IMSECCheckListViewModel imsEcCheckModel, ref string message);
        IMSECCheckListViewModel GetImsEcCheckDetailsBAL(int ecCode);
        bool FinalizeECCheckListBAL(int ecId, ref string meesage);
        string DeFinalizeECBAL(int ecId);
        #endregion

        #region EC File Upload
        bool AddImsEcFileUploadBAL(IMSEcFileUploadViewModel imsEcFileUpload, ref string message);
        Array ListImsFileUploadBAL(int stateCode, int year, int batch, int agencyCode, string fileType, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteImsFileUploadBAL(int FileCode, ref string message);
        bool EditImsFileUploadBAL(IMSEcFileUploadViewModel imsEcFileUploadModel, ref string message);
        IMSEcFileUploadViewModel GetImsEcFileUploadDetailsBAL(int fileCode);
        #endregion
        #region IMs EC Training
        bool AddImsEcTrainingBAL(IMSEcTrainingViewModel imsEcTraining, ref string message);
        Array ListImsEcTrainingBAL(int stateCode, int year, int desigCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteImsTrainingBAL(int TrainingCode, ref string message);
        bool EditImsTrainingBAL(IMSEcTrainingViewModel imsEcTrainingModel, ref string message);
        IMSEcTrainingViewModel GetImsEcTrainingDetailsBAL(int trainingCode);
        #endregion

        #region Quality Inspection & ATR Deletions

        Array QMViewInspectionDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords,
                                                int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string qmType);

        List<qm_inspection_list_atr_Result> ATRDetailsBAL(int stateCode, int monitorCode, int fromMonth, int fromYear,
                                                int toMonth, int toYear, string atrStatus, string rdStatus);
        #endregion

        #region MAINTENANCE_POLICY_UPLOAD

        bool AddMaintenancePolicyBAL(MaintenancePolicyViewModel model, ref string message);

        Array ListMaintenancePolicyBAL(int stateCode, int agencyCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool DeleteMaintenancePolicyBAL(int FileCode, ref string message);

        bool EditMaintenancePolicyBAL(MaintenancePolicyViewModel model, ref string message);

        MaintenancePolicyViewModel GetMaintenancePolicyDetailsBAL(int fileCode);

        #endregion

        #region BLOCKING AND LISTING FOR QUALITY MONITOR
        //Blocking of Quality Monitor
        bool BlockeQualityMonitor(string PAN);
        // by pradip for blocked QM
        Array ListBlockedQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords);

        #endregion


        #region MATRIX PARAMETERS
        //pp
        Array ListMatrixDetails(int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool AddMatrixParamDetails(MatrixParamModel carriageViewModel, ref string message);

        bool DeleteMatrixParamDetails(int matrixId);

        MatrixParamModel GetMatrixParamDetails(int matrixCode);

        bool EditMatrixParamDetails(MatrixParamModel carriageViewModel, ref string message);
        #endregion

        #region PMGSY state list.
        Array ListPmgsyStates(int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddStateBAL(PMGSYStatesViewModel masterAgencyViewModel, ref string message);
        #endregion


        #region Financial Year Target
        Array ListFinancialYearTarget(int stateCode, int year, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Boolean DeleteFinancialYearTarget(int pmgsyID, ref string message);

        #endregion

        #region Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC
        //Added by Hrishikesh to provide Quality Monitor Profile Information to NQM / SQM as Present in CQC / SQC--start
        MasterAdminQualityMonitorViewModel GetQMProfileInformationBAL(int userId);
        #endregion
    }
}