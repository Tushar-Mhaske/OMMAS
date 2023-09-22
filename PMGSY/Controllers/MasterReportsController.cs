#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MasterReportsController.cs        
        * Description   :   Listing of Records for State Details.
        * Author        :   Pranav Nerkar 
        * Creation Date :   4/October/2013
 **/
#endregion

using PMGSY.BAL.MasterReports;
using PMGSY.DAL.MasterReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.MasterReports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace PMGSY.Controllers
{
    public class PageEventImplementer : PdfPageEventHelper
    {

        private Font rowfont = FontFactory.GetFont("Verdana", 8, Color.BLACK);

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            string space = "                                                                                                                                                                                                              ";
            HeaderFooter hf = new HeaderFooter(new Phrase("State Listing " + space + writer.PageNumber.ToString(), rowfont), false);
            document.Header = hf;
        }
    }
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class MasterReportsController : Controller
    {
        IMasterReportsBAL bal;
        CommonFunctions common = new CommonFunctions();
        //
        // GET: /MasterReport/

        public ActionResult StateDetails()
        {
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            SelectListItem state = new SelectListItem
            {
                Text = "State",
                Value = "S"
            };

            SelectListItem unionTerritory = new SelectListItem
            {
                Text = "Union Territory",
                Value = "U"
            };

            List<SelectListItem> stateOrUnion = new List<SelectListItem>();
            stateOrUnion.Add(all);
            stateOrUnion.Add(state);
            stateOrUnion.Add(unionTerritory);

            ViewData["STATE_UNION"] = stateOrUnion;

            SelectListItem regular = new SelectListItem
            {
                Text = "Regular",
                Value = "R"
            };

            SelectListItem island = new SelectListItem
            {
                Text = "Island",
                Value = "I"
            };

            SelectListItem northEast = new SelectListItem
            {
                Text = "North East",
                Value = "N"
            };

            SelectListItem hilly = new SelectListItem
            {
                Text = "Hilly",
                Value = "H"
            };

            SelectListItem northEastAndHilly = new SelectListItem
            {
                Text = "North East and Hilly",
                Value = "X"
            };

            List<SelectListItem> stateType = new List<SelectListItem>();
            stateType.Add(all);
            stateType.Add(regular);
            stateType.Add(island);
            stateType.Add(northEast);
            stateType.Add(hilly);
            stateType.Add(northEastAndHilly);
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;
            ViewData["STATE_TYPE"] = stateType;       

            return View();
        }

        [HttpPost]
    
        public ActionResult StateDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            string stateOrUnion = frmCollection["StateOrUnion"];
            string stateType = frmCollection["StateType"];
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.StateDetailsListingBAL(stateOrUnion, stateType,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        /****************************/

        /*********************************/

      
        public ActionResult createExcel()
        {

            List<USP_MAS_DATA_REPORT_Result> list = new List<USP_MAS_DATA_REPORT_Result>();
            list.Add(new USP_MAS_DATA_REPORT_Result()
            {
                MAST_STATE_CODE = 1,
                MAST_STATE_NAME = "Pranav",
                MAST_STATE_SHORT_CODE = "P",
                MAST_STATE_UT = "P",
                MAST_STATE_TYPE = "Good"
            });
            list.Add(new USP_MAS_DATA_REPORT_Result()
            {
                MAST_STATE_CODE = 2,
                MAST_STATE_NAME = "LOLZ",
                MAST_STATE_SHORT_CODE = "P",
                MAST_STATE_UT = "P",
                MAST_STATE_TYPE = "Good"
            });
            list.Add(new USP_MAS_DATA_REPORT_Result()
            {
                MAST_STATE_CODE = 3,
                MAST_STATE_NAME = "HA hA",
                MAST_STATE_SHORT_CODE = "P",
                MAST_STATE_UT = "P",
                MAST_STATE_TYPE = "Good"
            });
            string filepath = Server.MapPath("\\") + "Sample";
            //  CreateExcelFile.CreateExcelDocument(list,Server.MapPath("\\") + "Sample");
            return File(filepath, "application/vnd.ms-excel", "list.xlsx");
        }

  
        public ActionResult pressPDF()
        {
            bal = new MasterReportsBAL();
            int page = 0;
            int rows = 0;
            string sidx = "";
            string sord = "";
            int totalRecords = 0;
            string stateOrUnion = string.Empty;
            string stateType = string.Empty;
            string activeType = string.Empty;
            Array allRows = bal.StateDetailsListingBAL(stateOrUnion, stateType,activeType, page, rows, sidx, sord, out totalRecords);
            string[] columnNames = { "State Name", "State or Union Territory", "Type", "State Short Code" };
            PropertyInfo[] columnTypes = allRows.GetValue(0).GetType().GetProperties();
            List<string> columnTypeNames = columnTypes.Select(x => x.Name).ToList();
            columnTypeNames.Remove("MAST_STATE_CODE");
            string filepath = Server.MapPath("\\") + "Sample.pdf";
            StatePDF(allRows, columnNames, columnTypeNames, filepath);
            return File(filepath, "application/pdf", "list.pdf");
        }



    
        public void StatePDF(Array list, string[] columnNames, List<string> columnTypeNames, string filePath)
        {
            Font headerFont = FontFactory.GetFont("Verdana", 10, Color.BLACK);
            Font rowfont = FontFactory.GetFont("Verdana", 8, Color.BLACK);
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.OpenOrCreate));
            writer.PageEvent = new PageEventImplementer();
            document.Open();
            PdfPTable table = new PdfPTable(columnNames.Length);

            foreach (var column in columnNames)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column, headerFont));
                cell.BackgroundColor = Color.LIGHT_GRAY;
                table.AddCell(cell);
            }

            foreach (var item in list)
            {
                foreach (string typeName in columnTypeNames)
                {
                    string value = item.GetType().GetProperty(typeName).GetValue(item).ToString();
                    PdfPCell cell5 = new PdfPCell(new Phrase(value, rowfont));
                    table.AddCell(cell5);
                }
            }
            table.CalculateHeightsFast();
            string space = "                                                                                                                                                                                                              ";
            Phrase header = new Phrase("State Listing", rowfont);
            Phrase footer = new Phrase("CDAC e-Governance Solutions Group" + space + "Created at " + System.DateTime.Now, FontFactory.GetFont("Verdana", 7, Color.BLACK));
            document.AddTitle("State List");
            document.Footer = new HeaderFooter(footer, false);
            Paragraph heading = new Paragraph("State List");
            heading.SetAlignment("center");
            Paragraph heading2 = new Paragraph("");
            //heading.Chunks.
            document.Add(heading);
            document.Add(heading2);
            document.Add(table);
            document.Close();
        }
        [HttpPost]     
        public ActionResult DistrictDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string iapDistrict = frmCollection["IAP_DISTRICT"];
            string pmgsyIncluded = frmCollection["PMGSY_INCLUDED"];
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.DistrictDetailsListingBAL(stateCode, pmgsyIncluded, iapDistrict,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]     
        public ActionResult BlockDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string district = frmCollection["DistrictCode"] == "" ? "0" : frmCollection["DistrictCode"];
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(district) : PMGSYSession.Current.DistrictCode;
            string isDesert = frmCollection["IS_DESERT"];
            string isTribal = frmCollection["IS_TRIBAL"];
            string pmgsyIncluded = frmCollection["PMGSY_INCLUDED"];
            string schedule5 = frmCollection["IS_SCHEDULE5"];
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.BlockDetailsListingBAL(districtCode, stateCode, isDesert, isTribal, pmgsyIncluded, schedule5,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult VillageDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string district = frmCollection["DistrictCode"] == "" ? "0" : frmCollection["DistrictCode"];
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(district) : PMGSYSession.Current.DistrictCode;
            string isSchedule5 = frmCollection["IS_SCHEDULE5"];
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int censusYear = Convert.ToInt32(frmCollection["CENSUS_YEAR"]);
            int totalRecords;

            var inRows = bal.VillageDetailsListingBAL(censusYear, blockCode, districtCode, stateCode, isSchedule5,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult HabitationDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            string habitationStatus = frmCollection["HAB_STATUS"];
            string isSchedule5 = frmCollection["IS_SCHEDULE5"];
            int censusYear = Convert.ToInt32(frmCollection["CENSUS_YEAR"]);
            int villageCode = Convert.ToInt32(frmCollection["VillageCode"]);
            int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            int districtCode = Convert.ToInt32(frmCollection["DistrictCode"]);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.HabitationDetailsListingBAL(censusYear, villageCode, blockCode, districtCode, stateCode, habitationStatus, isSchedule5,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult MPConstituencyDetails()
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true; //change 11/12/2013

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;         
  
            ViewData["STATE"] = stateDd;
            return View(MasterReportViewModel);
        }

        [HttpPost]    
        public ActionResult MPConstituencyListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.MPConstituencyListingBAL(stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult MLAConstituencyDetails()
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true; //change 11/12/2013

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType; 
            ViewData["STATE"] = stateDd;
            return View(MasterReportViewModel);
        }

        [HttpPost]
       
        public ActionResult MLAConstituencyListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.MLAConstituencyListingBAL(stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult MPBlockDetails(FormCollection frmCollection)
        {
            
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
          
            ViewData["STATE"] = stateDd;

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem
                {
                    Text = "All Constituencies",
                    Value = "0"
                });
                foreach (SelectListItem i in new MasterReportsDAL().PopulateMPConstituency(Convert.ToInt32(frmCollection["StateCode"])))
                {
                    list.Add(i);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem
                {
                    Text = "All Constituencies",
                    Value = "0"
                });
                foreach (SelectListItem i in new MasterReportsDAL().PopulateMPConstituency(stateCode))
                {
                    list.Add(i);
                }
                ViewData["MP_CONSTITUENCY"] = list;
            }
            if (stateCode == 0)
            {
                List<SelectListItem> allConstituency = new List<SelectListItem>();
                allConstituency.Add(new SelectListItem
                {
                    Text = "All Constituencies",
                    Value = "0"
                });

                ViewData["MP_CONSTITUENCY"] = allConstituency;
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;             
            return View(MasterReportViewModel);
        }

        [HttpPost]
      
        public ActionResult MPBlockListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string constituency = frmCollection["ConstituencyCode"] == "" ? "0" : frmCollection["ConstituencyCode"];
            int constCode = Convert.ToInt32(constituency);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.MPBlockListingBAL(constCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


     
        public ActionResult MLABlockDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            ViewData["STATE"] = stateDd;
            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem
                {
                    Text = "All Constituencies",
                    Value = "0"
                });
                foreach (SelectListItem i in new MasterReportsDAL().PopulateMLAConstituency(Convert.ToInt32(frmCollection["StateCode"])))
                {
                    list.Add(i);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem
                {
                    Text = "All Constituencies",
                    Value = "0"
                });
                foreach (SelectListItem i in new MasterReportsDAL().PopulateMLAConstituency(stateCode))
                {
                    list.Add(i);
                }
                ViewData["MLA_CONSTITUENCY"] = list;
            }
            if (stateCode == 0)
            {
                List<SelectListItem> allConstituency = new List<SelectListItem>();
                allConstituency.Add(new SelectListItem
                {
                    Text = "All Constituencies",
                    Value = "0"
                });
                ViewData["MLA_CONSTITUENCY"] = allConstituency;
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType; 
            
            return View(MasterReportViewModel);
        }

        [HttpPost]
     
        public ActionResult MLABlockListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string constituency = frmCollection["ConstituencyCode"] == "" ? "0" : frmCollection["ConstituencyCode"];
            int constCode = Convert.ToInt32(constituency);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.MLABlockListingBAL(constCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult PanchayatDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            //  stateDd.Find(x => x.Value == "0").Text = "All States";
            List<SelectListItem> districtDd = common.PopulateDistrict(stateCode);
            districtDd.Find(x => x.Value == "0").Text = "All Districts";
            List<SelectListItem> blockDd = common.PopulateBlocks(districtCode);
            blockDd.Find(x => x.Value == "0").Text = "All Blocks";

            if (districtCode > 0)
            {
                districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
            }

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
            {
                List<SelectListItem> list = common.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
            {
                List<SelectListItem> list = common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;         
            ViewData["STATE"] = stateDd;
            ViewData["DISTRICT"] = districtDd;
            ViewData["BLOCK"] = blockDd;

            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult PanchayatListing(FormCollection frmCollection)
        {

            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string district = frmCollection["DistrictCode"] == "" ? "0" : frmCollection["DistrictCode"];
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(district) : PMGSYSession.Current.DistrictCode;
            int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.PanchayatListingBAL(blockCode, districtCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


       
        public ActionResult PanchayatHabitationDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            // stateDd.Find(x => x.Value == "0").Text = "Select State";
            List<SelectListItem> districtDd = common.PopulateDistrict(stateCode);
            districtDd.Find(x => x.Value == "0").Text = "Select District";
            List<SelectListItem> blockDd = common.PopulateBlocks(districtCode);
            blockDd.Find(x => x.Value == "0").Text = "Select Block";
            List<SelectListItem> panchayatDd = new List<SelectListItem>();
            panchayatDd.Add(new SelectListItem
            {
                Text = "All Panchayats",
                Value = "0",
                Selected = true
            });
            //panchayatDd.Concat(new MasterReportsDAL().PopulatePanchayat(Convert.ToInt32(frmCollection["BlockCode"]),districtCode,stateCode));

            if (districtCode > 0)
            {
                districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
            }

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["BlockCode"]) > 0)
            {

                List<SelectListItem> list = new MasterReportsDAL().PopulatePanchayat(Convert.ToInt32(frmCollection["BlockCode"]), Convert.ToInt32(frmCollection["DistrictCode"]),
                    Convert.ToInt32(frmCollection["StateCode"]));

                list.Insert(0, new SelectListItem
                {
                    Text = "All Panchayats",
                    Value = "0",
                    Selected = true
                });
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
            {
                return Json(common.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"])), JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
            {
                return Json(common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"])), JsonRequestBehavior.AllowGet);
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType; 
            
            ViewData["STATE"] = stateDd;
            ViewData["DISTRICT"] = districtDd;
            ViewData["BLOCK"] = blockDd;
            ViewData["PANCHAYAT"] = panchayatDd;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult PanchayatHabitationListing(FormCollection frmCollection)
        {

            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string district = frmCollection["DistrictCode"] == "" ? "0" : frmCollection["DistrictCode"];
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(district) : PMGSYSession.Current.DistrictCode;
            int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);
            int panchayatCode = Convert.ToInt32(frmCollection["PanchayatCode"]);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.PanchayatHabitationListingBAL(panchayatCode, blockCode, districtCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult RegionDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType; 
            
            return View(MasterReportViewModel);
        }

        [HttpPost]
       
        public ActionResult RegionDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.RegionListingBAL(stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult RegionDistrictDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            } 
            ViewData["STATE"] = stateDd;
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "All Regions",
                Value = "0"
            });

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                foreach (SelectListItem i in new MasterReportsDAL().PopulateRegion(Convert.ToInt32(frmCollection["StateCode"])))
                {
                    list.Add(i);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            if (stateCode > 0)
            {
                foreach (SelectListItem i in new MasterReportsDAL().PopulateRegion(stateCode))
                {
                    list.Add(i);
                }
                ViewData["REGION"] = list;
            }
            if (stateCode == 0)
            {
                ViewData["REGION"] = list;
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType; 
            
            return View(MasterReportViewModel);
        }

        [HttpPost]      
        public ActionResult RegionDistrictDetailsListing(FormCollection frmCollection)
        {

            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string region = frmCollection["RegionCode"] == "" ? "0" : frmCollection["RegionCode"];
            int regionCode = Convert.ToInt32(region);
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.RegionDistrictListingBAL(regionCode, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult UnitDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]      
        public ActionResult UnitDetailsListing(FormCollection frmCollection)
        {

            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.UnitListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


     
        public ActionResult RoadCategoryDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]       
        public ActionResult RoadCategoryDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.RoadCategoryListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        
        public ActionResult ScourFoundationDetails(FormCollection frmCollection)
        {
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%",
                Selected = true
            };
            SelectListItem scour = new SelectListItem
            {
                Text = "Scour",
                Value = "S"
            };
            SelectListItem foundation = new SelectListItem
            {
                Text = "Foundation",
                Value = "F"
            };
            List<SelectListItem> scourFoundationType = new List<SelectListItem>();
            scourFoundationType.Add(all);
            scourFoundationType.Add(scour);
            scourFoundationType.Add(foundation);
            ViewData["IMS_SC_FD_TYPE"] = scourFoundationType;
            return View();
        }

        [HttpPost]      
        public ActionResult ScourFoundationDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string scourFoundationType = frmCollection["IMS_SC_FD_TYPE"];
            int totalRecords;

            var inRows = bal.ScourFoundationListingBAL(scourFoundationType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


      
        public ActionResult SoilDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]       
        public ActionResult SoilDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.SoilListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


      
        public ActionResult StreamDetails(FormCollection frmCollection)
        {
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem pmgsy = new SelectListItem
            {
                Text = "PMGSY",
                Value = "P"
            };
            SelectListItem state = new SelectListItem
            {
                Text = "State",
                Value = "S"
            };
            SelectListItem others = new SelectListItem
            {
                Text = "Others",
                Value = "O"
            };
            List<SelectListItem> streamType = new List<SelectListItem>();
            streamType.Add(all);
            streamType.Add(pmgsy);
            streamType.Add(state);
            streamType.Add(others);

            ViewData["MAST_STREAM_TYPE"] = streamType;
            return View();
        }

        [HttpPost]    
        public ActionResult StreamDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string streamType = frmCollection["MAST_STREAM_TYPE"];
            int totalRecords;

            var inRows = bal.StreamListingBAL(streamType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult TerrainDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]       
        public ActionResult TerrainDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.TerrainListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        
        public ActionResult CDWorksLengthDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]       
        public ActionResult CDWorksLengthDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.CDWorksLengthListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult CDWorksTypeDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]      
        public ActionResult CDWorksTypeDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.CDWorksTypeListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult ComponentDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]        
        public ActionResult ComponentDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.ComponentListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


      
        public ActionResult GradeDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]        
        public ActionResult GradeDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.GradeListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


      
        public ActionResult DesignationDetails(FormCollection frmCollection)
        {
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            SelectListItem nodalOfficers = new SelectListItem
            {
                Text = "Nodal Officers",
                Value = "NO"
            };
            SelectListItem technicalAgency = new SelectListItem
            {
                Text = "Technical Agency",
                Value = "TA"
            };
            SelectListItem qualityControl = new SelectListItem
            {
                Text = "Quality Control",
                Value = "QC"
            };
            SelectListItem qualityMonitor = new SelectListItem
            {
                Text = "Quality Monitor",
                Value = "QM"
            };
            List<SelectListItem> designationType = new List<SelectListItem>();
            designationType.Add(all);
            designationType.Add(nodalOfficers);
            designationType.Add(technicalAgency);
            designationType.Add(qualityControl);
            designationType.Add(qualityMonitor);
            ViewData["MAST_DESIG_TYPE"] = designationType;
            return View();
        }

        [HttpPost]
       
        public ActionResult DesignationDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string designationType = frmCollection["MAST_DESIG_TYPE"];
            int totalRecords;

            var inRows = bal.DesignationListingBAL(designationType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult ReasonDetails(FormCollection frmCollection)
        {
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            SelectListItem habitationConnected = new SelectListItem
            {
                Text = "Habitation not Connected",
                Value = "H"
            };
            SelectListItem proposalNotIncluded = new SelectListItem
            {
                Text = "Proposal not Included",
                Value = "I"
            };
            SelectListItem delayInProject = new SelectListItem
            {
                Text = "Delay in Project",
                Value = "D"
            };
            SelectListItem accounts = new SelectListItem
            {
                Text = "Accounts",
                Value = "A"
            };
            SelectListItem sanctionRejection = new SelectListItem
            {
                Text = "Sanction Rejection",
                Value = "S"
            };

            List<SelectListItem> reasonType = new List<SelectListItem>();
            reasonType.Add(all);
            reasonType.Add(habitationConnected);
            reasonType.Add(proposalNotIncluded);
            reasonType.Add(delayInProject);
            reasonType.Add(accounts);
            reasonType.Add(sanctionRejection);

            ViewData["MAST_REASON_TYPE"] = reasonType;
            return View();
        }

        [HttpPost]       
        public ActionResult ReasonDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string reasonType = frmCollection["MAST_REASON_TYPE"];
            int totalRecords;

            var inRows = bal.ReasonListingBAL(reasonType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult CheckListPointDetails(FormCollection frmCollection)
        {
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            List<SelectListItem> checkListActiveList = new List<SelectListItem>();
            checkListActiveList.Add(all);
            checkListActiveList.Add(yes);
            checkListActiveList.Add(no);

            ViewData["MAST_CHECKLIST_ACTIVE"] = checkListActiveList;
            return View();
        }

        [HttpPost]
       
        public ActionResult CheckListPointDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string checkListActive = frmCollection["MAST_CHECKLIST_ACTIVE"];
            int totalRecords;

            var inRows = bal.CheckListPointListingBAL(checkListActive, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


     
        public ActionResult ExecutionItemDetails(FormCollection frmCollection)
        {
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            SelectListItem roads = new SelectListItem
            {
                Text = "Roads",
                Value = "R"
            };

            SelectListItem bridges = new SelectListItem
            {
                Text = "Bridges",
                Value = "L"
            };
            List<SelectListItem> headType = new List<SelectListItem>();
            headType.Add(all);
            headType.Add(roads);
            headType.Add(bridges);

            ViewData["MAST_HEAD_TYPE"] = headType;
            return View();
        }

        [HttpPost]       
        public ActionResult ExecutionItemDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string headType = frmCollection["MAST_HEAD_TYPE"];
            int totalRecords;

            var inRows = bal.ExecutionItemListingBAL(headType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult TaxesDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]        
        public ActionResult TaxesDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.TaxesListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult AgencyDetails(FormCollection frmCollection)
        {
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            SelectListItem government = new SelectListItem
            {
                Text = "Government",
                Value = "Government"
            };

            SelectListItem others = new SelectListItem
            {
                Text = "Others",
                Value = "Others"
            };

            List<SelectListItem> type1 = new List<SelectListItem>();
            type1.Add(all);
            type1.Add(government);
            type1.Add(others);

            ViewData["MAST_AGENCY_TYPE"] = type1;
            return View();
        }

        [HttpPost]       
        public ActionResult AgencyDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string agencyType = frmCollection["MAST_AGENCY_TYPE"];
            int totalRecords;

            var inRows = bal.AgencyListingBAL(agencyType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult TrafficDetails(FormCollection frmCollection)
        {
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            List<SelectListItem> trafficStatus = new List<SelectListItem>();
            trafficStatus.Add(all);
            trafficStatus.Add(yes);
            trafficStatus.Add(no);

            ViewData["MAST_TRAFFIC_STATUS"] = trafficStatus;
            return View();
        }

        [HttpPost]
                public ActionResult TrafficDetailsListing(FormCollection frmCollection)
        {

            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string trafficStatus = frmCollection["MAST_TRAFFIC_STATUS"];
            int totalRecords;

            var inRows = bal.TrafficListingBAL(trafficStatus, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult FundingAgencyDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]       
        public ActionResult FundingAgencyDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.FundingAgencyListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult QualificationDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]        
        public ActionResult QualificationDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.QualificationListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        
        public ActionResult LokSabhaTermDetails(FormCollection frmCollection)
        {
            return View();
        }

        [HttpPost]       
        public ActionResult LokSabhaTermDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.LokSabhaTermListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [Audit]
        public ActionResult ContractorSupplierDetails(FormCollection frmCollection)
        {
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Insert(0, new SelectListItem { Text = "All State", Value = "0" });
            stateDd.Find(x => x.Value == "1").Selected = true;
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
          
            SelectListItem all = new SelectListItem
            {
                
                Text = "All",
                Value = "%",
            };
            SelectListItem contractor = new SelectListItem
            {
               
                Text = "Contractor",
                Value = "C",
                Selected = true
            };
            SelectListItem supplier = new SelectListItem
            {
                Text = "Supplier",
                Value = "S"
            };
            SelectListItem dprAgency = new SelectListItem
            {
                Text = "DPR Agency",
                Value = "D"
            };
            SelectListItem government = new SelectListItem
            {
                Text = "Government",
                Value = "G"
            };

            SelectListItem active = new SelectListItem
            {
                
                Text = "Active",
                Value = "A",
                Selected = true
            };
            SelectListItem inactive = new SelectListItem
            {
                Text = "Inactive",
                Value = "I"
            };
            SelectListItem blackListed = new SelectListItem
            {
                Text = "BlackListed",
                Value = "B"
            };
            SelectListItem expired = new SelectListItem
            {
                Text = "Expired",
                Value = "E"
            };

            List<SelectListItem> contractorSupplierFlag = new List<SelectListItem>();
            contractorSupplierFlag.Add(all);
            contractorSupplierFlag.Add(contractor);
            contractorSupplierFlag.Add(supplier);
            contractorSupplierFlag.Add(dprAgency);
            contractorSupplierFlag.Add(government);

            List<SelectListItem> contractStatus = new List<SelectListItem>();
            contractStatus.Add(all);
            contractStatus.Add(active);
            contractStatus.Add(inactive);
            contractStatus.Add(blackListed);
            contractStatus.Add(expired);


            ViewData["MAST_CON_SUP_FLAG"] = new SelectList(contractorSupplierFlag,"Value","Text","C");
            ViewData["MAST_CON_STATUS"] = new SelectList(contractStatus, "Value", "Text", "A");
            ViewData["STATE"] = stateDd;
            return View();
        }

        [HttpPost]        
        public ActionResult ContractorSupplierDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            string contractorSupplierFlag = frmCollection["MAST_CON_SUP_FLAG"];
            string contractStatus = frmCollection["MAST_CON_STATUS"];
            int StateCode = Convert.ToInt32(frmCollection["STATE"]);
            int totalRecords;

            var inRows = bal.ContractorSupplierListingBAL(StateCode,contractorSupplierFlag, contractStatus, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };           
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult AutonomousBodyDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult AutonomousBodyDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.AutonomousBodyListingBAL(stateCode, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


       
        public ActionResult ContractorClassTypeDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            //  stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult ContractorClassTypeDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.ContractorClassTypeListingBAL(stateCode, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        
        public ActionResult OfficerCategoryDetails()
        {
            return View();
        }

        [HttpPost]       
        public ActionResult OfficerCategoryDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.OfficerCategoryListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        
        public ActionResult ContractorRegistrationDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;

            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            SelectListItem active = new SelectListItem
            {
                Text = "Active",
                Value = "A"
            };

            SelectListItem inactive = new SelectListItem
            {
                Text = "Inactive",
                Value = "I"
            };

            SelectListItem blackListed = new SelectListItem
            {
                Text = "Blacklisted",
                Value = "B"
            };

            SelectListItem expired = new SelectListItem
            {
                Text = "Expired",
                Value = "E"
            };

            List<SelectListItem> registrationStatus = new List<SelectListItem>();
            registrationStatus.Add(all);
            registrationStatus.Add(active);
            registrationStatus.Add(inactive);

            List<SelectListItem> activeStatus = new List<SelectListItem>();
            activeStatus.Add(all);
            activeStatus.Add(active);
            activeStatus.Add(inactive);
            activeStatus.Add(blackListed);
            activeStatus.Add(expired);

            ViewData["ACTIVE_STATUS"] = activeStatus;
            ViewData["REGISTRATION_STATUS"] = registrationStatus;

            return View(MasterReportViewModel);
        }

        [HttpPost]        
        public ActionResult ContractorRegistrationDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string activeStatus = frmCollection["ActiveStatus"];
            string registrationStatus = frmCollection["RegistrationStatus"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.ContractorRegistrationListingBAL(activeStatus, registrationStatus, stateCode, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }




        
        public ActionResult ContractorRegistrationBankDetails()
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
             
            int stateCode = PMGSYSession.Current.StateCode;

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            SelectListItem active = new SelectListItem
            {
                Text = "Active",
                Value = "A"
            };
            SelectListItem inactive = new SelectListItem
            {
                Text = "Inactive",
                Value = "I"
            };
            SelectListItem blacklisted = new SelectListItem
            {
                Text = "Blacklisted",
                Value = "B"
            };
            SelectListItem expired = new SelectListItem
            {
                Text = "Expired",
                Value = "E"
            };

            SelectListItem contractor = new SelectListItem
            {
                Text = "Contractor",
                Value = "C"
            };

            SelectListItem supplier = new SelectListItem
            {
                Text = "Supplier",
                Value = "S"
            };
            SelectListItem dprAgency = new SelectListItem
            {
                Text = "DPR Agency",
                Value = "D"
            };
            SelectListItem government = new SelectListItem
            {
                Text = "Government",
                Value = "G"
            };

            List<SelectListItem> contrStatus = new List<SelectListItem>();
            contrStatus.Add(all);
            contrStatus.Add(active);
            contrStatus.Add(inactive);
            contrStatus.Add(blacklisted);
            contrStatus.Add(expired);

            List<SelectListItem> contrSupFlag = new List<SelectListItem>();
            contrSupFlag.Add(all);
            contrSupFlag.Add(contractor);
            contrSupFlag.Add(supplier);
            contrSupFlag.Add(dprAgency);
            contrSupFlag.Add(government);

            ViewData["Contr_Sup_Flag"] = contrSupFlag;
            ViewData["Contr_STATUS"] = contrStatus;
            ViewData["STATE"] = stateDd;

            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult ContractorRegistrationBankDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string contractRegFlag = frmCollection["ContractRegFlag"];
            string contractStatus = frmCollection["ContractStatus"];

            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.ContractorRegistrationBankListingBAL(stateCode, contractRegFlag, contractStatus, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }




        
        public ActionResult DepartmentDistrictDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;


            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };


            SelectListItem srrda = new SelectListItem
            {
                Text = "SRRDA",
                Value = "S"
            };

            SelectListItem dpiu = new SelectListItem
            {
                Text = "DPIU",
                Value = "D"
            };
            List<SelectListItem> agency = new List<SelectListItem>();
            agency.Add(all);

            List<SelectListItem> officeType = new List<SelectListItem>();
            officeType.Add(all);
            officeType.Add(srrda);
            officeType.Add(dpiu);
            MasterReportsDAL dal = new MasterReportsDAL();
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                List<SelectListItem> list = dal.PopulateAgency(Convert.ToInt32(frmCollection["StateCode"]));
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            ViewData["OFFICE"] = officeType;
            ViewData["STATE"] = stateDd;
            ViewData["AGENCY"] = agency;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult DepartmentDistrictDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string agency = frmCollection["AgencyCode"] == "" ? "0" : frmCollection["AgencyCode"];
            int agencyCode = Convert.ToInt32(agency);
            string officeType = frmCollection["OfficeType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.DepartmentDistrictListingBAL(stateCode, agencyCode, officeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CarriageDetails()
        {
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };


            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };

            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> carriageStatus = new List<SelectListItem>();
            carriageStatus.Add(all);
            carriageStatus.Add(yes);
            carriageStatus.Add(no);
            ViewData["CarriageStatus"] = carriageStatus;
            return View();
        }

        [HttpPost]
     
        public ActionResult CarriageDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            string carriageStatus = frmCollection["CarriageStatus"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.CarriageListingBAL(carriageStatus, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult QMItemsDetails()
        {
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };


            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };

            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem nqm = new SelectListItem
            {
                Text = "NQM",
                Value = "N"
            };
            SelectListItem sqm = new SelectListItem
            {
                Text = "SQM",
                Value = "S"
            };

            List<SelectListItem> qmItemActiveStatus = new List<SelectListItem>();
            qmItemActiveStatus.Add(all);
            qmItemActiveStatus.Add(yes);
            qmItemActiveStatus.Add(no);

            List<SelectListItem> qmType = new List<SelectListItem>();
            qmType.Add(all);
            qmType.Add(nqm);
            qmType.Add(sqm);
            ViewData["QMType"] = qmType;
            ViewData["QMItemActive"] = qmItemActiveStatus;
            return View();
        }

        [HttpPost]     
        public ActionResult QMItemsDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            string qmType = frmCollection["QMType"];
            string qmItemActive = frmCollection["QMItemActive"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.QMItemsListingBAL(qmType, qmItemActive, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }



      
        public ActionResult TechnologyDetails()
        {
            return View();
        }

        [HttpPost]       
        public ActionResult TechnologyDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.TechnologyListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }



       
        public ActionResult TestDetails()
        {
            return View();
        }

        [HttpPost]        
        public ActionResult TestDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.TestListingBAL(page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult SQCDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;

            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };

            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };

            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };

            List<SelectListItem> activeStatus = new List<SelectListItem>();
            activeStatus.Add(all);
            activeStatus.Add(yes);
            activeStatus.Add(no);

            ViewData["ACTIVE"] = activeStatus;

            return View(MasterReportViewModel);
        }

        [HttpPost]        
        public ActionResult SQCDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string activeStatus = frmCollection["ActiveStatus"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.SQCListingBAL(activeStatus, stateCode, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        
        public ActionResult SRRDADetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;


            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };

            SelectListItem srrda = new SelectListItem
            {
                Text = "SRRDA",
                Value = "S"
            };

            SelectListItem dpiu = new SelectListItem
            {
                Text = "DPIU",
                Value = "D"
            };
            List<SelectListItem> agency = new List<SelectListItem>();
            agency.Add(all);

            List<SelectListItem> officeType = new List<SelectListItem>();
            officeType.Add(all);
            officeType.Add(srrda);
            officeType.Add(dpiu);
            MasterReportsDAL dal = new MasterReportsDAL();
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                List<SelectListItem> list = dal.PopulateAgency(Convert.ToInt32(frmCollection["StateCode"]));
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            ViewData["OFFICE"] = officeType;
            ViewData["STATE"] = stateDd;
            ViewData["AGENCY"] = agency;

            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult SRRDADetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string agency = frmCollection["AgencyCode"] == "" ? "0" : frmCollection["AgencyCode"];
            int agencyCode = Convert.ToInt32(agency);
            string officeType = frmCollection["OfficeType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.SRRDAListingBAL(stateCode, agencyCode, officeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult VidhanSabhaTermDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult VidhanSabhaTermDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.VidhanSabhaTermListingBAL(stateCode, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


      
        public ActionResult NodalOfficerDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };

            SelectListItem srrda = new SelectListItem
            {
                Text = "SRRDA",
                Value = "S"
            };

            SelectListItem dpiu = new SelectListItem
            {
                Text = "DPIU",
                Value = "D"
            };


            SelectListItem yes = new SelectListItem
            {
                Text = "YES",
                Value = "Y"
            };

            SelectListItem no = new SelectListItem
            {
                Text = "NO",
                Value = "N"
            };
            List<SelectListItem> active = new List<SelectListItem>();
            active.Add(all);
            active.Add(yes);
            active.Add(no);

            List<SelectListItem> agency = new List<SelectListItem>();
            agency.Add(all);

            List<SelectListItem> officeType = new List<SelectListItem>();
            officeType.Add(all);
            officeType.Add(srrda);
            officeType.Add(dpiu);
            MasterReportsDAL dal = new MasterReportsDAL();
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                List<SelectListItem> list = dal.PopulateAgency(Convert.ToInt32(frmCollection["StateCode"]));
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            ViewData["OFFICE"] = officeType;
            ViewData["STATE"] = stateDd;
            ViewData["AGENCY"] = agency;
            ViewData["ACTIVE"] = active;

            return View(MasterReportViewModel);
        }

        [HttpPost]      
        public ActionResult NodalOfficerDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string agency = frmCollection["AgencyCode"] == "" ? "0" : frmCollection["AgencyCode"];
            int agencyCode = Convert.ToInt32(agency);
            string officeType = frmCollection["OfficeType"];
            string activeType = frmCollection["ActiveType"];

            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.NodalOfficerListingBAL(stateCode, agencyCode, officeType, activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult QualityMonitorDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };

            SelectListItem nqm = new SelectListItem
            {
                Text = "NQM",
                Value = "I"
            };

            SelectListItem sqm = new SelectListItem
            {
                Text = "SQM",
                Value = "S"
            };

            SelectListItem dpiu = new SelectListItem
            {
                Text = "DPIU",
                Value = "P"
            };

            SelectListItem director = new SelectListItem
            {
                Text = "Director",
                Value = "D"
            };

            SelectListItem yes = new SelectListItem
            {
                Text = "YES",
                Value = "Y"
            };

            SelectListItem no = new SelectListItem
            {
                Text = "NO",
                Value = "N"
            };
            List<SelectListItem> active = new List<SelectListItem>();
            active.Add(all);
            active.Add(yes);
            active.Add(no);


            List<SelectListItem> qmType = new List<SelectListItem>();
            qmType.Add(all);
            qmType.Add(nqm);
            qmType.Add(sqm);
            qmType.Add(dpiu);
            qmType.Add(director);
            MasterReportsDAL dal = new MasterReportsDAL();
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }


            ViewData["QMTYPE"] = qmType;
            ViewData["STATE"] = stateDd;
            ViewData["ACTIVE"] = active;

            return View(MasterReportViewModel);
        }

        [HttpPost]        
        public ActionResult QualityMonitorDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string qmType = frmCollection["QmType"];
            string activeType = frmCollection["ActiveType"];

            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.QualityMonitorListingBAL(stateCode, qmType, activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        
        public ActionResult TechnicalAgencyDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem sta = new SelectListItem
            {
                Text = "STA",
                Value = "S"
            };

            SelectListItem pta = new SelectListItem
            {
                Text = "PTA",
                Value = "P"
            };
            List<SelectListItem> taType = new List<SelectListItem>();
            taType.Add(all);
            taType.Add(pta);
            taType.Add(sta);
            ViewData["STATE"] = stateDd;
            ViewData["TATYPE"] = taType;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult TechnicalAgencyDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string taType = frmCollection["TaType"];

            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.TechnicalAgencyListingBAL(stateCode, taType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

       

        
        public ActionResult TechnicalAgencyStateMappingDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            SelectListItem pta = new SelectListItem
            {
                Selected = true,
                Text = "PTA",
                Value = "P"
            };
            SelectListItem sta = new SelectListItem
            {
             
                Text = "STA",
                Value = "S"
            };
            SelectListItem yes = new SelectListItem
            {

                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {

                Text = "No",
                Value = "N"
            };

            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);

            List<SelectListItem> taType = new List<SelectListItem>();           
            taType.Add(pta);
            taType.Add(sta);


            ViewData["STATE"] = stateDd;
            ViewData["TATYPE"] = taType;
            ViewData["ACTIVE"] = activeType;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult TechnicalAgencyStateMappingDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string taType = frmCollection["TaType"];
            string activeType = frmCollection["Active"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.TechnicalAgencyStateMappingListingBAL(stateCode, taType,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        
        public ActionResult FeedbackDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
           
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult FeedbackDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;
            var inRows = bal.FeedbackListingBAL(page, rows, sidx, sord, out totalRecords);
            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

   
       
        public ActionResult MLAMemberDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "0"
            };
            List<SelectListItem> constituencyList = new List<SelectListItem>();
            constituencyList.Add(all);
            List<SelectListItem> termList = new List<SelectListItem>();
            termList.Add(all);
            MasterReportsDAL dal = new MasterReportsDAL();
            if (Convert.ToInt32(frmCollection["StateCode"]) > 0 && Convert.ToInt32(frmCollection["ConstituencyFlag"]) == 0)
            {
                foreach (SelectListItem i in dal.PopulateMLAConstituency(Convert.ToInt32(frmCollection["StateCode"])))
                {
                    constituencyList.Add(i);
                }
                return Json(constituencyList, JsonRequestBehavior.AllowGet);
            }
            if (stateCode > 0)
            {
                foreach (SelectListItem i in dal.PopulateMLAConstituency(stateCode))
                {
                    constituencyList.Add(i);
                }
                termList.Clear();
                termList.Add(all);
                foreach (SelectListItem i in dal.PopulateVidhanSabhaTerm(stateCode))
                {
                    termList.Add(i);
                }
             
                ViewData["TERM"] = termList;
                ViewData["MLA_CONSTITUENCY"] = constituencyList;
            }

            if (Convert.ToInt32(frmCollection["ConstituencyFlag"]) == 1)
            {
                termList.Clear();
                termList.Add(all);
                foreach (SelectListItem i in dal.PopulateVidhanSabhaTerm(Convert.ToInt32(frmCollection["StateCode"])))
                {
                    termList.Add(i);
                }
                return Json(termList, JsonRequestBehavior.AllowGet);
            }
            if (stateCode == 0)
            {
                ViewData["TERM"] = termList;
                ViewData["MLA_CONSTITUENCY"] = constituencyList;
            }
            SelectListItem allactive = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(allactive);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;
            return View(MasterReportViewModel);
        }

        [HttpPost]        
        public ActionResult MLAMemberDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string constituency = frmCollection["MLAConstituency"] == "" ? "0" : frmCollection["MLAConstituency"];
            string term = frmCollection["Term"] == "" ? "0" : frmCollection["Term"];
            string activeType = frmCollection["ActiveType"];
          
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.MLAMemberListingBAL(constituency, term, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult MPMemberDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
           
            int stateCode = PMGSYSession.Current.StateCode;
            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }
            ViewData["STATE"] = stateDd;
            SelectListItem all = new SelectListItem
            {
                Text = "All",
                Value = "0"
            };
            List<SelectListItem> constituencyList = new List<SelectListItem>();
            constituencyList.Add(all);
            List<SelectListItem> termList = new List<SelectListItem>();
            termList.Add(all);
            MasterReportsDAL dal = new MasterReportsDAL();
            if (Convert.ToInt32(frmCollection["StateCode"]) > 0 && Convert.ToInt32(frmCollection["ConstituencyFlag"]) == 0)
            {
                foreach (SelectListItem i in dal.PopulateMPConstituency(Convert.ToInt32(frmCollection["StateCode"])))
                {
                    constituencyList.Add(i);
                }
                return Json(constituencyList, JsonRequestBehavior.AllowGet);
            }
            if (stateCode > 0)
            {
                foreach (SelectListItem i in dal.PopulateMPConstituency(stateCode))
                {
                    constituencyList.Add(i);
                }
                termList.Clear();
                termList.Add(all);
                foreach (SelectListItem i in dal.PopulateLokSabhaTerm())
                {
                    termList.Add(i);
                }

                ViewData["MP_CONSTITUENCY"] = constituencyList;
                ViewData["TERM"] = termList;
            }
            if (Convert.ToInt32(frmCollection["ConstituencyFlag"]) == 1)
            {
                termList.Clear();
                termList.Add(all);
                foreach (SelectListItem i in dal.PopulateLokSabhaTerm())
                {
                    termList.Add(i);
                }
                return Json(termList, JsonRequestBehavior.AllowGet);
            }
            if (stateCode == 0)
            {
                ViewData["MP_CONSTITUENCY"] = constituencyList;
                ViewData["TERM"] = termList;
            }
            SelectListItem allactive = new SelectListItem
            {
                Text = "All",
                Value = "%"
            };
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(allactive);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;
            return View(MasterReportViewModel);
        }

        [HttpPost]       
        public ActionResult MPMemberDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            string constituency = frmCollection["MPConstituency"] == "" ? "0" : frmCollection["MPConstituency"];
            string term = frmCollection["Term"] == "" ? "0" : frmCollection["Term"];
            string activeType = frmCollection["ActiveType"];
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.MPMemberListingBAL(constituency, term, stateCode,activeType, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        
        public ActionResult SurfaceDetails(FormCollection frmCollection)
        {
           
            return View();
        }

        [HttpPost]        
        public ActionResult SurfaceDetailsListing(FormCollection frmCollection)
        {
            //Adde By Abhishek kamble 1-May-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 1-May-2014 end
            bal = new MasterReportsBAL();
            int stateCode = Convert.ToInt32(frmCollection["StateCode"]);
            int page = Convert.ToInt32(frmCollection["page"]) - 1;
            int rows = Convert.ToInt32(frmCollection["rows"]);
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            int totalRecords;

            var inRows = bal.SurfaceListingBAL(stateCode, page, rows, sidx, sord, out totalRecords);

            var jsonData = new
            {
                rows = inRows,
                total = totalRecords <= Convert.ToInt32(frmCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(frmCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(frmCollection["rows"]) : (totalRecords / Convert.ToInt32(frmCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords,

            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


       
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            int stateCode = PMGSYSession.Current.StateCode;
            // State Drop Down
            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
           // stateDd.Find(x => x.Value == "0").Text = "All States";
            // stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            ViewData["STATE"] = stateDd;
            // Other Filters           
            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            List<SelectListItem> iapDistrict = new List<SelectListItem>();
            iapDistrict.Add(all);
            iapDistrict.Add(yes);
            iapDistrict.Add(no);

            List<SelectListItem> pmgsyIncludedDd = new List<SelectListItem>();
            pmgsyIncludedDd.Add(all);
            pmgsyIncludedDd.Add(yes);
            pmgsyIncludedDd.Add(no);
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;
            ViewData["IAP_DISTRICT"] = iapDistrict;
            ViewData["PMGSY_INCLUDED"] = pmgsyIncludedDd;
            return View(MasterReportViewModel);
        }


        
        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;

            List<SelectListItem> stateDd = common.PopulateStates(false);
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            //stateDd.Find(x => x.Value == "0").Text = "All States";
            List<SelectListItem> districtDd = common.PopulateDistrict(stateCode);
            districtDd.Find(x => x.Value == "0").Text = "All District";

            if (districtCode > 0)
            {
                districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
            }

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
            {
                // return Json(common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]),true), JsonRequestBehavior.AllowGet);

                List<SelectListItem> list = common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                list.Find(x => x.Value == "-1").Value = "0";
                if (districtCode > 0)
                {
                    list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            List<SelectListItem> isDesert = new List<SelectListItem>();
            isDesert.Add(all);
            isDesert.Add(yes);
            isDesert.Add(no);

            List<SelectListItem> pmgsyIncludedDd = new List<SelectListItem>();
            pmgsyIncludedDd.Add(all);
            pmgsyIncludedDd.Add(yes);
            pmgsyIncludedDd.Add(no);

            List<SelectListItem> schedule5 = new List<SelectListItem>();
            schedule5.Add(all);
            schedule5.Add(yes);
            schedule5.Add(no);

            List<SelectListItem> isTribal = new List<SelectListItem>();
            isTribal.Add(all);
            isTribal.Add(yes);
            isTribal.Add(no);
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);

            ViewData["Active_TYPE"] = activeType;
            ViewData["IS_DESERT"] = isDesert;          
            ViewData["PMGSY_INCLUDED"] = pmgsyIncludedDd;
            ViewData["IS_SCHEDULE5"] = schedule5;
            ViewData["IS_TRIBAL"] = isTribal;
            ViewData["DISTRICT"] = districtDd;
            ViewData["STATE"] = stateDd;
            return View(MasterReportViewModel);
        }


        //Change 11/12/2013

       
        public ActionResult VillageDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;

            List<SelectListItem> stateDd = common.PopulateStates(false); //Change 11/12/2013
            stateDd.Find(x => x.Value == "1").Selected = true;
        
            // stateDd.Find(x => x.Value == "0").Text = "All States"; //Change 11/12/2013
            List<SelectListItem> districtDd = common.PopulateDistrict(stateCode);
            districtDd.Find(x => x.Value == "0").Text = "All Districts";
            List<SelectListItem> blockDd = common.PopulateBlocks(districtCode);
            blockDd.Find(x => x.Value == "0").Text = "All Blocks";
            if (districtCode > 0)
            {
                districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
            }

            if (stateCode > 0)  //if state login
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
            {
                List<SelectListItem> list = common.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)    // if mord login
            {
                List<SelectListItem> list = common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                list.Find(x => x.Value == "-1").Value = "0";
                if (districtCode > 0)
                {
                    list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };

            List<SelectListItem> schedule5 = new List<SelectListItem>();
            schedule5.Add(all);
            schedule5.Add(yes);
            schedule5.Add(no);
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType;        
            ViewData["STATE"] = stateDd;
            ViewData["DISTRICT"] = districtDd;
            ViewData["BLOCK"] = blockDd;
            ViewData["IS_SCHEDULE5"] = schedule5;
            ViewData["CENSUS_YEAR"] = new MasterReportsDAL().PopulateCensus();
            return View(MasterReportViewModel);
        }


      
        public ActionResult HabitationDetails(FormCollection frmCollection)
        {
            MasterReportsModel MasterReportViewModel = new MasterReportsModel();
            MasterReportViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            MasterReportViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
            int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);

            List<SelectListItem> stateDd = common.PopulateStates(true);
            stateDd.Find(x => x.Value == "0").Text = "Select State";
            List<SelectListItem> districtDd = common.PopulateDistrict(stateCode);
            districtDd.Find(x => x.Value == "0").Text = "Select District";
            List<SelectListItem> blockDd = common.PopulateBlocks(districtCode);
            blockDd.Find(x => x.Value == "0").Text = "Select Block";
            List<SelectListItem> villageDd = new List<SelectListItem>();
            var villageList = new MasterReportsDAL().PopulateVillage(blockCode);
            villageDd.Add(new SelectListItem
            {
                Text = "All Villages",
                Value = "0",
                Selected = true
            });
            foreach (SelectListItem village in villageList)
                villageDd.Add(village);

            if (stateCode > 0)
            {
                stateDd.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (districtCode > 0)
            {
                districtDd.Find(x => x.Value == districtCode.ToString()).Selected = true;
            }

            if (blockCode > 0)
            {
                return Json(villageDd, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["BlockCode"]) > 0)
            {
                return Json(villageDd, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
            {
                return Json(common.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"])), JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                // return Json(common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"])), JsonRequestBehavior.AllowGet);
                List<SelectListItem> list = common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false); // false is passed on 22 April 2020. Before , it was true
                //list.Find(x => x.Value == "-1").Value = "0"; // Commented this on 22 April 2020
                if (districtCode > 0)
                {
                    list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            SelectListItem Unconnected = new SelectListItem
            {
                Text = "Unconnected",
                Value = "U"
            };
            SelectListItem Connected = new SelectListItem
            {
                Text = "Connected",
                Value = "C"
            };
            SelectListItem StateConnected = new SelectListItem
            {
                Text = "State Connected",
                Value = "S"
            };
            SelectListItem PMGSYConnected = new SelectListItem
            {
                Text = "PMGSY Connected",
                Value = "P"
            };
            SelectListItem NotFeasible = new SelectListItem
            {
                Text = "Not Feasible",
                Value = "F"
            };

            List<SelectListItem> schedule5 = new List<SelectListItem>();
            schedule5.Add(all);
            schedule5.Add(yes);
            schedule5.Add(no);

            List<SelectListItem> habitationStatus = new List<SelectListItem>();
            habitationStatus.Add(all);
            habitationStatus.Add(Unconnected);
            habitationStatus.Add(Connected);
            habitationStatus.Add(StateConnected);
            habitationStatus.Add(PMGSYConnected);
            habitationStatus.Add(NotFeasible);
            List<SelectListItem> activeType = new List<SelectListItem>();
            activeType.Add(all);
            activeType.Add(yes);
            activeType.Add(no);
            ViewData["Active_TYPE"] = activeType; 
            ViewData["STATE"] = stateDd;
            ViewData["DISTRICT"] = districtDd;
            ViewData["BLOCK"] = blockDd;
            ViewData["VILLAGE"] = villageDd;
            ViewData["IS_SCHEDULE5"] = schedule5;
            ViewData["HAB_STATUS"] = habitationStatus;
            ViewData["CENSUS_YEAR"] = new MasterReportsDAL().PopulateCensus();
            return View(MasterReportViewModel);
        }

        #region Location Master SSRS Reports
        public ActionResult LocationMasterHabitationLayout(FormCollection frmCollection)
        {
            LocationMasterHabitationReport model = new LocationMasterHabitationReport();
            model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
            model.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            int stateCode = PMGSYSession.Current.StateCode == 0 ? Convert.ToInt32(frmCollection["StateCode"]) : PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(frmCollection["DistrictCode"]) : PMGSYSession.Current.DistrictCode;
            int blockCode = Convert.ToInt32(frmCollection["BlockCode"]);

            model.StateList = common.PopulateStates(true);
            model.StateList.Find(x => x.Value == "0").Value = "-1";
            model.DistrictList = common.PopulateDistrict(stateCode);
            model.DistrictList.Find(x => x.Value == "0").Value = "-1";
            model.BlockList = common.PopulateBlocks(districtCode);
            model.BlockList.Find(x => x.Value == "0").Value = "-1";
            model.VillageList = new List<SelectListItem>();
            var villageList = new MasterReportsDAL().PopulateVillage(blockCode);
            model.VillageList.Add(new SelectListItem
            {
                Text = "All Villages",
                Value = "0",
                Selected = true
            });
            foreach (SelectListItem village in villageList)
                model.VillageList.Add(village);

            if (stateCode > 0)
            {
                model.StateList.Find(x => x.Value == stateCode.ToString()).Selected = true;
            }

            if (districtCode > 0)
            {
                model.DistrictList.Find(x => x.Value == districtCode.ToString()).Selected = true;
            }

            if (blockCode > 0)
            {
                return Json(model.VillageList, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["BlockCode"]) > 0)
            {
                return Json(model.VillageList, JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToInt32(frmCollection["DistrictCode"]) > 0)
            {
                return Json(common.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"])), JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToInt32(frmCollection["StateCode"]) > 0)
            {
                // return Json(common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"])), JsonRequestBehavior.AllowGet);
                List<SelectListItem> list = common.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                //list.Find(x => x.Value == "-1").Value = "0";
                if (districtCode > 0)
                {
                    list.Find(x => x.Value == districtCode.ToString()).Selected = true;
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            SelectListItem yes = new SelectListItem
            {
                Text = "Yes",
                Value = "Y"
            };
            SelectListItem no = new SelectListItem
            {
                Text = "No",
                Value = "N"
            };
            SelectListItem all = new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = "%"
            };
            SelectListItem Unconnected = new SelectListItem
            {
                Text = "Unconnected",
                Value = "U"
            };
            SelectListItem Connected = new SelectListItem
            {
                Text = "Connected",
                Value = "C"
            };
            SelectListItem StateConnected = new SelectListItem
            {
                Text = "State Connected",
                Value = "S"
            };
            SelectListItem PMGSYConnected = new SelectListItem
            {
                Text = "PMGSY Connected",
                Value = "P"
            };
            SelectListItem NotFeasible = new SelectListItem
            {
                Text = "Not Feasible",
                Value = "F"
            };

            model.Schedule5List = new List<SelectListItem>();
            model.Schedule5List.Add(all);
            model.Schedule5List.Add(yes);
            model.Schedule5List.Add(no);

            model.HabitationStatusList = new List<SelectListItem>();
            model.HabitationStatusList.Add(all);
            model.HabitationStatusList.Add(Unconnected);
            model.HabitationStatusList.Add(Connected);
            model.HabitationStatusList.Add(StateConnected);
            model.HabitationStatusList.Add(PMGSYConnected);
            model.HabitationStatusList.Add(NotFeasible);

            model.ActiveList = new List<SelectListItem>();
            model.ActiveList.Add(all);
            model.ActiveList.Add(yes);
            model.ActiveList.Add(no);

            //ViewData["Active_TYPE"] = model.ActiveList;
            //ViewData["STATE"] = model.StateList;
            //ViewData["DISTRICT"] = model.DistrictList;
            //ViewData["BLOCK"] = model.BlockList;
            //ViewData["VILLAGE"] = model.VillageList;
            //ViewData["IS_SCHEDULE5"] = model.Schedule5List;
            //ViewData["HAB_STATUS"] = model.HabitationStatusList;
            //ViewData["CENSUS_YEAR"] = new MasterReportsDAL().PopulateCensus();

            model.CensusYearList = new MasterReportsDAL().PopulateCensus();
            return View(model);
        }

        public ActionResult LocationMasterHabitationReport(LocationMasterHabitationReport model)
        {
            try
            {
                model.RptNo = 5;
                model.Panchayat = 0;
                model.Type1 = model.HabitationStatus == "All" ? "%" : model.HabitationStatus;
                model.Type2 = model.Schedule5 == "All" ? "%" : model.Schedule5;
                model.Type3 = model.Type3 == null ? "%" : model.Type3;
                model.Type4 = model.Type4 == null ? "%" : model.Type4;
                model.Type5 = model.Type5 == null ? "%" : model.Type5;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }
        #endregion
    }
}
