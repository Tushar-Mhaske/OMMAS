#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MaintenanceBAL.cs
        * Description   :   BAL Methods to call DAL methods of Creating , Editing, Deleting PCI Index of PMGSY Roads and Core Network Roads.
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   18/June/2013
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
using PMGSY.DAL.Maintenance;
using System.Web.Mvc;
using PMGSY.Common;

namespace PMGSY.BAL.Maintenance
{
    public class MaintenanceBAL : IMaintenanceBAL
    {
        #region Variable Declaration
        IMaintenanceDAL objMaintenanceDal = null;
        PMGSYEntities DbContext = null;
        #endregion
        
        #region PCI Index

        /// <summary>
        /// Method to Populate Pmgsy Road 
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
        public Array GetPmgsyRoadsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE)
        {
            objMaintenanceDal = new MaintenanceDAL();
            return objMaintenanceDal.GetPmgsyRoadsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE,ADMIN_ND_CODE, IMS_YEAR, MAST_BLOCK_CODE);
        }

        /// <summary>
        /// Save the PCI Index Information for PMGSY Roads
        /// </summary>
        /// <param name="pciIndexViewModel"></param>
        /// <returns></returns>
        public string SavePciForPmgsyRoadBAL(Models.Maintenance.PciIndexViewModel pciIndexViewModel)
        {
            try
            {
                DbContext = new PMGSYEntities();
                objMaintenanceDal = new MaintenanceDAL();
                CommonFunctions objCommon = new CommonFunctions();
                int MaxSegmentNumber = 0;
                
                if( DbContext.MANE_IMS_PCI_INDEX.Where(a=> a.IMS_PR_ROAD_CODE == pciIndexViewModel.IMS_PR_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Any())
                {
                    MaxSegmentNumber = DbContext.MANE_IMS_PCI_INDEX.Where( a => a.IMS_PR_ROAD_CODE == pciIndexViewModel.IMS_PR_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Select(a=> a.MANE_SEGMENT_NO).Max();
                }
                MaxSegmentNumber++;

                MANE_IMS_PCI_INDEX mane_ims_pci_index = new MANE_IMS_PCI_INDEX();                                
                mane_ims_pci_index.IMS_PR_ROAD_CODE = pciIndexViewModel.IMS_PR_ROAD_CODE;
                mane_ims_pci_index.MANE_SEGMENT_NO = MaxSegmentNumber;
                mane_ims_pci_index.MANE_PCI_YEAR = pciIndexViewModel.MANE_PCI_YEAR;
                mane_ims_pci_index.MANE_PCIINDEX = pciIndexViewModel.MANE_PCIINDEX;
                mane_ims_pci_index.MANE_STR_CHAIN = pciIndexViewModel.MANE_STR_CHAIN;
                mane_ims_pci_index.MANE_END_CHAIN = pciIndexViewModel.MANE_END_CHAIN;
                mane_ims_pci_index.MANE_SURFACE_TYPE = pciIndexViewModel.MANE_SURFACE_TYPE;
                mane_ims_pci_index.MANE_PCI_DATE = objCommon.GetStringToDateTime(pciIndexViewModel.MANE_PCI_DATE); //Convert.ToDateTime(pciIndexViewModel.MANE_PCI_DATE);

                return objMaintenanceDal.SavePciForPmgsyRoadDAL(mane_ims_pci_index);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);          
                return "Error Occured while processing your request.";
            }
            finally
            {
                DbContext.Dispose();
            }
        }

        /// <summary>
        /// Get the PCI list for PMGSY Roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPCIListForPmgsyRoadBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            objMaintenanceDal = new MaintenanceDAL();
            return objMaintenanceDal.GetPCIListForPmgsyRoadDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Get the Road Details ie. Last Entry of PCI Index 
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="MANE_PCI_YEAR"></param>
        /// <returns></returns>
        public string GetRoadDetailsBAL(int IMS_PR_ROAD_CODE, int MANE_PCI_YEAR)
        {
            try
            {
                DbContext= new PMGSYEntities();
                if (DbContext.MANE_IMS_PCI_INDEX.Where( c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && c.MANE_PCI_YEAR == MANE_PCI_YEAR).Any())
                {
                    decimal Road_Length = DbContext.IMS_SANCTIONED_PROJECTS.Where(a=> a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(a=> a.IMS_PAV_LENGTH).First();
 
                    decimal Max_End_Chain = (
                                                from c in DbContext.MANE_IMS_PCI_INDEX 
                                                where
                                                    c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && 
                                                    c.MANE_PCI_YEAR == MANE_PCI_YEAR            
                                                select
                                                    c.MANE_END_CHAIN
                                            ).Max();

                    if(Max_End_Chain == Road_Length)
                    {
                        return "-111";
                    }
                    else
                    {
                        return Max_End_Chain.ToString();
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);       
                return "-999";
            }
            finally
            {
                DbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete the PCI Index Data of Pmgsy Road
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="MANE_SEGMENT_NO"></param>
        /// <param name="MANE_PCI_YEAR"></param>
        /// <returns></returns>
        public string DeletePciforPmgsyRoadBAL(int IMS_PR_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR)
        {
            objMaintenanceDal = new MaintenanceDAL();
            return objMaintenanceDal.DeletePciforPmgsyRoadDAL(IMS_PR_ROAD_CODE, MANE_SEGMENT_NO, MANE_PCI_YEAR);
        }

        /// <summary>
        /// Populate the Road Type
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateRoadType()
        {
            List<SelectListItem> lstRaodType = new List<SelectListItem>();
            
            SelectListItem item = new SelectListItem();
            //item.Text = "PMGSY Road";
            //item.Value = "P";
            //item.Selected = true;
            //lstRaodType.Add(item);

            
            item.Text = "Core Network Road";
            item.Value = "C";
            item.Selected = true;
            lstRaodType.Add(item);

            return lstRaodType;
        }

        /// <summary>
        /// Method to List Core Network Roads
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
        public Array GetCNRoadsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID)
        {
            objMaintenanceDal = new MaintenanceDAL();
            return objMaintenanceDal.GetCNRoadsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, ADMIN_ND_CODE, IMS_BLOCK_ID);
        }

        /// <summary>
        /// Get the PCI List of CN Roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPCIListForCNRoadBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE)
        {
            objMaintenanceDal = new MaintenanceDAL();
            return objMaintenanceDal.GetPCIListForCNRoadDAL(page, rows, sidx, sord, out totalRecords, PLAN_CN_ROAD_CODE);
        }
            
        /// <summary>
        /// Get the CN Road Details
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <param name="MANE_IMS_YEAR"></param>
        /// <returns></returns>
        public string GetCNRoadDetailsBAL(int PLAN_CN_ROAD_CODE, int MANE_IMS_YEAR)
        {
            try
            {
                DbContext = new PMGSYEntities();
                if (DbContext.MANE_CN_PCI_INDEX.Where(c => c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && c.MANE_PCI_YEAR == MANE_IMS_YEAR).Any())
                {
                    decimal? Road_Length = DbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_LENGTH).First();

                    decimal Max_End_Chain = (
                                                from c in DbContext.MANE_CN_PCI_INDEX
                                                where
                                                    c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE &&
                                                    c.MANE_PCI_YEAR == MANE_IMS_YEAR
                                                select
                                                    c.MANE_END_CHAIN
                                            ).Max();

                    if (Max_End_Chain == Road_Length)
                    {
                        return "-111";
                    }
                    else
                    {
                        return Max_End_Chain.ToString();
                    }
                }
                else
                {
                    decimal? startChainage = DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(m=>m.PLAN_RD_FROM_CHAINAGE).FirstOrDefault();
                    if (startChainage == null)
                    {
                        return "0";
                    }
                    else
                    {
                        return startChainage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);              
                return "-999";
            }
            finally
            {
                DbContext.Dispose();
            }
        }

        /// <summary>
        /// Save the PCI for CN Road
        /// </summary>
        /// <param name="pciIndexViewModel"></param>
        /// <returns></returns>
        public string SavePciForCNRoadBAL(Models.Maintenance.PciIndexViewModel pciIndexViewModel)
        {
             try
            {
                DbContext = new PMGSYEntities();

                PLAN_ROAD planCNRoad = DbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).FirstOrDefault();

                objMaintenanceDal = new MaintenanceDAL();
                decimal chainage = 0;
                 //new code added by Vikram for validating total chainage of Core Network.
                if (DbContext.MANE_CN_PCI_INDEX.Any(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE))
                {
                    foreach (var item in DbContext.MANE_CN_PCI_INDEX.Where(m=>m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && m.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).ToList())
                    {
                        chainage += (item.MANE_END_CHAIN - item.MANE_STR_CHAIN);
                    }
                }

                if (chainage == null)
                {
                    chainage = 0;
                }

                chainage = chainage + (pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN);
                ///Changed by SAMMED A. PATIL on 27 APRIL 2017 to consider total length for candidate road
                if (planCNRoad != null && planCNRoad.PLAN_RD_TOTAL_LEN != null && planCNRoad.PLAN_RD_TOTAL_LEN != 0)
                {
                    if (chainage > planCNRoad.PLAN_RD_TOTAL_LEN.Value)
                    {
                        return "Sum of Chainage is exceeding the Road Length.";
                    }
                }
                else
                {
                    if (chainage > (DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_TO_CHAINAGE).FirstOrDefault() - DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_FROM_CHAINAGE).FirstOrDefault()))
                    {
                        return "Sum of Chainage is exceeding the Road Length.";
                    }
                }

                int MaxSegmentNumber = 0;
                
                if( DbContext.MANE_CN_PCI_INDEX.Where(a=> a.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Any())
                {
                    MaxSegmentNumber = DbContext.MANE_CN_PCI_INDEX.Where( a => a.PLAN_CN_ROAD_CODE == pciIndexViewModel.PLAN_CN_ROAD_CODE && a.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).Select(a=> a.MANE_SEGMENT_NO).Max();
                }
                MaxSegmentNumber++;

                MANE_CN_PCI_INDEX mane_cn_pci_index = new MANE_CN_PCI_INDEX();                                
                mane_cn_pci_index.PLAN_CN_ROAD_CODE = pciIndexViewModel.PLAN_CN_ROAD_CODE;
                mane_cn_pci_index.MANE_SEGMENT_NO = MaxSegmentNumber;
                mane_cn_pci_index.MANE_PCI_YEAR = pciIndexViewModel.MANE_PCI_YEAR;
                mane_cn_pci_index.MANE_PCIINDEX = pciIndexViewModel.MANE_PCIINDEX;
                mane_cn_pci_index.MANE_STR_CHAIN = pciIndexViewModel.MANE_STR_CHAIN;
                mane_cn_pci_index.MANE_END_CHAIN = pciIndexViewModel.MANE_END_CHAIN;
                mane_cn_pci_index.MANE_SURFACE_TYPE = pciIndexViewModel.MANE_SURFACE_TYPE;
                mane_cn_pci_index.MANE_PCI_DATE = Convert.ToDateTime(pciIndexViewModel.MANE_PCI_DATE);

                return objMaintenanceDal.SavePciForCNRoadDAL(mane_cn_pci_index);
            }
             catch (Exception ex)
             {
                 Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                 return "Error Occured while processing your request.";
            }
            finally
            {
                DbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete the PCI For CN Roads
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <param name="MANE_SEGMENT_NO"></param>
        /// <param name="MANE_PCI_YEAR"></param>
        /// <returns></returns>
        public string DeletePciforCNRoadBAL(int PLAN_CN_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR)
        {
            objMaintenanceDal = new MaintenanceDAL();
            return objMaintenanceDal.DeletePciforCNRoadDAL(PLAN_CN_ROAD_CODE, MANE_SEGMENT_NO, MANE_PCI_YEAR);
        }

        #endregion
    }

    public interface IMaintenanceBAL
    {
        Array GetPmgsyRoadsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_YEAR, int MAST_BLOCK_CODE);
        Array GetCNRoadsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int ADMIN_ND_CODE, int IMS_BLOCK_ID);
        string SavePciForPmgsyRoadBAL(Models.Maintenance.PciIndexViewModel pciIndexViewModel);
        Array GetPCIListForPmgsyRoadBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string GetRoadDetailsBAL(int IMS_PR_ROAD_CODE, int MANE_PCI_YEAR);
        string DeletePciforPmgsyRoadBAL(int IMS_PR_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR);
        List<SelectListItem> PopulateRoadType();
        Array GetPCIListForCNRoadBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int PLAN_CN_ROAD_CODE);
        string GetCNRoadDetailsBAL(int PLAN_CN_ROAD_CODE, int MANE_IMS_YEAR);
        string SavePciForCNRoadBAL(Models.Maintenance.PciIndexViewModel pciIndexViewModel);
        string DeletePciforCNRoadBAL(int PLAN_CN_ROAD_CODE, int MANE_SEGMENT_NO, int MANE_PCI_YEAR);
    }
}