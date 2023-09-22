using PMGSY.Areas.GPSVTSInstallationDetails.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Areas.GPSVTSInstallationDetails.DAL
{
    public class GPSVTSDetailsDAL
    {
        PMGSYEntities dbContext = new PMGSYEntities();
        public Array GPSVTSRoadListDAL(string WorkStatus,int state, int district, int block, int sanction_year, int batch, string proposalType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                bool isVTSLastDateExceed = false;
                DateTime VTS_ENTRY_LASTDATE = Convert.ToDateTime(ConfigurationManager.AppSettings["VTS_ENTRY_LASTDATE"].ToString());
                
                // To Get Server DateTime
                DateTime utcTime = DateTime.UtcNow;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime todaysDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local

                isVTSLastDateExceed = (VTS_ENTRY_LASTDATE > todaysDate) ? false : true;

                


                var resultList = new List<RoadListmodel>();
                
                var PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
                var RoadList = (from isp in dbContext.IMS_SANCTIONED_PROJECTS
                                //join er in dbContext.EXEC_ROADS_MONTHLY_STATUS
                                    //on isp.IMS_PR_ROAD_CODE equals er.IMS_PR_ROAD_CODE
                                join vts in dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                    on isp.IMS_PR_ROAD_CODE equals vts.IMS_PR_ROAD_CODE into vtsGroup
                                from vts in vtsGroup.DefaultIfEmpty()
                                join ms in dbContext.MASTER_STATE
                                    on isp.MAST_STATE_CODE equals ms.MAST_STATE_CODE into msGroup
                                from ms in msGroup.DefaultIfEmpty()
                                join md in dbContext.MASTER_DISTRICT
                                    on isp.MAST_DISTRICT_CODE equals md.MAST_DISTRICT_CODE into mdGroup
                                from md in mdGroup.DefaultIfEmpty()
                                join mb in dbContext.MASTER_BLOCK
                                    on isp.MAST_BLOCK_CODE equals mb.MAST_BLOCK_CODE into mbGroup
                                from mb in mbGroup.DefaultIfEmpty()
                                where (state == 0 || isp.MAST_STATE_CODE == state)
                                    && (district == 0 || isp.MAST_DISTRICT_CODE == district)
                                    && (block == 0 || isp.MAST_BLOCK_CODE == block)
                                    && (sanction_year == 0 || isp.IMS_YEAR == sanction_year)
                                    && (batch == 0 || isp.IMS_BATCH == batch)
                                    && isp.MAST_PMGSY_SCHEME == PMGSYScheme
                                    && isp.IMS_PROPOSAL_TYPE == "P"
									//&& (isp.IMS_ISCOMPLETED != "C" && isp.IMS_ISCOMPLETED != "X")
                                    //&& er.EXEC_ISCOMPLETED == "P"
                                    //&& (WorkStatus != "A" ? (WorkStatus == "F" ? vts == null : vts != null) : true)
                                    && isp.IMS_SANCTIONED == "Y"
                                    //&& er.EXEC_PROG_YEAR == dbContext.EXEC_ROADS_MONTHLY_STATUS
                                    //                           .Where(e => e.IMS_PR_ROAD_CODE == isp.IMS_PR_ROAD_CODE)
                                    //                           .Max(e => e.EXEC_PROG_YEAR)
                                    //&& er.EXEC_PROG_MONTH == dbContext.EXEC_ROADS_MONTHLY_STATUS
                                    //                            .Where(e => e.IMS_PR_ROAD_CODE == isp.IMS_PR_ROAD_CODE && e.EXEC_PROG_YEAR == er.EXEC_PROG_YEAR)
                                    //                            .Max(e => e.EXEC_PROG_MONTH)
                                select new
                                {
                                    ms.MAST_STATE_NAME,
                                    md.MAST_DISTRICT_NAME,
                                    mb.MAST_BLOCK_NAME,
                                    TYPE = isp.IMS_PROPOSAL_TYPE == "P" ? "Road" : "Bridge",
                                    isp.IMS_PR_ROAD_CODE,
                                    isp.IMS_ROAD_NAME,
                                    isp.IMS_PACKAGE_ID,
                                    isp.IMS_YEAR,
                                    isp.IMS_BATCH,
                                    isp.IMS_PAV_LENGTH,
                                    isp.IMS_SANCTIONED_BS_AMT,
                                    isp.MAST_PMGSY_SCHEME,
                                    GPSVTS_Established = vts != null ? "Yes" : "No",
                                    GPSVTS_Finalized = vts.IS_PDF_FINALIZED == null ? "No" : (vts.IS_PDF_FINALIZED.Equals("Y")? "Yes" : "No"),
                                    GPS_INSTALLED = vts.GPS_INSTALLED == "Y" ? "Yes" : vts.GPS_INSTALLED == "N" ? "No" : "-",
                                    //
                                    isFinalized = vts.IS_PDF_FINALIZED == null ? "N": vts.IS_PDF_FINALIZED
                                    //
                                }).OrderBy(v => v.MAST_STATE_NAME).ThenBy(v => v.MAST_DISTRICT_NAME).ThenBy(v => v.MAST_BLOCK_NAME).Distinct().ToList();


                totalRecords = RoadList.Count();

                foreach (var item in RoadList)
                {
                    resultList.Add(new RoadListmodel()
                    {
                        StateName = item.MAST_STATE_NAME,
                        DistrictName = item.MAST_DISTRICT_NAME,
                        BlockName = item.MAST_BLOCK_NAME,
                        PackageId = item.IMS_PACKAGE_ID,
                        Year = (item.IMS_YEAR).ToString() + "-" + (item.IMS_YEAR + 1).ToString().Substring(2, 2),
                        Batch = string.Concat("Batch-", item.IMS_BATCH.ToString()),
                        Length = item.IMS_PAV_LENGTH,
                        SanctionedAmt = item.IMS_SANCTIONED_BS_AMT,
                        GPSVTS_Established = item.GPSVTS_Established,
                        GPSVTS_Finalized=item.GPSVTS_Finalized,
                        RoadCode = item.IMS_PR_ROAD_CODE,
                        RoadName = item.IMS_ROAD_NAME,
                        Is_VTSEntryLastDate_Exceed = isVTSLastDateExceed,
                        isFinalized = item.isFinalized,
                        IsGPSInstalled = item.GPS_INSTALLED,
                        IsVTSWorkUnfreezed = dbContext.VTS_UNFREEZE_WORKS.Where(s=>s.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && s.IS_UNFREEZED.Equals("Y")).Any()                       
                    });
                }

                return resultList.Select(RoadDetails => new
                {
                    cell = new[] {
                        RoadDetails.StateName,
                        RoadDetails.DistrictName,
                        RoadDetails.BlockName,
                        RoadDetails.PackageId,
                        RoadDetails.Year,
                        RoadDetails.Batch.ToString(),
                        RoadDetails.Length.ToString(),
                        RoadDetails.SanctionedAmt.ToString(),
                        RoadDetails.RoadName,
                        RoadDetails.GPSVTS_Established,
                        RoadDetails.GPSVTS_Finalized,
                        
                        // VTS Details Add
                        "<span class='ui-icon ui-icon-circle-plus ui-align-center' title='Add GPS/VTS Details' onclick='Load_GPSVTS_Saved_Details(\""+URLEncrypt.EncryptParameters1(new string[]{"roadcode =" + RoadDetails.RoadCode.ToString().Trim()})+"\");' > </span>",
                        
                        // validation for if last date exceeded then check work unfreeze then only allow
                        //(isVTSLastDateExceed == true) 
                        //    ?  (RoadDetails.IsVTSWorkUnfreezed == true) ? "<span class='ui-icon ui-icon-circle-plus ui-align-center' title='Add GPS/VTS Details' onclick='Load_GPSVTS_Saved_Details(\""+URLEncrypt.EncryptParameters1(new string[]{"roadcode =" + RoadDetails.RoadCode.ToString().Trim()})+"\");' > </span>" : "<span class='ui-icon ui-icon-locked ui-align-center' title='Lock due to VTS entry last date exceeded..!!' onclick='LockVTSEntryDateMsg();' ></span>"
                        //    :  "<span class='ui-icon ui-icon-circle-plus ui-align-center' title='Add GPS/VTS Details' onclick='Load_GPSVTS_Saved_Details(\""+URLEncrypt.EncryptParameters1(new string[]{"roadcode =" + RoadDetails.RoadCode.ToString().Trim()})+"\");' > </span>",
                        
                        // PDF File Upload
                        (RoadDetails.IsGPSInstalled.Equals("Yes"))
                        ? "<span class='ui-icon ui-icon-circle-arrow-n ui-align-center' title='Upload File' onclick='UploadFile(\"" + URLEncrypt.EncryptParameters1(new string[] { "roadcode =" + RoadDetails.RoadCode.ToString().Trim() , "isFinalized =" + RoadDetails.isFinalized.ToString().Trim() }) + "\");' > </span>"
                        : "<span class='ui-icon ui-icon-locked ui-align-center' title='Work Freezed - Upload Not Allowed'></span>"

                        // validation for if last date exceeded then check work unfreeze & GPS Installed "Y" then only allow
                        //(isVTSLastDateExceed == true)
                        //    ? (RoadDetails.IsVTSWorkUnfreezed == true && RoadDetails.IsGPSInstalled.Equals("Yes")) ? "<span class='ui-icon ui-icon-circle-arrow-n ui-align-center' title='Upload File' onclick='UploadFile(\"" + URLEncrypt.EncryptParameters1(new string[] { "roadcode =" + RoadDetails.RoadCode.ToString().Trim() , "isFinalized =" + RoadDetails.isFinalized.ToString().Trim() }) + "\");' > </span>" : "<span class='ui-icon ui-icon-locked ui-align-center' title='Work Freezed - Upload Not Allowed'></span>"
                        //    : (RoadDetails.IsGPSInstalled.Equals("Yes")) 
                        //        ? "<span class='ui-icon ui-icon-circle-arrow-n ui-align-center' title='Upload File' onclick='UploadFile(\"" + URLEncrypt.EncryptParameters1(new string[] { "roadcode =" + RoadDetails.RoadCode.ToString().Trim() , "isFinalized =" + RoadDetails.isFinalized.ToString().Trim() }) + "\");' > </span>"
                        //        : "<span class='ui-icon ui-icon-locked ui-align-center' title='Work Freezed - Upload Not Allowed'></span>"


                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GPSVTSRoadListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public IMS_SANCTIONED_PROJECTS FindRoadDetail(int roadCode)
        {
            dbContext = new PMGSYEntities();
            IMS_SANCTIONED_PROJECTS objmodel = new IMS_SANCTIONED_PROJECTS();
            try
            {
                objmodel = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.IMS_SANCTIONED == "Y").FirstOrDefault();
                return objmodel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.FindRoadDetail()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }
        public List<SelectListItem> GetVEHICLEDetailsList()
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var Vts_Vehicle_Master_List = dbContext.VTS_VEHICLE_MASTER.Where(x => x.IS_ACTIVE == true).ToList();
                    var selectList = new List<SelectListItem>();

                    var firstItem = new SelectListItem
                    {
                        Text = "Select",
                        Value = "0"
                    };
                    selectList.Add(firstItem);

                    foreach (var item in Vts_Vehicle_Master_List)
                    {
                        var selectListObj = new SelectListItem
                        {
                            Text = item.VEHICLE_NAME.ToString(),
                            Value = item.VEHICLE_ID.ToString()
                        };
                        selectList.Add(selectListObj);
                    }

                    return selectList;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GetVEHICLEDetailsList()");
                throw;
            }

        }

        public bool SaveGPSVTSDetails(string is_GPSVTS_Installed, int Road_Code, List<GPSVTSDataModel> VTS_INSTRUMENT_GPS_Details)
        {
            try
            {
                long VTS_ROADWISE_GPS_AVAILABILITY_P_KEY = 0;
                long VTS_ROADWISE_GPS_VEHICLE_COUNT_P_KEY = 0;
                bool SaveStatus = false;
                switch (is_GPSVTS_Installed)
                {
                    case "Y":
                        {

                            using (var dbContext = new PMGSYEntities())
                            {
                                long VTS_GPS_Id = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Any() ? (dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Select(x => x.VTS_GPS_ID)).Max() : 0;
                                long VEHICLE_Id = dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT.Any() ? (dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT.Select(x => x.VEHICLE_ID)).Max() : 0;
                                long VTS_VEHICLE_GPS_Id = dbContext.VTS_VEHICLEWISE_GPS_DETAILS.Any() ? (dbContext.VTS_VEHICLEWISE_GPS_DETAILS.Select(x => x.VTS_VEHICLE_GPS_ID)).Max() : 0;
                                var VTSRecordAVAILABILITYID = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x => x.IMS_PR_ROAD_CODE == Road_Code && x.GPS_INSTALLED == "Y").Select(c => c.VTS_GPS_ID).FirstOrDefault();
                                using (var transactionScope = new TransactionScope())
                                {
                                    try
                                    {
                                        if (VTSRecordAVAILABILITYID == 0)
                                        {
                                            var roadwiseGPSAvailability = new VTS_ROADWISE_GPS_AVAILABILITY
                                            {
                                                //VTS_GPS_ID = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Any() ? (dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Select(x => x.VTS_GPS_ID)).Max() + 1 : 1,
                                                VTS_GPS_ID = VTS_GPS_Id + 1,
                                                IMS_PR_ROAD_CODE = Road_Code,
                                                GPS_INSTALLED = is_GPSVTS_Installed,
                                                ISACTIVE = true,
                                                DATA_SUBMISSION_DATE = DateTime.Now,
                                                IS_PDF_FINALIZED = "N",
                                                USERID = PMGSYSession.Current.UserId,
                                                IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                                            };

                                            dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Add(roadwiseGPSAvailability);
                                            //
                                            var recordsToRemove = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x=>x.IMS_PR_ROAD_CODE == Road_Code && x.GPS_INSTALLED == "N");
                                            dbContext.VTS_ROADWISE_GPS_AVAILABILITY.RemoveRange(recordsToRemove);

                                            //
                                            dbContext.SaveChanges();
                                            VTS_ROADWISE_GPS_AVAILABILITY_P_KEY = roadwiseGPSAvailability.VTS_GPS_ID;

                                        }else
                                        {
                                            VTS_ROADWISE_GPS_AVAILABILITY_P_KEY = VTSRecordAVAILABILITYID;
                                                                                  
                                        }
                                        foreach (var item in VTS_INSTRUMENT_GPS_Details)
                                        {
                                            int VEHICLE_TYPE_Id = Convert.ToInt32(item.Vehicle);
                                            int NO_OF_VEHICLEs = Convert.ToInt32(item.VehiclesCount);
                                            DateTime DATE_OF_INSTALLATIOn = Convert.ToDateTime(item.VTS_InstallationDate);

                                            var roadwiseGPSVehicleCount = new VTS_ROADWISE_GPS_VEHICLE_COUNT
                                            {
                                                //VEHICLE_ID = dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT.Any() ? (dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT.Select(x => x.VEHICLE_ID)).Max() + 1 : 1,
                                                VEHICLE_ID = VEHICLE_Id + 1,
                                                VTS_GPS_ID = VTS_ROADWISE_GPS_AVAILABILITY_P_KEY,
                                                VEHICLE_TYPE_ID = VEHICLE_TYPE_Id,
                                                NO_OF_VEHICLES = NO_OF_VEHICLEs,
                                                DATE_OF_INSTALLATION = DATE_OF_INSTALLATIOn,
                                                DATE_OF_SUBMISSION = DateTime.Now,
                                                USERID = PMGSYSession.Current.UserId,
                                                IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                                            };

                                            dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT.Add(roadwiseGPSVehicleCount);
                                            dbContext.SaveChanges();
                                            VTS_ROADWISE_GPS_VEHICLE_COUNT_P_KEY = roadwiseGPSVehicleCount.VEHICLE_ID;

                                            foreach (var itemGPS_DETAILS in item.VehiclesID)
                                            {
                                                var vehiclewiseGPSDetails = new VTS_VEHICLEWISE_GPS_DETAILS
                                                {
                                                    //VTS_VEHICLE_GPS_ID = dbContext.VTS_VEHICLEWISE_GPS_DETAILS.Any() ? (dbContext.VTS_VEHICLEWISE_GPS_DETAILS.Select(x => x.VTS_VEHICLE_GPS_ID)).Max() + 1 : 1,
                                                    VTS_VEHICLE_GPS_ID = VTS_VEHICLE_GPS_Id + 1,
                                                    VEHICLE_ID = VTS_ROADWISE_GPS_VEHICLE_COUNT_P_KEY,
                                                    VTS_GPS_ID = VTS_ROADWISE_GPS_AVAILABILITY_P_KEY,
                                                    VTS_INSTRUMENT_GPS_ID = itemGPS_DETAILS,
                                                    DATE_OF_SUBMISSION = DateTime.Now,
                                                    USERID = PMGSYSession.Current.UserId,
                                                    IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                                                };

                                                dbContext.VTS_VEHICLEWISE_GPS_DETAILS.Add(vehiclewiseGPSDetails);
                                                dbContext.SaveChanges();
                                                VTS_VEHICLE_GPS_Id++;
                                            }
                                            VEHICLE_Id++;
                                        }
                                       
                                        transactionScope.Complete();

                                        
                                        SaveStatus = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw;
                                    }
                                }
                            }


                            break;
                        }
                    case "N":
                        {
                            using (var dbContext = new PMGSYEntities())
                            {
                                var IsVTSRecordAvailable = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x => x.IMS_PR_ROAD_CODE == Road_Code && x.GPS_INSTALLED == "N").Select(c => c.VTS_GPS_ID).Any();
                                if (!IsVTSRecordAvailable)
                                {
                                    var Result = new VTS_ROADWISE_GPS_AVAILABILITY
                                    {
                                        VTS_GPS_ID = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Any() ? (dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Select(x => x.VTS_GPS_ID)).Max() + 1 : 1,
                                        IMS_PR_ROAD_CODE = Road_Code,
                                        GPS_INSTALLED = is_GPSVTS_Installed,
                                        ISACTIVE = true,
                                        DATA_SUBMISSION_DATE = DateTime.Now,
                                        IS_PDF_FINALIZED = "N",
                                        USERID = PMGSYSession.Current.UserId,
                                        IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                                    };
                                    dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Add(Result);
                                    dbContext.SaveChanges();
                                    VTS_ROADWISE_GPS_AVAILABILITY_P_KEY = Result.VTS_GPS_ID; 
                                }
                                SaveStatus = true;
                            }
                            break;
                        }
                }
                return SaveStatus;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.SaveGPSVTSDetails()");
                throw;
            }
        }

        public bool CheckVTS_PDFAvailable(int roadCode)
        {
            dbContext = new PMGSYEntities();
            
            try
            {
                if (dbContext.VTS_GPS_FILES_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == roadCode).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.CheckVTS_PDFAvailable()");
                throw;
            }
        }

        public Array GetGPSVTSSavedDetailsDAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            try
            {
                using (var dbContext = new PMGSYEntities())
                {

                    var query = (from ava in dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                 join cnt in dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT
                                 on new { ID = (long?)ava.VTS_GPS_ID, UserId = (int?)ava.USERID } equals new { ID = (long?)cnt.VTS_GPS_ID, UserId = (int?)cnt.USERID }
                                 into vehicleCountGroup
                                 from cnt in vehicleCountGroup.DefaultIfEmpty() // Left join here
                                 join det in dbContext.VTS_VEHICLEWISE_GPS_DETAILS
                                 on new { VID = (long?)(cnt != null ? cnt.VEHICLE_ID : (long?)null), VTS_GPS_Id = (long?)ava.VTS_GPS_ID, UserId = (int?)(cnt != null ? cnt.USERID : (int?)null) } equals new { VID = (long?)(det != null ? det.VEHICLE_ID : (long?)null), VTS_GPS_Id = (long?)(det != null ? det.VTS_GPS_ID : (long?)null), UserId = (int?)(det != null ? det.USERID : (int?)null) }
                                 into detGroup
                                 from det in detGroup.DefaultIfEmpty()
                                 let vehicleId = cnt != null ? cnt.VEHICLE_ID : (long?)null
                                 let userId = cnt != null ? cnt.USERID : (int?)null
                                 join vm in dbContext.VTS_VEHICLE_MASTER
                                 on cnt != null ? cnt.VEHICLE_TYPE_ID : (int?)null equals vm != null ? vm.VEHICLE_ID : (int?)null
                                 into vmGroup
                                 from vm in vmGroup.DefaultIfEmpty()
                                 where ava.IMS_PR_ROAD_CODE == roadCode
                                 select new
                                 {
                                     ava.GPS_INSTALLED,
                                     DATE_OF_INSTALLATION = cnt != null ? cnt.DATE_OF_INSTALLATION : (DateTime?)null,
                                     NO_OF_VEHICLES = cnt != null ? cnt.NO_OF_VEHICLES : (int?)null,
                                     VTS_INSTRUMENT_GPS_ID = det != null ? det.VTS_INSTRUMENT_GPS_ID : "--",
                                     VEHICLE_TYPE_ID = cnt != null ? cnt.VEHICLE_TYPE_ID : (int?)null,
                                     VehicleName = vm != null ? vm.VEHICLE_NAME : null,
                                     ava.VTS_GPS_ID,
                                     VEHICLE_ID = vehicleId,
                                     //VTS_VEHICLE_GPS_ID = det != null ? det.VTS_VEHICLE_GPS_ID : (long?)null,
                                     VTS_VEHICLE_GPS_ID = det != null ? det.VTS_INSTRUMENT_GPS_ID : "--",
                                     cnt.DATE_OF_SUBMISSION,
                                     ava.DATA_SUBMISSION_DATE,
                                     IS_PDF_FINALIZED = ava.IS_PDF_FINALIZED == null ? "N" : ava.IS_PDF_FINALIZED
                                 })
              .GroupBy(x => x.VEHICLE_ID)
              .Select(group => new
              {
                  VEHICLE_ID = group.Key,
                  VTS_VEHICLE_GPS_IDS = group.Select(item => item.VTS_VEHICLE_GPS_ID).ToList(),
                  GROUP_ITEMS = group.ToList()
              })
              .OrderByDescending(x => x.GROUP_ITEMS.FirstOrDefault().DATE_OF_SUBMISSION ?? x.GROUP_ITEMS.FirstOrDefault().DATA_SUBMISSION_DATE)
              .ToList();


                return query
                            .Select(x => new GPSVTSSavedDetailsModel
                            {
                                GPSInstalled = x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED == "Y" ? "✔" : x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED == "N" ? "✘" : "--",
                                DateOfInstallation = x.GROUP_ITEMS.FirstOrDefault()?.DATE_OF_INSTALLATION?.ToString("dd/MM/yyyy") ?? "--",
                                NumberOfVehicles = x.GROUP_ITEMS.FirstOrDefault()?.NO_OF_VEHICLES ?? 0,
                                VTSInstrumentGPSID = x.GROUP_ITEMS.Any() ? string.Join(",", x.GROUP_ITEMS.Select(item => item.VTS_INSTRUMENT_GPS_ID ?? "--")) : "--",
                                VehicleTypeID = x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_TYPE_ID ?? 0,
                                VehicleName = x.GROUP_ITEMS.FirstOrDefault()?.VehicleName ?? "--",
                                VTSGPSID = x.GROUP_ITEMS.FirstOrDefault()?.VTS_GPS_ID,
                                VehicleID = x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_ID,
                                VTSVehicleGPSID = x.GROUP_ITEMS.Any() ? string.Join(",", x.GROUP_ITEMS.Select(item => item.VTS_INSTRUMENT_GPS_ID ?? "--")) : "--",
                                //Edit = x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED == "Y" ? $"<span class='ui-icon ui-icon-pencil ui-align-center' title='Edit Details' onclick='EditGPSVTSDetails(\"{URLEncrypt.EncryptParameters1(new[] { "roadcode =" + roadCode.ToString().Trim(), "VTSGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_GPS_ID.ToString().Trim(), "VehicleID =" + x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_ID.ToString().Trim(), "VTSVehicleGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_VEHICLE_GPS_ID.ToString().Trim() })}\");' ></span>" : "--",
                                Edit = x.GROUP_ITEMS.FirstOrDefault().IS_PDF_FINALIZED.Equals("Y") ? "<span class='ui-icon ui-icon-locked ui-align-center'> </span>" : x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED == "Y" ? $"<span class='ui-icon ui-icon-pencil ui-align-center' title='Edit Details' onclick='EditGPSVTSDetails(\"{URLEncrypt.EncryptParameters1(new[] { "roadcode =" + roadCode.ToString().Trim(), "VTSGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_GPS_ID.ToString().Trim(), "VehicleID =" + x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_ID.ToString().Trim(), "VTSVehicleGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_VEHICLE_GPS_ID.ToString().Trim() })}\");' ></span>" : "--",
                                //Detete = x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED == "Y" ? $"<span class='ui-icon ui-icon-trash ui-align-center' title='Delete Details' onclick='DeleteGPSVTSDetails(\"{URLEncrypt.EncryptParameters1(new[] { "roadcode =" + roadCode.ToString().Trim(), "VTSGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_GPS_ID.ToString().Trim(), "VehicleID =" + x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_ID.ToString().Trim(), "VTSVehicleGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_VEHICLE_GPS_ID.ToString().Trim() })}\");' ></span>" : "--",
                                //Detete = $"<span class='ui-icon ui-icon-trash ui-align-center' title='Delete Details' onclick='DeleteGPSVTSDetails(\"{URLEncrypt.EncryptParameters1(new[] { "roadcode =" + roadCode.ToString().Trim(), "VTSGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_GPS_ID.ToString().Trim(), "VehicleID =" + x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_ID.ToString().Trim(), "VTSVehicleGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_VEHICLE_GPS_ID.ToString().Trim(), "GPS_INSTALLED="+ x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED.ToString().Trim() })}\");' ></span>",
                                Detete = x.GROUP_ITEMS.FirstOrDefault().IS_PDF_FINALIZED.Equals("Y") ? "<span class='ui-icon ui-icon-locked ui-align-center'> </span>" : $"<span class='ui-icon ui-icon-trash ui-align-center' title='Delete Details' onclick='DeleteGPSVTSDetails(\"{URLEncrypt.EncryptParameters1(new[] { "roadcode =" + roadCode.ToString().Trim(), "VTSGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_GPS_ID.ToString().Trim(), "VehicleID =" + x.GROUP_ITEMS.FirstOrDefault()?.VEHICLE_ID.ToString().Trim(), "VTSVehicleGPSID =" + x.GROUP_ITEMS.FirstOrDefault()?.VTS_VEHICLE_GPS_ID.ToString().Trim(), "GPS_INSTALLED=" + x.GROUP_ITEMS.FirstOrDefault()?.GPS_INSTALLED.ToString().Trim() })}\");' ></span>",
                                DateOfSubmission = x.GROUP_ITEMS.FirstOrDefault()?.DATE_OF_SUBMISSION?.ToString("dd/MM/yyyy hh:mm:ss")?? x.GROUP_ITEMS.FirstOrDefault()?.DATA_SUBMISSION_DATE.ToString("dd/MM/yyyy hh:mm:ss"),
                            })
                            .Select(x => new
                            {
                                cell = new object[]
                                {
                                    x.GPSInstalled,
                                    x.VehicleName,
                                    x.DateOfSubmission,
                                    x.NumberOfVehicles,
                                    x.DateOfInstallation,
                                    x.Edit,
                                    x.Detete,
                                    x.VTSGPSID,
                                    x.VehicleID,
                                    x.VTSVehicleGPSID,
                                }
                            })
                            .ToArray();
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GetGPSVTSSavedDetailsDAL()");
                throw;
            }

        }

        public GPSVTS_DetailsModel EditGPSVTSDetailsDAL(int roadCode, long VTSGPSID, long VehicleID)
        {
            try
            {
                GPSVTS_DetailsModel gPSVTS_DetailsModel = new GPSVTS_DetailsModel();
                using (var dbContext = new PMGSYEntities())
                {
                    if (dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Any(x => x.IMS_PR_ROAD_CODE == roadCode && x.VTS_GPS_ID == VTSGPSID))
                    {
                        var GPSVTSViewModel = FindRoadDetail(roadCode);
                        var SavedVehicleDetails = GetSavedVehicleDetails(roadCode, VTSGPSID, VehicleID);
                        gPSVTS_DetailsModel.Year = string.Concat(GPSVTSViewModel.IMS_YEAR.ToString(), "-", (GPSVTSViewModel.IMS_YEAR + 1).ToString());
                        gPSVTS_DetailsModel.Package = GPSVTSViewModel.IMS_PACKAGE_ID;
                        gPSVTS_DetailsModel.Batch = string.Concat("Batch-", GPSVTSViewModel.IMS_BATCH);
                        gPSVTS_DetailsModel.RoadCode = GPSVTSViewModel.IMS_PR_ROAD_CODE;
                        gPSVTS_DetailsModel.RoadName = GPSVTSViewModel.IMS_ROAD_NAME;
                        gPSVTS_DetailsModel.Is_GPSVTS_Installed = SavedVehicleDetails.Select(x => x.GPS_INSTALLED).FirstOrDefault();
                        gPSVTS_DetailsModel.VehiclesCount = SavedVehicleDetails.Select(x => x.NO_OF_VEHICLES).FirstOrDefault() ?? 0;
                        gPSVTS_DetailsModel.VehicleList = GetVEHICLEDetailsList();
                        gPSVTS_DetailsModel.VTS_InstallationDate = SavedVehicleDetails.Select(x => x.DATE_OF_INSTALLATION.HasValue ? x.DATE_OF_INSTALLATION.Value.ToString("dd/MM/yyyy") : "--").FirstOrDefault();
                        gPSVTS_DetailsModel.IsEditDetails = true;
                        gPSVTS_DetailsModel.VehiclesID = SavedVehicleDetails.Select(x => x.VTS_INSTRUMENT_GPS_ID).FirstOrDefault().Split(',');
                        gPSVTS_DetailsModel.Vehicle = SavedVehicleDetails.Select(x => x.Vehicle).FirstOrDefault() ?? 0;
                        gPSVTS_DetailsModel.Vehicle_Gps_Ids = (URLEncrypt.EncryptParameters1(new[] { "VTSVehicleGPSID =" + SavedVehicleDetails.Select(x => x.VTS_VEHICLE_GPS_IDS).FirstOrDefault(), "VTSGPSID=" + VTSGPSID.ToString().Trim(), "VehicleID=" + VehicleID.ToString().Trim() }));



                    }
                }
                return gPSVTS_DetailsModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.EditGPSVTSDetailsDAL()");
                throw;
            }
        }
        public List<SavedVehicleDetails> GetSavedVehicleDetails(int roadCode, long VTSGPSID, long VehicleID)
        {
            var dbContext = new PMGSYEntities();
            try
            {

                var Result = (from ava in dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                              where ava.IMS_PR_ROAD_CODE == roadCode && ava.VTS_GPS_ID == VTSGPSID
                              join cnt in dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT
                              on new { ID = (long?)ava.VTS_GPS_ID, UserId = (int?)ava.USERID } equals new { ID = (long?)cnt.VTS_GPS_ID, UserId = (int?)cnt.USERID }
                              into vehicleCountGroup
                              from cnt in vehicleCountGroup.DefaultIfEmpty() // Left join here
                              where cnt.VEHICLE_ID == VehicleID
                              join det in dbContext.VTS_VEHICLEWISE_GPS_DETAILS
                              on new { VID = (long?)(cnt != null ? cnt.VEHICLE_ID : (long?)null), VTS_GPS_Id = (long?)ava.VTS_GPS_ID, UserId = (int?)(cnt != null ? cnt.USERID : (int?)null) } equals new { VID = (long?)(det != null ? det.VEHICLE_ID : (long?)null), VTS_GPS_Id = (long?)(det != null ? det.VTS_GPS_ID : (long?)null), UserId = (int?)(det != null ? det.USERID : (int?)null) }
                              into detGroup
                              from det in detGroup.DefaultIfEmpty()
                                  //where det.VTS_VEHICLE_GPS_ID == VTSVehicleGPSID
                              let vehicleId = cnt != null ? cnt.VEHICLE_ID : (long?)null
                              let userId = cnt != null ? cnt.USERID : (int?)null
                              join vm in dbContext.VTS_VEHICLE_MASTER
                              on cnt != null ? cnt.VEHICLE_TYPE_ID : (int?)null equals vm != null ? vm.VEHICLE_ID : (int?)null
                              into vmGroup
                              from vm in vmGroup.DefaultIfEmpty()
                              where vm.VEHICLE_ID == cnt.VEHICLE_TYPE_ID
                              select new
                              {
                                  ava.GPS_INSTALLED,
                                  DATE_OF_INSTALLATION = cnt != null ? cnt.DATE_OF_INSTALLATION : (DateTime?)null,
                                  NO_OF_VEHICLES = cnt != null ? cnt.NO_OF_VEHICLES : (int?)null,
                                  VTS_INSTRUMENT_GPS_ID = det != null ? det.VTS_INSTRUMENT_GPS_ID : "--",
                                  VEHICLE_TYPE_ID = cnt != null ? cnt.VEHICLE_TYPE_ID : (int?)null,
                                  VehicleName = vm != null ? vm.VEHICLE_NAME : null,
                                  ava.VTS_GPS_ID,
                                  VEHICLE_ID = vehicleId,
                                  IMS_PR_ROAD_CODE = ava.IMS_PR_ROAD_CODE,
                                  VTS_VEHICLE_GPS_ID = det != null ? det.VTS_VEHICLE_GPS_ID : 0,
                              })
          .AsEnumerable()
          .GroupBy(x => x.VEHICLE_ID)
          .Select(group => new
          {
              VEHICLE_ID = group.Key,
              VTS_INSTRUMENT_GPS_IDS = group.Select(item => item.VTS_INSTRUMENT_GPS_ID).ToList(),
              GROUP_ITEMS = group.ToList(),
              VTS_VEHICLE_GPS_IDS = group.Select(i => i.VTS_VEHICLE_GPS_ID).ToList(),
          })
          .Select(x => new SavedVehicleDetails
          {
              IMS_PR_ROAD_CODE = x.GROUP_ITEMS.FirstOrDefault().IMS_PR_ROAD_CODE,
              GPS_INSTALLED = x.GROUP_ITEMS.FirstOrDefault().GPS_INSTALLED,
              NO_OF_VEHICLES = x.GROUP_ITEMS.FirstOrDefault().NO_OF_VEHICLES,
              DATE_OF_INSTALLATION = x.GROUP_ITEMS.FirstOrDefault().DATE_OF_INSTALLATION,
              VTS_INSTRUMENT_GPS_ID = x.GROUP_ITEMS.Any() ? string.Join(",", x.GROUP_ITEMS.Select(item => item.VTS_INSTRUMENT_GPS_ID ?? "--")) : "--",
              Vehicle = x.GROUP_ITEMS.FirstOrDefault().VEHICLE_TYPE_ID,
              VEHICLE_ID = x.VEHICLE_ID,
              VTS_VEHICLE_GPS_IDS = x.GROUP_ITEMS.Any() ? string.Join(",", x.GROUP_ITEMS.Select(item => item.VTS_VEHICLE_GPS_ID)) : "--",
          })
          .ToList();



                return Result;


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GetSavedVehicleDetails()");
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public bool UpdateGPSVTSDetailsDAL(int Road_Code, List<GPSVTSDataModel> VTS_INSTRUMENT_GPS_Details, long VehicleID, long VTSGPSID, string VTSVehicleGPSID)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        try
                        {
                            // Update VTS_ROADWISE_GPS_AVAILABILITY
                            var resultAvailability = dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                .FirstOrDefault(x => x.IMS_PR_ROAD_CODE == Road_Code && x.VTS_GPS_ID == VTSGPSID);

                            if (resultAvailability != null)
                            {
                                resultAvailability.DATA_SUBMISSION_DATE = DateTime.Now;
                                resultAvailability.USERID = PMGSYSession.Current.UserId;
                                resultAvailability.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            }

                            // Update VTS_ROADWISE_GPS_VEHICLE_COUNT
                            var resultCount = dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT
                                .FirstOrDefault(x => x.VTS_GPS_ID == VTSGPSID && x.VEHICLE_ID == VehicleID);

                            if (resultCount != null)
                            {
                                var gpsDataModel = VTS_INSTRUMENT_GPS_Details.FirstOrDefault();
                                if (gpsDataModel != null)
                                {
                                    resultCount.VEHICLE_TYPE_ID = Convert.ToInt32(gpsDataModel.Vehicle);
                                    resultCount.DATE_OF_INSTALLATION = Convert.ToDateTime(gpsDataModel.VTS_InstallationDate);
                                }
                                resultCount.DATE_OF_SUBMISSION = DateTime.Now;
                                resultCount.USERID = PMGSYSession.Current.UserId;
                                resultCount.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            }

                            // Update VTS_VEHICLEWISE_GPS_DETAILS
                            var vehiclesIDs = VTSVehicleGPSID.Split(',').Select(id => Convert.ToInt64(id)).ToList();
                            var resultDetails = dbContext.VTS_VEHICLEWISE_GPS_DETAILS
                                .Where(x => vehiclesIDs.Contains(x.VTS_VEHICLE_GPS_ID) && x.VEHICLE_ID == VehicleID).ToList();

                            int index = 0;
                            foreach (var item in resultDetails)
                            {
                                if (index < VTS_INSTRUMENT_GPS_Details[0].VehiclesID.Count)
                                {
                                    item.VTS_INSTRUMENT_GPS_ID = VTS_INSTRUMENT_GPS_Details[0].VehiclesID[index];
                                    item.DATE_OF_SUBMISSION = DateTime.Now;
                                    item.USERID = PMGSYSession.Current.UserId;
                                    item.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                    index++;
                                }
                            }

                            dbContext.SaveChanges();
                            transactionScope.Complete();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "GPSVTSDetailsDAL.UpdateGPSVTSDetailsDAL()");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeteteGPSVTSDetailsDAL(int Road_Code, long VehicleID, long VTSGPSID,string GPS_INSTALLED)
        {
            try
            {
                switch(GPS_INSTALLED)
                {
                    case "Y":
                        {
                            if (VTSGPSID != 0)
                            {
                                using (var dbContext = new PMGSYEntities())
                                {
                                    using (var transactionScope = new TransactionScope())
                                    {
                                        //Get the VEHICLE_IDs from VTS_VEHICLEWISE_GPS_DETAILS based on the given VEHICLE_ID
                                        var vehicleIdsToDelete = dbContext.VTS_VEHICLEWISE_GPS_DETAILS
                                            .Where(v => v.VEHICLE_ID == VehicleID && v.VTS_GPS_ID == VTSGPSID)
                                            .Select(v => v.VTS_VEHICLE_GPS_ID);

                                        // Delete records from VTS_VEHICLEWISE_GPS_DETAILS
                                        var vehicleWiseGpsDetailsToDelete = dbContext.VTS_VEHICLEWISE_GPS_DETAILS
                                            .Where(v => vehicleIdsToDelete.Contains(v.VTS_VEHICLE_GPS_ID));

                                        dbContext.VTS_VEHICLEWISE_GPS_DETAILS.RemoveRange(vehicleWiseGpsDetailsToDelete);

                                        // Delete the corresponding record from VTS_ROADWISE_GPS_VEHICLE_COUNT
                                        var recordToDelete = dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT
                                            .FirstOrDefault(vc => vc.VTS_GPS_ID == VTSGPSID && vc.VEHICLE_ID == VehicleID);

                                        if (recordToDelete != null)
                                        {
                                            dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT.Remove(recordToDelete);

                                            // Save changes to commit the deletions in VTS_ROADWISE_GPS_VEHICLE_COUNT
                                            dbContext.SaveChanges();

                                            // Check if there are any other records in VTS_ROADWISE_GPS_VEHICLE_COUNT with the same VTS_GPS_ID
                                            bool hasOtherReferences = dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT
                                                .Any(vc => vc.VTS_GPS_ID == VTSGPSID);

                                            // If there are no other references, then delete the corresponding record from VTS_ROADWISE_GPS_AVAILABILITY
                                            if (!hasOtherReferences)
                                            {
                                                var availabilityRecordToDelete = dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                                    .FirstOrDefault(av => av.VTS_GPS_ID == VTSGPSID);

                                                if (availabilityRecordToDelete != null)
                                                {
                                                    dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Remove(availabilityRecordToDelete);
                                                }
                                            }

                                            // Save changes to commit the deletions in VTS_ROADWISE_GPS_AVAILABILITY
                                            dbContext.SaveChanges();
                                        }

                                        // If everything was successful, complete the transaction
                                        transactionScope.Complete();
                                    }
                                }
                            }
                            break;
                        }
                    case "N":
                        {
                            using (var dbContext = new PMGSYEntities())
                            {
                                var RecordToDelete = dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                                    .FirstOrDefault(av => av.VTS_GPS_ID == VTSGPSID && av.IMS_PR_ROAD_CODE == Road_Code);
                                if (RecordToDelete != null)
                                {
                                    dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Remove(RecordToDelete);
                                }
                                dbContext.SaveChanges();
                            }

                                break;
                        }
                }
                return true;
            }
            catch (InvalidOperationException ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.DeteteGPSVTSDetailsDAL()_1");
            }
            catch (TransactionAbortedException ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.DeteteGPSVTSDetailsDAL()_2");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.DeteteGPSVTSDetailsDAL()");
            }

            // If an exception occurs or no VTS_GPS_ID was provided, return false to indicate failure
            return false;
        }

        public bool DetailsAlreadyPresent(int Road_Code)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    return  dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x => x.IMS_PR_ROAD_CODE == Road_Code && x.GPS_INSTALLED == "Y").Any();

                }

            }catch(Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.DetailsAlreadyPresent()");
                throw;
            }
        }

        //public string GPSInstrumentIDAlreadyExistsDAL(List<string> gpsInstrumentIDs,string VehiclesID)
        //Commented for allow vehicle for different road
        public string GPSInstrumentIDAlreadyExistsDAL(List<string> gpsInstrumentIDs, string VehiclesID,int RoadCode)
        {
            try
            {
                int VehiclesId = Convert.ToInt32(VehiclesID);
                using (var dbContext = new PMGSYEntities())
                {
                    var result = (from cou in dbContext.VTS_ROADWISE_GPS_VEHICLE_COUNT
                                    join det in dbContext.VTS_VEHICLEWISE_GPS_DETAILS
                                    on new { p1 = (long?)cou.VEHICLE_ID, p2 = cou.VTS_GPS_ID } equals new { p1 = det.VEHICLE_ID, p2 = det.VTS_GPS_ID }
                                    into joinedData
                                    from det in joinedData.DefaultIfEmpty()
                                    join ava in dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                    on cou.VTS_GPS_ID equals ava.VTS_GPS_ID 
                                    into joinedAvaCou
                                    from ava in joinedAvaCou.DefaultIfEmpty()
                                    where cou.VEHICLE_TYPE_ID == VehiclesId
                                    && gpsInstrumentIDs.Contains(det.VTS_INSTRUMENT_GPS_ID)
                                    && ava.IMS_PR_ROAD_CODE == RoadCode
                                  select new
                                    {
                                        det.VTS_INSTRUMENT_GPS_ID
                                    }).ToList();
                    if (result == null || result.Count == 0)
                    {

                        return "";
                    }

                    string resultString = string.Join(",", result.Select(x=>x.VTS_INSTRUMENT_GPS_ID));

                    return resultString;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GPSInstrumentIDAlreadyExistsDAL()");
                throw;
            }
            
        }

        public string ValidatePDFFileDAL(int FileSize, string FileExtension)
        {
            try
            {
                if (FileExtension.ToUpper() != ".PDF")
                {
                    return "File is not PDF File";
                }
                if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_MAX_SIZE"]))
                {
                    return "File Size Exceed the Maximum File Limit";
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.ValidatePDFFileDAL()");
                throw;
            }
            return string.Empty;
        }

        public int GetFileMaxCountDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                if (dbContext.VTS_GPS_FILES_DETAILS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return dbContext.VTS_GPS_FILES_DETAILS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count();
                }else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GetFileMaxCountDAL()");
                throw;
            }
        }
       
        public string SaveFileDetailsDAL(List<VTS_GPS_FILES_DETAILS> fileUploadViewModels)
        {
            try
            {
                using (dbContext = new PMGSYEntities())
                {
                    int maxID = dbContext.VTS_GPS_FILES_DETAILS.Any()
                        ? dbContext.VTS_GPS_FILES_DETAILS.Max(c => (Int32?)c.FILE_ID) ?? 0
                        : 0;

                    foreach (VTS_GPS_FILES_DETAILS fileModel in fileUploadViewModels)
                    {
                        ++maxID;
                        fileModel.FILE_ID = Convert.ToInt32(maxID);
                        fileModel.USERID = PMGSYSession.Current.UserId;
                        fileModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.VTS_GPS_FILES_DETAILS.Add(fileModel);
                    }

                    dbContext.SaveChanges();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.SaveFileDetailsDAL()");
                return ("An Error Occurred While Processing Your Request.");
            }
        }

        public Array GetPDFFilesListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                //List<IMS_PROPOSAL_FILES> listProposalFiles = PMGSYSession.Current.PMGSYScheme == 3 ? dbContext.IMS_PROPOSAL_FILES.Where(p => p.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && p.ISPF_TYPE == "P" && p.ISPF_UPLOAD_BY == "D").ToList() : dbContext.IMS_PROPOSAL_FILES.Where(p => p.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && p.ISPF_TYPE == "C" && p.ISPF_UPLOAD_BY == "D").ToList();
                List<VTS_GPS_FILES_DETAILS> vTS_GPS_FILES_DETAILs = dbContext.VTS_GPS_FILES_DETAILS.Where(p => p.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && p.FILE_TYPE == "P").ToList();
                IQueryable<VTS_GPS_FILES_DETAILS> query = vTS_GPS_FILES_DETAILs.AsQueryable<VTS_GPS_FILES_DETAILS>();

                var IsVTS_PDF_finalized = false;
                IsVTS_PDF_finalized = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(s => s.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && s.IS_PDF_FINALIZED.Equals("Y")).Any();

                totalRecords = vTS_GPS_FILES_DETAILs.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }


                
                return query.Select(fileDetails => new
                {
                    id = fileDetails.FILE_ID + "$" + fileDetails.IMS_PR_ROAD_CODE,
                    cell = new[] {
                                    URLEncrypt.EncryptParameters(new string[] { fileDetails.FILE_NAME.ToString()  }),
                                    fileDetails.FILE_DESC,

                                    // Edit Button
                                    ( IsVTS_PDF_finalized == false) 
                                        ? "<a href='#' title='Click here to Edit the File Details' class='ui-icon ui-icon-pencil ui-align-center' onClick=EditPDFDetails('" +  fileDetails.FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() +"'); return false;>Edit</a>" 
                                        : "<span class='ui-icon ui-icon-locked ui-align-center'> </span>",

                                    // Delete Button
                                    ( IsVTS_PDF_finalized == false) ? ("<a href='#' title='Click here to delete the File Details' class='ui-icon ui-icon-trash ui-align-center'" +
                                                                        " onClick=\"DeletePDFFileDetails('" + URLEncrypt.EncryptParameters1(new[] {
                                                                            "roadcode=" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim(),
                                                                            "FILE_ID=" + fileDetails.FILE_ID.ToString().Trim(),
                                                                            "FILE_NAME=" + fileDetails.FILE_NAME.ToString().Trim()
                                                                        }) + "'); return false;\">Delete</a>")
                                                                    : "<span class='ui-icon ui-icon-locked ui-align-center'> </span>",


                                   "<center><table><tr><td style='border-color:white'><a href='#' style='float:left' id='btnSave" +  fileDetails.FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"' title='Click here to Save the File Details' class='ui-icon ui-icon-disk ui-align-center' onClick=SavePDFDetails('" +  fileDetails.FILE_ID.ToString().Trim() + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "');></a><a href='#' style='float:right' id='btnCancel" +  fileDetails.FILE_ID.ToString().Trim()  + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"' title='Click here to Cancel the File Edit' class='ui-icon ui-icon-closethick ui-align-center' onClick= CancelSavePDFDetails('" +  fileDetails.FILE_ID.ToString().Trim() + "$" + fileDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "');></a></td></tr></table></center>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GetPDFFilesListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public string UpdatePDFDetailsDAL(VTS_GPS_FILES_DETAILS vTS_GPS_FILES_DETAILS)
        {
            try
            {
                using (dbContext = new PMGSYEntities())
                {
                    var existingFile = dbContext.VTS_GPS_FILES_DETAILS.SingleOrDefault(
                        a => a.FILE_ID == vTS_GPS_FILES_DETAILS.FILE_ID &&
                        a.IMS_PR_ROAD_CODE == vTS_GPS_FILES_DETAILS.IMS_PR_ROAD_CODE &&
                        a.FILE_TYPE == "P");

                    if (existingFile != null)
                    {
                        existingFile.FILE_DESC = vTS_GPS_FILES_DETAILS.FILE_DESC;
                        existingFile.USERID = PMGSYSession.Current.UserId;
                        existingFile.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        existingFile.FILE_UPLOAD_DATE = DateTime.Now;
                        dbContext.SaveChanges();
                        return string.Empty;
                    }
                    else
                    {
                        return ("File not found or doesn't match the criteria.");
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.UpdatePDFDetailsDAL()_DbEntityValidationException");
                return ("Validation Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.UpdatePDFDetailsDAL()");
                return ("An Error Occurred While Processing Your Request.");
            }
        }

        public string DeleteFileDetailsDAL(long FILE_ID, int IMS_PR_ROAD_CODE, string FILE_NAME)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var fileDetails = dbContext.VTS_GPS_FILES_DETAILS.FirstOrDefault(
                        a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                        a.FILE_ID == FILE_ID &&
                        a.FILE_NAME == FILE_NAME);

                    if (fileDetails != null)
                    {
                        fileDetails.USERID = PMGSYSession.Current.UserId;
                        fileDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(fileDetails).State = EntityState.Modified;
                        dbContext.VTS_GPS_FILES_DETAILS.Remove(fileDetails);
                        dbContext.SaveChanges();
                    }else
                    {
                        return ("File details not found.");
                    }

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.DeleteFileDetailsDAL()");
                return "An error occurred while processing the request.";
            }
        }
        //
        public string FinalizeDetailsDAL (int ROADCODE)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var Details = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.FirstOrDefault(
                        a => a.IMS_PR_ROAD_CODE == ROADCODE);

                    if (Details != null)
                    {
                        Details.IS_PDF_FINALIZED = "Y";
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        return ("Details not found.");
                    }

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.FinalizeDetailsDAL()");
                return "An error occurred while processing the request.";
            }
        }
        //

        public Array GPSVTSUnfreezeWorkDetailsRoadListDAL(string WorkStatus, int state, int district, int block, int sanction_year, int batch, string proposalType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {

                DateTime VTS_ENTRY_LASTDATE = Convert.ToDateTime(ConfigurationManager.AppSettings["VTS_ENTRY_LASTDATE"].ToString());
                
                var resultList = new List<UnfreezeWorkDetailsRoadListmodel>();

                var PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
                var RoadList = (from isp in dbContext.IMS_SANCTIONED_PROJECTS
                                    // join er in dbContext.EXEC_ROADS_MONTHLY_STATUS
                                    //    on isp.IMS_PR_ROAD_CODE equals er.IMS_PR_ROAD_CODE
                                join vts in dbContext.VTS_ROADWISE_GPS_AVAILABILITY
                                    on isp.IMS_PR_ROAD_CODE equals vts.IMS_PR_ROAD_CODE into vtsGroup
                                from vts in vtsGroup.DefaultIfEmpty()
                                join ms in dbContext.MASTER_STATE
                                    on isp.MAST_STATE_CODE equals ms.MAST_STATE_CODE into msGroup
                                from ms in msGroup.DefaultIfEmpty()
                                join md in dbContext.MASTER_DISTRICT
                                    on isp.MAST_DISTRICT_CODE equals md.MAST_DISTRICT_CODE into mdGroup
                                from md in mdGroup.DefaultIfEmpty()
                                join mb in dbContext.MASTER_BLOCK
                                    on isp.MAST_BLOCK_CODE equals mb.MAST_BLOCK_CODE into mbGroup
                                from mb in mbGroup.DefaultIfEmpty()
                                    //
                                join vuw in dbContext.VTS_UNFREEZE_WORKS
                                    on isp.IMS_PR_ROAD_CODE equals vuw.IMS_PR_ROAD_CODE into vuwGroup
                                from vuw in vuwGroup.DefaultIfEmpty()
                                    //
                                where (state == 0 || isp.MAST_STATE_CODE == state)
                                    && (district == 0 || isp.MAST_DISTRICT_CODE == district)
                                    && (block == 0 || isp.MAST_BLOCK_CODE == block)
                                    && (sanction_year == 0 || isp.IMS_YEAR == sanction_year)
                                    && (batch == 0 || isp.IMS_BATCH == batch)
                                    && isp.MAST_PMGSY_SCHEME == PMGSYScheme
                                    && isp.IMS_PROPOSAL_TYPE == "P"
                                     //&& er.EXEC_ISCOMPLETED == "P"
                                     //&& (WorkStatus != "A" ? (WorkStatus == "F" ? ((vts == null && vuw == null) || (vts != null && vts.DATA_SUBMISSION_DATE > VTS_ENTRY_LASTDATE && vuw == null)) : vuw != null) : true)
                                     && (WorkStatus != "A" ? (WorkStatus == "F" ? ((vts == null && vuw == null) || (vts != null  && vuw == null)) : vuw != null) : true)
                                    && isp.IMS_SANCTIONED == "Y"
									//&& (isp.IMS_ISCOMPLETED != "C" && isp.IMS_ISCOMPLETED != "X")
                                //&& er.EXEC_PROG_YEAR == dbContext.EXEC_ROADS_MONTHLY_STATUS
                                //                           .Where(e => e.IMS_PR_ROAD_CODE == isp.IMS_PR_ROAD_CODE)
                                //                           .Max(e => e.EXEC_PROG_YEAR)
                                // && er.EXEC_PROG_MONTH == dbContext.EXEC_ROADS_MONTHLY_STATUS
                                //                           .Where(e => e.IMS_PR_ROAD_CODE == isp.IMS_PR_ROAD_CODE && e.EXEC_PROG_YEAR == er.EXEC_PROG_YEAR)
                                //                            .Max(e => e.EXEC_PROG_MONTH)

                                select new
                                {
                                    ms.MAST_STATE_NAME,
                                    md.MAST_DISTRICT_NAME,
                                    mb.MAST_BLOCK_NAME,
                                    TYPE = isp.IMS_PROPOSAL_TYPE == "P" ? "Road" : "Bridge",
                                    isp.IMS_PR_ROAD_CODE,
                                    isp.IMS_ROAD_NAME,
                                    isp.IMS_PACKAGE_ID,
                                    isp.IMS_YEAR,
                                    isp.IMS_BATCH,
                                    isp.IMS_PAV_LENGTH,
                                    isp.IMS_SANCTIONED_BS_AMT,
                                    isp.MAST_PMGSY_SCHEME,
                                    WORK_STATUS = vuw != null ? "Not freezed" : "Freezed",
                                    GPS_INSTALLED = vts.GPS_INSTALLED == "Y" ? "Yes" : vts.GPS_INSTALLED == "N" ? "No" : "No",
                                    //
                                    isFinalized = vts.IS_PDF_FINALIZED == null ? "No" : vts.IS_PDF_FINALIZED == "Y" ? "Yes" : "No"

                                }).OrderBy(v => v.MAST_STATE_NAME).ThenBy(v => v.MAST_DISTRICT_NAME).ThenBy(v => v.MAST_BLOCK_NAME).Distinct().ToList();


                totalRecords = RoadList.Count();

                foreach (var item in RoadList)
                {
                    resultList.Add(new UnfreezeWorkDetailsRoadListmodel()
                    {
                        StateName = item.MAST_STATE_NAME,
                        DistrictName = item.MAST_DISTRICT_NAME,
                        BlockName = item.MAST_BLOCK_NAME,
                        PackageId = item.IMS_PACKAGE_ID,
                        Year = (item.IMS_YEAR).ToString() + "-" + (item.IMS_YEAR + 1).ToString().Substring(2, 2),
                        Batch = string.Concat("Batch-", item.IMS_BATCH.ToString()),
                        Length = item.IMS_PAV_LENGTH,
                        SanctionedAmt = item.IMS_SANCTIONED_BS_AMT,
                        WorkStatus = item.WORK_STATUS,
                        RoadCode = item.IMS_PR_ROAD_CODE,
                        RoadName = item.IMS_ROAD_NAME,
                        //
                        isFinalized = item.isFinalized,
                        //
                        isGPSINSTALLED = item.GPS_INSTALLED
                    });
                }

                return resultList.Select(RoadDetails => new
                {
                    cell = new[] {
                        RoadDetails.StateName,
                        RoadDetails.DistrictName,
                        RoadDetails.BlockName,
                        RoadDetails.PackageId,
                        RoadDetails.Year,
                        RoadDetails.Batch.ToString(),
                        RoadDetails.Length.ToString(),
                        RoadDetails.SanctionedAmt.ToString(),
                        RoadDetails.RoadName,
                        RoadDetails.WorkStatus,
                        RoadDetails.isGPSINSTALLED,
                        RoadDetails.isFinalized,
                        RoadDetails.WorkStatus == "Freezed"?"<span class='ui-icon ui-icon-circle-plus ui-align-center' title='UnFreeze Work' onclick='UnFreezeWork(\"" + URLEncrypt.EncryptParameters1(new string[] { "roadcode =" + RoadDetails.RoadCode.ToString().Trim()}) + "\");' > </span>"
                                                  :"<span class='ui-icon ui-icon-locked ui-align-center' title='Work Already UnFreezed'></span>"

                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.GPSVTSUnfreezeWorkDetailsRoadListDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public string UnFreezeWorkDetailsDAL(int ROADCODE)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    int VTS_UID = dbContext.VTS_UNFREEZE_WORKS.Any() ? (dbContext.VTS_UNFREEZE_WORKS.Select(x => x.VTS_UID)).Max() : 0;

                    var vTS_UNFREEZE_WORKS = new VTS_UNFREEZE_WORKS
                    {
                       
                        VTS_UID = VTS_UID + 1,
                        IMS_PR_ROAD_CODE = ROADCODE,
                        IS_UNFREEZED = "Y",
                        UNFREEZED_BY = PMGSYSession.Current.UserId,
                        UNFREEZED_DATE = DateTime.Now,
                        USER_ID = PMGSYSession.Current.UserId,
                        IP_ADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                    };

                    dbContext.VTS_UNFREEZE_WORKS.Add(vTS_UNFREEZE_WORKS);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsDAL.UnFreezeWorkDetailsDAL()");
                return "An error occurred while processing the request.";
            }
        }
    }
}