using PMGSY.Areas.ContractorGrievances.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PMGSY.Areas.ContractorGrievances.DAL
{
    public class ContractorGrievancesDAL : IContractorGrievancesDAL
    {
        public ContractorGrievancesViewModel GetProfileDetails()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ContractorGrievancesViewModel model = new ContractorGrievancesViewModel();
                CONTRACTOR_REGISTRATION_DETAILS RegDetails = dbContext.CONTRACTOR_REGISTRATION_DETAILS.FirstOrDefault(s => s.USER_ID == PMGSYSession.Current.UserId);
                CommonFunctions comm = new CommonFunctions();
                if (RegDetails != null && RegDetails.BANK_NAME == null)
                {
                    model.CON_FIRM_NAME = RegDetails.CON_FIRM_NAME;
                    model.EMAIL_ID = RegDetails.EMAIL_ID;
                    model.MOBILE_NO = RegDetails.MOBILE_NO;
                    model.PAN_NO = RegDetails.PAN_NO;
                    model.lstBankNames = comm.PopulatePFMSBankNames();
                }
                else if (RegDetails != null && RegDetails.BANK_NAME != null)
                {
                    model.CON_FIRM_NAME = RegDetails.CON_FIRM_NAME;
                    model.EMAIL_ID = RegDetails.EMAIL_ID;
                    model.MOBILE_NO = RegDetails.MOBILE_NO;
                    model.PAN_NO = RegDetails.PAN_NO;
                    model.BANK_NAME = RegDetails.BANK_NAME;
                    model.lstBankNames = comm.PopulatePFMSBankNames();
                    model.BANK_ACCOUNT_NO = RegDetails.BANK_ACCOUNT_NO;
                    model.BRANCH_NAME = RegDetails.BRANCH_NAME;
                    model.IFSC_CODE = RegDetails.IFSC_CODE;
                }
                else
                {
                    ContractorGrievancesLog("Contractor Grievances", ConfigurationManager.AppSettings["ContractorGrievancesLog"].ToString(), "No records found to populate", "");
                    return null;
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL.GetProfileDetails()");
                return null;
            }
        }

        public void ContractorGrievancesLog(string module, string logPath, string message, string fileName)
        {
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(logPath + "\\ContractorGrievances" + module + "Log_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date : " + DateTime.Now.ToString());
                    sw.WriteLine("FileName : " + fileName);
                    sw.WriteLine("status : " + message);
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievances.ContractorGrievancesLog");
            }
        }

        public bool SaveContractorBankDetails(ContractorGrievancesViewModel model, ref string message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CONTRACTOR_REGISTRATION_DETAILS contractorRecord = dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.PAN_NO == PMGSYSession.Current.UserName).FirstOrDefault();
                contractorRecord.BANK_NAME = model.BANK_NAME;
                contractorRecord.BRANCH_NAME = model.BRANCH_NAME;
                contractorRecord.BANK_ACCOUNT_NO = model.BANK_ACCOUNT_NO;
                contractorRecord.IFSC_CODE = model.IFSC_CODE;
                dbContext.Entry(contractorRecord).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveContractorBankDetails(DbUpdateException ex).DAL");
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

        public Array AgreementListDAL(int state, int district, int agreement_year, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string pan_no = "AGTPM9686R"; //PMGSYSession.Current.UserName; 
                string account_no = "30872536670"; //dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.PAN_NO == pan_no).Select(x => x.BANK_ACCOUNT_NO).FirstOrDefault(); // 
                var AgreementList = dbContext.USP_CONTRACTOR_FEEDBACK_GET_AGREEMENT_DETAILS(state, district, agreement_year, pan_no, account_no).ToList();

                var resultList = new List<AgreementDetailsModel>();

                foreach (var item in AgreementList)
                {
                    resultList.Add(new AgreementDetailsModel
                    {
                        Agreement_Code = item.TEND_AGREEMENT_CODE,
                        Agreement_Number = item.TEND_AGREEMENT_NUMBER,
                        State = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == item.MAST_STATE_CODE).First().MAST_STATE_NAME,
                        District = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == item.MAST_DISTRICT_CODE).First().MAST_DISTRICT_NAME,
                        Agreement_Date = item.TEND_DATE_OF_AGREEMENT,
                        Agreement_Amount = item.TEND_AGREEMENT_AMOUNT,
                        Package_Number = item.PACKAGE_NUMBER,
                        Road_Name = item.IMS_ROAD_NAME,
                        IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE,
                    });
                }

                totalRecords = AgreementList.Count();

                return resultList.Select(AgreementDetails => new
                {
                    id = AgreementDetails.Agreement_Code.ToString(),
                    cell = new[] {    
                        AgreementDetails.Agreement_Number.ToString(),
                        AgreementDetails.State,   
                        AgreementDetails.District,      
                        AgreementDetails.Agreement_Date.ToShortDateString(),    
                        AgreementDetails.Agreement_Amount.ToString(), 
                        AgreementDetails.Package_Number,
                        AgreementDetails.Road_Name,
                        "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Register Grievance' onClick ='RegisterGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"AgreementCode="+AgreementDetails.Agreement_Code.ToString().Trim(),"RoadCode="+AgreementDetails.IMS_PR_ROAD_CODE.ToString().Trim(),"State="+AgreementDetails.State.ToString().Trim(),"District="+AgreementDetails.District.ToString().Trim()}) + "\");'></span></td></tr></table></center>",
                        "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon 	ui-icon-circle-zoomout' title='Track Grievance' onClick ='TrackGrievance(\""+AgreementDetails.IMS_PR_ROAD_CODE.ToString().Trim()+"\");'></span></td></tr></table></center>"
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL().AgreementListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SaveContractorGrievanceDAL(AgreementDetailsModel grievanceData, HttpFileCollectionBase multipleFile, out string referenceNo)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            referenceNo = null;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CONTRACTOR_GRIEVANCE_DETAILS conDetails = new CONTRACTOR_GRIEVANCE_DETAILS();

                    conDetails.DETAIL_ID = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Any() ? dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Max(s => s.DETAIL_ID) + 1 : 1;
                    conDetails.FEEDBACK_COMPLAINT = grievanceData.Feedback_Complaint == true ? "C" : "F";
                    conDetails.GRIEVANCE_TYPE = grievanceData.Grievance_Type == 1 ? "P" : grievanceData.Grievance_Type == 2 ? "F" : grievanceData.Grievance_Type == 3 ? "Q" : "";
                    conDetails.GRIEVANCE_ID = grievanceData.Grievance_SubType;//dbContext.MASTER_CONTRACTOR_GRIEVANCE.Where(x => x.GRIEVANCE_TYPE == conDetails.GRIEVANCE_TYPE /*&& x.GRIEVANCE_SUBTYPE == grievanceData.Grievance_SubType*/ && x.IS_VALID == "1").Select(x => x.GRIEVANCE_ID).FirstOrDefault();
                    conDetails.REFERENCE_NUMBER = GenerateReferenceNumber();
                    conDetails.GRIEVANCE_DESC = grievanceData.Grievance_Description;
                    conDetails.GRIEVANCE_BY_USERID = dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.CON_FIRM_NAME == PMGSYSession.Current.UserName).Select(x => x.USER_ID).FirstOrDefault();
                    conDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    conDetails.IS_FINALIZED = "N";
                    conDetails.FINALIZATION_DATE = null;
                    conDetails.IS_CLOSED = "N";
                    conDetails.CLOSURE_DATE = null;
                    conDetails.IS_REOPENED = "N";
                    conDetails.REOPENED_DATE = null;
                    conDetails.IMS_PR_ROAD_CODE = grievanceData.IMS_PR_ROAD_CODE;
                    conDetails.ENTRY_DATE = DateTime.Now;
                    conDetails.TEND_AGREEMENT_CODE = grievanceData.Agreement_Code;

                    dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Add(conDetails);
                    dbContext.SaveChanges();
                    HttpPostedFileBase FileBase = null;

                    for (int i = 0; i < multipleFile.Count; i++)
                    {
                        CONTRACTOR_GRIEVANCE_FILE_DETAILS fileDetails = new CONTRACTOR_GRIEVANCE_FILE_DETAILS();

                        fileDetails.G_FILE_ID = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Any() ? dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Max(s => s.G_FILE_ID) + 1 : 1;
                        fileDetails.DETAIL_ID = conDetails.DETAIL_ID;
                        FileBase = multipleFile[i];
                        var filename = FileBase.FileName;
                        fileDetails.FILENAME = filename;
                        fileDetails.FILEPATH = ConfigurationManager.AppSettings["ContractorGrievanceFilePath"] + "\\" + filename;
                        fileDetails.FILE_UPLOAD_DATE = DateTime.Now;
                        fileDetails.UPLOADED_BY = "C";
                        fileDetails.ROUND_CNT = 1;
                        fileDetails.USER_ID = dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.CON_FIRM_NAME == PMGSYSession.Current.UserName).Select(x => x.USER_ID).FirstOrDefault();
                        fileDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Add(fileDetails);
                        dbContext.SaveChanges();
                    }
                    referenceNo = conDetails.REFERENCE_NUMBER;

                    ts.Complete();

                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveContractorGrievanceDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveContractorGrievanceDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool SaveGrievanceFileDAL(string FileName, HttpPostedFileBase filebase, out bool isFileSaved)
        {
            isFileSaved = false;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                if (!string.IsNullOrEmpty(FileName.Trim()))
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["ContractorGrievanceFilePath"])))
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(@"" + ConfigurationManager.AppSettings["ContractorGrievanceFilePath"]));

                    filebase.SaveAs(ConfigurationManager.AppSettings["ContractorGrievanceFilePath"] + "\\" + FileName);
                    isFileSaved = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL.SaveGrievanceFileDAL()");
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

        public string GenerateReferenceNumber()
        {
            Random ran = new Random();
            Int64 i64 = ran.Next(10000000, 99999999);
            i64 = (i64 * 100000000) + ran.Next(0, 999999999);
            var v16 = Math.Abs(i64);
            return Convert.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Ticks + v16).Substring(0, 10);
        }

        public Array GrievanceListDAL(int roadCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var GrievanceList = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.GRIEVANCE_BY_USERID == PMGSYSession.Current.UserId).ToList();

                var resultList = new List<GrievanceDetailsModel>();
                var AgreementCode = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(x => x.TEND_AGREEMENT_CODE).ToList();

                String AgreementNumber = String.Empty;
                String MultipleAgreementNum = String.Empty;

                foreach (var agcode in AgreementCode)
                {
                    AgreementNumber = dbContext.TEND_AGREEMENT_MASTER.Where(x => x.TEND_AGREEMENT_CODE == agcode).Select(x => x.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                    MultipleAgreementNum += AgreementNumber + ", ";
                }

                foreach (var item in GrievanceList)
                {
                    resultList.Add(new GrievanceDetailsModel
                    {
                        DETAIL_ID = item.DETAIL_ID,
                        Grievance_ID = item.REFERENCE_NUMBER,
                        Grievance_SubmittedOn = item.ENTRY_DATE,
                        Agreement_Number = MultipleAgreementNum,
                        Grievance_Type = item.FEEDBACK_COMPLAINT == "F" ? "Feedback" : "Complaint",
                        Grievance_Category = item.GRIEVANCE_TYPE == "P" ? "Progress" : item.GRIEVANCE_TYPE == "F" ? "Finance" : item.GRIEVANCE_TYPE == "Q" ? "Quality" : item.GRIEVANCE_TYPE == "T" ? "Technical" : "",
                        Grievance_Status = item.IS_CLOSED == "N" ? "In Progress" : "Closed",
                        Is_Finalized = item.IS_FINALIZED,
                        Is_Closed = item.IS_CLOSED,
                        Is_Reopened = item.IS_REOPENED,
                        Filename = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FILENAME).FirstOrDefault(),
                    });
                }

                totalRecords = GrievanceList.Count();

                return resultList.Select(GrievanceDetails => new
                {
                    id = GrievanceDetails.DETAIL_ID.ToString(),
                    cell = new[] {    
                        GrievanceDetails.Grievance_ID,
                        GrievanceDetails.Grievance_SubmittedOn.Value.ToShortDateString(),   
                        GrievanceDetails.Agreement_Number,      
                        GrievanceDetails.Grievance_Type,    
                        GrievanceDetails.Grievance_Category, 
                        GrievanceDetails.Grievance_Status,
                        GrievanceDetails.Is_Finalized=="N"?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Grievance' onClick ='ViewEditGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(),"RoadCode="+roadCode.ToString().Trim(),"Operation=E"}) + "\");'></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-search' title='View Grievance' onClick ='ViewEditGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(),"RoadCode="+roadCode.ToString().Trim(),"Operation=V"}) + "\");'></span></td></tr></table></center>", 
                        GrievanceDetails.Is_Finalized=="N"?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-unlocked ui-align-center ' title='Finalize Grievance' onClick ='FinalizeGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(),"RoadCode="+roadCode.ToString().Trim()}) + "\");'></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center' title='Finalized Grievance'></span></td></tr></table></center>",                   
                          "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-circle-arrow-s' title='Download Grievance Files' onClick ='LoadDownloadFileGrid(\""+GrievanceDetails.DETAIL_ID.ToString().Trim()+"\");'></span></td></tr></table></center>"
                        //URLEncrypt.EncryptParameters(new string[] { GrievanceDetails.Filename + "$" +  GrievanceDetails.DETAIL_ID }),    
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL().GetContractorGrievanceList");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string EditContractorGrievanceDAL(AgreementDetailsModel model, HttpFileCollectionBase multipleFile, out string referenceNo)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            referenceNo = null;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CONTRACTOR_GRIEVANCE_DETAILS conDetails = new CONTRACTOR_GRIEVANCE_DETAILS();

                    conDetails = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == model.Detail_Id).FirstOrDefault();
                    conDetails.FEEDBACK_COMPLAINT = model.Feedback_Complaint == true ? "C" : "F";
                    conDetails.GRIEVANCE_TYPE = model.Grievance_Type == 1 ? "P" : model.Grievance_Type == 2 ? "F" : model.Grievance_Type == 3 ? "Q" : "";
                    conDetails.GRIEVANCE_ID = model.Grievance_SubType;//dbContext.MASTER_CONTRACTOR_GRIEVANCE.Where(x => x.GRIEVANCE_TYPE == conDetails.GRIEVANCE_TYPE /*&& x.GRIEVANCE_SUBTYPE == grievanceData.Grievance_SubType*/ && x.IS_VALID == "1").Select(x => x.GRIEVANCE_ID).FirstOrDefault();
                    conDetails.GRIEVANCE_DESC = model.Grievance_Description;
                    conDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    conDetails.ENTRY_DATE = DateTime.Now;
                    dbContext.Entry(conDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    HttpPostedFileBase FileBase = null;

                    for (int i = 0; i < multipleFile.Count; i++)
                    {

                        CONTRACTOR_GRIEVANCE_FILE_DETAILS fileDetails = new CONTRACTOR_GRIEVANCE_FILE_DETAILS();

                        fileDetails.G_FILE_ID = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Any() ? dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Max(s => s.G_FILE_ID) + 1 : 1;
                        fileDetails.DETAIL_ID = conDetails.DETAIL_ID;
                        FileBase = multipleFile[i];
                        var filename = FileBase.FileName;
                        fileDetails.FILENAME = filename;
                        fileDetails.FILEPATH = ConfigurationManager.AppSettings["ContractorGrievanceFilePath"] + "\\" + filename;
                        fileDetails.FILE_UPLOAD_DATE = DateTime.Now;
                        fileDetails.UPLOADED_BY = "C";
                        fileDetails.ROUND_CNT = 1;
                        fileDetails.USER_ID = dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.PAN_NO == PMGSYSession.Current.UserName).Select(x => x.USER_ID).FirstOrDefault();
                        fileDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Add(fileDetails);
                        dbContext.SaveChanges();
                    }
                    referenceNo = conDetails.REFERENCE_NUMBER;
                    ts.Complete();

                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "EditContractorGrievanceDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditContractorGrievanceDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeContractorGrievanceDAL(int DetailId, out int roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool status = false;
            roadCode = 0;
            try
            {
                CONTRACTOR_GRIEVANCE_DETAILS conDetails = new CONTRACTOR_GRIEVANCE_DETAILS();

                conDetails = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == DetailId).FirstOrDefault();

                conDetails.IS_FINALIZED = "Y";
                conDetails.FINALIZATION_DATE = DateTime.Now;
                roadCode = conDetails.IMS_PR_ROAD_CODE ?? default(int);
                dbContext.Entry(conDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                status = true;

                return status;
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

        public Array FilesListDAL(int detailId, string uploadedBy, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            var FilesList = new List<CONTRACTOR_GRIEVANCE_FILE_DETAILS>();
            try
            {
                if (PMGSYSession.Current.StateCode > 0 || PMGSYSession.Current.DistrictCode > 0) //Listing of files in case of SRRDA and PIU login
                {
                    FilesList = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Where(x => x.DETAIL_ID == detailId && x.UPLOADED_BY == uploadedBy).ToList();
                }
                else  //Listing of files in case of Contractor login
                {
                    FilesList = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Where(x => x.DETAIL_ID == detailId && x.USER_ID == PMGSYSession.Current.UserId).ToList();
                }
                var resultList = new List<GrievanceDetailsModel>();

                foreach (var item in FilesList)
                {
                    resultList.Add(new GrievanceDetailsModel
                    {
                        FileId = item.G_FILE_ID,
                        Filename = item.FILENAME,
                        FileUploadDate = item.FILE_UPLOAD_DATE,
                        DETAIL_ID = item.DETAIL_ID,
                        FileUploadedBy = item.UPLOADED_BY == "C" ? "Contractor" : item.UPLOADED_BY == "S" ? "SRRDA" : item.UPLOADED_BY == "D" ? "PIU" : "",

                    });
                }

                totalRecords = FilesList.Count();

                return resultList.Select(FilesDetails => new
                {
                    id = FilesDetails.FileId.ToString(),
                    cell = new[] {    
                        FilesDetails.Filename, 
                        FilesDetails.FileUploadedBy,
                        FilesDetails.FileUploadDate.Value.ToShortDateString(),                             
                        URLEncrypt.EncryptParameters(new string[] { FilesDetails.Filename + "$" +  FilesDetails.DETAIL_ID }),    
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL().FilesListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PopulateGrievanceListSrrdaDAL(int state, int district, int package_year, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var GrievanceList = dbContext.USP_CONTRACTOR_GRIEVANCE_LIST(state, district, package_year).ToList();

                var resultList = new List<GrievanceDetailsModel>();

                foreach (var item in GrievanceList)
                {
                    resultList.Add(new GrievanceDetailsModel
                    {
                        DETAIL_ID = item.DETAIL_ID,
                        Grievance_ID = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.REFERENCE_NUMBER).FirstOrDefault(),
                        Contractor_FirmName = item.CONTRACTOR_FIRM_NAME,
                        Finalization_Date = item.SUBMISSION_DATE,
                        Agreement_Number = item.TEND_AGREEMENT_NUMBER,
                        RoadName = item.IMS_ROAD_NAME,
                        State = PMGSYSession.Current.StateName,
                        District = district > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == district).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts",
                        Agreement_Year = package_year > 0 ? package_year : 2021,
                        Grievance_Type = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FEEDBACK_COMPLAINT).FirstOrDefault() == "F" ? "Feedback" : "Complaint",
                        Grievance_Category = item.GRIEVANCE_TYPE,
                        Grievance_Status = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.IS_CLOSED).FirstOrDefault() == "N" ? "In Progress" : "Closed",
                        Is_Finalized = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.IS_FINALIZED).FirstOrDefault(),
                        Is_Closed = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.IS_CLOSED).FirstOrDefault(),
                        Is_Reopened = item.IS_REOPENED,
                        Filename = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FILENAME).FirstOrDefault(),
                        Forwarded_To_Piu = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Any(),
                        Finalized_By_Piu = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FINALIZED_BY_PIU).FirstOrDefault(),
                    });

                }

                totalRecords = GrievanceList.Count();

                return resultList.Select(GrievanceDetails => new
                {
                    id = GrievanceDetails.DETAIL_ID.ToString(),
                    cell = new[] {    
                        GrievanceDetails.Grievance_ID,
                        GrievanceDetails.Contractor_FirmName,
                        GrievanceDetails.Finalization_Date,  
                         GrievanceDetails.Is_Reopened=="N"?"<center><table><tr><td  style='border:none'><span>No</span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span>Yes</span></td></tr></table></center>",                   
                        GrievanceDetails.Agreement_Number,  
                        GrievanceDetails.RoadName,
                        GrievanceDetails.State,
                        GrievanceDetails.District,
                        GrievanceDetails.Agreement_Year.ToString(),
                        GrievanceDetails.Grievance_Type,    
                        GrievanceDetails.Grievance_Category, 
                        GrievanceDetails.Grievance_Status,
                        GrievanceDetails.Forwarded_To_Piu == false ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-arrowreturnthick-1-e' title='Forward Grievance To PIU' onClick ='ForwardGrievanceToPiu(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(), "Form=srrdaAdd"}) + "\");'></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span title='Click to upload files' onClick ='ForwardGrievanceToPiu(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(),"Form=srrdaRead"}) + "\");'>Forwarded</span></td></tr></table></center>",
                        GrievanceDetails.Finalized_By_Piu=="Y"?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-check ui-align-center ' title='PIU Action on Grievance''></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-closethick ui-align-center' title='PIU Action on Grievance'></span></td></tr></table></center>" ,
                       GrievanceDetails.Is_Closed=="N"?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-unlocked ui-align-center ' title='Close Grievance'' onClick ='CloseGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim()}) + "\");'></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center' title='Closed Grievance'></span></td></tr></table></center>",                           
                          //"<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-circle-arrow-s' title='Download Grievance Files' onClick ='LoadDownloadFileGrid(\""+GrievanceDetails.DETAIL_ID.ToString().Trim()+"\");'></span></td></tr></table></center>"
                        //URLEncrypt.EncryptParameters(new string[] { GrievanceDetails.Filename + "$" +  GrievanceDetails.DETAIL_ID }),    
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL().PopulateGrievanceListSrrdaDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SaveGrievanceTrackingDAL(TrackingDetailsModel model, HttpFileCollectionBase multipleFile)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CONTRACTOR_GRIEVANCE_TRACKING_DETAILS trackDetails = new CONTRACTOR_GRIEVANCE_TRACKING_DETAILS();
                    if (!String.IsNullOrEmpty(model.SRRDA_REMARKS))
                    {
                        trackDetails.TRACK_ID = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Any() ? dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Max(s => s.DETAIL_ID) + 1 : 1;
                        trackDetails.DETAIL_ID = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == model.DETAIL_ID).Select(x => x.DETAIL_ID).FirstOrDefault();
                        trackDetails.FORWARD_TO_PIU = "Y";
                        trackDetails.FORWARD_DATE = DateTime.Now;
                        trackDetails.SRRDA_REMARKS = model.SRRDA_REMARKS;
                        trackDetails.FINALIZED_BY_PIU = "N";
                        trackDetails.PIU_FINALIZATION_DATE = null;
                        trackDetails.PIU_REMARKS = null;
                        trackDetails.ROUND_CNT = 1;
                        trackDetails.IS_LATEST = "Y";
                        trackDetails.SRRDA_USER_ID = PMGSYSession.Current.UserId;
                        trackDetails.SRRDA_IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        trackDetails.PIU_USER_ID = null;
                        trackDetails.PIU_IPADDR = null;

                        dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Add(trackDetails);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        trackDetails.DETAIL_ID = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == model.DETAIL_ID).Select(x => x.DETAIL_ID).FirstOrDefault();
                    }
                    HttpPostedFileBase FileBase = null;

                    for (int i = 0; i < multipleFile.Count; i++)
                    {
                        CONTRACTOR_GRIEVANCE_FILE_DETAILS fileDetails = new CONTRACTOR_GRIEVANCE_FILE_DETAILS();

                        fileDetails.G_FILE_ID = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Any() ? dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Max(s => s.G_FILE_ID) + 1 : 1;
                        fileDetails.DETAIL_ID = trackDetails.DETAIL_ID;
                        FileBase = multipleFile[i];
                        var filename = FileBase.FileName;
                        fileDetails.FILENAME = filename;
                        fileDetails.FILEPATH = ConfigurationManager.AppSettings["ContractorGrievanceFilePath"] + "\\" + filename;
                        fileDetails.FILE_UPLOAD_DATE = DateTime.Now;
                        fileDetails.UPLOADED_BY = "S";
                        fileDetails.ROUND_CNT = 1;
                        fileDetails.USER_ID = PMGSYSession.Current.UserId; //dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.CON_FIRM_NAME == PMGSYSession.Current.UserName).Select(x => x.USER_ID).FirstOrDefault();
                        fileDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Add(fileDetails);
                        dbContext.SaveChanges();
                    }

                    ts.Complete();

                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveGrievanceTrackingDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveGrievanceTrackingDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PopulateGrievanceListPiuDAL(int state, int district, int package_year, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var GrievanceList = dbContext.USP_CONTRACTOR_GRIEVANCE_LIST(state, district, package_year).ToList();

                var resultList = new List<GrievanceDetailsModel>();

                foreach (var item in GrievanceList)
                {
                    if (dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Any())
                    {
                        resultList.Add(new GrievanceDetailsModel
                        {
                            DETAIL_ID = item.DETAIL_ID,
                            Grievance_ID = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.REFERENCE_NUMBER).FirstOrDefault(),
                            Contractor_FirmName = item.CONTRACTOR_FIRM_NAME,
                            Finalization_Date = item.SUBMISSION_DATE,
                            Agreement_Number = item.TEND_AGREEMENT_NUMBER,
                            RoadName = item.IMS_ROAD_NAME,
                            State = PMGSYSession.Current.StateName,
                            District = PMGSYSession.Current.DistrictName,
                            Agreement_Year = package_year > 0 ? package_year : 2021,
                            Grievance_Type = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FEEDBACK_COMPLAINT).FirstOrDefault() == "F" ? "Feedback" : "Complaint",
                            Grievance_Category = item.GRIEVANCE_TYPE,
                            Grievance_Status = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.IS_CLOSED).FirstOrDefault() == "N" ? "In Progress" : "Closed",
                            Is_Finalized = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.IS_FINALIZED).FirstOrDefault(),
                            Is_Closed = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.IS_CLOSED).FirstOrDefault(),
                            Is_Reopened = item.IS_REOPENED,
                            Filename = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FILENAME).FirstOrDefault(),
                            Finalized_By_Piu = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID).Select(x => x.FINALIZED_BY_PIU).FirstOrDefault(),
                            Action_Taken_By_Piu = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == item.DETAIL_ID && x.PIU_REMARKS != null).Any(),
                        });
                    }

                }

                totalRecords = resultList.Count();

                return resultList.Select(GrievanceDetails => new
                {
                    id = GrievanceDetails.DETAIL_ID.ToString(),
                    cell = new[] {    
                        GrievanceDetails.Grievance_ID,
                        GrievanceDetails.Contractor_FirmName,
                        GrievanceDetails.Finalization_Date,  
                       GrievanceDetails.Is_Reopened=="N"?"<center><table><tr><td  style='border:none'><span>No</span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span>Yes</span></td></tr></table></center>",                   
                        GrievanceDetails.Agreement_Number,  
                        GrievanceDetails.RoadName,
                        GrievanceDetails.State,
                        GrievanceDetails.District,
                        GrievanceDetails.Agreement_Year.ToString(),
                        GrievanceDetails.Grievance_Type,    
                        GrievanceDetails.Grievance_Category,  
                        GrievanceDetails.Grievance_Status,
                       GrievanceDetails.Action_Taken_By_Piu == false?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil ui-align-center ' title='PIU Action on Grievance'' onClick ='PiuActionOnGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(),"Form=piuAdd"}) + "\");'></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span  class='ui-icon ui-icon-search' title='Click here to upload files' onClick ='PiuActionOnGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim(),"Form=piuRead"}) + "\");'></span></td></tr></table></center>",
                          GrievanceDetails.Finalized_By_Piu=="N"?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-unlocked ui-align-center ' title='Click here to Finalize' onClick ='FinalizeGrievanceTracking(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim()}) + "\");'></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center' title='Finalized By PIU'></span></td></tr></table></center>",        
                        // GrievanceDetails.Is_Closed=="N"?"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-unlocked ui-align-center ' title='Close Grievance'' onClick ='CloseGrievance(\"" + URLEncrypt.EncryptParameters1(new string[] {"DetailId="+GrievanceDetails.DETAIL_ID.ToString().Trim()}) + "\");'></span></td></tr></table></center>"
                        //:"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center' title='Closed Grievance'></span></td></tr></table></center>",        
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesDAL().PopulateGrievanceListPiuDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SavePIUactionOnGrievanceDAL(TrackingDetailsModel model, HttpFileCollectionBase multipleFile)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CONTRACTOR_GRIEVANCE_TRACKING_DETAILS trackDetails = new CONTRACTOR_GRIEVANCE_TRACKING_DETAILS();

                    trackDetails = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == model.DETAIL_ID && x.IS_LATEST == "Y").FirstOrDefault();
                    if (!String.IsNullOrEmpty(model.PIU_REMARKS))
                    {
                        //In case of read only form PIU remarks are already present in table and model will have null value ; In case of add form model will mandatory have remarks value 
                        trackDetails.PIU_REMARKS = model.PIU_REMARKS;
                        trackDetails.PIU_USER_ID = PMGSYSession.Current.UserId;
                        trackDetails.PIU_IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(trackDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    HttpPostedFileBase FileBase = null;

                    for (int i = 0; i < multipleFile.Count; i++)
                    {
                        CONTRACTOR_GRIEVANCE_FILE_DETAILS fileDetails = new CONTRACTOR_GRIEVANCE_FILE_DETAILS();

                        fileDetails.G_FILE_ID = dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Any() ? dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Max(s => s.G_FILE_ID) + 1 : 1;
                        fileDetails.DETAIL_ID = trackDetails.DETAIL_ID;
                        FileBase = multipleFile[i];
                        var filename = FileBase.FileName;
                        fileDetails.FILENAME = filename;
                        fileDetails.FILEPATH = ConfigurationManager.AppSettings["ContractorGrievanceFilePath"] + "\\" + filename;
                        fileDetails.FILE_UPLOAD_DATE = DateTime.Now;
                        fileDetails.UPLOADED_BY = "D";
                        fileDetails.ROUND_CNT = 1;
                        fileDetails.USER_ID = PMGSYSession.Current.UserId; //dbContext.CONTRACTOR_REGISTRATION_DETAILS.Where(x => x.CON_FIRM_NAME == PMGSYSession.Current.UserName).Select(x => x.USER_ID).FirstOrDefault();
                        fileDetails.IPADDR = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS.Add(fileDetails);
                        dbContext.SaveChanges();
                    }

                    ts.Complete();

                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SavePIUactionOnGrievanceDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SavePIUactionOnGrievanceDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FinalizeGrievanceTrackingDAL(int DetailId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool status = false;
            try
            {
                CONTRACTOR_GRIEVANCE_TRACKING_DETAILS trackDetails = new CONTRACTOR_GRIEVANCE_TRACKING_DETAILS();

                trackDetails = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == DetailId).FirstOrDefault();

                trackDetails.FINALIZED_BY_PIU = "Y";
                trackDetails.PIU_FINALIZATION_DATE = DateTime.Now;
                dbContext.Entry(trackDetails).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                status = true;

                return status;
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

        public bool CloseGrievanceDAL(int DetailId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool status = false;
            try
            {
                CONTRACTOR_GRIEVANCE_TRACKING_DETAILS trackDetails = new CONTRACTOR_GRIEVANCE_TRACKING_DETAILS();
                trackDetails = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == DetailId && x.IS_LATEST == "Y").FirstOrDefault();
                if (trackDetails != null)
                {
                    CONTRACTOR_GRIEVANCE_DETAILS conDetails = new CONTRACTOR_GRIEVANCE_DETAILS();

                    conDetails = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == DetailId).FirstOrDefault();

                    conDetails.IS_CLOSED = "Y";
                    conDetails.CLOSURE_DATE = DateTime.Now;
                    dbContext.Entry(conDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                }
                return status;
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
      
    }

    public interface IContractorGrievancesDAL
    {
        ContractorGrievancesViewModel GetProfileDetails();
        void ContractorGrievancesLog(string module, string logPath, string message, string fileName);
        bool SaveContractorBankDetails(ContractorGrievancesViewModel model, ref string message);
        Array AgreementListDAL(int state, int district, int agreement_year, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string SaveContractorGrievanceDAL(AgreementDetailsModel grievanceData, HttpFileCollectionBase multipleFile, out string referenceNo);
        bool SaveGrievanceFileDAL(string FileName, HttpPostedFileBase filebase, out bool isFileSaved);
        string GenerateReferenceNumber();
        Array GrievanceListDAL(int roadCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string EditContractorGrievanceDAL(AgreementDetailsModel model, HttpFileCollectionBase multipleFile, out string referenceNo);
        bool FinalizeContractorGrievanceDAL(int DetailId, out int roadCode);
        Array FilesListDAL(int detailId, string uploadedBy, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Array PopulateGrievanceListSrrdaDAL(int state, int district, int agreement_year, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string SaveGrievanceTrackingDAL(TrackingDetailsModel model, HttpFileCollectionBase multipleFile);
        Array PopulateGrievanceListPiuDAL(int state, int district, int package_year, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string SavePIUactionOnGrievanceDAL(TrackingDetailsModel model, HttpFileCollectionBase multipleFile);
        bool FinalizeGrievanceTrackingDAL(int DetailId);
        bool CloseGrievanceDAL(int DetailId);
    }
}