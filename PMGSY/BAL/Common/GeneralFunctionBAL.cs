#region header 
/* This file contains all general functions which will be used in application
 * 
 */


#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using PMGSY.DAL.Common;
using System.Resources;
using System.Globalization;

namespace PMGSY.BLL.Common
{
    public enum Mode
    {
        DataEntry,
        Reports
    }
     
    public class GeneralFunctionsBAL : IGeneralFunctions
    {
        private IGeneralFunctionsDAL objGeneralFunctionsDAL;
       
        const int NoOfBackYears = 5;
       
        public GeneralFunctionsBAL()
        {
            objGeneralFunctionsDAL = new GeneralFunctionsDAL();
        }

              

        public static List<SelectListItem> PopulateYesNo()
        {
            List<SelectListItem> PopulateYesNo = new List<SelectListItem>();
            PopulateYesNo.Add(new SelectListItem { Text = "--Select--", Value = "" });
            PopulateYesNo.Add(new SelectListItem { Text = "Yes", Value = "true" });
            PopulateYesNo.Add(new SelectListItem { Text = "No", Value = "false" });
            return PopulateYesNo;
        }

        public static bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;
            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);   
                isDate = false;
            }
            return isDate;
        }
       
        
        public List<SelectListItem> PopulateMonths(int intSelected)
        {
            try
            {

                int count = 1;
                List<SelectListItem> ddlMonths = new List<SelectListItem>();

                ddlMonths.Add(new SelectListItem { Text = "January", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;
                ddlMonths.Add(new SelectListItem { Text = "February", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;
                ddlMonths.Add(new SelectListItem { Text = "March", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;
                ddlMonths.Add(new SelectListItem { Text = "April", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "May", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "June", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "July", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "August", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "September", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "October", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "November", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                ++count;

                ddlMonths.Add(new SelectListItem { Text = "December", Value = count.ToString(), Selected = (count == intSelected ? true : false) });
                return ddlMonths;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);            
                throw ex;
            }
        }

        //By default Current Year - 5 years
        public List<SelectListItem> PopulateYears(int intSelected)
        {
            return PopulateYears(NoOfBackYears, intSelected);
        }

        //Current Year - Number of back years
        public List<SelectListItem> PopulateYears(int NumberOfBackYears, int intSelected)
        {
            return PopulateYears(NumberOfBackYears, 0, intSelected);
        }

        //Current Year - Number of back years to Current Year + Number of Advance Year
        public List<SelectListItem> PopulateYears(int NumberOfBackYears, int NumberOfAdvanceYears, int intSelected)
        {
            int Year = DateTime.Now.Year;
            int LastYear = Year + NumberOfAdvanceYears;
            int FirstYear = Year - NumberOfBackYears;
            int count = LastYear - FirstYear;
            if (intSelected == 0)
                intSelected = DateTime.Now.Year;
            List<SelectListItem> ddlYear = new List<SelectListItem>();
            for (int i = 0; i < count; ++i)
            {

                ddlYear.Add(new SelectListItem { Text = LastYear.ToString(), Value = LastYear.ToString(), Selected = (intSelected == LastYear ? true : false) });
                --LastYear;
            }
            return ddlYear;
        }


      
        
        //public List<SelectListItem> GetFinancialYear(char level, int StateCode, int? DistrictCode, int? BlockCode, long? GramPanchayatCode,int Selected)
        //{
        //    List<FinancialYearDTO> objListFinancialYearDTO = new List<FinancialYearDTO>();
        //    List<SelectListItem> lstFinancialYear = new List<SelectListItem>();
        //    objListFinancialYearDTO = objGeneralServiceDAL.GetFinancialYear(level, StateCode, DistrictCode, BlockCode, GramPanchayatCode);
        //    lstFinancialYear.Add(new SelectListItem { Text = "--Select Year--", Value = "0" });
        //    foreach (var objList in objListFinancialYearDTO)
        //    {

        //        if (Selected.ToString() == objList.OBYear)
        //        {
        //            lstFinancialYear.Add(new SelectListItem { Text = objList.OBYear.ToString(), Value = objList.OBYear.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            lstFinancialYear.Add(new SelectListItem { Text = objList.OBYear.ToString(), Value = objList.OBYear.ToString()});
        //        }
        //    }
        //    return lstFinancialYear;
        //}

        //private string GetMode(Mode m)
        //{
        //    switch (m)
        //    {
        //        case Mode.DataEntry:
        //            return "--Select--";

        //        case Mode.Reports:
        //            return "All";

        //        default:
        //            return "";

        //    }
        //}

        //public List<SelectListItem> PopulateState(Mode m)
        //{
        //    List<SelectListItem> lstPopulateState = new List<SelectListItem>();
        //    List<StateMasterDTO> lstobjStateMasterDTO = new List<StateMasterDTO>();
        //    lstobjStateMasterDTO = objGeneralServiceDAL.PopulateState();
        //    lstPopulateState.Add(new SelectListItem { Text = GetMode(m), Value = "0", Selected=true });
        //    foreach (var objList in lstobjStateMasterDTO)
        //    {
        //        lstPopulateState.Add(new SelectListItem { Text = objList.StateName, Value = objList.StateCode });
        //    }
        //    return lstPopulateState;
        //}


        //public List<SelectListItem> PopulateSelectedState(Int64 intSelectedStateCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateState = new List<SelectListItem>();
        //    List<StateMasterDTO> lstobjStateMasterDTO = new List<StateMasterDTO>();
        //    lstobjStateMasterDTO = objGeneralServiceDAL.PopulateState();
        //    lstPopulateState.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstobjStateMasterDTO)
        //    {
        //        if (intSelectedStateCode.ToString() == objList.StateCode)
        //        {
        //            lstPopulateState.Add(new SelectListItem { Text = objList.StateName, Value = objList.StateCode,Selected=true });
        //        }
        //        else
        //        {
        //            lstPopulateState.Add(new SelectListItem { Text = objList.StateName, Value = objList.StateCode });
        //        }
        //    }
        //    return lstPopulateState;
        //}

        ////used for 192.168.11.25
        //public List<SelectListItem> GetDistrictsByState(Int32 StateCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateDistrict = new List<SelectListItem>();
        //    List<DistrictMasterDTO> lstObjDistrictMasterDTO = new List<DistrictMasterDTO>();
        //    lstObjDistrictMasterDTO = objGeneralServiceDAL.PopulateDistrict(StateCode);
        //    lstPopulateDistrict.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstObjDistrictMasterDTO)
        //    {
        //        lstPopulateDistrict.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.DistrictName.ToLower()), Value = objList.DistrictCode.ToString() });
        //    }
        //    return lstPopulateDistrict;
        //}

        //public List<SelectListItem> PopulateSelectedDistrictByState(Int32 StateCode, Int64 DistrictCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateDistrict = new List<SelectListItem>();
        //    List<DistrictMasterDTO> lstObjDistrictMasterDTO = new List<DistrictMasterDTO>();
        //    lstObjDistrictMasterDTO = objGeneralServiceDAL.PopulateDistrict(StateCode);
        //    lstPopulateDistrict.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstObjDistrictMasterDTO)
        //    {
        //        if (objList.DistrictCode.ToString() == DistrictCode.ToString())
        //        {
        //            lstPopulateDistrict.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.DistrictName.ToLower()), Value = objList.DistrictCode.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            lstPopulateDistrict.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.DistrictName.ToLower()), Value = objList.DistrictCode.ToString() });
        //        }
        //    }
        //    return lstPopulateDistrict;
        //}

        //public List<SelectListItem> PopulateSelectedBlockByDistrict(Int32 DistrictCode, Int32 BlockCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateBlock = new List<SelectListItem>();
        //    List<MasterBlockDTO> lstObjBlockMasterDTO = new List<MasterBlockDTO>();
        //    lstObjBlockMasterDTO = objGeneralServiceDAL.PopulateBlock(DistrictCode);
        //    lstPopulateBlock.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstObjBlockMasterDTO)
        //    {
        //        if (objList.BlockCode.ToString() == BlockCode.ToString())
        //        {
        //            lstPopulateBlock.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.BlockName.ToLower()), Value = objList.BlockCode.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            lstPopulateBlock.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.BlockName.ToLower()), Value = objList.BlockCode.ToString() });
        //        }
        //    }
        //    return lstPopulateBlock;
        //}

        //public List<SelectListItem> PopulateSelectedGPByBlock(Int32 BlockCode, Int32 GPCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateGP = new List<SelectListItem>();
        //    List<GramPanchayatDTO> lstObjGPMasterDTO = new List<GramPanchayatDTO>();
        //    lstObjGPMasterDTO = objGeneralServiceDAL.PopulateGramPanchayats(BlockCode);
        //    lstPopulateGP.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstObjGPMasterDTO)
        //    {
        //        if (objList.GramPanchayatCode.ToString() == GPCode.ToString())
        //        {
        //            lstPopulateGP.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.GramPanchayatName.ToLower()), Value = objList.GramPanchayatCode.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            lstPopulateGP.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.GramPanchayatName.ToLower()), Value = objList.GramPanchayatCode.ToString() });
        //        }
        //    }
        //    return lstPopulateGP; 
        //}

        //public List<SelectListItem> GetBlocksByLevel(Int32 DistrictCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateBlock = new List<SelectListItem>();
        //    List<MasterBlockDTO> lstObjBlockMasterDTO = new List<MasterBlockDTO>();
        //    lstObjBlockMasterDTO = objGeneralServiceDAL.PopulateBlock(DistrictCode);
        //    lstPopulateBlock.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstObjBlockMasterDTO)
        //    {
        //        lstPopulateBlock.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.BlockName.ToLower()), Value = objList.BlockCode.ToString() });
        //    }
        //    return lstPopulateBlock;
        //}
                
        //public List<SelectListItem> GetGramPanchayatByLevel(Int64 BlockCode, Mode m)
        //{
        //    List<SelectListItem> lstPopulateGram = new List<SelectListItem>();
        //    List<GramPanchayatDTO> lstObjGramMasterDTO = new List<GramPanchayatDTO>();
        //    lstObjGramMasterDTO = objGeneralServiceDAL.PopulateGramPanchayats(BlockCode);
        //    lstPopulateGram.Add(new SelectListItem { Text = GetMode(m), Value = "0" });
        //    foreach (var objList in lstObjGramMasterDTO)
        //    {
        //        lstPopulateGram.Add(new SelectListItem { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(objList.GramPanchayatName.ToLower()), Value = objList.GramPanchayatCode.ToString() });
        //    }
        //    return lstPopulateGram;
        //}
        
        //public List<SelectListItem> GetStateByDistrict(long DistrictCode)
        //{
        //    List<SelectListItem> lstState = new List<SelectListItem>();
        //    return (objGeneralServiceDAL.GetStateByDistrict(DistrictCode));
        //}
        
        //public string GetLocationNameById(int levelId, long locationId)
        //{
        //    try
        //    {

        //        string locationName = string.Empty;
        //        switch (levelId)
        //        {

        //            case 1:
        //                locationName = "NLMA";
        //                break;
        //            case 3:
        //                locationName = objGeneralServiceDAL.PopulateState().Single(l => l.StateCode == locationId.ToString()).StateName;
        //                break;

        //            case 4:
        //                locationName = objGeneralServiceDAL.PopulateState().Single(l => l.StateCode == locationId.ToString()).StateName;
        //                break;

        //            case 5:
        //                locationName = objGeneralServiceDAL.GetLocationName(locationId).ToString().Trim();
        //                //locationName = objGeneralServiceDAL.PopulateDistrict().Single(l => l.DistrictCode == locationId).DistrictName;
        //                break;

        //            case 6:
        //                locationName = objGeneralServiceDAL.GetLocationName(locationId).ToString().Trim();
        //                //locationName = objGeneralServiceDAL.PopulateDistrict().Single(l => l.DistrictCode == locationId).DistrictName;
        //                break;

        //            case 7:
        //                locationName = objGeneralServiceDAL.GetLocationName(locationId).ToString().Trim();
        //                // locationName = objGeneralServiceDAL.PopulateBlock().Single(l => l.BlockCode == locationId).BlockName;
        //                break;

        //            case 9:
        //                locationName = objGeneralServiceDAL.GetLocationName(locationId).ToString().Trim();
        //                //locationName = objGeneralServiceDAL.PopulateGramPanchayats().Single(l => l.GramPanchayatCode == locationId).GramPanchayatName;
        //                break;
        //        }
        //        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(locationName.ToLower());
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}
        
        //public List<SelectListItem> PopulateMonthsLocalized(int intSelected, String LanguageID)
        //{
        //    System.Globalization.CultureInfo Culture;
        //    Culture = new System.Globalization.CultureInfo(LanguageID.ToString());
        //    int count = 0;
        //    if (intSelected == 0)
        //        intSelected = DateTime.Now.Month;
        //    List<SelectListItem> ddlMonths = new List<SelectListItem>();
        //    for (int mon = 1; mon <= 12; mon++)
        //    {
        //        ddlMonths.Add(new SelectListItem { Text = HttpContext.GetGlobalResourceObject("Months", mon.ToString(), Culture).ToString(), Value = mon.ToString(), Selected = (mon == intSelected ? true : false) });
        //       count++;
        //    }
        //    return ddlMonths;

        //}
        //public List<SelectListItem> PopulateYears(int intSelected)
        //{
        //    return PopulateYears(NoOfBackYears, intSelected);
        //}
        
        ////Current Year - Number of back years
        //public List<SelectListItem> PopulateYears(int NumberOfBackYears, int intSelected)
        //{
        //    return PopulateYears(NumberOfBackYears, 0, intSelected);
        //}
        ////Current Year - Number of back years to Current Year + Number of Advance Year
        //public List<SelectListItem> PopulateYears(int NumberOfBackYears, int NumberOfAdvanceYears, int intSelected)
        //{
        //    int Year = DateTime.Now.Year;
        //    int LastYear = Year + NumberOfAdvanceYears;
        //    int FirstYear = Year - NumberOfBackYears;
        //    int count = LastYear - FirstYear;
        //    if (intSelected == 0)
        //        intSelected = DateTime.Now.Year;
        //    List<SelectListItem> ddlYear = new List<SelectListItem>();
        //    for (int i = 0; i < count; ++i)
        //    {

        //        ddlYear.Add(new SelectListItem { Text = LastYear.ToString(), Value = LastYear.ToString(), Selected = (intSelected == LastYear ? true : false) });
        //        --LastYear;
        //    }
        //    return ddlYear;
        //}

        //public List<SelectListItem> PopulateFinancialYear(int SelectedYear)
        //{
        //    try
        //    {
        //        List<SelectListItem> listSelectItem = new List<SelectListItem>();
                
        //        int Year = System.DateTime.Today.Year + 1;
        //        for (int i = 0; i < 5; ++i)
        //        {
        //            bool boolSelected = false;
        //            if (Year - i - 1 == SelectedYear)
        //                boolSelected = true;
        //            SelectListItem objSelectListItem = new SelectListItem { Text = (Year - i - 1).ToString() + "-" + (Year - i).ToString(), Value = (Year - i - 1).ToString(), Selected = boolSelected };
        //            listSelectItem.Add(objSelectListItem);
        //        }
        //        return listSelectItem;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

       
        //public string GetMonthNameByID(int monthID)
        //{
        //    string monthName = string.Empty;

        //    switch (monthID)
        //    {
        //        case 1:
        //            monthName = "January";
        //            break;
        //        case 2:
        //            monthName = "February";
        //            break;
        //        case 3:
        //            monthName = "March";
        //            break;
        //        case 4:
        //            monthName = "April";
        //            break;
        //        case 5:
        //            monthName = "May";
        //            break;
        //        case 6:
        //            monthName = "June";
        //            break;
        //        case 7:
        //            monthName = "July";
        //            break;
        //        case 8:
        //            monthName = "August";
        //            break;
        //        case 9:
        //            monthName = "September";
        //            break;
        //        case 10:
        //            monthName = "October";
        //            break;
        //        case 11:
        //            monthName = "November";
        //            break;
        //        case 12:
        //            monthName = "December";
        //            break;   

        //    }
        //    return monthName;
        //}
      
       
        
        //public Int16 GetLocationLevelFromLocationCode(Int32 intLocationCode)
        //{
        //    try
        //    {
        //        Int16 smallLocationLevel = 0;
        //        smallLocationLevel = objGeneralServiceDAL.GetLocationLevelFromLocationCode(intLocationCode);
        //        return smallLocationLevel;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

        //public Int32 GetHigherLocationId(Int32 intLocationCode)
        //{
        //    try
        //    {
        //        Int32 intHigherLocationId = 0;
        //        intHigherLocationId = objGeneralServiceDAL.GetHigherLocationId(intLocationCode);
        //        return intHigherLocationId;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

        
        
       
    }

    public interface IGeneralFunctions
    {
        //List<SelectListItem> PopulateState(Mode m);
        //List<SelectListItem> GetDistrictsByState(Int32 StateCode, Mode m);
        //List<SelectListItem> GetBlocksByLevel(Int32 DistrictCode, Mode m);
        //List<SelectListItem> GetGramPanchayatByLevel(Int64 BlockCode, Mode m);
        //List<SelectListItem> PopulateSelectedState(Int64 intStateCode, Mode m);
        //List<SelectListItem> PopulateSelectedDistrictByState(Int32 StateCode, Int64 DistrictCode, Mode m);
        //List<SelectListItem> PopulateSelectedBlockByDistrict(Int32 DistrictCode, Int32 BlockCode, Mode m);
        //List<SelectListItem> PopulateSelectedGPByBlock(Int32 BlockCode, Int32 GPCode, Mode m);
        //String GetLocationNameById(int levelId, long locationId);
      
        //List<SelectListItem> GetFinancialYear(char level, int StateCode, int? DistrictCode, int? BlockCode, long? GramPanchayatCode,int Selected);
        //List<SelectListItem> PopulateMonthsLocalized(int intSelected, String LanguageID);
        //List<SelectListItem> PopulateYears(int intSelected);
        //List<SelectListItem> PopulateYears(int NumberOfBackYears, int intSelected);
        //List<SelectListItem> PopulateYears(int NumberOfBackYears, int NumberOfAdvanceYears, int intSelected);
        //List<SelectListItem> PopulateFinancialYear(int SelectedYear);   

        //List<SelectListItem> GetStateByDistrict(long DistrictCode);
        //String GetMonthNameByID(int monthID);
       

        //Int16 GetLocationLevelFromLocationCode(Int32 intLocationCode);
        //Int32 GetHigherLocationId(Int32 intLocationCode);
        //Int32 InsertActivityLogDetails(int UserId, Int16 LayerType, string Activity, string Description, string Exception, string pageUrl, Int16 TypeLog, string IPAddress);
      
         
    }

}
