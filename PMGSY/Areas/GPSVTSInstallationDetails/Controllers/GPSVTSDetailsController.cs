using AttributeRouting.Web.Mvc;
using PMGSY.Areas.GPSVTSInstallationDetails.DAL;
using PMGSY.Areas.GPSVTSInstallationDetails.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.Areas.GPSVTSInstallationDetails.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
  
    public class GPSVTSDetailsController : Controller
    {
        Dictionary<string, string> decryptedParameters = null;
        GPSVTSDetailsDAL objDAL = null;

        [HttpGet]
        public ActionResult GPSVTSLayout()
        {
            GPSVTS_FiltersModel model = new GPSVTS_FiltersModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                model.DistrictList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.DistrictList = comm.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode);
                }
                else if (PMGSYSession.Current.DistrictCode > 0)
                {

                    model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                }

                model.BlockList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    model.BlockList = comm.PopulateBlocks((PMGSYSession.Current.DistrictCode), true);

                    model.BlockList.Find(x => x.Value == "-1").Value = "0";
                    model.BlockList.Find(x => x.Value == model.BlockCode.ToString()).Selected = true;
                }
                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();

                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                model.ProposalType = "P";
                model.ProposalTypeList = new List<SelectListItem>();
                model.ProposalTypeList.Insert(0, new SelectListItem { Value = "P", Text = "Road" });
               // model.ProposalTypeList.Insert(1, new SelectListItem { Value = "L", Text = "Bridge" });

                model.WorkStatus = "A";
                model.WorkStatusList = new List<SelectListItem>();
                model.WorkStatusList.Insert(0, new SelectListItem { Value = "A", Text = "All" });
                model.WorkStatusList.Insert(1, new SelectListItem { Value = "F", Text = "Freeze" });
                model.WorkStatusList.Insert(2, new SelectListItem { Value = "U", Text = "Not freezed" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.GPSVTSLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GPSVTSRoadList(int? page, int? rows, string sidx, string sord)
        {
            objDAL = new GPSVTSDetailsDAL();
            long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0;
            string proposalType = string.Empty;
            //
            string WorkStatus = "";
            //

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["block"]))
                {
                    block = Convert.ToInt32(Request.Params["block"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
                {
                    sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["proposaltype"]))
                {
                    proposalType = Request.Params["proposaltype"];

                }
                //
                if(!string.IsNullOrEmpty(Request.Params["WorkStatus"]))
                {
                    WorkStatus = Request.Params["WorkStatus"];
                }
                //
                var jsonData = new
                {
                    //rows = objDAL.GPSVTSRoadListDAL(state, district, block, sanction_year, batch, proposalType, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    rows = objDAL.GPSVTSRoadListDAL(WorkStatus,state, district, block, sanction_year, batch, proposalType, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.GPSVTSRoadList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddGPSVTS_DetailsView(String parameter, String hash, String key)
        {
            try
            {
                int RoadCode = 0;
                objDAL = new GPSVTSDetailsDAL();
                GPSVTS_DetailsModel objModel = new GPSVTS_DetailsModel();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    RoadCode = Convert.ToInt32(decryptedParameters["roadcode"].ToString());
                    var TempModel = objDAL.FindRoadDetail(RoadCode);

                    objModel.Year = string.Concat(TempModel.IMS_YEAR.ToString(), "-", (TempModel.IMS_YEAR + 1).ToString());
                    objModel.Package = TempModel.IMS_PACKAGE_ID;
                    objModel.Batch = string.Concat("Batch-", TempModel.IMS_BATCH);
                    objModel.RoadCode = TempModel.IMS_PR_ROAD_CODE;
                    objModel.RoadName = TempModel.IMS_ROAD_NAME;
                    objModel.Is_GPSVTS_Installed = "N";
                    objModel.Vehicle = 0;
                    objModel.VehicleList = objDAL.GetVEHICLEDetailsList();
                    objModel.VTS_InstallationDate = "";
                    objModel.IsEditDetails = false;
                    //
                    objModel.IsDetailsAlreadyPresent = objDAL.DetailsAlreadyPresent(RoadCode);
                    if(objModel.IsDetailsAlreadyPresent)
                    {
                        objModel.Is_GPSVTS_Installed = "Y";
                    }
                    //
                    return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/AddGPSVTS_DetailsView.cshtml", objModel);
                }
                return null;
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "GPSVTSDetailsController.AddGPSVTS_DetailsView()");
                return null;
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SaveGPSVTSDetails(GPSVTSDetailsDataModel gPSVTSDetailsDataModel)
        {
            try
            {
                List<GPSVTSDataModel> VTS_INSTRUMENT_GPS_Details = gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details;
                string roadCodeValue = gPSVTSDetailsDataModel.RoadCodeValue;
                string is_GPSVTS_Installed = gPSVTSDetailsDataModel.Is_GPSVTS_Installed;

                objDAL = new GPSVTSDetailsDAL();

                switch (is_GPSVTS_Installed)
                {
                    case "Y":
                        string errormsg;
                        if (!ValidateGPSVTSDetails(VTS_INSTRUMENT_GPS_Details, out errormsg))
                        {
                            return Json(new { success = false, message = errormsg });
                        }
                        break;
                    case "N":
                        break;
                    default:
                        return Json(new { success = false, message = "Invalid GPS/VTS Installed." });
                }

                int road_Code;
                if (!int.TryParse(roadCodeValue, out road_Code))
                {
                    return Json(new { success = false, message = "Invalid roadCode." });
                }

                bool SaveStatus = objDAL.SaveGPSVTSDetails(is_GPSVTS_Installed, road_Code, VTS_INSTRUMENT_GPS_Details);

                if (SaveStatus)
                {
                    //return Json(new { success = true, message = "Data saved successfully." });
                    return Json(new { success = true, message = "Data saved successfully.", encCode = (URLEncrypt.EncryptParameters1(new[] { "roadcode =" + roadCodeValue.ToString().Trim() })) });
                }

                return Json(new { success = false, message = "An error occurred while saving the data." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.SaveGPSVTSDetails()");
                return Json(new { success = false, message = "An error occurred while saving the data." });
            }
        }

        public bool ValidateGPSVTSDetails(List<GPSVTSDataModel> VTS_INSTRUMENT_GPS_Details, out string Errormsg)
        {
            try
            {
                Errormsg = "";


                foreach (var item in VTS_INSTRUMENT_GPS_Details)
                {

                    if (string.IsNullOrEmpty(item.Vehicle) || item.Vehicle == "0")
                    {
                        Errormsg = "Please select a vehicle.";
                        return false;
                    }


                    if (!int.TryParse(item.VehiclesCount, out int count) || count <= 0)
                    {
                        Errormsg = "Please enter a valid number of vehicles.";
                        return false;
                    }


                    if (string.IsNullOrEmpty(item.VTS_InstallationDate))
                    {
                        Errormsg = "Please enter a valid date of installation.";
                        return false;
                    }


                    if (item.VehiclesID == null || item.VehiclesID.Any(string.IsNullOrEmpty))
                    {
                        Errormsg = "Please enter a GPS Instrument ID.";
                        return false;
                    }
                }


                return true;
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "GPSVTSDetailsController.ValidateGPSVTSDetails()");
                Errormsg = "An error occurred during validation.";
                return false;
            }
        }

        [HttpPost]
        public ActionResult GetGPSVTSSavedDetails(String parameter, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int Road_Code = 0;
                long totalRecords = 0;
                objDAL = new GPSVTSDetailsDAL();

                if (!string.IsNullOrEmpty(parameter))
                {

                    string[] splitValues = parameter.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { splitValues[0], splitValues[1], splitValues[2] });
                    if (decryptedParameters.Count() > 0)
                    {
                        Road_Code = Convert.ToInt32(decryptedParameters["roadcode"].ToString());
                        //
                    }
                }else
                {

                }
               
                var jsonData = new
                {
                    rows = objDAL.GetGPSVTSSavedDetailsDAL(Road_Code, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.GetGPSVTSSavedDetails()");
                return null;
            }

        }



        [HttpPost]
        public ActionResult EditGPSVTSDetails(String parameter)
        {
            GPSVTS_DetailsModel model = new GPSVTS_DetailsModel();
            int RoadCode = 0;
            long VTSGPSID = 0;
            long VehicleID = 0;
           

            objDAL = new GPSVTSDetailsDAL();
            try
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string[] splitValues = parameter.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { splitValues[0], splitValues[1], splitValues[2] });
                    if (decryptedParameters != null)
                    {
                        RoadCode = Convert.ToInt32(decryptedParameters["roadcode"]);
                        VTSGPSID = Convert.ToInt64(decryptedParameters["VTSGPSID"]);
                        VehicleID = Convert.ToInt64(decryptedParameters["VehicleID"]);
                        //VTSVehicleGPSID = Convert.ToInt64(decryptedParameters["VTSVehicleGPSID"]);
                    }

                    model = objDAL.EditGPSVTSDetailsDAL(RoadCode, VTSGPSID, VehicleID);
                    return PartialView("AddGPSVTS_DetailsView", model); 
                }else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
               
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.EditGPSVTSDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }
        [HttpPost]
        public ActionResult DeteteGPSVTSDetails(String parameter)
        {
            int RoadCode = 0;
            long VTSGPSID = 0;
            long VehicleID = 0;
            //
            string GPS_INSTALLED = "";
            //

            objDAL = new GPSVTSDetailsDAL();
            try
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string[] splitValues = parameter.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { splitValues[0], splitValues[1], splitValues[2] });
                    if (decryptedParameters != null)
                    {
                        RoadCode = Convert.ToInt32(decryptedParameters["roadcode"]);
                        VTSGPSID = Convert.ToInt64(decryptedParameters["VTSGPSID"]);
                        VehicleID = Convert.ToInt64(decryptedParameters["VehicleID"] != ""? decryptedParameters["VehicleID"] :"0" );
                        GPS_INSTALLED = Convert.ToString(decryptedParameters["GPS_INSTALLED"]);
                        bool Is_PDF_FileAvailable = objDAL.CheckVTS_PDFAvailable(RoadCode);
                        if (Is_PDF_FileAvailable)
                        {
                            return Json(new { success = false, message = "Please delete VTS uploaded PDF Files before delete VTS details..!! " });
                        }
                        else
                        {
                            bool DeleteStatus = objDAL.DeteteGPSVTSDetailsDAL(RoadCode, VehicleID, VTSGPSID, GPS_INSTALLED);
                            //bool DeleteStatus = false;
                            if (DeleteStatus)
                            {
                                return Json(new { success = true, message = "Data Deleted successfully." });
                            }
                        }


                    } 
                }
                return Json(new { success = false, message = "An error occurred while deleting the data." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.DeteteGPSVTSDetails()");
                return Json(new { success = false, message = "An error occurred while deleting the data." });
            }
        }

        [HttpPost]
        public ActionResult UpdateGPSVTSDetails (GPSVTSDetailsDataModel gPSVTSDetailsDataModel)
        {
            try
            {
                List<GPSVTSDataModel> VTS_INSTRUMENT_GPS_Details = gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details;
                string roadCodeValue = gPSVTSDetailsDataModel.RoadCodeValue;
                //string is_GPSVTS_Installed = gPSVTSDetailsDataModel.Is_GPSVTS_Installed;
                string VTSVehicleGPSID = "";
                long VTSGPSID = 0;
                long VehicleID = 0;

                //
                string[] splitValues = gPSVTSDetailsDataModel.Vehicle_Gps_Ids.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { splitValues[0], splitValues[1], splitValues[2] });
                if (decryptedParameters != null)
                {
                    VTSVehicleGPSID = decryptedParameters["VTSVehicleGPSID"];
                    VTSGPSID = Convert.ToInt64(decryptedParameters["VTSGPSID"]);
                    VehicleID = Convert.ToInt64(decryptedParameters["VehicleID"]);
                }
                //
                objDAL = new GPSVTSDetailsDAL();

                        string errormsg;
                        if (!ValidateGPSVTSDetails(VTS_INSTRUMENT_GPS_Details, out errormsg))
                        {
                            return Json(new { success = false, message = errormsg });
                        }
                

                int road_Code;
                if (!int.TryParse(roadCodeValue, out road_Code))
                {
                    return Json(new { success = false, message = "Invalid roadCode." });
                }

                bool UpdateStatus = objDAL.UpdateGPSVTSDetailsDAL(road_Code, VTS_INSTRUMENT_GPS_Details, VehicleID, VTSGPSID , VTSVehicleGPSID);

                if (UpdateStatus)
                {
                    return Json(new { success = true, message = "Data Updated successfully." , encCode = (URLEncrypt.EncryptParameters1(new[] { "roadcode =" + road_Code.ToString().Trim() })) });
                }

                return Json(new { success = false, message = "An error occurred while updating the data." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.UpdateGPSVTSDetails()");
                return Json(new { success = false, message = "An error occurred while updating the data." });
            }
        }

        [HttpPost]


        public ActionResult GPSInstrumentIDAlreadyExists(GPSVTSDetailsDataModel gPSVTSDetailsDataModel)
        {
            objDAL = new GPSVTSDetailsDAL();
            try
            {
                if (gPSVTSDetailsDataModel != null)
                {
                    // Check if any VehiclesID inside GPSVTSDataModel is null
                    if (gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details != null && gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details.All(gpsVTSData => gpsVTSData.VehiclesID != null))
                    {
                        List<string> gpsInstrumentIDs = gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details
                                                       .SelectMany(gpsVTSData => gpsVTSData.VehiclesID)
                                                       .ToList();
                        //string GPSInstrumentIDAlreadyExist = objDAL.GPSInstrumentIDAlreadyExistsDAL(gpsInstrumentIDs, gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details.Select(s => s.Vehicle).FirstOrDefault());
                        //Commented for allow vehicle for different road
                        string GPSInstrumentIDAlreadyExist = objDAL.GPSInstrumentIDAlreadyExistsDAL(gpsInstrumentIDs, gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details.Select(s => s.Vehicle).FirstOrDefault(),Convert.ToInt32(gPSVTSDetailsDataModel.RoadCodeValue));
                        string ResMessage = "";
                        if (!string.IsNullOrEmpty(GPSInstrumentIDAlreadyExist))
                        {
                            ResMessage = "GPS Instrument ID(s) [" + GPSInstrumentIDAlreadyExist + "] is already present with the corresponding vehicle.";
                            return Json(new { success = true, message = ResMessage });
                        }
                        //Added By Tushar on 11 Sep 2023 for check GPS Instrument ID present in same work
                        string GPSInstrumentIDAlreadyExistForSameWork = objDAL.GPSInstrumentIDAlreadyExistsForSameWorkDAL(gpsInstrumentIDs, Convert.ToInt32(gPSVTSDetailsDataModel.RoadCodeValue));
                     
                        if (!string.IsNullOrEmpty(GPSInstrumentIDAlreadyExistForSameWork))
                        {
                            ResMessage = "GPS Instrument ID(s) [" + GPSInstrumentIDAlreadyExistForSameWork + "] is already present with the corresponding vehicle for same work.";
                            return Json(new { success = true, message = ResMessage });
                        }
                        //Added By Tushar on 11 Sep 2023 for check same GPS Instrument ID and same corresponding vehicle present for multiple works
                        int vehicle = Convert.ToInt32(gPSVTSDetailsDataModel.VTS_INSTRUMENT_GPS_Details.Select(c => c.Vehicle).FirstOrDefault());
                        string SameGPSInstrumentIDAlreadyExistForWork = objDAL.IsSameGPSInstrumentIDAlreadyExistsForWorkDAL(gpsInstrumentIDs, Convert.ToInt32(gPSVTSDetailsDataModel.RoadCodeValue), vehicle);
                        
                        if (!string.IsNullOrEmpty(SameGPSInstrumentIDAlreadyExistForWork))
                        {
                            ResMessage = "GPS Instrument ID(s) [" + SameGPSInstrumentIDAlreadyExistForWork + "] is already assigned to another vehicle.";
                            return Json(new { success = true, message = ResMessage });
                        }
                        //End By Tushar on 11 Sep 2023
                        return Json(new { success = true, message = ResMessage });
                    }
                    else
                    {
                        return Json(new { success = true, message = "" });
                    }
                } 
                else
                {
                    return Json(new { success = true, message = "An error occurred while getting the data." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.GPSInstrumentIDAlreadyExists()");
                return Json(new { success = false, message = "An error occurred while getting the data." });
            }
        }
        /// <summary>
        /// Upload File
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FileUpload(String parameter, String hash, String key)
        {
            try
            {
                int RoadCode = 0;
                string isFinalized = string.Empty;
                objDAL = new GPSVTSDetailsDAL();
                GPSVTS_DetailsModel gPSVTS_DetailsModel = new GPSVTS_DetailsModel();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    RoadCode = Convert.ToInt32(decryptedParameters["roadcode"].ToString());
                    isFinalized = decryptedParameters["isFinalized"].ToString();
                    var RoadDetails = objDAL.FindRoadDetail(RoadCode);

                    gPSVTS_DetailsModel.Year = string.Concat(RoadDetails.IMS_YEAR.ToString(), "-", (RoadDetails.IMS_YEAR + 1).ToString());
                    gPSVTS_DetailsModel.Package = RoadDetails.IMS_PACKAGE_ID;
                    gPSVTS_DetailsModel.Batch = string.Concat("Batch-", RoadDetails.IMS_BATCH);
                    gPSVTS_DetailsModel.RoadCode = RoadDetails.IMS_PR_ROAD_CODE;
                    gPSVTS_DetailsModel.RoadName = RoadDetails.IMS_ROAD_NAME;
                    gPSVTS_DetailsModel.isFinalized = isFinalized;
                }
                return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/FileUpload.cshtml", gPSVTS_DetailsModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.FileUpload()");
                throw;
            }

        }
        /// <summary>
        /// Pdf File Upload
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        // public ActionResult PdfFileUpload(string id)
        public ActionResult PdfFileUpload(string roadCode, string isFinalized)
        {
            try
            {
                FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
                fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(roadCode);
                fileUploadViewModel.isFinalized = isFinalized;
                fileUploadViewModel.ErrorMessage = string.Empty;
                return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/PdfFileUpload.cshtml", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.PdfFileUpload(string id)");
                throw;
            }
        }

        [HttpPost]
        public ActionResult PdfFileUpload(FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                objDAL = new GPSVTSDetailsDAL();

                 
                if (!Directory.Exists(ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"].ToString()))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"].ToString());
                }

                if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"], Request)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/PdfFileUpload.cshtml", fileUploadViewModel);
                    //return View("PdfFileUpload", fileUploadViewModel);

                }


                foreach (string file in Request.Files)
                {
                    string status = objDAL.ValidatePDFFileDAL(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/PdfFileUpload.cshtml", fileUploadViewModel);
                    }
                }

                var fileData = new List<FileUploadViewModel>();

                int IMS_PR_ROAD_CODE = 0;
                if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
                {
                    IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
                }
                else
                {
                    try
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                    }
                    catch
                    {
                        if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                        {
                            IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                        }
                    }
                }
                foreach (string file in Request.Files)
                {
                    UploadPDFFile(Request, fileData, IMS_PR_ROAD_CODE);
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.PdfFileUpload()");
                throw;
            }
        }

        public void UploadPDFFile(HttpRequestBase request, List<FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            try
            {
                objDAL = new GPSVTSDetailsDAL();
                string StorageRoot = ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"];

                int maxCount = objDAL.GetFileMaxCountDAL(IMS_PR_ROAD_CODE) + 1;
                int RandomNum = new Random().Next(10000);
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    var fileId = IMS_PR_ROAD_CODE;

                    string fileExtension = Path.GetExtension(file.FileName);
                    var fileName = $"{IMS_PR_ROAD_CODE}-{maxCount}-{RandomNum}{fileExtension}";
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    List<VTS_GPS_FILES_DETAILS> vtsFilesDetails = new List<VTS_GPS_FILES_DETAILS>
                        {
                            new VTS_GPS_FILES_DETAILS
                            {
                                FILE_TYPE = "P",
                                FILE_NAME = fileName,
                                FILE_DESC = request.Params["PdfDescription[]"],
                                FILE_UPLOAD_DATE = DateTime.Now,
                                IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE
                            }
                        };

                    string status = objDAL.SaveFileDetailsDAL(vtsFilesDetails);

                    if (status == string.Empty)
                    {
                        file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"], fileName));
                    }
                    else
                    {
                        // Handle error
                    }

                    maxCount++;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.UploadPDFFile()");
                throw;
            }
        }

        public ActionResult DownloadFile(String parameter, String hash, String key)
        {
            try
            {
                objDAL = new GPSVTSDetailsDAL();
                string FileName = string.Empty;
                string FullFileLogicalPath = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);

                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf")
                {
                    //
                    string VirtualDirectoryPath = ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"].ToString();
                    string VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath.Replace(@"\\", @"//").Replace(@"\", @"/"), FileName);

                    string physicalFullPath = Path.Combine(ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"], FileName);

                    FileInfo file = new FileInfo(physicalFullPath);
                    if (file.Exists)
                    {
                        string ext = Path.GetExtension(FileName);

                        string type = string.Empty;

                        if (ext != null)
                        {
                            switch (ext.ToLower())
                            {
                                case ".pdf":
                                    type = "Application/pdf";
                                    break;
                                case ".doc":
                                case ".docx":
                                    type = "Application/msword";
                                    break;

                                default:
                                    type = "Application";
                                    break;
                            }
                        }
                         //return File(VirtualDirectoryfullPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                        return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                    }
                    //
                }else
                {
                    return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.DownloadFile()");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }
     
        public JsonResult ListPDFFiles(FormCollection formCollection)
        {
            try
            {
                objDAL = new GPSVTSDetailsDAL();
                CommonFunctions commonFunction = new CommonFunctions();
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }

                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                int totalRecords;

                var jsonData = new
                {
                    rows = objDAL.GetPDFFilesListDAL(
                        Convert.ToInt32(formCollection["page"]) - 1,
                        Convert.ToInt32(formCollection["rows"]),
                        formCollection["sidx"],
                        formCollection["sord"],
                        out totalRecords,
                        IMS_PR_ROAD_CODE),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.ListPDFFiles()");
                throw;
            }
        }
        [HttpPost]
 
        public JsonResult UpdatePDFDetails(FormCollection formCollection)
        {
            try
            {
                objDAL = new GPSVTSDetailsDAL();
                string[] arrKey = formCollection["id"].Split('$');
                int imsPrRoadCode = Convert.ToInt32(arrKey[1]);
                int fileId = Convert.ToInt32(arrKey[0]);

                string description = formCollection["Description"].Trim();
                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");

                if (!regex.IsMatch(description) || string.IsNullOrEmpty(description))
                {
                    return Json("Invalid PDF Description, Only Alphabets, Numbers, and [,.()-] are allowed");
                }

                VTS_GPS_FILES_DETAILS vtsGpsFilesDetails = new VTS_GPS_FILES_DETAILS
                {
                    FILE_ID = fileId,
                    FILE_DESC = description,
                    IMS_PR_ROAD_CODE = imsPrRoadCode
                };

                string status = objDAL.UpdatePDFDetailsDAL(vtsGpsFilesDetails);

                if (string.IsNullOrEmpty(status))
                {
                    return Json(true);
                }
                else
                {
                    return Json("An error occurred while processing your request.");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.UpdatePDFDetails()");
                return Json("An error occurred while processing your request.");
            }
        }

        [HttpPost]


        public JsonResult DeleteFileDetails(string parameter)
        {
            objDAL = new GPSVTSDetailsDAL();

            try
            {
                var splitValues = parameter.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters1(splitValues);

                if (decryptedParameters.Count() == 0)
                {
                    return Json(new { Success = false, ErrorMessage = "Invalid parameters." });
                }

                long FILE_ID = Convert.ToInt64(decryptedParameters["FILE_ID"]);
                int IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["roadcode"]);
                string FILE_NAME = Convert.ToString(decryptedParameters["FILE_NAME"]);

                string status = objDAL.DeleteFileDetailsDAL(FILE_ID, IMS_PR_ROAD_CODE, FILE_NAME);

                if (string.IsNullOrEmpty(status))
                {
                    string physicalPath = ConfigurationManager.AppSettings["VTS_GPS_PDF_FILE_UPLOAD"];
                    string fullPath = Path.Combine(physicalPath, FILE_NAME);

                    try
                    {
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "GPSVTSDetailsController.DeleteFileDetails() - File Exists");
                        return Json(new { Success = true, ErrorMessage = "An error occurred while processing your request." });
                    }

                    return Json(new { Success = true, ErrorMessage = "File deleted successfully." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.DeleteFileDetails()");
                return Json(new { Success = false, ErrorMessage = "An error occurred while processing your request." });
            }
        }

        public ActionResult GPSVTSReportLayout()
        {
            GPSVTSReportModel model = new GPSVTSReportModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates();
                }
                else if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                }

                model.DistrictList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                }
                if (PMGSYSession.Current.DistrictCode > 0)
                {

                    model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                }

                model.BlockList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    model.BlockList = comm.PopulateBlocks((PMGSYSession.Current.DistrictCode), true);

                    model.BlockList.Find(x => x.Value == "-1").Value = "0";
                    model.BlockList.Find(x => x.Value == model.BlockCode.ToString()).Selected = true;
                }
                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();


                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });


                model.Scheme = "4";
                model.SchemeList = new List<SelectListItem>();
                model.SchemeList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                model.SchemeList.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY-I" });
                model.SchemeList.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY-II" });
                model.SchemeList.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
                model.SchemeList.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY-III" });
             

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.GPSVTSReportLayout()");
                return null;
            }
        }


        public ActionResult PopulateDistrictsbyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();
                lstDistrict = objCommon.PopulateDistrict(stateCode, false);
                lstDistrict.RemoveAt(0);
                lstDistrict.Insert(0, new SelectListItem { Value = "0", Text = "All District" });
                return Json(lstDistrict);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }


        public ActionResult PopulateBlocksbyDistrictCode(int districtCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstBlock = new List<SelectListItem>();
                lstBlock = objCommon.PopulateBlocks(districtCode, false);
                lstBlock.RemoveAt(0);
                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Block" });
                return Json(lstBlock);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public ActionResult GetGPSVTSReport(GPSVTSReportModel model)
        {
            try
            {
                return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/GetGPSVTSReport.cshtml", model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController/GetGPSVTSReport");
                return null;
            }
        }


              //
        public JsonResult FinalizeDetails(string ROAD_CODE)
        {
            objDAL = new GPSVTSDetailsDAL();
            try
            {
                int ROADCODE = Convert.ToInt32(ROAD_CODE);
                string status = objDAL.FinalizeDetailsDAL(ROADCODE);
                if (string.IsNullOrEmpty(status))
                {
                    return Json(new { Success = true, ErrorMessage = "Details Finalized successfully." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }

            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController/FinalizeDetails");
                return Json(new { Success = false, ErrorMessage = "An error occurred while processing your request." });
            }
        }
        //

        [HttpGet]
        public ActionResult UnfreezeWorkDetailsLayout()
        {
            UnfreezeWorkDetailsModel model = new UnfreezeWorkDetailsModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                //model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                var stateList = comm.PopulateStates(true);
                stateList.RemoveAt(0);
                stateList.Insert(0,new SelectListItem(){ Text = "Select State", Value = "-1" });
                model.StateList = stateList;

                model.DistrictList = new List<SelectListItem>();
                 model.DistrictList.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
              
                model.BlockList = new List<SelectListItem>();
                
                    model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
               
                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();

                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                model.ProposalType = "P";
                model.ProposalTypeList = new List<SelectListItem>();
                model.ProposalTypeList.Insert(0, new SelectListItem { Value = "P", Text = "Road" });

                model.WorkStatus = "F";
                model.WorkStatusList = new List<SelectListItem>();
                model.WorkStatusList.Insert(0, new SelectListItem { Value = "F", Text = "Freeze" });
                model.WorkStatusList.Insert(1, new SelectListItem { Value = "U", Text = "Unfreezed" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.UnfreezeWorkDetailsLayout()");
                return null;
            }
        }
        
        public ActionResult PopulateBlocksbyDistrictCodeUnfreezeWorkDetails(int districtCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstBlock = new List<SelectListItem>();
                lstBlock = objCommon.PopulateBlocks((districtCode), false);
                lstBlock.RemoveAt(0);
                lstBlock.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                return Json(lstBlock);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }
        public ActionResult PopulateDistrictsbyStateCodeUnfreezeWorkDetails(int stateCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();
                lstDistrict = objCommon.PopulateDistrictForSRRDA(stateCode,true);
                return Json(lstDistrict);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [HttpPost]
        public ActionResult GPSVTSUnfreezeWorkDetailsRoadList(int? page, int? rows, string sidx, string sord)
        {
            objDAL = new GPSVTSDetailsDAL();
            long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0;
            string proposalType = string.Empty;
            //
            string WorkStatus = "";
            //

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["block"]))
                {
                    block = Convert.ToInt32(Request.Params["block"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
                {
                    sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["proposaltype"]))
                {
                    proposalType = Request.Params["proposaltype"];

                }
                //
                if (!string.IsNullOrEmpty(Request.Params["WorkStatus"]))
                {
                    WorkStatus = Request.Params["WorkStatus"];
                }
                //
                var jsonData = new
                {
                    //rows = objDAL.GPSVTSRoadListDAL(state, district, block, sanction_year, batch, proposalType, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    rows = objDAL.GPSVTSUnfreezeWorkDetailsRoadListDAL(WorkStatus, state, district, block, sanction_year, batch, proposalType, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.GPSVTSUnfreezeWorkDetailsRoadList()");
                return null;
            }
        }

        public JsonResult UnFreezeWorkDetails(string parameter)
        {
            try
            {
                objDAL = new GPSVTSDetailsDAL();
                var splitValues = parameter.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters1(splitValues);

                if (decryptedParameters.Count() == 0)
                {
                    return Json(new { Success = false, ErrorMessage = "Invalid parameters." });
                }
                int IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["roadcode"]);

                string status = objDAL.UnFreezeWorkDetailsDAL(IMS_PR_ROAD_CODE);
                if (string.IsNullOrEmpty(status))
                {
                    return Json(new { Success = true, ErrorMessage = "Details UnFreezed successfully." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.UnFreezeWorkDetails()");
                return Json(new { Success = false, ErrorMessage = "An error occurred while processing your request." });
            }
           
        }
        [Route("GPSVTSInstallationDetails/GPSVTSDetails/ViewGPSVTSDetailsNewTab")]
        public ActionResult ViewGPSVTSDetailsNewTab(String parameter)
        {
            try
            {
                int Road_Code = 0;
                long totalRecords = 0;
                objDAL = new GPSVTSDetailsDAL();
                ViewGPSVTSDetailsNewTabDetailsModel viewGPSVTSDetailsNewTabDetailsModel = new ViewGPSVTSDetailsNewTabDetailsModel();
                if (!string.IsNullOrEmpty(parameter))
                {

                    string[] splitValues = parameter.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { splitValues[0], splitValues[1], splitValues[2] });
                    if (decryptedParameters.Count() > 0)
                    {
                        Road_Code = Convert.ToInt32(decryptedParameters["roadcode"].ToString());
                        viewGPSVTSDetailsNewTabDetailsModel.RoadName = decryptedParameters["RoadName"].ToString();
                        viewGPSVTSDetailsNewTabDetailsModel.Batch = decryptedParameters["Batch"].ToString();
                        viewGPSVTSDetailsNewTabDetailsModel.PackageId = decryptedParameters["Package"].ToString();
                        viewGPSVTSDetailsNewTabDetailsModel.Year = decryptedParameters["Year"].ToString();
                        //
                    }
                }else
                {
                    return null;
                }
                

                var Result = objDAL.GetGPSVTSSavedDetailsDAL(Road_Code, 0, 0, "", "", out totalRecords);

                viewGPSVTSDetailsNewTabDetailsModel.viewGPSVTSDetailsNewTabModels = new List<ViewGPSVTSDetailsNewTabModel>();

                foreach (var anonymousItem in Result)
                {
                    var cellProperty = anonymousItem.GetType().GetProperty("cell");
                    object[] cell = (object[])cellProperty.GetValue(anonymousItem);

                    DateTime dateOfInstallation;
                    if (DateTime.TryParse((string)cell[4], out dateOfInstallation))
                    {
                        ViewGPSVTSDetailsNewTabModel model = new ViewGPSVTSDetailsNewTabModel
                        {
                            GPS_INSTALLED = (string)cell[0],//GPSInstalled
                            VehicleName = (string)cell[1],//VehicleName
                            DATE_OF_INSTALLATION = dateOfInstallation,//DateOfInstallation
                            NO_OF_VEHICLES = (int)cell[3],//NumberOfVehicles
                            VTS_INSTRUMENT_GPS_ID = (string)cell[9]//VTSVehicleGPSID
                        };

                        viewGPSVTSDetailsNewTabDetailsModel.viewGPSVTSDetailsNewTabModels.Add(model);
                    }
                    
                }

                return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/ViewGPSVTSDetailsNewTab.cshtml", viewGPSVTSDetailsNewTabDetailsModel);

            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.ViewGPSVTSDetailsNewTab()");
                return null;
            }
        }

        public JsonResult ListPDFFilesUnfreezeWorkDetails(FormCollection formCollection)
        {
            try
            {
                objDAL = new GPSVTSDetailsDAL();
                CommonFunctions commonFunction = new CommonFunctions();
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }

                int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["parameter"]);
                int totalRecords;

                var jsonData = new
                {
                    rows = objDAL.GetPDFFilesListUnfreezeWorkDetailsDAL(
                        Convert.ToInt32(formCollection["page"]) - 1,
                        Convert.ToInt32(formCollection["rows"]),
                        formCollection["sidx"],
                        formCollection["sord"],
                        out totalRecords,
                        IMS_PR_ROAD_CODE),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GPSVTSDetailsController.ListPDFFilesUnfreezeWorkDetails()");
                throw;
            }
        }

        public ActionResult GetListPDFFilesUnfreezeWorkDetails(string parameter)
        {
            objDAL = new GPSVTSDetailsDAL();
            
            if (!string.IsNullOrEmpty(parameter))
            {
                string[] splitValues = parameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { splitValues[0], splitValues[1], splitValues[2] });

                if (decryptedParameters.Count() > 0)
                {
                    try
                    {
                        ListPdfFileUnfreezeWork listPdfFileUnfreezeWork = new ListPdfFileUnfreezeWork();
                        listPdfFileUnfreezeWork.RoadCode = Convert.ToInt32(decryptedParameters["roadcode"].ToString());
                        listPdfFileUnfreezeWork.RoadName = decryptedParameters["RoadName"].ToString();
                        listPdfFileUnfreezeWork.Batch = decryptedParameters["Batch"].ToString();
                        listPdfFileUnfreezeWork.Package = decryptedParameters["Package"].ToString();
                        listPdfFileUnfreezeWork.Year = decryptedParameters["Year"].ToString();

                        return View("~/Areas/GPSVTSInstallationDetails/Views/GPSVTSDetails/ListPdfFilesUnfreezeWorks.cshtml", listPdfFileUnfreezeWork);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "GPSVTSDetailsController.GetListPDFFilesUnfreezeWorkDetails()");
                        return Json(new { Success = false, ErrorMessage = "An error occurred while processing your request." }, JsonRequestBehavior.AllowGet);
                        
                    }
                }
                else
                {
                 
                    return Json(new { Success = false, ErrorMessage = "Invalid parameters. Please check the input." }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = false, ErrorMessage = "An error occurred while processing your request." }, JsonRequestBehavior.AllowGet);

        }



    }

}
