using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Proposal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.Proposal
{
    public class MRDProposalDAL : IProposalDAL1
    {
        Models.PMGSYEntities dbContext;

        public Array ListMrdClearanceDAL(int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int maxCleranceRevsionCode = 0;

                var lstOriginalList = from filelist in dbContext.MRD_CLEARANCE_LETTERS
                                      join state in dbContext.MASTER_STATE on filelist.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                      join agency in dbContext.MASTER_AGENCY on filelist.ADMIN_ND_CODE equals agency.MAST_AGENCY_CODE
                                      join batchs in dbContext.MASTER_BATCH on filelist.MAST_BATCH equals batchs.MAST_BATCH_CODE
                                      join fund in dbContext.MASTER_FUNDING_AGENCY on filelist.MAST_COLLABORATION equals fund.MAST_FUNDING_AGENCY_CODE
                                      //join drop in dbContext.MRD_DROPPED_LETTERS on filelist.MRD_CLEARANCE_CODE equals drop.MRD_CLEARANCE_CODE
                                      //into a from s in a.DefaultIfEmpty()
                                      where
                                      (stateCode <= 0 ? 1 : filelist.MAST_STATE_CODE) == (stateCode <= 0 ? 1 : stateCode) &&
                                      (agencyCode <= 0 ? 1 : filelist.ADMIN_ND_CODE) == (agencyCode <= 0 ? 1 : agencyCode) &&
                                      (year <= 0 ? 1 : filelist.MAST_YEAR) == (year <= 0 ? 1 : year) &&
                                      (batch <= 0 ? 1 : filelist.MAST_BATCH) == (batch <= 0 ? 1 : batch) &&
                                      (collaboration <= 0 ? 1 : filelist.MAST_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                                      filelist.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
                                      filelist.MRD_CLEARANCE_STATUS == "O" &&
                                      !(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_STATUS == "R").Select(m => m.MRD_ORG_CLEARANCE_CODE).Contains(filelist.MRD_CLEARANCE_CODE))
                                      select new
                                      {
                                          state.MAST_STATE_NAME,
                                          agency.MAST_AGENCY_NAME,
                                          agency.MAST_AGENCY_TYPE,
                                          batchs.MAST_BATCH_NAME,
                                          filelist.MAST_YEAR,
                                          fund.MAST_FUNDING_AGENCY_NAME,
                                          filelist.MRD_CLEARANCE_CODE,
                                          filelist.MRD_CLEARANCE_DATE,
                                          filelist.MRD_CLEARANCE_NUMBER,
                                          filelist.MRD_TOTAL_ROADS,
                                          filelist.MRD_TOTAL_LSB,
                                          filelist.MRD_ROAD_MORD_SHARE_AMT,
                                          filelist.MRD_ROAD_STATE_SHARE_AMT,
                                          filelist.MRD_ROAD_TOTAL_AMT,
                                          filelist.MRD_LSB_MORD_SHARE_AMT,
                                          filelist.MRD_LSB_STATE_SHARE_AMT,
                                          filelist.MRD_LSB_TOTAL_AMT,
                                          filelist.MRD_TOTAL_MORD_SHARE_AMT,
                                          filelist.MRD_TOTAL_STATE_SHARE_AMT,
                                          filelist.MRD_TOTAL_SANCTIONED_AMT,
                                          filelist.MRD_TOTAL_ROAD_LENGTH,
                                          filelist.MRD_TOTAL_LSB_LENGTH,
                                          filelist.MRD_HAB_1000,
                                          filelist.MRD_HAB_500,
                                          filelist.MRD_HAB_250_ELIGIBLE,
                                          filelist.MRD_HAB_100_ELIGIBLE,
                                          filelist.MRD_CLEARANCE_PDF_FILE,
                                          filelist.MRD_ORG_CLEARANCE_CODE,
                                          filelist.MRD_CLEARANCE_STATUS,
                                          filelist.MRD_ROAD_PDF_FILE,
                                          filelist.MRD_ROAD_EXCEL_FILE,
                                      };

                var lstRevisionList = from filelist in dbContext.MRD_CLEARANCE_LETTERS
                                      join state in dbContext.MASTER_STATE on filelist.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                      join agency in dbContext.MASTER_AGENCY on filelist.ADMIN_ND_CODE equals agency.MAST_AGENCY_CODE
                                      join batchs in dbContext.MASTER_BATCH on filelist.MAST_BATCH equals batchs.MAST_BATCH_CODE
                                      join fund in dbContext.MASTER_FUNDING_AGENCY on filelist.MAST_COLLABORATION equals fund.MAST_FUNDING_AGENCY_CODE
                                      //join drop in dbContext.MRD_DROPPED_LETTERS on filelist.MRD_CLEARANCE_CODE equals drop.MRD_CLEARANCE_CODE
                                      //into a from s in a.DefaultIfEmpty()
                                      where
                                      (stateCode <= 0 ? 1 : filelist.MAST_STATE_CODE) == (stateCode <= 0 ? 1 : stateCode) &&
                                      (agencyCode <= 0 ? 1 : filelist.ADMIN_ND_CODE) == (agencyCode <= 0 ? 1 : agencyCode) &&
                                      (year <= 0 ? 1 : filelist.MAST_YEAR) == (year <= 0 ? 1 : year) &&
                                      (batch <= 0 ? 1 : filelist.MAST_BATCH) == (batch <= 0 ? 1 : batch) &&
                                      (collaboration <= 0 ? 1 : filelist.MAST_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                                      filelist.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
                                          //filelist.MRD_CLEARANCE_STATUS == "R" &&
                                      (from item in dbContext.MRD_CLEARANCE_LETTERS where item.MRD_CLEARANCE_STATUS == "R" group item by item.MRD_ORG_CLEARANCE_CODE into revisionDetails let MRD_CLEARANCE_CODE = revisionDetails.Max(m => m.MRD_CLEARANCE_CODE) select MRD_CLEARANCE_CODE).Contains(filelist.MRD_CLEARANCE_CODE)
                                      select new
                                      {
                                          state.MAST_STATE_NAME,
                                          agency.MAST_AGENCY_NAME,
                                          agency.MAST_AGENCY_TYPE,
                                          batchs.MAST_BATCH_NAME,
                                          filelist.MAST_YEAR,
                                          fund.MAST_FUNDING_AGENCY_NAME,
                                          filelist.MRD_CLEARANCE_CODE,
                                          filelist.MRD_CLEARANCE_DATE,
                                          filelist.MRD_CLEARANCE_NUMBER,
                                          filelist.MRD_TOTAL_ROADS,
                                          filelist.MRD_TOTAL_LSB,
                                          filelist.MRD_ROAD_MORD_SHARE_AMT,
                                          filelist.MRD_ROAD_STATE_SHARE_AMT,
                                          filelist.MRD_ROAD_TOTAL_AMT,
                                          filelist.MRD_LSB_MORD_SHARE_AMT,
                                          filelist.MRD_LSB_STATE_SHARE_AMT,
                                          filelist.MRD_LSB_TOTAL_AMT,
                                          filelist.MRD_TOTAL_MORD_SHARE_AMT,
                                          filelist.MRD_TOTAL_STATE_SHARE_AMT,
                                          filelist.MRD_TOTAL_SANCTIONED_AMT,
                                          filelist.MRD_TOTAL_ROAD_LENGTH,
                                          filelist.MRD_TOTAL_LSB_LENGTH,
                                          filelist.MRD_HAB_1000,
                                          filelist.MRD_HAB_500,
                                          filelist.MRD_HAB_250_ELIGIBLE,
                                          filelist.MRD_HAB_100_ELIGIBLE,
                                          filelist.MRD_CLEARANCE_PDF_FILE,
                                          filelist.MRD_ORG_CLEARANCE_CODE,
                                          filelist.MRD_CLEARANCE_STATUS,
                                          filelist.MRD_ROAD_PDF_FILE,
                                          filelist.MRD_ROAD_EXCEL_FILE,
                                      };

                var list = lstOriginalList.Union(lstRevisionList);

                totalRecords = list.Count();

                if (totalRecords > 0)
                {
                    maxCleranceRevsionCode = list.Max(a => a.MRD_CLEARANCE_CODE);
                }

                var s = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == 4).Select(m => m.MRD_DROPPED_PDF_FILE).FirstOrDefault();
                var s1 = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == 4).Select(m => m.MRD_ROAD_PDF_FILE).FirstOrDefault();
                var s2 = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == 1).Select(m => m.MRD_ROAD_EXCEL_FILE).FirstOrDefault();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Year":
                                list = list.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Batch":
                                list = list.OrderBy(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Agency":
                                list = list.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Collaboration":
                                list = list.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                        //list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Year":
                                list = list.OrderByDescending(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Batch":
                                list = list.OrderByDescending(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Agency":
                                list = list.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Collaboration":
                                list = list.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                        //list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(claerancelist => new
                {
                    claerancelist.MAST_STATE_NAME,
                    claerancelist.MAST_AGENCY_NAME,
                    claerancelist.MAST_AGENCY_TYPE,
                    claerancelist.MAST_BATCH_NAME,
                    claerancelist.MAST_YEAR,
                    claerancelist.MAST_FUNDING_AGENCY_NAME,
                    claerancelist.MRD_CLEARANCE_DATE,
                    claerancelist.MRD_CLEARANCE_CODE,
                    claerancelist.MRD_CLEARANCE_NUMBER,
                    claerancelist.MRD_TOTAL_ROADS,
                    claerancelist.MRD_TOTAL_LSB,
                    claerancelist.MRD_ROAD_MORD_SHARE_AMT,
                    claerancelist.MRD_ROAD_STATE_SHARE_AMT,
                    claerancelist.MRD_ROAD_TOTAL_AMT,
                    claerancelist.MRD_LSB_MORD_SHARE_AMT,
                    claerancelist.MRD_LSB_STATE_SHARE_AMT,
                    claerancelist.MRD_LSB_TOTAL_AMT,
                    claerancelist.MRD_TOTAL_MORD_SHARE_AMT,
                    claerancelist.MRD_TOTAL_STATE_SHARE_AMT,
                    claerancelist.MRD_TOTAL_SANCTIONED_AMT,
                    claerancelist.MRD_TOTAL_ROAD_LENGTH,
                    claerancelist.MRD_TOTAL_LSB_LENGTH,
                    claerancelist.MRD_HAB_1000,
                    claerancelist.MRD_HAB_500,
                    claerancelist.MRD_HAB_250_ELIGIBLE,
                    claerancelist.MRD_HAB_100_ELIGIBLE,
                    claerancelist.MRD_CLEARANCE_PDF_FILE,
                    claerancelist.MRD_ORG_CLEARANCE_CODE,
                    claerancelist.MRD_CLEARANCE_STATUS,
                    claerancelist.MRD_ROAD_PDF_FILE,
                    claerancelist.MRD_ROAD_EXCEL_FILE,
                }).ToArray();


                return result.Select(clearanceDetails => new
                {
                    id = clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),//URLEncrypt.EncryptParameters1(new string[] { "AdminCode =" + adminDetails.ADMIN_ND_CODE.ToString().Trim() }),

                    cell = new[]{
                    
     /*1*/          "<center><a href='#' class='ui-icon ui-icon-plusthick'  title='Add Dropped Letter' onclick='loadMrdDroppedLetterDetails(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim() }) + "\"); return false;'>Drop</a></center>",

     /*2*/          clearanceDetails.MAST_STATE_NAME == null ? string.Empty : clearanceDetails.MAST_STATE_NAME.ToString(),
     /*3*/          clearanceDetails.MAST_AGENCY_NAME == null ? string.Empty : clearanceDetails.MAST_AGENCY_NAME.ToString(),
     /*4*/          clearanceDetails.MRD_CLEARANCE_DATE == null ? "" : ConvertDateToString(clearanceDetails.MRD_CLEARANCE_DATE),
     /*5*/          clearanceDetails.MRD_CLEARANCE_NUMBER == null ? "" : clearanceDetails.MRD_CLEARANCE_NUMBER,
     /*6*/          clearanceDetails.MAST_FUNDING_AGENCY_NAME == null?string.Empty:clearanceDetails.MAST_FUNDING_AGENCY_NAME.ToString(),
     /*7*/          (clearanceDetails.MAST_YEAR+"-"+ (clearanceDetails.MAST_YEAR+1)).ToString(),
     /*8*/          clearanceDetails.MAST_BATCH_NAME == null?string.Empty:clearanceDetails.MAST_BATCH_NAME.ToString(),

     /*9*/         Convert.ToString(clearanceDetails.MRD_ROAD_MORD_SHARE_AMT + clearanceDetails.MRD_LSB_MORD_SHARE_AMT),
     /*10*/         Convert.ToString(clearanceDetails.MRD_ROAD_STATE_SHARE_AMT + clearanceDetails.MRD_LSB_STATE_SHARE_AMT),
     /*11*/         Convert.ToString(clearanceDetails.MRD_ROAD_TOTAL_AMT + clearanceDetails.MRD_LSB_TOTAL_AMT),

//     /*16*/         clearanceDetails.MRD_TOTAL_MORD_SHARE_AMT.ToString(),
//     /*17*/         clearanceDetails.MRD_TOTAL_STATE_SHARE_AMT.ToString(),
//     /*18*/         clearanceDetails.MRD_TOTAL_SANCTIONED_AMT.ToString(), */

     /*12*/          clearanceDetails.MRD_TOTAL_ROADS.ToString(),
     /*13*/         clearanceDetails.MRD_TOTAL_ROAD_LENGTH.ToString(),
     /*14*/          clearanceDetails.MRD_TOTAL_LSB.ToString(),
     /*15*/         clearanceDetails.MRD_TOTAL_LSB_LENGTH.ToString(),

     /*16*/         clearanceDetails.MRD_HAB_1000.ToString(),
     /*17*/         clearanceDetails.MRD_HAB_500.ToString(),
     /*18*/         clearanceDetails.MRD_HAB_250_ELIGIBLE.ToString(),
     /*19*/         clearanceDetails.MRD_HAB_100_ELIGIBLE.ToString(),

                    //"<a href='#' title='Click here to Download File List' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownloadDroppedFile(\"" + URLEncrypt.EncryptParameters(new string[]{"ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"ClearanceRevisionCode="+clearanceDetails.MRD_ORG_CLEARANCE_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) +"\"); return false;'>Download</a>" ,
                    
                     
                    //clearanceDetails.MRD_CLEARANCE_CODE==maxCleranceRevsionCode?
                    //"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditClearanceRevisionDetail(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"ClearanceRevisionCode="+clearanceDetails.MRD_ORG_CLEARANCE_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) + "\"); return false;'>Edit</a></center>"
                    //:("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    // clearanceDetails.MRD_CLEARANCE_CODE==maxCleranceRevsionCode?
                    //"<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteClearancRevisioneDetail(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"ClearanceRevisionCode="+clearanceDetails.MRD_ORG_CLEARANCE_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) + "\"); return false;'>Delete</a></center>"        
                    //:("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    //"<center><a href='#' class='ui-icon ui-icon-zoomin' onclick='ViewClearanceRevisionDetail(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"ClearanceRevisionCode="+clearanceDetails.MRD_ORG_CLEARANCE_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) + "\"); return false;'>View</a></center>"        
                  
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array ListMrdDroppedDAL(int clrId, int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int maxCleranceRevsionCode = 0;

                var list = from filelist in dbContext.MRD_DROPPED_LETTERS
                           join state in dbContext.MASTER_STATE on filelist.MAST_STATE_CODE equals state.MAST_STATE_CODE
                           join agency in dbContext.MASTER_AGENCY on filelist.ADMIN_ND_CODE equals agency.MAST_AGENCY_CODE
                           join batchs in dbContext.MASTER_BATCH on filelist.MAST_BATCH equals batchs.MAST_BATCH_CODE
                           join fund in dbContext.MASTER_FUNDING_AGENCY on filelist.MAST_COLLABORATION equals fund.MAST_FUNDING_AGENCY_CODE
                           //join drop in dbContext.MRD_DROPPED_LETTERS on filelist.MRD_CLEARANCE_CODE equals drop.MRD_CLEARANCE_CODE
                           //into a from s in a.DefaultIfEmpty()
                           where
                           filelist.MRD_CLEARANCE_CODE == clrId
                           //(stateCode <= 0 ? 1 : filelist.MAST_STATE_CODE) == (stateCode <= 0 ? 1 : stateCode) &&
                           //(agencyCode <= 0 ? 1 : filelist.ADMIN_ND_CODE) == (agencyCode <= 0 ? 1 : agencyCode) &&
                           //(year <= 0 ? 1 : filelist.MAST_YEAR) == (year <= 0 ? 1 : year) &&
                           //(batch <= 0 ? 1 : filelist.MAST_BATCH) == (batch <= 0 ? 1 : batch) &&
                           //(collaboration <= 0 ? 1 : filelist.MAST_COLLABORATION) == (collaboration <= 0 ? 1 : collaboration) &&
                           //filelist.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme 

                           select new
                           {
                               filelist.MRD_DROPPED_CODE,
                               state.MAST_STATE_NAME,
                               agency.MAST_AGENCY_NAME,
                               agency.MAST_AGENCY_TYPE,
                               batchs.MAST_BATCH_NAME,
                               filelist.MAST_YEAR,
                               fund.MAST_FUNDING_AGENCY_NAME,
                               filelist.MRD_CLEARANCE_CODE,
                               filelist.MRD_DROPPED_DATE,
                               filelist.MRD_DROPPED_LETTER_NUMBER,
                               filelist.MRD_TOTAL_ROADS,
                               filelist.MRD_TOTAL_LSB,
                               filelist.MRD_ROAD_MORD_SHARE_AMT,
                               filelist.MRD_ROAD_STATE_SHARE_AMT,
                               filelist.MRD_ROAD_TOTAL_AMT,
                               filelist.MRD_LSB_MORD_SHARE_AMT,
                               filelist.MRD_LSB_STATE_SHARE_AMT,
                               filelist.MRD_LSB_TOTAL_AMT,
                               filelist.MRD_TOTAL_MORD_SHARE_AMT,
                               filelist.MRD_TOTAL_STATE_SHARE_AMT,
                               filelist.MRD_TOTAL_SANCTIONED_AMT,
                               filelist.MRD_TOTAL_ROAD_LENGTH,
                               filelist.MRD_TOTAL_LSB_LENGTH,
                               filelist.MRD_HAB_1000,
                               filelist.MRD_HAB_500,
                               filelist.MRD_HAB_250_ELIGIBLE,
                               filelist.MRD_HAB_100_ELIGIBLE,
                               filelist.MRD_DROPPED_PDF_FILE,
                               filelist.MRD_ROAD_PDF_FILE,
                               filelist.MRD_ROAD_EXCEL_FILE,
                           };

                //                var list = lstOriginalList.Union(lstRevisionList);

                totalRecords = list.Count();

                //if (totalRecords > 0)
                //{
                //    maxCleranceRevsionCode = list.Max(a => a.MRD_DROPPED_CODE);
                //}

                var s = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == 4).Select(m => m.MRD_DROPPED_PDF_FILE).FirstOrDefault();
                var s1 = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == 4).Select(m => m.MRD_ROAD_PDF_FILE).FirstOrDefault();
                var s2 = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == 1).Select(m => m.MRD_ROAD_EXCEL_FILE).FirstOrDefault();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Year":
                                list = list.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Batch":
                                list = list.OrderBy(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Agency":
                                list = list.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Collaboration":
                                list = list.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                        //list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Year":
                                list = list.OrderByDescending(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Batch":
                                list = list.OrderByDescending(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Agency":
                                list = list.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Collaboration":
                                list = list.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                        //list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    list = list.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = list.Select(claerancelist => new
                {
                    claerancelist.MRD_DROPPED_CODE,
                    claerancelist.MAST_STATE_NAME,
                    claerancelist.MAST_AGENCY_NAME,
                    claerancelist.MAST_AGENCY_TYPE,
                    claerancelist.MAST_BATCH_NAME,
                    claerancelist.MAST_YEAR,
                    claerancelist.MAST_FUNDING_AGENCY_NAME,
                    claerancelist.MRD_DROPPED_DATE,
                    claerancelist.MRD_CLEARANCE_CODE,
                    claerancelist.MRD_DROPPED_LETTER_NUMBER,
                    claerancelist.MRD_TOTAL_ROADS,
                    claerancelist.MRD_TOTAL_LSB,
                    claerancelist.MRD_ROAD_MORD_SHARE_AMT,
                    claerancelist.MRD_ROAD_STATE_SHARE_AMT,
                    claerancelist.MRD_ROAD_TOTAL_AMT,
                    claerancelist.MRD_LSB_MORD_SHARE_AMT,
                    claerancelist.MRD_LSB_STATE_SHARE_AMT,
                    claerancelist.MRD_LSB_TOTAL_AMT,
                    claerancelist.MRD_TOTAL_MORD_SHARE_AMT,
                    claerancelist.MRD_TOTAL_STATE_SHARE_AMT,
                    claerancelist.MRD_TOTAL_SANCTIONED_AMT,
                    claerancelist.MRD_TOTAL_ROAD_LENGTH,
                    claerancelist.MRD_TOTAL_LSB_LENGTH,
                    claerancelist.MRD_HAB_1000,
                    claerancelist.MRD_HAB_500,
                    claerancelist.MRD_HAB_250_ELIGIBLE,
                    claerancelist.MRD_HAB_100_ELIGIBLE,
                    claerancelist.MRD_DROPPED_PDF_FILE,
                    claerancelist.MRD_ROAD_PDF_FILE,
                    claerancelist.MRD_ROAD_EXCEL_FILE,
                }).ToArray();

                return result.Select(clearanceDetails => new
                {
                    id = clearanceDetails.MRD_DROPPED_CODE.ToString().Trim(),//URLEncrypt.EncryptParameters1(new string[] { "AdminCode =" + adminDetails.ADMIN_ND_CODE.ToString().Trim() }),

                    cell = new[]{
                    
     /*1*/          //"<center><a href='#' class='ui-icon ui-icon-plusthick'  title='Add Dropped Letter' onclick='loadSaveMrdDroppedLetter(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim() }) + "\"); return false;'>Drop</a></center>",

     /*2*/          clearanceDetails.MAST_STATE_NAME == null ? string.Empty : clearanceDetails.MAST_STATE_NAME.ToString(),
     /*3*/          clearanceDetails.MAST_AGENCY_NAME == null ? string.Empty : clearanceDetails.MAST_AGENCY_NAME.ToString(),
     /*4*/          clearanceDetails.MRD_DROPPED_DATE == null ? "" : ConvertDateToString(clearanceDetails.MRD_DROPPED_DATE),
     /*5*/          clearanceDetails.MRD_DROPPED_LETTER_NUMBER == null ? "" : clearanceDetails.MRD_DROPPED_LETTER_NUMBER,

     /*5*/           (dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_DROPPED_PDF_FILE).FirstOrDefault() == null || dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_DROPPED_PDF_FILE).FirstOrDefault() == "") 
                     ? "-" : "<a href='#' title='Click here to Download File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownloadDroppedFile(\"" + URLEncrypt.EncryptParameters(new string[]{ dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_DROPPED_PDF_FILE).FirstOrDefault().ToString().Trim() }) +"\"); return false;'>Download</a>",
     /*6*/           (dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_ROAD_PDF_FILE).FirstOrDefault() == null || dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_ROAD_PDF_FILE).FirstOrDefault() == "") 
                     ? "-" : "<a href='#' title='Click here to Download File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownloadDroppedFile(\"" + URLEncrypt.EncryptParameters(new string[]{ dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_ROAD_PDF_FILE).FirstOrDefault().ToString().Trim() }) +"\"); return false;'>Download</a>",
     /*7*/           (dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_ROAD_EXCEL_FILE).FirstOrDefault() == null || dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_ROAD_EXCEL_FILE).FirstOrDefault() == "") 
                     ? "-" : "<a href='#' title='Click here to Download File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownloadDroppedFile(\"" + URLEncrypt.EncryptParameters(new string[]{ dbContext.MRD_DROPPED_LETTERS.Where(a=>a.MRD_DROPPED_CODE==clearanceDetails.MRD_DROPPED_CODE).Select(m=>m.MRD_ROAD_EXCEL_FILE).FirstOrDefault().ToString().Trim() }) +"\"); return false;'>Download</a>",

     /*6*/          clearanceDetails.MAST_FUNDING_AGENCY_NAME == null?string.Empty:clearanceDetails.MAST_FUNDING_AGENCY_NAME.ToString(),
     /*7*/          (clearanceDetails.MAST_YEAR+"-"+ (clearanceDetails.MAST_YEAR+1)).ToString(),
     /*8*/          clearanceDetails.MAST_BATCH_NAME == null?string.Empty:clearanceDetails.MAST_BATCH_NAME.ToString(),

     /*9*/         Convert.ToString(clearanceDetails.MRD_ROAD_MORD_SHARE_AMT + clearanceDetails.MRD_LSB_MORD_SHARE_AMT),
     /*10*/         Convert.ToString(clearanceDetails.MRD_ROAD_STATE_SHARE_AMT + clearanceDetails.MRD_LSB_STATE_SHARE_AMT),
     /*11*/         Convert.ToString(clearanceDetails.MRD_ROAD_TOTAL_AMT + clearanceDetails.MRD_LSB_TOTAL_AMT),

//     /*16*/         clearanceDetails.MRD_TOTAL_MORD_SHARE_AMT.ToString(),
//     /*17*/         clearanceDetails.MRD_TOTAL_STATE_SHARE_AMT.ToString(),
//     /*18*/         clearanceDetails.MRD_TOTAL_SANCTIONED_AMT.ToString(), */

     /*12*/          clearanceDetails.MRD_TOTAL_ROADS.ToString(),
     /*13*/         clearanceDetails.MRD_TOTAL_ROAD_LENGTH.ToString(),
     /*14*/          clearanceDetails.MRD_TOTAL_LSB.ToString(),
     /*15*/         clearanceDetails.MRD_TOTAL_LSB_LENGTH.ToString(),

     /*16*/         clearanceDetails.MRD_HAB_1000.ToString(),
     /*17*/         clearanceDetails.MRD_HAB_500.ToString(),
     /*18*/         clearanceDetails.MRD_HAB_250_ELIGIBLE.ToString(),
     /*19*/         clearanceDetails.MRD_HAB_100_ELIGIBLE.ToString(),

                    //"<a href='#' title='Click here to Download File List' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownloadDroppedFile(\"" + URLEncrypt.EncryptParameters(new string[]{"ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"ClearanceRevisionCode="+clearanceDetails.MRD_ORG_CLEARANCE_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) +"\"); return false;'>Download</a>" ,
                    
                     
                    dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == clearanceDetails.MRD_DROPPED_CODE).Any() ?
                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='loadEditMrdDroppedLetter(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"DroppedCode="+clearanceDetails.MRD_DROPPED_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) + "\"); return false;'>Edit</a></center>"
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                     dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == clearanceDetails.MRD_DROPPED_CODE).Any() ?
                    "<center><a href='#' class='ui-icon ui-icon-trash' onclick='deleteMrdDroppedLetter(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"DroppedCode="+clearanceDetails.MRD_DROPPED_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) + "\"); return false;'>Delete</a></center>"        
                    :("<center><table><tr><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"),
                    //"<center><a href='#' class='ui-icon ui-icon-zoomin' onclick='ViewClearanceRevisionDetail(\"" +  URLEncrypt.EncryptParameters1(new string[]{ "ClearanceCode="+clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"DroppedCode="+clearanceDetails.MRD_DROPPED_CODE.ToString().Trim(),"Status=R".ToString().Replace("/", "")}) + "\"); return false;'>View</a></center>"        
                  
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        #region  ListMrdClearanceFileDAL
        //public Array ListMrdClearanceFileDAL(int cleranceCode, string clearanceStatus, int page, int rows, string sidx, string sord, out long totalRecords)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        var list = from filelist in dbContext.MRD_CLEARANCE_LETTERS
        //                   join state in dbContext.MASTER_STATE on filelist.MAST_STATE_CODE equals state.MAST_STATE_CODE
        //                   join agency in dbContext.MASTER_AGENCY on filelist.ADMIN_ND_CODE equals agency.MAST_AGENCY_CODE
        //                   join batchs in dbContext.MASTER_BATCH on filelist.MAST_BATCH equals batchs.MAST_BATCH_CODE
        //                   join fund in dbContext.MASTER_FUNDING_AGENCY on filelist.MAST_COLLABORATION equals fund.MAST_FUNDING_AGENCY_CODE
        //                   where
        //                   filelist.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
        //                   filelist.MRD_CLEARANCE_CODE == cleranceCode &&
        //                   filelist.MRD_CLEARANCE_STATUS == clearanceStatus
        //                   select new
        //                   {
        //                       filelist.MRD_CLEARANCE_CODE,
        //                       state.MAST_STATE_NAME,
        //                       agency.MAST_AGENCY_NAME,
        //                       agency.MAST_AGENCY_TYPE,
        //                       batchs.MAST_BATCH_NAME,
        //                       filelist.MAST_YEAR,
        //                       fund.MAST_FUNDING_AGENCY_NAME,
        //                       filelist.MRD_CLEARANCE_NUMBER,
        //                       filelist.MRD_CLEARANCE_DATE,
        //                       filelist.MRD_TOTAL_ROADS,
        //                       filelist.MRD_TOTAL_LSB,
        //                       filelist.MRD_ROAD_MORD_SHARE_AMT,
        //                       filelist.MRD_ROAD_STATE_SHARE_AMT,
        //                       filelist.MRD_ROAD_TOTAL_AMT,
        //                       filelist.MRD_LSB_MORD_SHARE_AMT,
        //                       filelist.MRD_LSB_STATE_SHARE_AMT,
        //                       filelist.MRD_LSB_TOTAL_AMT,
        //                       filelist.MRD_TOTAL_MORD_SHARE_AMT,
        //                       filelist.MRD_TOTAL_STATE_SHARE_AMT,
        //                       filelist.MRD_TOTAL_SANCTIONED_AMT,
        //                       filelist.MRD_TOTAL_ROAD_LENGTH,
        //                       filelist.MRD_TOTAL_LSB_LENGTH,
        //                       filelist.MRD_HAB_1000,
        //                       filelist.MRD_HAB_250_ELIGIBLE,
        //                       filelist.MRD_HAB_100_ELIGIBLE,
        //                       filelist.MRD_CLEARANCE_PDF_FILE,
        //                       filelist.MRD_ROAD_PDF_FILE,
        //                       filelist.MRD_ROAD_EXCEL_FILE,
        //                       filelist.MRD_CLEARANCE_STATUS,
        //                       filelist.MRD_ORG_CLEARANCE_CODE
        //                   };

        //        totalRecords = list.Count();
        //        int MaxCleranceCode = 0;
        //        if (list.Select(a => a.MRD_CLEARANCE_STATUS).FirstOrDefault().ToString() == "O")
        //        {
        //            MaxCleranceCode = dbContext.MRD_CLEARANCE_LETTERS.Where(a => a.MRD_ORG_CLEARANCE_CODE == list.Select(b => b.MRD_CLEARANCE_CODE).FirstOrDefault()).Count();
        //        }
        //        else if (list.Select(a => a.MRD_CLEARANCE_STATUS).FirstOrDefault().ToString() == "R")
        //        {
        //            MaxCleranceCode = dbContext.MRD_CLEARANCE_LETTERS.Where(a => a.MRD_ORG_CLEARANCE_CODE == list.Select(b => b.MRD_ORG_CLEARANCE_CODE).FirstOrDefault()).Max(a => a.MRD_CLEARANCE_CODE);
        //        }

        //        if (sidx.Trim() != string.Empty)
        //        {
        //            if (sord.ToString() == "asc")
        //            {
        //                switch (sidx)
        //                {
        //                    case "MAST_STATE_NAME":
        //                        list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Year":
        //                        list = list.OrderBy(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Batch":
        //                        list = list.OrderBy(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Agency":
        //                        list = list.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Collaboration":
        //                        list = list.OrderBy(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    default:
        //                        list = list.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                }
        //                //list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //            }
        //            else
        //            {
        //                switch (sidx)
        //                {
        //                    case "MAST_STATE_NAME":
        //                        list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Year":
        //                        list = list.OrderByDescending(x => x.MAST_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Batch":
        //                        list = list.OrderByDescending(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Agency":
        //                        list = list.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    case "Collaboration":
        //                        list = list.OrderByDescending(x => x.MAST_FUNDING_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                    default:
        //                        list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //                        break;
        //                }
        //                list = list.OrderByDescending(x => x.MAST_STATE_NAME).ThenBy(x => x.MRD_CLEARANCE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //            }
        //        }
        //        else
        //        {
        //            list = list.OrderBy(x => x.MAST_STATE_NAME).ThenBy(x => x.MAST_AGENCY_NAME).ThenBy(x => x.MRD_CLEARANCE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
        //        }

        //        var result = list.Select(claerancelist => new
        //        {
        //            claerancelist.MRD_CLEARANCE_CODE,
        //            claerancelist.MAST_STATE_NAME,
        //            claerancelist.MAST_AGENCY_NAME,
        //            claerancelist.MAST_AGENCY_TYPE,
        //            claerancelist.MAST_BATCH_NAME,
        //            claerancelist.MAST_YEAR,
        //            claerancelist.MAST_FUNDING_AGENCY_NAME,
        //            claerancelist.MRD_CLEARANCE_NUMBER,
        //            claerancelist.MRD_CLEARANCE_DATE,
        //            claerancelist.MRD_TOTAL_ROADS,
        //            claerancelist.MRD_TOTAL_LSB,
        //            claerancelist.MRD_ROAD_MORD_SHARE_AMT,
        //            claerancelist.MRD_ROAD_STATE_SHARE_AMT,
        //            claerancelist.MRD_ROAD_TOTAL_AMT,
        //            claerancelist.MRD_LSB_MORD_SHARE_AMT,
        //            claerancelist.MRD_LSB_STATE_SHARE_AMT,
        //            claerancelist.MRD_LSB_TOTAL_AMT,
        //            claerancelist.MRD_TOTAL_MORD_SHARE_AMT,
        //            claerancelist.MRD_TOTAL_STATE_SHARE_AMT,
        //            claerancelist.MRD_TOTAL_SANCTIONED_AMT,
        //            claerancelist.MRD_TOTAL_ROAD_LENGTH,
        //            claerancelist.MRD_TOTAL_LSB_LENGTH,
        //            claerancelist.MRD_HAB_1000,
        //            claerancelist.MRD_HAB_250_ELIGIBLE,
        //            claerancelist.MRD_HAB_100_ELIGIBLE,
        //            claerancelist.MRD_CLEARANCE_PDF_FILE,
        //            claerancelist.MRD_ROAD_PDF_FILE,
        //            claerancelist.MRD_ROAD_EXCEL_FILE,
        //            claerancelist.MRD_CLEARANCE_STATUS
        //        }).ToArray();

        //        return result.Select(clearanceDetails => new
        //        {
        //            id = clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),//URLEncrypt.EncryptParameters1(new string[] { "AdminCode =" + adminDetails.ADMIN_ND_CODE.ToString().Trim() }),

        //            cell = new[]{                   

        //            clearanceDetails.MRD_CLEARANCE_STATUS=="O"?                   
        //            (clearanceDetails.MRD_CLEARANCE_PDF_FILE==null||clearanceDetails.MRD_CLEARANCE_PDF_FILE==string.Empty)? "NA" :
        //             MaxCleranceCode==0?
        //            "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Clerance Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CP", "File =" + clearanceDetails.MRD_CLEARANCE_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Clerance Pdf File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CP", "File =" + clearanceDetails.MRD_CLEARANCE_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>"
        //            :"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Clerance Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CP", "File =" + clearanceDetails.MRD_CLEARANCE_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"
        //            :(clearanceDetails.MRD_CLEARANCE_PDF_FILE==null||clearanceDetails.MRD_CLEARANCE_PDF_FILE==string.Empty)? "NA" :
        //            MaxCleranceCode==clearanceDetails.MRD_CLEARANCE_CODE?
        //             "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Revised Clerance  Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CPR", "File =" + clearanceDetails.MRD_CLEARANCE_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Revised Clerance  Pdf File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CPR", "File =" + clearanceDetails.MRD_CLEARANCE_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>"
        //            : "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Clerance  Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CPR", "File =" + clearanceDetails.MRD_CLEARANCE_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>",

        //             clearanceDetails.MRD_CLEARANCE_STATUS=="O"?
        //             (clearanceDetails.MRD_ROAD_PDF_FILE==null||clearanceDetails.MRD_ROAD_PDF_FILE==string.Empty)? "NA" :
        //              MaxCleranceCode==0?
        //            "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Road List Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RP", "File =" + clearanceDetails.MRD_ROAD_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Road Pdf File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RP", "File =" + clearanceDetails.MRD_ROAD_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>" 
        //            :"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Road List Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RP", "File =" + clearanceDetails.MRD_ROAD_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>"                    
        //            :(clearanceDetails.MRD_ROAD_PDF_FILE==null||clearanceDetails.MRD_ROAD_PDF_FILE==string.Empty)? "NA" :
        //             MaxCleranceCode==clearanceDetails.MRD_CLEARANCE_CODE?
        //             "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Revised Road List  Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RPR", "File =" + clearanceDetails.MRD_ROAD_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Revised Road List  Pdf File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RPR", "File =" + clearanceDetails.MRD_ROAD_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>"
        //             : "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Revised Road List  Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RPR", "File =" + clearanceDetails.MRD_ROAD_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>",

        //            clearanceDetails.MRD_CLEARANCE_STATUS=="O"?
        //            (clearanceDetails.MRD_ROAD_EXCEL_FILE==null ||clearanceDetails.MRD_ROAD_EXCEL_FILE==string.Empty)? "NA" :
        //            MaxCleranceCode==0?
        //            "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Road List Excel File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RE", "File =" + clearanceDetails.MRD_ROAD_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Road List Excel File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RE", "File =" + clearanceDetails.MRD_ROAD_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>" 
        //            :"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Road List Excel File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RE", "File =" + clearanceDetails.MRD_ROAD_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>" 
        //             :(clearanceDetails.MRD_ROAD_EXCEL_FILE==null ||clearanceDetails.MRD_ROAD_EXCEL_FILE==string.Empty)? "NA" :
        //             MaxCleranceCode==clearanceDetails.MRD_CLEARANCE_CODE?
        //            "<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Revised Road List Excel File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RER", "File =" + clearanceDetails.MRD_ROAD_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Revised Road List  Excel File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RER", "File =" + clearanceDetails.MRD_ROAD_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>"
        //            :"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Revised Road List Excel File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RER", "File =" + clearanceDetails.MRD_ROAD_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Locked' class='ui-icon ui-icon-locked  ui-align-center'>Locked</a></td></tr></table></center>" ,

        //            //(clearanceDetails.MRD_CLEARANCE_REVISED_PDF_FILE==null ||clearanceDetails.MRD_CLEARANCE_REVISED_PDF_FILE==string.Empty)? "NA" :                   
        //            //"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Clerance Revised Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CPR", "File =" + clearanceDetails.MRD_CLEARANCE_REVISED_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Clerance Revised Pdf File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = CPR", "File =" + clearanceDetails.MRD_CLEARANCE_REVISED_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>" ,
        //            //(clearanceDetails.MRD_ROAD_REVISED_PDF_FILE==null ||clearanceDetails.MRD_ROAD_REVISED_PDF_FILE==string.Empty)? "NA" :
        //            //"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Road Revised Pdf File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RPR", "File =" + clearanceDetails.MRD_ROAD_REVISED_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Revised Road Pdf File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RPR", "File =" + clearanceDetails.MRD_ROAD_REVISED_PDF_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>" ,
        //            //(clearanceDetails.MRD_ROAD_REVISED_EXCEL_FILE==null ||clearanceDetails.MRD_ROAD_REVISED_EXCEL_FILE==string.Empty)? "NA" :
        //            //"<center><table><tr><td style='border:none'><a href='#' title='Click here to Download an Road Revised Excel File' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownLoadFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RER", "File =" + clearanceDetails.MRD_ROAD_REVISED_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Download</a> </td><td style='border:none'><a href='#' title='Click here to Delete Revised Road Excel File' class='ui-icon ui-icon-trash' onClick=DeleteFile(\"" + URLEncrypt.EncryptParameters(new string[] {"ClearanceCode=" + clearanceDetails.MRD_CLEARANCE_CODE.ToString().Trim(),"Type = RER", "File =" + clearanceDetails.MRD_ROAD_REVISED_EXCEL_FILE.ToString().Replace("/", "")}) +"\"); return false;'>Delete</a> </td></tr></table></center>" ,
        //        }
        //        }).ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

        //        totalRecords = 0;
        //        return null;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        #endregion

        public bool DeleteMrdClearanceDAL(int clearanceCode, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MRD_CLEARANCE_LETTERS clearanceModel = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == clearanceCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013
                clearanceModel.USERID = PMGSYSession.Current.UserId;
                clearanceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(clearanceModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                dbContext.MRD_CLEARANCE_LETTERS.Remove(clearanceModel);
                dbContext.SaveChanges();
                message = "Clearance details deleted successfully.";
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "You can not delete this details.";
                return false;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public bool saveDroppedLettersDAL(PMGSY.Models.Proposal.MrdDroppedViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            //string message = "";
            try
            {
                int dropId = 0;
                int recordCount = 0;
                DateTime dt = Convert.ToDateTime(model.MRD_CLEARANCE_DATE);
                PMGSY.Models.MRD_DROPPED_LETTERS MrdDroppedLetters = new MRD_DROPPED_LETTERS();

                dbContext = new PMGSYEntities();
                recordCount = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == model.MRD_CLEARANCE_CODE && m.MRD_DROPPED_LETTER_NUMBER == model.MRD_CLEARANCE_NUMBER).Count();
                if (recordCount > 0)
                {
                    message = "Dropped Letter details already exist.";
                    return false;
                }
                MrdDroppedLetters.MRD_CLEARANCE_CODE = model.MRD_CLEARANCE_CODE;
                MrdDroppedLetters.MAST_YEAR = model.PhaseYear;
                MrdDroppedLetters.MAST_BATCH = model.Batch;
                MrdDroppedLetters.MAST_COLLABORATION = Convert.ToInt32(model.IMS_COLLABORATION);
                MrdDroppedLetters.ADMIN_ND_CODE = model.Mast_Agency;
                MrdDroppedLetters.MAST_STATE_CODE = model.StateCode;
                MrdDroppedLetters.MRD_TOTAL_ROADS = model.MRD_TOTAL_ROADS;
                MrdDroppedLetters.MRD_TOTAL_LSB = model.MRD_TOTAL_LSB;
                MrdDroppedLetters.MRD_ROAD_MORD_SHARE_AMT = model.MRD_ROAD_MORD_SHARE_AMT;
                MrdDroppedLetters.MRD_ROAD_STATE_SHARE_AMT = model.MRD_ROAD_STATE_SHARE_AMT;
                MrdDroppedLetters.MRD_ROAD_TOTAL_AMT = model.MRD_ROAD_TOTAL_AMT;
                MrdDroppedLetters.MRD_LSB_MORD_SHARE_AMT = model.MRD_LSB_MORD_SHARE_AMT;
                MrdDroppedLetters.MRD_LSB_STATE_SHARE_AMT = model.MRD_LSB_STATE_SHARE_AMT;
                MrdDroppedLetters.MRD_LSB_TOTAL_AMT = model.MRD_LSB_TOTAL_AMT;
                MrdDroppedLetters.MRD_TOTAL_MORD_SHARE_AMT = model.MRD_TOTAL_MORD_SHARE_AMT;
                MrdDroppedLetters.MRD_TOTAL_STATE_SHARE_AMT = model.MRD_TOTAL_STATE_SHARE_AMT;
                MrdDroppedLetters.MRD_TOTAL_SANCTIONED_AMT = model.MRD_TOTAL_SANCTIONED_AMT;
                MrdDroppedLetters.MRD_TOTAL_ROAD_LENGTH = model.MRD_TOTAL_ROAD_LENGTH;
                MrdDroppedLetters.MRD_TOTAL_LSB_LENGTH = model.MRD_TOTAL_LSB_LENGTH;
                MrdDroppedLetters.MRD_HAB_1000 = model.MRD_HAB_1000;
                MrdDroppedLetters.MRD_HAB_500 = model.MRD_HAB_500;
                MrdDroppedLetters.MRD_HAB_250_ELIGIBLE = model.MRD_HAB_250_ELIGIBLE;
                MrdDroppedLetters.MRD_HAB_100_ELIGIBLE = model.MRD_HAB_100_ELIGIBLE;
                MrdDroppedLetters.MRD_DROPPED_CODE = model.MRD_DROPPED_CODE;
                MrdDroppedLetters.MRD_DROPPED_DATE = Convert.ToDateTime(model.MRD_CLEARANCE_DATE);
                MrdDroppedLetters.MRD_DROPPED_LETTER_NUMBER = model.MRD_CLEARANCE_NUMBER;
                MrdDroppedLetters.MRD_DROPPED_PDF_FILE = model.MRD_DROPPED_PDF_FILE == null ? null : model.MRD_DROPPED_PDF_FILE;
                MrdDroppedLetters.MRD_ROAD_PDF_FILE = model.MRD_ROAD_PDF_FILE == null ? null : model.MRD_ROAD_PDF_FILE;
                MrdDroppedLetters.MRD_ROAD_EXCEL_FILE = model.MRD_ROAD_EXCEL_FILE == null ? null : model.MRD_ROAD_EXCEL_FILE;
                MrdDroppedLetters.USERID = PMGSYSession.Current.UserId;
                MrdDroppedLetters.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                MrdDroppedLetters.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                MrdDroppedLetters.MRD_UPGRADE_CONNECT = model.UPGRADE_CONNECT;
                MrdDroppedLetters.MRD_STAGE_COMPLETE = model.STAGE_COMPLETE;
                MrdDroppedLetters.MRD_DROPPED_REMARKS = model.MRD_DROPPED_REMARKS;
                dbContext.MRD_DROPPED_LETTERS.Add(MrdDroppedLetters);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }

        }

        public bool editDroppedLettersDAL(PMGSY.Models.Proposal.MrdDroppedViewModel model)
        {
            dbContext = new PMGSYEntities();
            string message = "";
            try
            {
                int recordCount = 0;

                PMGSY.Models.MRD_DROPPED_LETTERS MrdDroppedLetters = new MRD_DROPPED_LETTERS();

                dbContext = new PMGSYEntities();

                var s = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == model.MRD_CLEARANCE_CODE && m.MRD_DROPPED_CODE != model.MRD_DROPPED_CODE).ToList();
                foreach(var itm in s)
                {
                    if(itm.MRD_DROPPED_LETTER_NUMBER == model.MRD_CLEARANCE_NUMBER)
                    return false;
                }

                MrdDroppedLetters = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == model.MRD_DROPPED_CODE).FirstOrDefault();

                MrdDroppedLetters.MRD_CLEARANCE_CODE = model.MRD_CLEARANCE_CODE;
                MrdDroppedLetters.MAST_YEAR = model.PhaseYear;
                MrdDroppedLetters.MAST_BATCH = model.Batch;
                MrdDroppedLetters.MAST_COLLABORATION = Convert.ToInt32(model.IMS_COLLABORATION);
                MrdDroppedLetters.ADMIN_ND_CODE = model.Mast_Agency;
                MrdDroppedLetters.MAST_STATE_CODE = model.StateCode;
                MrdDroppedLetters.MRD_TOTAL_ROADS = model.MRD_TOTAL_ROADS;
                MrdDroppedLetters.MRD_TOTAL_LSB = model.MRD_TOTAL_LSB;
                MrdDroppedLetters.MRD_ROAD_MORD_SHARE_AMT = model.MRD_ROAD_MORD_SHARE_AMT;
                MrdDroppedLetters.MRD_ROAD_STATE_SHARE_AMT = model.MRD_ROAD_STATE_SHARE_AMT;
                MrdDroppedLetters.MRD_ROAD_TOTAL_AMT = model.MRD_ROAD_TOTAL_AMT;
                MrdDroppedLetters.MRD_LSB_MORD_SHARE_AMT = model.MRD_LSB_MORD_SHARE_AMT;
                MrdDroppedLetters.MRD_LSB_STATE_SHARE_AMT = model.MRD_LSB_STATE_SHARE_AMT;
                MrdDroppedLetters.MRD_LSB_TOTAL_AMT = model.MRD_LSB_TOTAL_AMT;
                MrdDroppedLetters.MRD_TOTAL_MORD_SHARE_AMT = model.MRD_TOTAL_MORD_SHARE_AMT;
                MrdDroppedLetters.MRD_TOTAL_STATE_SHARE_AMT = model.MRD_TOTAL_STATE_SHARE_AMT;
                MrdDroppedLetters.MRD_TOTAL_SANCTIONED_AMT = model.MRD_TOTAL_SANCTIONED_AMT;
                MrdDroppedLetters.MRD_TOTAL_ROAD_LENGTH = model.MRD_TOTAL_ROAD_LENGTH;
                MrdDroppedLetters.MRD_TOTAL_LSB_LENGTH = model.MRD_TOTAL_LSB_LENGTH;
                MrdDroppedLetters.MRD_HAB_1000 = model.MRD_HAB_1000;
                MrdDroppedLetters.MRD_HAB_500 = model.MRD_HAB_500;
                MrdDroppedLetters.MRD_HAB_250_ELIGIBLE = model.MRD_HAB_250_ELIGIBLE;
                MrdDroppedLetters.MRD_HAB_100_ELIGIBLE = model.MRD_HAB_100_ELIGIBLE;
                MrdDroppedLetters.MRD_DROPPED_CODE = model.MRD_DROPPED_CODE;
                MrdDroppedLetters.MRD_DROPPED_DATE = Convert.ToDateTime(model.MRD_CLEARANCE_DATE);
                MrdDroppedLetters.MRD_DROPPED_LETTER_NUMBER = model.MRD_CLEARANCE_NUMBER;
                //MrdDroppedLetters.MRD_DROPPED_PDF_FILE = model.MRD_DROPPED_PDF_FILE == null ? null : model.MRD_DROPPED_PDF_FILE;
                //MrdDroppedLetters.MRD_ROAD_PDF_FILE = model.MRD_ROAD_PDF_FILE == null ? null : model.MRD_ROAD_PDF_FILE;
                //MrdDroppedLetters.MRD_ROAD_EXCEL_FILE = model.MRD_ROAD_EXCEL_FILE == null ? null : model.MRD_ROAD_EXCEL_FILE;
                if (model.MRD_DROPPED_PDF_FILE != null)
                {
                    MrdDroppedLetters.MRD_DROPPED_PDF_FILE = model.MRD_DROPPED_PDF_FILE;
                }
                if (model.MRD_ROAD_PDF_FILE != null)
                {
                    MrdDroppedLetters.MRD_ROAD_PDF_FILE = model.MRD_ROAD_PDF_FILE;
                }
                if (model.MRD_ROAD_EXCEL_FILE != null)
                {
                    MrdDroppedLetters.MRD_ROAD_EXCEL_FILE = model.MRD_ROAD_EXCEL_FILE;
                }
                MrdDroppedLetters.USERID = PMGSYSession.Current.UserId;
                MrdDroppedLetters.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                MrdDroppedLetters.MRD_DROPPED_REMARKS = model.MRD_DROPPED_REMARKS;
                MrdDroppedLetters.MRD_UPGRADE_CONNECT = model.UPGRADE_CONNECT;
                MrdDroppedLetters.MRD_STAGE_COMPLETE = model.STAGE_COMPLETE;
                //MrdDroppedLetters.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;

                //dbContext.MRD_DROPPED_LETTERS.Add(MrdDroppedLetters);
                dbContext.Entry(MrdDroppedLetters).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }

        }

        public bool deleteDroppedLettersDAL(int dropCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                MRD_DROPPED_LETTERS MrdDroppedLetters = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropCode).FirstOrDefault();
                //Added by abhishek kamble 27-nov-2013
                MrdDroppedLetters.USERID = PMGSYSession.Current.UserId;
                MrdDroppedLetters.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //dbContext.Entry(clearanceModel).State = System.Data.Entity.EntityState.Modified;
                //dbContext.SaveChanges();
                dbContext.MRD_DROPPED_LETTERS.Remove(MrdDroppedLetters);
                dbContext.SaveChanges();
                //message = "Clearance details deleted successfully.";
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public string ConvertDateToString(DateTime? date)
        {
            try
            {
                return Convert.ToDateTime(date).ToString("dd/MM/yyyy");
            }
            catch
            {
                return null;
            }
        }
    }

    public interface IProposalDAL1
    {
        Array ListMrdClearanceDAL(int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListMrdDroppedDAL(int clrId, int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords);
        //MrdDroppedViewModel GetMrdDroppedDetailsDAL(int clearanceCode);
        bool saveDroppedLettersDAL(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters, ref string msg);

        bool editDroppedLettersDAL(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters);

        bool deleteDroppedLettersDAL(int dropCode);
    }
}