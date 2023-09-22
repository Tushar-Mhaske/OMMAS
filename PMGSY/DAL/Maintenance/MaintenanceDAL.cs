#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MaintenanceDAL.cs     
        * Description   :   Data Methods for Creating , Editing, Deleting PCI Index of PMGSY Roads and Core Network Roads.
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   04/April/2013
 **/
#endregion

using PMGSY.DAL.Proposal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.Proposal;
using PMGSY.Extensions;
using PMGSY.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;


namespace PMGSY.DAL.Maintenance
{
    public class MaintenanceDAL : IMaintenanceDAL
    {
        PMGSYEntities dbContext = null;

        /// <summary>
        /// Get the List of PMGSY Roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="MAST_STATE_CODE"></param>
        /// <param name="MAST_DISTRICT_CODE"></param>
        /// <param name="ADMIN_ND_CODE"></param>
        /// <param name="IMS_YEAR"></param>
        /// <param name="MAST_BLOCK_CODE"></param>
        /// <returns></returns>
        public Array GetPmgsyRoadsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                List<IMS_SANCTIONED_PROJECTS> list_ims_sanctioned_project =(
                                                                                from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                join d in dbContext.EXEC_ROADS_MONTHLY_STATUS
                                                                                on  c.IMS_PR_ROAD_CODE equals d.IMS_PR_ROAD_CODE
                                                                                where
                                                                                    c.IMS_PR_ROAD_CODE == d.IMS_PR_ROAD_CODE &&
                                                                                    c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                    c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
                                                                                    c.MAST_DPIU_CODE == ADMIN_ND_CODE &&
                                                                                    (MAST_BLOCK_CODE > 0 ? c.MAST_BLOCK_CODE : 1) == (MAST_BLOCK_CODE > 0 ? MAST_BLOCK_CODE : 1) &&
                                                                                    c.IMS_YEAR == IMS_YEAR &&
                                                                                    c.IMS_PROPOSAL_TYPE == "P" &&
                                                                                    (c.IMS_ISCOMPLETED == "C" || c.IMS_ISCOMPLETED == "X")&&
                                                                                    d.EXEC_ISCOMPLETED == "C" &&
                                                                                    c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //new change done by Vikram on 10 Feb 2014
                                                                                select c
                                                                            ).ToList<IMS_SANCTIONED_PROJECTS>();
                totalRecords = list_ims_sanctioned_project.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        list_ims_sanctioned_project = list_ims_sanctioned_project.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        list_ims_sanctioned_project = list_ims_sanctioned_project.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    list_ims_sanctioned_project = list_ims_sanctioned_project.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                return list_ims_sanctioned_project.Select(propDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { propDetails.IMS_PR_ROAD_CODE.ToString() }),
                    cell = new[] {                         
                                    (propDetails.IMS_ROAD_NAME == null || propDetails.IMS_ROAD_NAME == "" )? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_PAV_LENGTH.ToString(),                                                               
                                    (propDetails.MANE_IMS_PCI_INDEX.Where(a=> a.IMS_PR_ROAD_CODE == propDetails.IMS_PR_ROAD_CODE).Any()) ? propDetails.MANE_IMS_PCI_INDEX.Max(a=> a.MANE_PCI_YEAR).ToString() : "-",
                                    "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForPmgsyRoad(\"" + URLEncrypt.EncryptParameters(new string[] { propDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) + "\"); return false;'>CBR Details</a>" 
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

        /// <summary>
        /// Get the Core Network Roads for CN Roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="MAST_STATE_CODE"></param>
        /// <param name="MAST_DISTRICT_CODE"></param>
        /// <param name="ADMIN_ND_CODE"></param>
        /// <param name="IMS_BLOCK_ID"></param>
        /// <returns></returns>
        public Array GetCNRoadsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();

                List<int> RoadCatagoryFilter = new List<int>();
                RoadCatagoryFilter.Add(1); // National Highway
                RoadCatagoryFilter.Add(2); // State Highway
                RoadCatagoryFilter.Add(3); // Major District Road

                List<PLAN_ROAD> ListCNRoads = (
                                                    from c in dbContext.PLAN_ROAD
                                                    join e in dbContext.MASTER_EXISTING_ROADS
                                                    on c.MAST_ER_ROAD_CODE equals e.MAST_ER_ROAD_CODE 
                                                    join f in dbContext.MASTER_ROAD_CATEGORY 
                                                    on e.MAST_ROAD_CAT_CODE equals f.MAST_ROAD_CAT_CODE                                                    
                                                    where
                                                    c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                    c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE &&
                                                    c.MAST_BLOCK_CODE == (IMS_BLOCK_ID <= 0 ? c.MAST_BLOCK_CODE : IMS_BLOCK_ID) &&

                                                  // (IMS_BLOCK_ID > 0 ? c.MAST_BLOCK_CODE : 1) == (IMS_BLOCK_ID > 0 ? IMS_BLOCK_ID : 1) 
                                                    //&& !RoadCatagoryFilter.Contains(f.MAST_ROAD_CAT_CODE)


                                                    c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme //as per the change done in application for scheme 
                                                    select c
                                               ).OrderBy(c => c.PLAN_CN_ROAD_NUMBER).ToList<PLAN_ROAD>();

                totalRecords = ListCNRoads.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ListCNRoads = ListCNRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListCNRoads = ListCNRoads.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    ListCNRoads = ListCNRoads.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }        

                ListCNRoads.Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));

                return ListCNRoads.Select(CNDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString() }),
                    cell = new[] {                         
                                    (CNDetails.PLAN_RD_NAME == null || CNDetails.PLAN_RD_NAME == "" ) ? "NA" :  CNDetails.PLAN_RD_NAME,
                                    CNDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                                    CNDetails.MASTER_EXISTING_ROADS.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                                    CNDetails.PLAN_RD_FROM_CHAINAGE .ToString(),
                                    CNDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                                    CNDetails.PLAN_RD_TOTAL_LEN == null ? CNDetails.PLAN_RD_LENGTH.ToString() : CNDetails.PLAN_RD_TOTAL_LEN.ToString(),
                                    (CNDetails.MANE_CN_PCI_INDEX.Where(a=> a.PLAN_CN_ROAD_CODE == CNDetails.PLAN_CN_ROAD_CODE).Any()) ? CNDetails.MANE_CN_PCI_INDEX.Max(a => a.MANE_PCI_YEAR).ToString() : "-",                                    
                                     "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='AddPCIIndexForCNRoad(\"" + URLEncrypt.EncryptParameters(new string[] { CNDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) + "\"); return false;'>PCI Index</a>" 
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

        /// <summary>
        /// Save the PCI Index Information for PMGSY Roads
        /// </summary>
        /// <param name="mane_ims_pci_index"></param>
        /// <returns></returns>
        public string SavePciForPmgsyRoadDAL(MANE_IMS_PCI_INDEX mane_ims_pci_index)
        {
            try
            {
                dbContext = new PMGSYEntities();
                mane_ims_pci_index.USERID = PMGSYSession.Current.UserId;
                mane_ims_pci_index.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.MANE_IMS_PCI_INDEX.Add(mane_ims_pci_index);
                dbContext.SaveChanges();
                return string.Empty;
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
         
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                dbContext.Dispose();                
            }
        }

        /// <summary>
        /// Save the PCI for CN Roads
        /// </summary>
        /// <param name="mane_cn_pci_index"></param>
        /// <returns></returns>
        public string SavePciForCNRoadDAL(MANE_CN_PCI_INDEX mane_cn_pci_index)
        {
            try
            {
                dbContext = new PMGSYEntities();
                mane_cn_pci_index.USERID = PMGSYSession.Current.UserId;
                mane_cn_pci_index.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.MANE_CN_PCI_INDEX.Add(mane_cn_pci_index);
                dbContext.SaveChanges();
                return string.Empty;
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
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Get the PCI List of Pmgsy Roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPCIListForPmgsyRoadDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                List<MANE_IMS_PCI_INDEX> lstPciIndex = (from c in dbContext.MANE_IMS_PCI_INDEX
                                                        where c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE
                                                        select c).OrderByDescending(c => c.MANE_PCI_YEAR).ToList<MANE_IMS_PCI_INDEX>();
                totalRecords = lstPciIndex.Count();

                //lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(rows).ToList<MANE_IMS_PCI_INDEX>();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstPciIndex = lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).OrderBy(x => x.MANE_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstPciIndex = lstPciIndex.OrderBy(x => x.MANE_PCI_YEAR).OrderBy(x => x.MANE_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstPciIndex = lstPciIndex.OrderBy(x => x.MANE_PCI_YEAR).OrderBy(x => x.MANE_SEGMENT_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                lstPciIndex = lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).ToList();

                return lstPciIndex.Select(PciDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] {  PciDetails.IMS_PR_ROAD_CODE.ToString() + "$" + PciDetails.MANE_SEGMENT_NO + "$" + PciDetails.MANE_PCI_YEAR  }),
                    cell = new[] {                         
                                    PciDetails.MANE_PCI_YEAR.ToString(),
                                    PciDetails.MANE_SEGMENT_NO.ToString(),
                                    PciDetails.MANE_STR_CHAIN.ToString(),
                                    PciDetails.MANE_END_CHAIN.ToString(),
                                    PciDetails.MANE_PCIINDEX.ToString(),
                                    PciDetails.MASTER_SURFACE.MAST_SURFACE_NAME,
                                    PciDetails.MANE_PCI_DATE != null ? Convert.ToDateTime(PciDetails.MANE_PCI_DATE).ToString("dd-MMM-yyyy") : "NA",
                                    (dbContext.MANE_IMS_PCI_INDEX.Where(a=> a.IMS_PR_ROAD_CODE == PciDetails.IMS_PR_ROAD_CODE && a.MANE_PCI_YEAR == PciDetails.MANE_PCI_YEAR).Select(a=> a.MANE_END_CHAIN).Max() == PciDetails.MANE_END_CHAIN )  ? "<a href='#' title='Click here to delete Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePciForPmgsyRoadDetails(\"" +URLEncrypt.EncryptParameters(new string[] {  PciDetails.IMS_PR_ROAD_CODE.ToString() + "$" + PciDetails.MANE_SEGMENT_NO + "$" + PciDetails.MANE_PCI_YEAR  }) +"\"); return false;'>Show Details</a>"  :  "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='return false;'",
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

        /// <summary>
        /// Delete the PCI Index Data of Pmgsy Road
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="MANE_SEGMENT_NO"></param>
        /// <param name="MANE_PCI_YEAR"></param>
        /// <returns></returns>
        public string DeletePciforPmgsyRoadDAL(int IMS_PR_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();

                decimal ToKm = dbContext.MANE_IMS_PCI_INDEX.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                                                                  a.MANE_PCI_YEAR == MANE_PCI_YEAR).Select(a => a.MANE_END_CHAIN).Max();

                MANE_IMS_PCI_INDEX mane_ims_pci_index = dbContext.MANE_IMS_PCI_INDEX.Where(
                                                        a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                                                        a.MANE_SEGMENT_NO == MANE_SEGMENT_NO &&
                                                        a.MANE_PCI_YEAR == MANE_PCI_YEAR).First();

                //mane_ims_pci_index.USERID = PMGSYSession.Current.UserId;
                //mane_ims_pci_index.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //dbContext.Entry(mane_ims_pci_index).State = System.Data.Entity.EntityState.Modified;
                //dbContext.SaveChanges();

                if (mane_ims_pci_index.MANE_END_CHAIN == ToKm)
                {
                    dbContext.MANE_IMS_PCI_INDEX.Remove(mane_ims_pci_index);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    return "Only Last Entry in Year Can be Deleted.";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                         
                return "Error occured while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete the PCI For CN Road
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <param name="MANE_SEGMENT_NO"></param>
        /// <param name="MANE_PCI_YEAR"></param>
        /// <returns></returns>
        public string DeletePciforCNRoadDAL(int PLAN_CN_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();

                decimal ToKm = dbContext.MANE_CN_PCI_INDEX.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE  &&
                                                                  a.MANE_PCI_YEAR == MANE_PCI_YEAR).Select(a => a.MANE_END_CHAIN).Max();

                MANE_CN_PCI_INDEX mane_cn_pci_index = dbContext.MANE_CN_PCI_INDEX.Where(
                                                        a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE &&
                                                        a.MANE_SEGMENT_NO == MANE_SEGMENT_NO &&
                                                        a.MANE_PCI_YEAR == MANE_PCI_YEAR).First();

                //mane_cn_pci_index.USERID = PMGSYSession.Current.UserId;
                //mane_cn_pci_index.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //dbContext.Entry(mane_cn_pci_index).State = System.Data.Entity.EntityState.Modified;
                //dbContext.SaveChanges();

                if (mane_cn_pci_index.MANE_END_CHAIN == ToKm)
                {
                    dbContext.MANE_CN_PCI_INDEX.Remove(mane_cn_pci_index);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else
                {
                    return "Only Last Entry in Year Can be Deleted.";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "Error occured while processing your request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Get the PCI List for CN Roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPCIListForCNRoadDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                List<MANE_CN_PCI_INDEX> lstPciIndex = (from c in dbContext.MANE_CN_PCI_INDEX
                                                        where c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE
                                                       select c).OrderByDescending(c => c.MANE_PCI_YEAR).ToList<MANE_CN_PCI_INDEX>();
                totalRecords = lstPciIndex.Count();

                //lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(rows).ToList<MANE_CN_PCI_INDEX>();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        lstPciIndex = lstPciIndex.OrderBy(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        lstPciIndex = lstPciIndex.OrderByDescending(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                else
                {
                    lstPciIndex = lstPciIndex.OrderBy(x => x.MANE_PCI_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }        



                return lstPciIndex.Select(PciDetails => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { PciDetails.PLAN_CN_ROAD_CODE.ToString() + "$" + PciDetails.MANE_SEGMENT_NO + "$" + PciDetails.MANE_PCI_YEAR }),
                    cell = new[] {                         
                                    PciDetails.MANE_PCI_YEAR.ToString(),
                                    PciDetails.MANE_SEGMENT_NO.ToString(),
                                    PciDetails.MANE_STR_CHAIN.ToString(),
                                    PciDetails.MANE_END_CHAIN.ToString(),
                                    PciDetails.MANE_PCIINDEX.ToString(),
                                    PciDetails.MANE_SURFACE_TYPE == null ? "" : PciDetails.MASTER_SURFACE.MAST_SURFACE_NAME,
                                    PciDetails.MANE_PCI_DATE != null ? Convert.ToDateTime(PciDetails.MANE_PCI_DATE).ToString("dd-MMM-yyyy") : "NA",                                  
                                    (dbContext.MANE_CN_PCI_INDEX.Where(a=> a.PLAN_CN_ROAD_CODE == PciDetails.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == PciDetails.MANE_PCI_YEAR).Select(a=> a.MANE_END_CHAIN).Max() == PciDetails.MANE_END_CHAIN )  ? "<a href='#' title='Click here to delete Road Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePciForCNRoadDetails(\"" +URLEncrypt.EncryptParameters(new string[] {  PciDetails.PLAN_CN_ROAD_CODE.ToString() + "$" + PciDetails.MANE_SEGMENT_NO + "$" + PciDetails.MANE_PCI_YEAR  }) +"\"); return false;'>Show Details</a>"   :  "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='return false;'"
                                    
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
    }

    public interface IMaintenanceDAL
    {
        Array GetPmgsyRoadsDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE);
        Array GetCNRoadsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID);
        string SavePciForPmgsyRoadDAL(MANE_IMS_PCI_INDEX mane_ims_pci_index);
        Array GetPCIListForPmgsyRoadDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string DeletePciforPmgsyRoadDAL(int IMS_PR_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR);
        Array GetPCIListForCNRoadDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE);
        string SavePciForCNRoadDAL(MANE_CN_PCI_INDEX mane_cn_pci_index);
        string DeletePciforCNRoadDAL(int PLAN_CN_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR);
    }
}