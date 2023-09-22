using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models.MPMLAProposal;
using System.Data.Entity;
using System.Data.Entity.Core;

namespace PMGSY.DAL.MPMLAProposal
{
    public class MPMLAProposalDAL : IMPMLAProposalDAL
    {
        PMGSYEntities dbContext = null;
        #region MP Proposal

            //populate MP Constituency
            public List<SelectListItem> PopulateMPConstatuency()
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    List<SelectListItem> lstConstatuency = null;

                    lstConstatuency = new SelectList(dbContext.MASTER_MP_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_MP_CONST_CODE", "MAST_MP_CONST_NAME").ToList();

                    lstConstatuency.Insert(0, new SelectListItem() { Text = "select Constituency", Value = "0" });

                    return lstConstatuency;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    throw ex;
                }
                finally {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }

            public Array ListMPProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
            {
                try
                {   
                    dbContext = new PMGSYEntities();

                    var lstMPProposedRoads = (from MpProposedRoads in dbContext.IMS_MP_PROPOSAL_STATUS
                                              where ((yearCode == -1 ? 1 : MpProposedRoads.IMS_YEAR) == (yearCode == -1 ? 1 : yearCode)) &&
                                                   ((constituencyCode == 0 ? 1 : MpProposedRoads.MAST_MP_CONST_CODE) == (constituencyCode == 0 ? 1 : constituencyCode))
                                                   select MpProposedRoads
                                                   ).ToList();

                    totalRecords = lstMPProposedRoads.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "IMS_ROAD_DETAILS":
                                    lstMPProposedRoads = lstMPProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                default:
                                    lstMPProposedRoads = lstMPProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                            }
                        }
                        else
                        {

                            switch (sidx)
                            {
                                case "IMS_ROAD_DETAILS":
                                    lstMPProposedRoads = lstMPProposedRoads.OrderByDescending(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                default:
                                    lstMPProposedRoads = lstMPProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        lstMPProposedRoads = lstMPProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    return lstMPProposedRoads.Select(item => new
                    {   
                        cell = new[]{  
                                      item.IMS_ROAD_DETAILS == null?"":item.IMS_ROAD_DETAILS.Trim(),
                                      item.IMS_YEAR == null?"-":item.IMS_YEAR.ToString(),
                                      item.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME == null?"-":item.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME.ToString(),
                                      //item.IMS_INCLUDED_IN_CN == "Y"?item.PLAN_ROAD.PLAN_RD_NAME:(item.MASTER_REASON.MAST_REASON_NAME ==null?"":item.MASTER_REASON.MAST_REASON_NAME.ToString()),
                                      item.IMS_INCLUDED_IN_CN == "Y"?"Yes - Core Network :"+(item.PLAN_CN_ROAD_CODE == null?"NA":item.PLAN_ROAD.PLAN_RD_NAME):(item.IMS_INCLUDED_IN_CN == "N"?"NA":"No - Reason :"+(item.IMS_REASON_ID_1 ==null?"":item.MASTER_REASON.MAST_REASON_NAME.ToString())),
                                      //item.IMS_INCLUDED_IN_PROPOSAL == "Y"?item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME:(item.MASTER_REASON1.MAST_REASON_NAME == null?"": item.MASTER_REASON1.MAST_REASON_NAME.ToString()),
                                      item.IMS_INCLUDED_IN_PROPOSAL == "Y"?"Yes - Proposal :"+(item.IMS_PR_ROAD_CODE == null?"NA":item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME):(item.IMS_INCLUDED_IN_PROPOSAL == "N"?"NA":"No - Reason :"+(item.IMS_REASON_ID_2 == null?"": item.MASTER_REASON1.MAST_REASON_NAME.ToString())),
                                       URLEncrypt.EncryptParameters1(new string[]{
                                           "MPConstCode="+item.MAST_MP_CONST_CODE.ToString().Trim(),
                                           "ImsYear="+item.IMS_YEAR.ToString().Trim(),
                                           "ImsRoadId="+item.IMS_ROAD_ID.ToString().Trim(),
                                       }),
                                       URLEncrypt.EncryptParameters1(new string[]{
                                           "MPConstCode="+item.MAST_MP_CONST_CODE.ToString().Trim(),
                                           "ImsYear="+item.IMS_YEAR.ToString().Trim(),
                                           "ImsRoadId="+item.IMS_ROAD_ID.ToString().Trim(),
                                       }),
                                       URLEncrypt.EncryptParameters1(new string[]{
                                           "MPConstCode="+item.MAST_MP_CONST_CODE.ToString().Trim(),
                                           "ImsYear="+item.IMS_YEAR.ToString().Trim(),
                                           "ImsRoadId="+item.IMS_ROAD_ID.ToString().Trim(),
                                       }),
                                }
                    }).ToArray();
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    totalRecords = 0;
                    return null;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

            public bool AddMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message)
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    int? maxCode = 0;
                
                    IMS_MP_PROPOSAL_STATUS mpProposalModel= new IMS_MP_PROPOSAL_STATUS();

                    if (dbContext.IMS_MP_PROPOSAL_STATUS.Where(s => s.MAST_MP_CONST_CODE == mpProposalViewModel.MAST_MP_CONST_CODE && s.IMS_YEAR == mpProposalViewModel.IMS_YEAR).Any())
                    {   
                        maxCode = dbContext.IMS_MP_PROPOSAL_STATUS.Where(s => s.IMS_YEAR == mpProposalViewModel.IMS_YEAR && s.MAST_MP_CONST_CODE == mpProposalViewModel.MAST_MP_CONST_CODE).Max(s => s.IMS_ROAD_ID);
                        maxCode = maxCode + 1;
                    }
                    else {
                        maxCode = 1;
                    }
                    //data copy view model to actual model
                    mpProposalModel.IMS_ROAD_ID = (int)maxCode;
                    mpProposalModel.MAST_MP_CONST_CODE = mpProposalViewModel.MAST_MP_CONST_CODE;
                    mpProposalModel.IMS_YEAR = mpProposalViewModel.IMS_YEAR;
                    mpProposalModel.IMS_ROAD_DETAILS = mpProposalViewModel.IMS_ROAD_DETAILS;
                    mpProposalModel.IMS_INCLUDED_IN_CN = "N";
                    mpProposalModel.IMS_REASON_ID_1 = null;
                    mpProposalModel.PLAN_CN_ROAD_CODE = null;
                    mpProposalModel.IMS_INCLUDED_IN_PROPOSAL = "N";
                    mpProposalModel.IMS_REASON_ID_2 = null;
                    mpProposalModel.IMS_PR_ROAD_CODE = null;

                    dbContext.IMS_MP_PROPOSAL_STATUS.Add(mpProposalModel);
                    dbContext.SaveChanges();
                    message = "MP Proposed Road details saved successfully.";
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

            public bool EditMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message)
            {
                try
                {
                    Dictionary<string, string> decryptedParameters = null;
                    string[] encryptedParameters = null;
                    dbContext = new PMGSYEntities();
                    int mpConstituencyCode = 0;
                    int imsYear = 0;
                    int ImsRoadId = 0;

                    encryptedParameters = mpProposalViewModel.EncryptedCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstCode"].ToString());
                    imsYear = Convert.ToInt32(decryptedParameters["ImsYear"].ToString());
                    ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"].ToString());

                    IMS_MP_PROPOSAL_STATUS mpProposalStatusModel = dbContext.IMS_MP_PROPOSAL_STATUS.Where(m => m.MAST_MP_CONST_CODE == mpConstituencyCode && m.IMS_YEAR == imsYear && m.IMS_ROAD_ID == ImsRoadId).FirstOrDefault();

                    if (mpProposalStatusModel == null)
                    {
                        message = "An Error Occured While processing your request.";
                        return false;
                    }

                    //copy view model data into Actual Model
                    mpProposalStatusModel.IMS_ROAD_DETAILS = mpProposalViewModel.IMS_ROAD_DETAILS;

                    dbContext.Entry(mpProposalStatusModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    message = "MP Proposed Road details updated successfully.";
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }


            }

            public bool DeleteMPProposedRoadDetails(int imsYear, int mpConstituencyCode, int ImsRoadId,ref string message)
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    IMS_MP_PROPOSAL_STATUS mpProposalStatusModel= dbContext.IMS_MP_PROPOSAL_STATUS.Where(m => m.IMS_ROAD_ID==ImsRoadId && m.IMS_YEAR==imsYear && m.MAST_MP_CONST_CODE==mpConstituencyCode).FirstOrDefault();

                    if (mpProposalStatusModel == null)
                    {
                        message = "An Error Occurred While Your Processing Request.";
                        return false;
                    }

                    dbContext.IMS_MP_PROPOSAL_STATUS.Remove(mpProposalStatusModel);
                    dbContext.SaveChanges();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "MP Proposal details can not be deleted because other details for mp proposal are entered.";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occurred While Your Processing Request.";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

            public MPProposalViewModel GetMPProposedRoadDetails(int imsYear, int constCode,int roadId)
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    IMS_MP_PROPOSAL_STATUS mpProposalStatusModel = dbContext.IMS_MP_PROPOSAL_STATUS.Where(m => m.IMS_YEAR== imsYear && m.MAST_MP_CONST_CODE==constCode && m.IMS_ROAD_ID==roadId).FirstOrDefault();

                    MPProposalViewModel mpProposalViewModel = new MPProposalViewModel();

                    if (mpProposalStatusModel != null)
                    {
                        mpProposalViewModel.MAST_MP_CONST_CODE = mpProposalStatusModel.MAST_MP_CONST_CODE;
                        mpProposalViewModel.IMS_YEAR = mpProposalStatusModel.IMS_YEAR;
                        mpProposalViewModel.IMS_ROAD_DETAILS = mpProposalStatusModel.IMS_ROAD_DETAILS.Trim();
                        mpProposalViewModel.EncryptedCode=URLEncrypt.EncryptParameters1(new string[]{
                                           "MPConstCode="+mpProposalStatusModel.MAST_MP_CONST_CODE.ToString().Trim(),
                                           "ImsYear="+mpProposalStatusModel.IMS_YEAR.ToString().Trim(),
                                           "ImsRoadId="+mpProposalStatusModel.IMS_ROAD_ID.ToString().Trim(),
                                       });

                        mpProposalViewModel.display_Year = mpProposalStatusModel.IMS_YEAR+"-"+((mpProposalStatusModel.IMS_YEAR)+1);
                        mpProposalViewModel.display_Constituency = mpProposalStatusModel.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME;
                    }
                    return mpProposalViewModel;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally { dbContext.Dispose(); }
            }

        #endregion MP Proposal

        #region MP Proposal Inclusion

            public bool AddMPProposalRoadInclusionDetails(MPRoadProposalInclusionViewModel mpProposalInclusionViewModel, ref string message)
            {
                try
                {
                    Dictionary<string, string> decryptedParameters = null;
                    string[] encryptedParameters = null;
                    dbContext = new PMGSYEntities();
                    int mpConstituencyCode = 0;
                    int imsYear = 0;
                    int ImsRoadId = 0;

                    encryptedParameters = mpProposalInclusionViewModel.EncryptedCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    mpConstituencyCode = Convert.ToInt32(decryptedParameters["MPConstCode"].ToString());
                    imsYear = Convert.ToInt32(decryptedParameters["ImsYear"].ToString());
                    ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"].ToString());

                    IMS_MP_PROPOSAL_STATUS mpProposalStatusModel = dbContext.IMS_MP_PROPOSAL_STATUS.Find(mpConstituencyCode, imsYear, ImsRoadId);

                    if (mpProposalStatusModel == null)
                    {
                        message = "An Error Occured While processing your request.";
                        return false;
                    }

                    //data copy view model to actual model start

                    mpProposalStatusModel.IMS_INCLUDED_IN_CN = mpProposalInclusionViewModel.IMS_INCLUDED_IN_CN;
                    mpProposalStatusModel.IMS_REASON_ID_1 = mpProposalInclusionViewModel.IMS_REASON_ID_1 == 0 ? null : mpProposalInclusionViewModel.IMS_REASON_ID_1;
                    mpProposalStatusModel.PLAN_CN_ROAD_CODE = mpProposalInclusionViewModel.PLAN_CN_ROAD_CODE == 0 ? null : mpProposalInclusionViewModel.PLAN_CN_ROAD_CODE;
                    mpProposalStatusModel.IMS_INCLUDED_IN_PROPOSAL = mpProposalInclusionViewModel.IMS_INCLUDED_IN_PROPOSAL;
                    mpProposalStatusModel.IMS_REASON_ID_2 = mpProposalInclusionViewModel.IMS_REASON_ID_2 == 0 ? null : mpProposalInclusionViewModel.IMS_REASON_ID_2;
                    mpProposalStatusModel.IMS_PR_ROAD_CODE = mpProposalInclusionViewModel.IMS_PR_ROAD_CODE == 0 ? null : mpProposalInclusionViewModel.IMS_PR_ROAD_CODE;

                    //data copy view model to actual model end

                    dbContext.Entry(mpProposalStatusModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "MP Proposed Road Inclusion details saved successfully.";
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

            public MPRoadProposalInclusionViewModel GetMPProposalRoadInclusionDetails(int imsYear, int constCode, int roadId)
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    IMS_MP_PROPOSAL_STATUS mpProposalStatusModel = dbContext.IMS_MP_PROPOSAL_STATUS.Where(m => m.IMS_YEAR == imsYear && m.MAST_MP_CONST_CODE == constCode && m.IMS_ROAD_ID == roadId).FirstOrDefault();

                    MPRoadProposalInclusionViewModel mpProposalInclusionViewModel = new MPRoadProposalInclusionViewModel();

                    if (mpProposalStatusModel != null)
                    {
                        mpProposalInclusionViewModel.display_Year = mpProposalStatusModel.IMS_YEAR + "-" + ((mpProposalStatusModel.IMS_YEAR) + 1);
                        mpProposalInclusionViewModel.display_Constituency = mpProposalStatusModel.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME;
                        mpProposalInclusionViewModel.IMS_ROAD_DETAILS = mpProposalStatusModel.IMS_ROAD_DETAILS;

                        mpProposalInclusionViewModel.IMS_INCLUDED_IN_CN = mpProposalStatusModel.IMS_INCLUDED_IN_CN;
                        mpProposalInclusionViewModel.IMS_REASON_ID_1 = mpProposalStatusModel.IMS_REASON_ID_1;
                        mpProposalInclusionViewModel.PLAN_CN_ROAD_CODE = mpProposalStatusModel.PLAN_CN_ROAD_CODE;
                        mpProposalInclusionViewModel.IMS_INCLUDED_IN_PROPOSAL = mpProposalStatusModel.IMS_INCLUDED_IN_PROPOSAL;
                        mpProposalInclusionViewModel.IMS_REASON_ID_2 = mpProposalStatusModel.IMS_REASON_ID_2;
                        mpProposalInclusionViewModel.IMS_PR_ROAD_CODE = mpProposalStatusModel.IMS_PR_ROAD_CODE;

                        mpProposalInclusionViewModel.EncryptedCode = URLEncrypt.EncryptParameters1(new string[]{
                                           "MPConstCode="+mpProposalStatusModel.MAST_MP_CONST_CODE.ToString().Trim(),
                                           "ImsYear="+mpProposalStatusModel.IMS_YEAR.ToString().Trim(),
                                           "ImsRoadId="+mpProposalStatusModel.IMS_ROAD_ID.ToString().Trim(),
                                       });

                        //set drrp Block drop down id
                        if (mpProposalStatusModel.PLAN_CN_ROAD_CODE != null)
                        {
                            mpProposalInclusionViewModel.DRRP_BLOCK = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == mpProposalStatusModel.PLAN_CN_ROAD_CODE).Select(s => s.MAST_BLOCK_CODE).FirstOrDefault();
                        }

                        //set Ims Block,Year drop down id
                        if (mpProposalStatusModel.IMS_PR_ROAD_CODE != null)
                        {
                            IMS_SANCTIONED_PROJECTS imsSanctionedProjectsModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == mpProposalStatusModel.IMS_PR_ROAD_CODE).FirstOrDefault();
                            mpProposalInclusionViewModel.IMS_BLOCK = imsSanctionedProjectsModel.MAST_BLOCK_CODE;
                            mpProposalInclusionViewModel.inclusion_year = imsSanctionedProjectsModel.IMS_YEAR;
                        }
                    }
                    return mpProposalInclusionViewModel;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally { dbContext.Dispose(); }
            }

        #endregion MP Proposal Inclusion

        #region MLA Proposal

        //populate MLA Constituency
        public List<SelectListItem> PopulateMLAConstatuency()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstConstatuency = null;

                lstConstatuency = new SelectList(dbContext.MASTER_MLA_CONSTITUENCY.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_MLA_CONST_CODE", "MAST_MLA_CONST_NAME").ToList();

                lstConstatuency.Insert(0, new SelectListItem() { Text = "select Constituency", Value = "0" });

                return lstConstatuency;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array ListMLAProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var lstMLAProposedRoads = (from MlaProposedRoads in dbContext.IMS_MLA_PROPOSAL_STATUS
                                          where ((yearCode == -1 ? 1 : MlaProposedRoads.IMS_YEAR) == (yearCode == -1 ? 1 : yearCode)) &&
                                               ((constituencyCode == 0 ? 1 : MlaProposedRoads.MAST_MLA_CONST_CODE) == (constituencyCode == 0 ? 1 : constituencyCode))
                                          select MlaProposedRoads
                                               ).ToList();

                totalRecords = lstMLAProposedRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "IMS_ROAD_DETAILS":
                                lstMLAProposedRoads = lstMLAProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMLAProposedRoads = lstMLAProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "IMS_ROAD_DETAILS":
                                lstMLAProposedRoads = lstMLAProposedRoads.OrderByDescending(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstMLAProposedRoads = lstMLAProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstMLAProposedRoads = lstMLAProposedRoads.OrderBy(x => x.IMS_ROAD_DETAILS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstMLAProposedRoads.Select(item => new
                {
                    cell = new[]{  
                                  item.IMS_ROAD_DETAILS == null?"-": item.IMS_ROAD_DETAILS.Trim(),
                                  item.IMS_YEAR == null?"-":item.IMS_YEAR.ToString(),
                                  item.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME == null?"-":item.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME.ToString(),
                                  //item.IMS_INCLUDED_IN_CN == "Y"?item.PLAN_ROAD.PLAN_RD_NAME:(item.MASTER_REASON.MAST_REASON_NAME ==null?"":item.MASTER_REASON.MAST_REASON_NAME.ToString()),
                                  item.IMS_INCLUDED_IN_CN == "Y"?"Yes - Core Network :"+(item.PLAN_CN_ROAD_CODE == null?"NA":item.PLAN_ROAD.PLAN_RD_NAME):(item.IMS_INCLUDED_IN_CN == "N"?"NA":"No - Reason :"+(item.IMS_REASON_ID_1 ==null?"":item.MASTER_REASON.MAST_REASON_NAME.ToString())),
                                  //item.IMS_INCLUDED_IN_PROPOSAL == "Y"?item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME:(item.MASTER_REASON1.MAST_REASON_NAME == null?"": item.MASTER_REASON1.MAST_REASON_NAME.ToString()),
                                  item.IMS_INCLUDED_IN_PROPOSAL == "Y"?"Yes - Proposal :"+(item.IMS_PR_ROAD_CODE == null?"NA":item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME):(item.IMS_INCLUDED_IN_PROPOSAL == "N"?"NA":"No - Reason :"+(item.IMS_REASON_ID_2 == null?"": item.MASTER_REASON1.MAST_REASON_NAME.ToString())),
                                   URLEncrypt.EncryptParameters1(new string[]{
                                       "MLAConstCode="+item.MAST_MLA_CONST_CODE.ToString().Trim(),
                                       "ImsYear="+item.IMS_YEAR.ToString().Trim(),
                                       "ImsRoadId="+item.IMS_ROAD_ID.ToString().Trim(),
                                   }),
                                   URLEncrypt.EncryptParameters1(new string[]{
                                       "MLAConstCode="+item.MAST_MLA_CONST_CODE.ToString().Trim(),
                                       "ImsYear="+item.IMS_YEAR.ToString().Trim(),
                                       "ImsRoadId="+item.IMS_ROAD_ID.ToString().Trim(),
                                   }),
                                   URLEncrypt.EncryptParameters1(new string[]{
                                       "MLAConstCode="+item.MAST_MLA_CONST_CODE.ToString().Trim(),
                                       "ImsYear="+item.IMS_YEAR.ToString().Trim(),
                                       "ImsRoadId="+item.IMS_ROAD_ID.ToString().Trim(),
                                   }),
                            }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool AddMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int? maxCode = 0;

                IMS_MLA_PROPOSAL_STATUS mlaProposalModel = new IMS_MLA_PROPOSAL_STATUS();

                if (dbContext.IMS_MLA_PROPOSAL_STATUS.Where(s => s.MAST_MLA_CONST_CODE == mlaProposalViewModel.MAST_MLA_CONST_CODE && s.IMS_YEAR == mlaProposalViewModel.IMS_YEAR).Any())
                {
                    maxCode = dbContext.IMS_MLA_PROPOSAL_STATUS.Where(s => s.IMS_YEAR == mlaProposalViewModel.IMS_YEAR && s.MAST_MLA_CONST_CODE == mlaProposalViewModel.MAST_MLA_CONST_CODE).Max(s => s.IMS_ROAD_ID);
                    maxCode = maxCode + 1;
                }
                else
                {
                    maxCode = 1;
                }
                //data copy view model to actual model
                mlaProposalModel.IMS_ROAD_ID = (int)maxCode;
                mlaProposalModel.MAST_MLA_CONST_CODE = mlaProposalViewModel.MAST_MLA_CONST_CODE;
                mlaProposalModel.IMS_YEAR = mlaProposalViewModel.IMS_YEAR;
                mlaProposalModel.IMS_ROAD_DETAILS = mlaProposalViewModel.IMS_ROAD_DETAILS;
                mlaProposalModel.IMS_INCLUDED_IN_CN = "N";
                mlaProposalModel.IMS_REASON_ID_1 = null;
                mlaProposalModel.PLAN_CN_ROAD_CODE = null;
                mlaProposalModel.IMS_INCLUDED_IN_PROPOSAL = "N";
                mlaProposalModel.IMS_REASON_ID_2 = null;
                mlaProposalModel.IMS_PR_ROAD_CODE = null;

                dbContext.IMS_MLA_PROPOSAL_STATUS.Add(mlaProposalModel);
                dbContext.SaveChanges();
                message = "MLA Proposed Road details saved successfully.";
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occured While proccessing your request";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool EditMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParameters = null;
                dbContext = new PMGSYEntities();
                int mlaConstituencyCode = 0;
                int imsYear = 0;
                int ImsRoadId = 0;

                encryptedParameters = mlaProposalViewModel.EncryptedCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstCode"].ToString());
                imsYear = Convert.ToInt32(decryptedParameters["ImsYear"].ToString());
                ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"].ToString());

                IMS_MLA_PROPOSAL_STATUS mlaProposalStatusModel = dbContext.IMS_MLA_PROPOSAL_STATUS.Where(m => m.MAST_MLA_CONST_CODE == mlaConstituencyCode && m.IMS_YEAR == imsYear && m.IMS_ROAD_ID == ImsRoadId).FirstOrDefault();

                if (mlaProposalStatusModel == null)
                {
                    message = "An Error Occured While processing your request.";
                    return false;
                }

                //copy view model data into Actual Model
                mlaProposalStatusModel.IMS_ROAD_DETAILS = mlaProposalViewModel.IMS_ROAD_DETAILS;

                dbContext.Entry(mlaProposalStatusModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                message = "MLA Proposed Road details updated successfully.";
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (UpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occured While proccessing your request";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occured While proccessing your request";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }


        }

        public bool DeleteMLAProposedRoadDetails(int imsYear, int mlaConstituencyCode, int ImsRoadId, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();

                IMS_MLA_PROPOSAL_STATUS mlaProposalStatusModel = dbContext.IMS_MLA_PROPOSAL_STATUS.Where(m => m.IMS_ROAD_ID == ImsRoadId && m.IMS_YEAR == imsYear && m.MAST_MLA_CONST_CODE == mlaConstituencyCode).FirstOrDefault();

                if (mlaProposalStatusModel == null)
                {
                    message = "An Error Occurred While Your Processing Request.";
                    return false;
                }

                dbContext.IMS_MLA_PROPOSAL_STATUS.Remove(mlaProposalStatusModel);
                dbContext.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "MLA Proposal details can not be deleted because other details for mp proposal are entered.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public MLAProposalViewModel GetMLAProposedRoadDetails(int imsYear, int constCode, int roadId)
        {
            try
            {
                dbContext = new PMGSYEntities();

                IMS_MLA_PROPOSAL_STATUS mlaProposalStatusModel = dbContext.IMS_MLA_PROPOSAL_STATUS.Where(m => m.IMS_YEAR == imsYear && m.MAST_MLA_CONST_CODE == constCode && m.IMS_ROAD_ID == roadId).FirstOrDefault();

                MLAProposalViewModel mlaProposalViewModel = new MLAProposalViewModel();

                if (mlaProposalViewModel != null)
                {
                    mlaProposalViewModel.MAST_MLA_CONST_CODE = mlaProposalStatusModel.MAST_MLA_CONST_CODE;
                    mlaProposalViewModel.IMS_YEAR = mlaProposalStatusModel.IMS_YEAR;
                    mlaProposalViewModel.IMS_ROAD_DETAILS = mlaProposalStatusModel.IMS_ROAD_DETAILS.Trim();
                    mlaProposalViewModel.EncryptedCode = URLEncrypt.EncryptParameters1(new string[]{
                                       "MLAConstCode="+mlaProposalStatusModel.MAST_MLA_CONST_CODE.ToString().Trim(),
                                       "ImsYear="+mlaProposalStatusModel.IMS_YEAR.ToString().Trim(),
                                       "ImsRoadId="+mlaProposalStatusModel.IMS_ROAD_ID.ToString().Trim(),
                                   });

                    mlaProposalViewModel.display_Year = mlaProposalStatusModel.IMS_YEAR + "-" + ((mlaProposalStatusModel.IMS_YEAR) + 1);
                    mlaProposalViewModel.display_Constituency = mlaProposalStatusModel.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME;
                }
                return mlaProposalViewModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally { dbContext.Dispose(); }
        }

        #endregion MP Proposal

        #region MLA Proposal Inclusion

             public bool AddMLAProposalRoadInclusionDetails(MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel, ref string message)
            {
                try
                {
                    Dictionary<string, string> decryptedParameters = null;
                    string[] encryptedParameters = null;
                    dbContext = new PMGSYEntities();
                    int mlaConstituencyCode = 0;
                    int imsYear = 0;
                    int ImsRoadId = 0;

                    encryptedParameters = mlaProposalInclusionViewModel.EncryptedCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    mlaConstituencyCode = Convert.ToInt32(decryptedParameters["MLAConstCode"].ToString());
                    imsYear = Convert.ToInt32(decryptedParameters["ImsYear"].ToString());
                    ImsRoadId = Convert.ToInt32(decryptedParameters["ImsRoadId"].ToString());

                    IMS_MLA_PROPOSAL_STATUS mlaProposalStatusModel = dbContext.IMS_MLA_PROPOSAL_STATUS.Find(mlaConstituencyCode, imsYear, ImsRoadId);

                    if (mlaProposalStatusModel == null)
                    {
                        message = "An Error Occured While processing your request.";
                        return false;
                    }

                    //data copy view model to actual model start

                    mlaProposalStatusModel.IMS_INCLUDED_IN_CN = mlaProposalInclusionViewModel.IMS_INCLUDED_IN_CN;
                    mlaProposalStatusModel.IMS_REASON_ID_1 = mlaProposalInclusionViewModel.IMS_REASON_ID_1 == 0 ? null : mlaProposalInclusionViewModel.IMS_REASON_ID_1;
                    mlaProposalStatusModel.PLAN_CN_ROAD_CODE = mlaProposalInclusionViewModel.PLAN_CN_ROAD_CODE == 0 ? null : mlaProposalInclusionViewModel.PLAN_CN_ROAD_CODE;
                    mlaProposalStatusModel.IMS_INCLUDED_IN_PROPOSAL = mlaProposalInclusionViewModel.IMS_INCLUDED_IN_PROPOSAL;
                    mlaProposalStatusModel.IMS_REASON_ID_2 = mlaProposalInclusionViewModel.IMS_REASON_ID_2 == 0 ? null : mlaProposalInclusionViewModel.IMS_REASON_ID_2;
                    mlaProposalStatusModel.IMS_PR_ROAD_CODE = mlaProposalInclusionViewModel.IMS_PR_ROAD_CODE == 0 ? null : mlaProposalInclusionViewModel.IMS_PR_ROAD_CODE;

                    //data copy view model to actual model end

                    dbContext.Entry(mlaProposalStatusModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "MLA Proposed Road Inclusion details saved successfully.";
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

             public MLARoadProposalInclusionViewModel GetMLAProposalRoadInclusionDetails(int imsYear, int constCode, int roadId)
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    IMS_MLA_PROPOSAL_STATUS mlaProposalStatusModel = dbContext.IMS_MLA_PROPOSAL_STATUS.Where(m => m.IMS_YEAR == imsYear && m.MAST_MLA_CONST_CODE == constCode && m.IMS_ROAD_ID == roadId).FirstOrDefault();

                    MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel = new MLARoadProposalInclusionViewModel();

                    if (mlaProposalStatusModel != null)
                    {
                        mlaProposalInclusionViewModel.display_Year = mlaProposalStatusModel.IMS_YEAR + "-" + ((mlaProposalStatusModel.IMS_YEAR) + 1);
                        mlaProposalInclusionViewModel.display_Constituency = mlaProposalStatusModel.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME;
                        mlaProposalInclusionViewModel.IMS_ROAD_DETAILS = mlaProposalStatusModel.IMS_ROAD_DETAILS;

                        mlaProposalInclusionViewModel.IMS_INCLUDED_IN_CN = mlaProposalStatusModel.IMS_INCLUDED_IN_CN;
                        mlaProposalInclusionViewModel.IMS_REASON_ID_1 = mlaProposalStatusModel.IMS_REASON_ID_1;
                        mlaProposalInclusionViewModel.PLAN_CN_ROAD_CODE = mlaProposalStatusModel.PLAN_CN_ROAD_CODE;
                        mlaProposalInclusionViewModel.IMS_INCLUDED_IN_PROPOSAL = mlaProposalStatusModel.IMS_INCLUDED_IN_PROPOSAL;
                        mlaProposalInclusionViewModel.IMS_REASON_ID_2 = mlaProposalStatusModel.IMS_REASON_ID_2;
                        mlaProposalInclusionViewModel.IMS_PR_ROAD_CODE = mlaProposalStatusModel.IMS_PR_ROAD_CODE;

                        mlaProposalInclusionViewModel.EncryptedCode = URLEncrypt.EncryptParameters1(new string[]{
                                           "MLAConstCode="+mlaProposalStatusModel.MAST_MLA_CONST_CODE.ToString().Trim(),
                                           "ImsYear="+mlaProposalStatusModel.IMS_YEAR.ToString().Trim(),
                                           "ImsRoadId="+mlaProposalStatusModel.IMS_ROAD_ID.ToString().Trim(),
                                       });

                        //set drrp Block drop down id
                        if (mlaProposalStatusModel.PLAN_CN_ROAD_CODE != null)
                        {
                            mlaProposalInclusionViewModel.DRRP_BLOCK = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == mlaProposalStatusModel.PLAN_CN_ROAD_CODE).Select(s => s.MAST_BLOCK_CODE).FirstOrDefault();
                        }

                        //set Ims Block,Year drop down id
                        if (mlaProposalStatusModel.IMS_PR_ROAD_CODE != null)
                        {
                            IMS_SANCTIONED_PROJECTS imsSanctionedProjectsModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == mlaProposalStatusModel.IMS_PR_ROAD_CODE).FirstOrDefault();
                            mlaProposalInclusionViewModel.IMS_BLOCK = imsSanctionedProjectsModel.MAST_BLOCK_CODE;
                            mlaProposalInclusionViewModel.inclusion_year = imsSanctionedProjectsModel.IMS_YEAR;
                        }
                    }
                    return mlaProposalInclusionViewModel;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally { dbContext.Dispose(); }
            }

        #endregion MLA Proposal Inclusion

        #region Pupulate Data
             /// <summary>
             /// populate the year 
             /// </summary>
             /// <returns></returns>
             public List<SelectListItem> PopulateYear(int SelectedYear = 0, bool populateFirstItem = true, bool isAllYearsSelected = false)
             {
                 List<SelectListItem> lstYears = new List<SelectListItem>();
                 SelectListItem item = new SelectListItem();
                 if (populateFirstItem && isAllYearsSelected == false)
                 {
                     item.Text = "Select Year";
                     item.Value = "0";
                     item.Selected = true;
                     lstYears.Add(item);
                 }
                 if (populateFirstItem && isAllYearsSelected)
                 {
                     item.Text = "All Years";
                     item.Value = "-1";
                     item.Selected = true;
                     lstYears.Add(item);
                 }
                 for (int i = 2000; i < DateTime.Now.Year + 1; i++)
                 {
                     item = new SelectListItem();
                     item.Text = i + " - " + (i + 1);
                     item.Value = i.ToString();
                     lstYears.Add(item);
                 }
                 return lstYears;
             }

             public List<SelectListItem> PopulateReasons()
             {
                 try
                 {
                     dbContext = new PMGSYEntities();
                     List<SelectListItem> lstReasons = null;
                     lstReasons = new SelectList(dbContext.MASTER_REASON.Where(m => m.MAST_REASON_TYPE == "I"), "MAST_REASON_CODE", "MAST_REASON_NAME").ToList();
                     lstReasons.Insert(0, new SelectListItem() { Text = "select Reason", Value = "0" });
                     return lstReasons;
                 }
                 catch (Exception ex)
                 {
                     Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                     throw ex;
                 }
                 finally
                 {
                     if (dbContext != null)
                     {
                         dbContext.Dispose();
                     }
                 }
             }

             public List<SelectListItem> PopulateImsSanctionedRoads(int block_code, int year_code)
             {
                 try
                 {
                     if (block_code != 0 && year_code != 0)
                     {
                         dbContext = new PMGSYEntities();
                         List<SelectListItem> lstImsRoads = null;
                         lstImsRoads = new SelectList(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.MAST_BLOCK_CODE == block_code && m.IMS_YEAR == year_code), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
                         lstImsRoads.Insert(0, new SelectListItem() { Text = "select Road Name", Value = "0" });
                         return lstImsRoads;
                     }
                     else
                     {
                         List<SelectListItem> lstImsRoads = new List<SelectListItem>();
                         lstImsRoads.Insert(0, new SelectListItem() { Text = "select Road Name", Value = "0" });
                         return lstImsRoads;
                     }
                 }
                 catch (Exception ex)
                 {
                     Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                     throw ex;
                 }
                 finally
                 {
                     if (dbContext != null)
                     {
                         dbContext.Dispose();
                     }
                 }
             }

             public List<SelectListItem> PopulateDrrpRoads(int block_code)
             {
                 try
                 {
                     if (block_code == 0)
                     {
                         List<SelectListItem> lstDrrpRoads = new List<SelectListItem>();
                         lstDrrpRoads.Insert(0, new SelectListItem() { Text = "select Road Name", Value = "0" });
                         return lstDrrpRoads;
                     }
                     else
                     {
                         dbContext = new PMGSYEntities();
                         List<SelectListItem> lstDrrpRoads = null;
                         lstDrrpRoads = new SelectList(dbContext.PLAN_ROAD.Where(m => m.MAST_BLOCK_CODE == block_code), "PLAN_CN_ROAD_CODE", "PLAN_RD_NAME").ToList();
                         lstDrrpRoads.Insert(0, new SelectListItem() { Text = "select Road Name", Value = "0" });
                         return lstDrrpRoads;
                     }
                 }
                 catch (Exception ex)
                 {
                     Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                     throw ex;
                 }
                 finally
                 {
                     if (dbContext != null)
                     {
                         dbContext.Dispose();
                     }
                 }
             }
        #endregion


    }




    public interface IMPMLAProposalDAL
    {

        #region MP Proposal

            List<SelectListItem> PopulateYear(int SelectedYear = 0, bool populateFirstItem = true, bool isAllYearsSelected = false);
            List<SelectListItem> PopulateMPConstatuency();
            Array ListMPProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
            bool AddMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message);
            bool EditMPProposedRoadDetails(MPProposalViewModel mpProposalViewModel, ref string message);
            bool DeleteMPProposedRoadDetails(int imsYear, int mpConstituencyCode, int ImsRoadId, ref string message);
            MPProposalViewModel GetMPProposedRoadDetails(int imsYear, int constCode, int roadId);

            List<SelectListItem> PopulateReasons();
            List<SelectListItem> PopulateDrrpRoads(int block_code);
            List<SelectListItem> PopulateImsSanctionedRoads(int block_code, int year_code);

        #endregion MP Proposal

        #region MP Proposed Road Inclusion Details
            bool AddMPProposalRoadInclusionDetails(MPRoadProposalInclusionViewModel mpProposalInclusionViewModel, ref string message);
            MPRoadProposalInclusionViewModel GetMPProposalRoadInclusionDetails(int imsYear, int constCode, int roadId);
        #endregion MP Proposed Road Inclusion Details

        #region MLA Proposal

            List<SelectListItem> PopulateMLAConstatuency();
            Array ListMLAProposedRoadList(int yearCode, int constituencyCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
            bool AddMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message);
            bool EditMLAProposedRoadDetails(MLAProposalViewModel mlaProposalViewModel, ref string message);
            bool DeleteMLAProposedRoadDetails(int imsYear, int mlaConstituencyCode, int ImsRoadId, ref string message);
            MLAProposalViewModel GetMLAProposedRoadDetails(int imsYear, int constCode, int roadId);

        #endregion MLA Proposal

        #region MLA Proposal Inclusion

            bool AddMLAProposalRoadInclusionDetails(MLARoadProposalInclusionViewModel mlaProposalInclusionViewModel, ref string message);
            MLARoadProposalInclusionViewModel GetMLAProposalRoadInclusionDetails(int imsYear, int constCode, int roadId);

        #endregion MLA Proposal Inclusion
    }
}