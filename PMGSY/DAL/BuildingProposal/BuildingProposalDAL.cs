#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalDAL.cs     
        * Description   :   Data Methods for Creating , Editing, Deleting Road Proposal and Related Screens of Road Proposals Habitation Details
                            Traffic Intensity , CBR Index and File Upload                            
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   04/April/2013
        * Modified By   :   Shyam Yadav
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;
using PMGSY.Controllers;
using System.Transactions;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using Microsoft.SqlServer.Server;
using PMGSY.Models.BuildingProposal;
using System.Data.Entity.Core;

namespace PMGSY.DAL.BuildingProposal
{
    public class BuildingProposalDAL : IBuildingProposalDAL
    {
        Models.PMGSYEntities dbContext;
        public BuildingProposalDAL()
        {
            dbContext = new Models.PMGSYEntities();
        }

        public List<BuildingProposalViewModel> BuildingProposalListPIU(BuildingProposalViewModel bm)
        {
            try
            {

                int EACode = dbContext.ADMIN_DEPARTMENT.Where(d => d.ADMIN_ND_CODE == bm.MAST_DPIU_CODE).FirstOrDefault().MAST_AGENCY_CODE;
                List<BuildingProposalViewModel> BuildingList = (from B in dbContext.USP_BUILDING_PROPOSAL_LIST(bm.MAST_STATE_CODE, bm.MAST_DISTRICT_CODE, bm.MAST_BLOCK_CODE, bm.IMS_YEAR, bm.IMS_BATCH, bm.IMS_COLLABORATION, EACode, bm.IMS_UPGRADE_CONNECT, bm.PMGSYScheme).ToList<USP_BUILDING_PROPOSAL_LIST_Result>()
                                                               
                                                                select new BuildingProposalViewModel
                                                                {
                                                                    StateName = B.MAST_STATE_NAME,
                                                                    DistrictName = B.MAST_DISTRICT_NAME,
                                                                    MAST_BLOCK_NAME = B.MAST_BLOCK_NAME,
                                                                    IMS_PACKAGE_ID = B.IMS_PACKAGE_ID,
                                                                    IMS_YEAR_FINANCIAL = B.IMS_YEAR,
                                                                    IMS_ROAD_NAME = B.BUILDING_NAME,
                                                                    IMS_PAV_EST_COST = B.BUILDING_AMT,
                                                                    IMS_BATCH = B.IMS_BATCH,
                                                                    MAST_FUNDING_AGENCY_NAME = B.IMS_COLLABORATION,
                                                                    IMS_PR_ROAD_CODE = B.IMS_PR_ROAD_CODE,
                                                                    IMS_ISCOMPLETED = B.IMS_ISCOMPLETED,
                                                                    IMS_SANCTIONED = B.IMS_SANCTIONED,
                                                                    IMS_SANCTIONED_BY = B.IMS_SANCTIONED_BY,
                                                                    IMS_SANCTIONED_DATE = B.IMS_SANCTIONED_DATE,
                                                                    IMS_LOCK_STATUS = B.IMS_LOCK_STATUS


                                                                }).ToList<BuildingProposalViewModel>();

                return BuildingList;




            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        //public List<IMS_SANCTIONED_PROJECTS> BuildingProposalList(int state, int district, int block, int syear, int batch, int agency, int stream, string proptype, string propstatus, string propconnect, string PTAStatus, string STAStatus, string MRDStatus)
        public List<BuildingProposalViewModel> BuildingProposalListMRD(BuildingProposalViewModel bm)
        {
            try
            {
                

                List<BuildingProposalViewModel> BuildingList =(from B in dbContext.USP_BUILDING_PROPOSAL_LIST(bm.MAST_STATE_CODE, bm.MAST_DISTRICT_CODE,bm.MAST_BLOCK_CODE, bm.IMS_YEAR, bm.IMS_BATCH,bm.IMS_COLLABORATION, bm.MAST_DPIU_CODE, bm.IMS_UPGRADE_CONNECT,bm.PMGSYScheme).ToList<USP_BUILDING_PROPOSAL_LIST_Result>()
                                                               where /*(B.IMS_ISCOMPLETED=="D" || B.IMS_ISCOMPLETED=="M" ) &&*/ B.IMS_LOCK_STATUS=="Y"
                                                               select new BuildingProposalViewModel
                                                               {
                                                                   StateName=B.MAST_STATE_NAME,
                                                                   DistrictName=B.MAST_DISTRICT_NAME,
                                                                   MAST_BLOCK_NAME=B.MAST_BLOCK_NAME,
                                                                   IMS_PACKAGE_ID=B.IMS_PACKAGE_ID,
                                                                   IMS_YEAR_FINANCIAL = B.IMS_YEAR,
                                                                   IMS_ROAD_NAME=B.BUILDING_NAME,
                                                                   IMS_PAV_EST_COST=B.BUILDING_AMT,
                                                                   IMS_BATCH=B.IMS_BATCH,
                                                                   MAST_FUNDING_AGENCY_NAME=B.IMS_COLLABORATION,
                                                                   IMS_PR_ROAD_CODE=B.IMS_PR_ROAD_CODE,
                                                                   IMS_ISCOMPLETED=B.IMS_ISCOMPLETED,
                                                                   IMS_SANCTIONED=B.IMS_SANCTIONED,
                                                                   IMS_SANCTIONED_BY=B.IMS_SANCTIONED_BY,
                                                                   IMS_SANCTIONED_DATE=B.IMS_SANCTIONED_DATE,
                                                                   IMS_LOCK_STATUS=B.IMS_LOCK_STATUS


                                                               }).ToList<BuildingProposalViewModel>();

                if (bm.IMS_SANCTIONED != "A")
                    return BuildingList.Where(b => b.IMS_SANCTIONED == bm.IMS_SANCTIONED).ToList(); 
                else
                    return BuildingList;
                
               
               

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        /// <summary>
        /// Save the Road Proposal
        /// </summary>
        /// <param name="objProposal"></param>
        /// <returns></returns>
        public string SaveBuildingProposalDAL(IMS_SANCTIONED_PROJECTS objProposal)
        {
            
            try
            {
                Int32? MaxID;
                if (!dbContext.IMS_SANCTIONED_PROJECTS.Any())
                {
                    MaxID = 0;
                }
                else
                {
                    MaxID = (from c in dbContext.IMS_SANCTIONED_PROJECTS select (Int32?)c.IMS_PR_ROAD_CODE ?? 0).Max();
                }

                objProposal.IMS_PR_ROAD_CODE = Convert.ToInt32(MaxID) + 1;
                objProposal.USERID = PMGSYSession.Current.UserId;
                objProposal.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.IMS_SANCTIONED_PROJECTS.Add(objProposal);

                dbContext.SaveChanges();

               
                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }

                return new CommonFunctions().FormatErrorMessage(modelstate);
            }

            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }









        public bool BuildingProposalDelete(int ims_pr_road_code, ref string message)
        {
            try
            {
                ///Changes by SAMMED A. PATIL on 22FEB2018
                var adapter = (IObjectContextAdapter)dbContext;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 0;

                IMS_SANCTIONED_PROJECTS isp = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == ims_pr_road_code).FirstOrDefault();

                dbContext.IMS_SANCTIONED_PROJECTS.Remove(isp);
                dbContext.SaveChanges();
                message = "Buiding details deleted successfully.";

                return true;
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                message = new CommonFunctions().FormatErrorMessage(modelstate);
                return false;
            }

            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this details.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public bool BuildingProposalUpdate(BuildingProposalViewModel buildingViewModel, ref string message)
        {
            try
            {
                IMS_SANCTIONED_PROJECTS objProposal = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == buildingViewModel.IMS_PR_ROAD_CODE).FirstOrDefault();

                
                objProposal.IMS_YEAR = buildingViewModel.IMS_YEAR;
                objProposal.IMS_BATCH = buildingViewModel.IMS_BATCH;
                // New Package or Exising Package
                if (buildingViewModel.IMS_EXISTING_PACKAGE.ToUpper() == "N")
                {
                    objProposal.IMS_PACKAGE_ID = buildingViewModel.PACKAGE_PREFIX + buildingViewModel.IMS_PACKAGE_ID;
                }
                else
                {
                    objProposal.IMS_PACKAGE_ID = buildingViewModel.EXISTING_IMS_PACKAGE_ID;
                }


                objProposal.MAST_BLOCK_CODE = buildingViewModel.MAST_BLOCK_CODE;

                objProposal.IMS_ROAD_NAME = buildingViewModel.IMS_ROAD_NAME;
                objProposal.IMS_COLLABORATION = buildingViewModel.IMS_COLLABORATION;
                objProposal.IMS_STREAMS = buildingViewModel.IMS_STREAMS;

                objProposal.IMS_EXISTING_PACKAGE = buildingViewModel.IMS_EXISTING_PACKAGE;
                objProposal.IMS_PAV_EST_COST = buildingViewModel.IMS_PAV_EST_COST;
                objProposal.IMS_ZP_RESO_OBTAINED ="Y";

                objProposal.IMS_MAINTENANCE_YEAR1 = buildingViewModel.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = buildingViewModel.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = buildingViewModel.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = buildingViewModel.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = buildingViewModel.IMS_SANCTIONED_MAN_AMT5;

                objProposal.IMS_SANCTIONED_MAN_AMT1 = buildingViewModel.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = buildingViewModel.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = buildingViewModel.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = buildingViewModel.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = buildingViewModel.IMS_SANCTIONED_MAN_AMT5;
                objProposal.IMS_REMARKS = buildingViewModel.IMS_REMARKS;

                
                dbContext.Entry(objProposal).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "Buiding details update successfully.";

                return true;
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                message = new CommonFunctions().FormatErrorMessage(modelstate);
                return false;
            }

            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not update this details.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool BuildingProposalMoRDSacntionUpdate(BuildingSanctionViewModel buildingMoRDSacntionViewModel, ref string message)
        {
            try
            {
                IMS_SANCTIONED_PROJECTS objProposal = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == buildingMoRDSacntionViewModel.IMS_PR_ROAD_CODE).FirstOrDefault();

                objProposal.IMS_SANCTIONED="Y";
                objProposal.IMS_ISCOMPLETED="M";
                objProposal.IMS_SANCTIONED_DATE = Convert.ToDateTime(buildingMoRDSacntionViewModel.IMS_SANCTIONED_DATE);
                objProposal.IMS_SANCTIONED_BY=buildingMoRDSacntionViewModel.IMS_SANCTIONED_BY;
                objProposal.IMS_PROG_REMARKS = buildingMoRDSacntionViewModel.IMS_PROG_REMARKS;
                objProposal.IMS_SANCTIONED_PAV_AMT = objProposal.IMS_PAV_EST_COST;
                dbContext.Entry(objProposal).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                message = "MoRD Sanction details Update successfully.";

                return true;
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ModelStateDictionary modelstate = new ModelStateDictionary();

                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelstate.AddModelError(ve.ErrorMessage, ve.ErrorMessage);
                    }
                }
                message = new CommonFunctions().FormatErrorMessage(modelstate);
                return false;
            }

            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not update this details.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Processing Your Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public BuildingProposalViewModel GetBuildingProposal(int id)
        {
            var objProposal = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == id).FirstOrDefault();
            BuildingProposalViewModel BuildingModel = new BuildingProposalViewModel
            {
                
                IMS_YEAR = objProposal.IMS_YEAR,
                IMS_BATCH = objProposal.IMS_BATCH,
                IMS_PR_ROAD_CODE = objProposal.IMS_PR_ROAD_CODE,
                IMS_PACKAGE_ID = objProposal.IMS_PACKAGE_ID,
                EXISTING_IMS_PACKAGE_ID = objProposal.IMS_PACKAGE_ID,
                MAST_STATE_CODE = objProposal.MAST_STATE_CODE,
                StateName = objProposal.MASTER_STATE.MAST_STATE_NAME,
                DistrictName =  objProposal.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                MAST_DISTRICT_CODE = objProposal.MAST_DISTRICT_CODE,
                MAST_BLOCK_NAME =  objProposal.MASTER_BLOCK.MAST_BLOCK_NAME,
                MAST_BLOCK_CODE = objProposal.MAST_BLOCK_CODE,
                MAST_DPIU_CODE = objProposal.MAST_DPIU_CODE,
                IMS_ROAD_NAME = objProposal.IMS_ROAD_NAME,
                IMS_PROPOSAL_TYPE = objProposal.IMS_PROPOSAL_TYPE,
                IMS_UPGRADE_CONNECT = objProposal.IMS_UPGRADE_CONNECT,
                IMS_COLLABORATION = objProposal.IMS_COLLABORATION,
                MAST_FUNDING_AGENCY_NAME=objProposal.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME,
               // IMS_STREAMS = objProposal.IMS_STREAMS,


                IMS_PAV_EST_COST = objProposal.IMS_PAV_EST_COST,
               // IMS_ZP_RESO_OBTAINED = objProposal.IMS_ZP_RESO_OBTAINED,

                IMS_SANCTIONED_MAN_AMT1 = objProposal.IMS_MAINTENANCE_YEAR1,
                IMS_SANCTIONED_MAN_AMT2 = objProposal.IMS_MAINTENANCE_YEAR2,
                IMS_SANCTIONED_MAN_AMT3 = objProposal.IMS_MAINTENANCE_YEAR3,
                IMS_SANCTIONED_MAN_AMT4 = objProposal.IMS_MAINTENANCE_YEAR4,
                IMS_SANCTIONED_MAN_AMT5 = objProposal.IMS_MAINTENANCE_YEAR5,
                IMS_REMARKS = objProposal.IMS_REMARKS,
                
                STA_SANCTIONED=objProposal.STA_SANCTIONED,
                STA_SANCTIONED_DATE=objProposal.STA_SANCTIONED_DATE.HasValue?objProposal.STA_SANCTIONED_DATE.Value.ToString():"",
                MS_STA_REMARKS=objProposal.IMS_STA_REMARKS,
                STA_SANCTIONED_BY=objProposal.STA_SANCTIONED_BY,

                PTA_SANCTIONED=objProposal.PTA_SANCTIONED,
                PTA_SANCTIONED_DATE = objProposal.PTA_SANCTIONED_DATE.HasValue ? objProposal.PTA_SANCTIONED_DATE.Value.ToString() : "",
                MS_PTA_REMARKS = objProposal.IMS_PTA_REMARKS,
                PTA_SANCTIONED_BY = objProposal.PTA_SANCTIONED_BY,

                IMS_SANCTIONED=objProposal.IMS_SANCTIONED,
                IMS_SANCTIONED_DATE = objProposal.IMS_SANCTIONED_DATE.HasValue ? objProposal.IMS_SANCTIONED_DATE.Value.ToString() : "",
                IMS_REASON = objProposal.IMS_REASON,
                IMS_SANCTIONED_BY = objProposal.IMS_SANCTIONED_BY,
                IMS_LOCK_STATUS=objProposal.IMS_LOCK_STATUS,
                IMS_ISCOMPLETED=objProposal.IMS_ISCOMPLETED,
                IMS_PROG_REMARKS=objProposal.IMS_PROG_REMARKS
            };
            return BuildingModel;
        }


        public bool PIUFinalizedBuildingProposal(int id)
        {
            try
            {
                var objProposal = dbContext.IMS_SANCTIONED_PROJECTS.Where(sp => sp.IMS_PR_ROAD_CODE == id).FirstOrDefault();
                objProposal.IMS_ISCOMPLETED = "D";
                objProposal.IMS_LOCK_STATUS = "Y";
                dbContext.Entry(objProposal).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
   
}