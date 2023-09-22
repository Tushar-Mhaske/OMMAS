using PMGSY.Areas.EFORMArea.Model;
using PMGSY.Areas.EFORMArea.PiuBridgeModel;
using PMGSY.Areas.EFORMArea.QMBridgeModel;
using PMGSY.Areas.EFORMArea.QMModels;
using PMGSY.Areas.EFORMArea.TestReportModel;
using PMGSY.Extensions;
using PMGSY.Models;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Common
{
    public class CommonFunctions
    {


        PMGSYEntities dbContext = null;
        /// <summary>
        ///  Modified this function to set selected district value on 06/07/2013
        /// </summary>
        /// <param name="StateCode"></param>
        /// <param name="isAllSelected"></param>
        /// <param name="selectedDistrictCode"></param>
        /// <returns></returns>

        public List<SelectListItem> PopulateMonths(bool isPopulateFirstItem = true)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstMonths = new SelectList(dbContext.MASTER_MONTH, "MAST_MONTH_CODE", "MAST_MONTH_FULL_NAME").ToList();
                if (isPopulateFirstItem)
                {
                    lstMonths.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0", Selected = true }));
                }
                return lstMonths;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateMonths()");
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        public List<SelectListItem> PopulateYears(bool isPopulateFirstItem = true)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 1)).OrderByDescending(m => m.MAST_YEAR_CODE), "MAST_YEAR_CODE", "MAST_YEAR_CODE").ToList();
                if (isPopulateFirstItem)
                {
                    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
                }
                return lstYears;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateYears()");
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public List<SelectListItem> PopulateDistrict(Int32 StateCode, bool isAllSelected = false, Int32 selectedDistrictCode = 0, bool IsPopulateInactiveDistrictsForTOB = false, bool IsPopulateAllActiveInactiveDistricts = false)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDistrict = null;

                if (IsPopulateAllActiveInactiveDistricts)//Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                {
                    lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                }
                else
                {
                    if (IsPopulateInactiveDistrictsForTOB)
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode /*&& m.MAST_DISTRICT_ACTIVE == "N"*/).OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }
                    else
                    {
                        lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();
                    }

                }
                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "-1", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateDistrict()");
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        public DateTime GetStringToDateTime(string strDate)
        {
            string[] formats = { "dd/MM/yyyy" };
            return DateTime.ParseExact(strDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
        }
        public List<SelectListItem> PopulateStates(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;

            if (isPopulateFirstItem)
            {
                item = new SelectListItem();
                item.Text = "Select State";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All States";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);

            }

            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateStates()");
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<SelectListItem> PopulateStatesInterStateSQM(int admnQmCode, bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;

            if (isPopulateFirstItem)
            {
                item = new SelectListItem();
                item.Text = "Select State";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All States";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);

            }
            try
            {
                dbContext = new PMGSYEntities();
                item = new SelectListItem();
                item.Text = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_STATE_NAME).FirstOrDefault().ToString();
                item.Value = PMGSYSession.Current.StateCode.ToString();
                StatesList.Add(item);
                var query = (from c in dbContext.MASTER_STATE
                             join IS in dbContext.ADMIN_QUALITY_MONITORS_INTER_STATE on c.MAST_STATE_CODE equals IS.ALLOWED_STATE_CODE
                             where c.MAST_STATE_ACTIVE == "Y" && IS.ADMIN_QM_CODE == admnQmCode
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.PopulateStatesInterStateSQM()");
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        //addition by koustubh nakate on 20/06/2013 to validate grid partameters
        /// <summary>
        /// Used to validate grid partameters
        /// </summary>
        /// <param name="homeFormCollection">FormCollection object</param>
        /// <returns>bool</returns>
        public bool ValidateGridParameters(GridParams gridParams)
        {

            try
            {
                bool outSearch;
                int outParam;
                bool result = true;
                long outNd;

                Regex regex = new Regex(@"^[a-zA-Z0-9 _,.-]*$");

                //string InvalidCharacters = "<>;!@#$%^&*()=+?'";
                //char[] Charray = InvalidCharacters.ToCharArray();

                if (!string.IsNullOrEmpty(gridParams.Search.ToString()))
                {
                    if (!Boolean.TryParse(gridParams.Search.ToString(), out outSearch))
                    {
                        result = false;
                    }

                }
                else
                {
                    result = false;
                }


                //if (!string.IsNullOrEmpty(Request.Params["nd"]))
                //{
                //    if (!long.TryParse(Request.Params["nd"].ToString(), out outNd))
                //    {
                //        result = false;
                //    }
                //}
                //else
                //{
                //    result = false;
                //}

                if (!long.TryParse(gridParams.Nd.ToString(), out outNd))
                {
                    result = false;
                }

                if (!int.TryParse(gridParams.Page.ToString(), out outParam))
                {
                    result = false;
                }

                if (!int.TryParse(gridParams.Rows.ToString(), out outParam))
                {
                    result = false;
                }

                if (!string.IsNullOrEmpty(gridParams.Sidx))
                {
                    if (!regex.IsMatch(gridParams.Sidx))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }

                if (!string.IsNullOrEmpty(gridParams.Sord))
                {

                    if (!regex.IsMatch(gridParams.Sord))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }


                return result;
            }
            catch
            {
                return false;
            }

        }

        #region Code To Set PreFilled Data
        public static void SetPrefilledDataToField(PDFPreFilledDataMutatorModel pDFPreFilledDataMutatorModel)
        {
            try
            {
                switch (pDFPreFilledDataMutatorModel.FieldType)
                {
                    case PDFFieldType.TextBox:
                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFPreFilledDataMutatorModel.LoadedForm.Fields[pDFPreFilledDataMutatorModel.FieldName];
                        txtbxField.Text = pDFPreFilledDataMutatorModel.Value;
                        txtbxField.ReadOnly = pDFPreFilledDataMutatorModel.IsReadOnly;
                        break;
                    case PDFFieldType.CheckBox:
                        //PdfLoadedCheckBoxField loadedField = (PdfLoadedCheckBoxField)LoadedForm.Fields[FieldName];
                        //loadedField.se = Value;
                        //loadedField.ReadOnly = IsReadOnly;
                        break;
                    case PDFFieldType.RadioButton:
                        PdfLoadedRadioButtonListField RadioField = (PdfLoadedRadioButtonListField)pDFPreFilledDataMutatorModel.LoadedForm.Fields[pDFPreFilledDataMutatorModel.FieldName];
                        RadioField.SelectedValue = pDFPreFilledDataMutatorModel.Value;
                        RadioField.ReadOnly = pDFPreFilledDataMutatorModel.IsReadOnly;
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SetPrefilledDataToField");
                throw;
            }
        }
        #endregion

        #region Code to PreFiled Data
        public void GeneratePDFPreFilledDataModel(object T, PdfLoadedForm loadedForm)
        {
            try
            {
                FieldTypeAttribute fieldTypeAttribute = new FieldTypeAttribute();
                var T_type = T.GetType();
                var AttributeType = fieldTypeAttribute.GetType();
                List<PropertyInfo> PropertyList = T_type.GetProperties().ToList();
                PDFPreFilledDataMutatorModel pDFPreFilledDataMutatorModel = new PDFPreFilledDataMutatorModel();
                pDFPreFilledDataMutatorModel.LoadedForm = loadedForm;
                pDFPreFilledDataMutatorModel.IsReadOnly = true;
                foreach (PropertyInfo item in PropertyList)
                {
                    CustomAttributeData AttributeData = item.CustomAttributes.Where(m => m.AttributeType == AttributeType).FirstOrDefault();
                    pDFPreFilledDataMutatorModel.FieldType = (PDFFieldType)AttributeData.NamedArguments.FirstOrDefault().TypedValue.Value;
                    pDFPreFilledDataMutatorModel.FieldName = item.Name;
                    pDFPreFilledDataMutatorModel.Value = Convert.ToString(item.GetValue(T));
                    CommonFunctions.SetPrefilledDataToField(pDFPreFilledDataMutatorModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GeneratePDFPreFilledDataModel");
                throw;
            }
        }
        #endregion

        #region Get Filled Data From PDF Field
        public void GetFilledDataFromPDFField(PDFFiledDataInspectorModel pDFFiledDataInspectorModel)
        {
            try
            {
                switch (pDFFiledDataInspectorModel.FieldType)
                {
                    case PDFFieldType.TextBox:
                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                        pDFFiledDataInspectorModel.Value = txtbxField.Text;

                        break;
                    case PDFFieldType.CheckBox:
                        PdfLoadedCheckBoxField CheckField = (PdfLoadedCheckBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                        if (CheckField.Checked == true)
                        {
                            pDFFiledDataInspectorModel.Value = CheckField.ToolTip;
                        }
                        else
                        {
                            pDFFiledDataInspectorModel.Value = "N";
                        }
                        break;
                    case PDFFieldType.RadioButton:
                        PdfLoadedRadioButtonListField RadioField = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                        pDFFiledDataInspectorModel.Value = RadioField.SelectedValue;

                        break;
                    case PDFFieldType.ComboBox:
                        PdfLoadedComboBoxField ComboField = (PdfLoadedComboBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];

                        pDFFiledDataInspectorModel.Value = ComboField.SelectedValue;



                        break;



                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetFileDataFromPDFField");
                throw;
            }
        }
        #endregion

        #region RegexMatching
        public bool MatchRegexString(string value, string Pattern)
        {
            try
            {
                Regex re = new Regex(Pattern);
                return re.IsMatch(value);
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "CommonFunctions.MatchRegexString()");
                throw;
            }
        }

        public bool MatchDateFormat(string value)
        {
            try
            {
                if (value.Contains(' '))
                {
                    value = value.Split(' ')[0];
                }
                string tempDate = value;
                DateTime fromDateValue;
                var formats = new[] { "dd/MM/yyyy" };
                if (DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
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
                ErrorLog.LogError(ex, "CommonFunctions.MatchDateFormat()");
                throw;
            }
        }
        #endregion    

        #region Code to get Filled details
        public List<string> FetchPDFFilledDataToModel(object T, PdfLoadedForm loadedForm)
        {
            bool skip = false;
            string erroradd = string.Empty;
            string officialType = string.Empty;
            string officialTableNo = string.Empty;
            bool isdateField = false;
            List<string> ErrorList = new List<string>();
            PMGSYEntities eformdbContext = new PMGSYEntities();
            try
            {
                FieldTypeAttribute fieldTypeAttribute = new FieldTypeAttribute();
                RoadStatusDependableAttribute roadStatusDependableAttribute = new RoadStatusDependableAttribute();


                RegularExpressionAttribute regularExpressionAttribute = new RegularExpressionAttribute("");
                StringLengthAttribute StringLengthAttribute = new StringLengthAttribute(0);
                RequiredAttribute requiredAttribute = new RequiredAttribute();
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                var T_type = T.GetType();
                var AttributeType = fieldTypeAttribute.GetType();
                var RoadStatusAttributeType = roadStatusDependableAttribute.GetType();


                var RegexAttributeType = regularExpressionAttribute.GetType();
                var StringLengthAttributeType = StringLengthAttribute.GetType();
                var requiredAttributeType = requiredAttribute.GetType();
                List<PropertyInfo> PropertyList = T_type.GetProperties().ToList();
                pDFFiledDataInspectorModel.LoadedForm = loadedForm;
                EFORM_PIU_VIEWMODEL objViewModel = new EFORM_PIU_VIEWMODEL();





                foreach (PropertyInfo item in PropertyList)
                {
                    isdateField = false;
                    skip = false;
                    erroradd = "";
                    officialType = "";
                    officialTableNo = "";
                    CustomAttributeData RoadStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == RoadStatusAttributeType).FirstOrDefault();


                    CustomAttributeData AttributeData = item.CustomAttributes.Where(m => m.AttributeType == AttributeType).FirstOrDefault();
                    pDFFiledDataInspectorModel.FieldType = (PDFFieldType)AttributeData.NamedArguments.FirstOrDefault().TypedValue.Value;
                    if (pDFFiledDataInspectorModel.FieldType == PDFFieldType.Skip)
                        continue;

                    pDFFiledDataInspectorModel.FieldName = item.Name;



                    if (T_type.Name == "EFORM_NEW_TECHNOLOGY_DETAILS_PIU")
                    {
                        pDFFiledDataInspectorModel.FieldName += Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }

                    if (T_type.Name == "EFORM_PREVIOUS_INSP_DETAILS_PIU")
                    {
                        double templateVers = 0;
                        try
                        {
                            PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                            if (templateVersion.Text != "")
                            {
                                templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                            }

                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToModel()");
                        }
                        if (templateVers < 2.0)
                        {
                            if (pDFFiledDataInspectorModel.FieldName.Equals("INSP_ID"))
                            {
                                continue;
                            }
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        PMGSYEntities dbContext = new PMGSYEntities();
                        PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                        int e_formId = Convert.ToInt32(eformId.Text);
                        int count = dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS.Where(s => s.EFORM_ID == e_formId).Count();
                        int l = pDFFiledDataInspectorModel.FieldName.ToString().Split('_').Length;
                        skip = false;
                        if (Convert.ToInt16(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1]) > count || Convert.ToInt16(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1]) > 8)
                        {
                            skip = true;
                        }


                        erroradd = "row " + pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1].ToString();
                    }

                    if (T_type.Name == "EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0")
                    {
                        double templateVers = 0;
                        try
                        {
                            PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                            if (templateVersion.Text != "")
                            {
                                templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                            }

                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToModel()");
                        }
                        if (templateVers < 2.0)
                        {
                            if (pDFFiledDataInspectorModel.FieldName.Equals("INSP_ID"))
                            {
                                continue;
                            }
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        PMGSYEntities dbContext = new PMGSYEntities();
                        PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                        int e_formId = Convert.ToInt32(eformId.Text);
                        int count = dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS.Where(s => s.EFORM_ID == e_formId).Count();
                        int l = pDFFiledDataInspectorModel.FieldName.ToString().Split('_').Length;
                        skip = false;
                        if (Convert.ToInt16(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1]) > count || Convert.ToInt16(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1]) > 8)
                        {
                            skip = true;
                        }


                        erroradd = "row " + pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1].ToString();
                    }

                    if (T_type.Name == "EFORM_GENERAL_INFO_PIU")
                    {

                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }



                        PdfLoadedRadioButtonListField WorkStatus = loadedForm.Fields["WORK_STATUS"] as PdfLoadedRadioButtonListField;
                        if (WorkStatus.SelectedValue == "P")
                        {
                            if (item.Name == "ACTUAL_COMPLETION_DATE")
                            {
                                continue;
                            }
                        }


                        if (WorkStatus.SelectedValue == "P")
                        {
                            if (item.Name == "COMPLETION_COST")
                            {
                                continue;
                            }
                        }



                        if (item.Name == "TOTAL_LEN")
                        {
                            string Presentvalue = Convert.ToString(PropertyList.Where(m => m.Name == "WORK_TYPE").FirstOrDefault().GetValue(T));
                            if (Presentvalue == "N")
                            {
                                pDFFiledDataInspectorModel.FieldName += "_N";
                                erroradd = " New Connectivity";
                            }
                            else
                            {
                                pDFFiledDataInspectorModel.FieldName += "_U";
                                erroradd = " Up-gradation";
                            }
                        }

                        if (item.Name == "CARRIAGEWAY_WIDTH_NEW")
                        {
                            string Presentvalue = Convert.ToString(PropertyList.Where(m => m.Name == "WORK_TYPE").FirstOrDefault().GetValue(T));
                            if (Presentvalue != "N")
                            {
                                continue;
                            }
                        }
                        if (item.Name == "CARRIAGEWAY_WIDTH_TYPE")
                        {
                            string Presentvalue = Convert.ToString(PropertyList.Where(m => m.Name == "WORK_TYPE").FirstOrDefault().GetValue(T));
                            if (Presentvalue == "N")
                            {
                                continue;
                            }
                        }
                        if (item.Name == "CARRIAGEWAY_WIDTH_PROPOSED")
                        {
                            string Presentvalue = Convert.ToString(PropertyList.Where(m => m.Name == "CARRIAGEWAY_WIDTH_TYPE").FirstOrDefault().GetValue(T));
                            if (Presentvalue == "W")
                            {
                                pDFFiledDataInspectorModel.FieldName += "_W";
                                erroradd = " With widening";
                            }
                            else
                            {
                                pDFFiledDataInspectorModel.FieldName += "_O";
                                erroradd = " Without widening";
                            }
                        }
                        if (item.Name == "CARRIAGEWAY_LENGTH")
                        {
                            string Presentvalue = Convert.ToString(PropertyList.Where(m => m.Name == "CARRIAGEWAY_WIDTH_TYPE").FirstOrDefault().GetValue(T));
                            if (Presentvalue == "W")
                            {
                                pDFFiledDataInspectorModel.FieldName += "_W";
                                erroradd = " With widening";
                            }
                            else
                            {
                                pDFFiledDataInspectorModel.FieldName += "_O";
                                erroradd = " Without widening";
                            }
                        }


                        PdfLoadedRadioButtonListField WORK_TYPE_value = loadedForm.Fields["WORK_TYPE"] as PdfLoadedRadioButtonListField;
                        if (WORK_TYPE_value.SelectedValue == "U")
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "TOTAL_LEN_N" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_NEW")
                            {
                                skip = true;
                            }
                            PdfLoadedRadioButtonListField CARRIAGEWAY_WIDTH_TYPE_value = loadedForm.Fields["CARRIAGEWAY_WIDTH_TYPE"] as PdfLoadedRadioButtonListField;
                            if (CARRIAGEWAY_WIDTH_TYPE_value.SelectedValue == "W")
                            {
                                if (pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_PROPOSED_O" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_LENGTH_O")
                                {
                                    skip = true;
                                }
                            }
                            if (CARRIAGEWAY_WIDTH_TYPE_value.SelectedValue == "O")
                            {
                                if (pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_EXISTING" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_PROPOSED_W" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_LENGTH_W")
                                {
                                    skip = true;
                                }
                            }
                            if (CARRIAGEWAY_WIDTH_TYPE_value.SelectedValue == null)
                            {
                                if (pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_EXISTING" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_PROPOSED_W"
                                || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_PROPOSED_O" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_LENGTH_W"
                                || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_LENGTH_O")
                                {
                                    skip = true;
                                }
                            }

                        }
                        if (WORK_TYPE_value.SelectedValue == "N")
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "TOTAL_LEN_U" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_TYPE"
                                || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_EXISTING" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_PROPOSED_W"
                                || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_WIDTH_PROPOSED_O" || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_LENGTH_W"
                                || pDFFiledDataInspectorModel.FieldName == "CARRIAGEWAY_LENGTH_O")
                            {
                                skip = true;
                            }

                        }
                    }
                    if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        int item_id = Convert.ToInt32(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "P" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault();



                    }

                    if (T_type.Name == "EFORM_MIX_DESIGN_DETAILS_PIU")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        int item_id = Convert.ToInt32(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "M" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault();

                    }

                    if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                    {
                        string tableNo = string.Empty;
                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                        if (OfficeType == "C")
                        {
                            tableNo = "IV. ";
                            if (pDFFiledDataInspectorModel.FieldName == "IDENTITY_NUMBER" || pDFFiledDataInspectorModel.FieldName == "FROM_DATE" || pDFFiledDataInspectorModel.FieldName == "TO_DATE")
                                continue;
                        }
                        if (OfficeType == "S")
                        {
                            tableNo = "VII. ";
                            if (pDFFiledDataInspectorModel.FieldName == "MOBILE_NO")
                                continue;
                        }
                        if (OfficeType == "L" || OfficeType == "J" || OfficeType == "A" || OfficeType == "S" || OfficeType == "E")
                        {
                            tableNo = (OfficeType == "L" ? "VI. " : ((OfficeType == "J" ? "IX. " : (OfficeType == "A" ? "VIII. " : ((OfficeType == "S" ? "VII. " : ((OfficeType == "E" ? "V. " : "IV. "))))))));
                            if (pDFFiledDataInspectorModel.FieldName == "PAN" || pDFFiledDataInspectorModel.FieldName == "EMAIL_ID")
                                continue;
                        }
                        pDFFiledDataInspectorModel.FieldName = OfficeType + "_" + pDFFiledDataInspectorModel.FieldName + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                        PdfLoadedTextBoxField txtbxField = loadedForm.Fields[pDFFiledDataInspectorModel.FieldName] as PdfLoadedTextBoxField;
                        erroradd = txtbxField.ToolTip;

                        if (OfficeType == "J")
                        {
                            officialType = "J";
                        }

                        officialTableNo = tableNo;
                    }

                    if (T_type.Name == "EFORM_QC_DETAILS_PIU")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }

                    }
                    try
                    {
                        GetFilledDataFromPDFField(pDFFiledDataInspectorModel);
                    }
                    catch (Exception ex)
                    {

                        ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToModel()");
                        ErrorList.Add("Page-1: Please contact OMMAS team. " + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                        objViewModel.ErrorOccured = true;
                        // return ErrorList;
                    }

                    //item.SetValue(T, pDFFiledDataInspectorModel.Value);
                    #region dateField or not
                    isdateField = false;
                    if (pDFFiledDataInspectorModel.FieldName == "ACTUAL_COMPLETION_DATE"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_8"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_9"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_10"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_8"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_9"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_10"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_8"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_9"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_10"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_8"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_9"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_10"
             || pDFFiledDataInspectorModel.FieldName == "PHOTO_UPLOAD_DATE"
             || pDFFiledDataInspectorModel.FieldName == "E_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "E_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "E_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "E_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "E_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "E_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "L_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "L_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "L_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "L_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "S_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "S_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "S_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "S_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "S_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "S_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "A_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "A_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "A_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "A_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "A_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "A_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "J_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "J_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "J_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "J_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "J_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "J_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "DESIGN_DATE_11"
             || pDFFiledDataInspectorModel.FieldName == "DESIGN_DATE_12"
             || pDFFiledDataInspectorModel.FieldName == "DESIGN_DATE_13"
             || pDFFiledDataInspectorModel.FieldName == "DESIGN_DATE_14"
             || pDFFiledDataInspectorModel.FieldName == "DESIGN_DATE_15"

             )
                    {
                        isdateField = true;
                    }
                    #endregion
                    string Value = string.Empty;
                    if (pDFFiledDataInspectorModel.Value != "")
                    {
                        Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    }
                    //  string Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(item.PropertyType);
                        List<CustomAttributeData> CustomValidatioList = item.CustomAttributes.Where(m => m.AttributeType != AttributeType).ToList();
                        try
                        {
                            if (Value != "" && Value != null)
                            {
                                T.GetType().InvokeMember(item.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                         Type.DefaultBinder, T, new object[] { converter.ConvertFromString(Value) });

                            }

                        }
                        catch (Exception e)
                        {
                            #region  catch code


                            foreach (CustomAttributeData Customitem in CustomValidatioList)
                            {
                                string errormsg = string.Empty;
                                switch (pDFFiledDataInspectorModel.FieldType)
                                {
                                    case PDFFieldType.TextBox:
                                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = txtbxField.ToolTip;
                                        break;
                                    case PDFFieldType.RadioButton:
                                        PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = radioButton.ToolTip;
                                        break;
                                }
                                if (skip == false)
                                {
                                    if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                    {
                                        string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                        if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                                        {
                                            string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                            if (OfficeType == "J")
                                            {
                                                ErrorMessage = ErrorMessage.Replace("-3", "-4");
                                            }

                                            ErrorMessage = ErrorMessage.Replace("Please enter", officialTableNo + " Please enter");
                                        }
                                        if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                                        {
                                            if (pDFFiledDataInspectorModel.FieldName.Contains("_9") || pDFFiledDataInspectorModel.FieldName.Contains("_10"))
                                            {
                                                ErrorMessage = ErrorMessage.Replace("-2", "-3");
                                            }
                                        }
                                        if (T_type.Name == "EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0")
                                        {
                                            if (pDFFiledDataInspectorModel.FieldName.Contains("INSP_LEVEL"))
                                            {
                                                if (Value == "0")
                                                {
                                                    objViewModel.ErrorOccured = true;

                                                    ErrorList.Add(ErrorMessage + erroradd);
                                                }
                                            }

                                        }
                                        if (Value == null || Value == "" || Value == " ")
                                        {
                                            objViewModel.ErrorOccured = true;

                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                                {
                                    string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                    if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                                    {
                                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                        if (OfficeType == "J")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-3", "-4");
                                        }

                                        ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                    }
                                    if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                                    {
                                        if (pDFFiledDataInspectorModel.FieldName.Contains("_9") || pDFFiledDataInspectorModel.FieldName.Contains("_10"))
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-2", "-3");
                                        }
                                    }

                                    if (Value != null && Value != "")
                                    {
                                        if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                        if (isdateField == true)
                                        {
                                            if (!MatchDateFormat(Convert.ToString(Value)))
                                            {
                                                objViewModel.ErrorOccured = true;
                                                ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                            }
                                        }
                                    }

                                }
                                if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                                {
                                    int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                    if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                                    {
                                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                        if (OfficeType == "J")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-3", "-4");
                                        }

                                        ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                    }
                                    if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                                    {
                                        if (pDFFiledDataInspectorModel.FieldName.Contains("_9") || pDFFiledDataInspectorModel.FieldName.Contains("_10"))
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-2", "-3");
                                        }
                                    }
                                    if (Value != null)
                                    {

                                        if (Convert.ToString(Value).Length > Length)
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                            }
                            #endregion
                            //return ErrorList;
                        }

                        foreach (CustomAttributeData Customitem in CustomValidatioList)
                        {
                            string errormsg = string.Empty;
                            switch (pDFFiledDataInspectorModel.FieldType)
                            {
                                case PDFFieldType.TextBox:
                                    PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = txtbxField.ToolTip;
                                    break;
                                case PDFFieldType.RadioButton:
                                    PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = radioButton.ToolTip;
                                    break;
                            }
                            if (skip == false)
                            {
                                if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                {
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                                    {
                                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                        if (OfficeType == "J")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-3", "-4");
                                        }

                                        ErrorMessage = ErrorMessage.Replace("Please enter", officialTableNo + " Please enter");
                                    }
                                    if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                                    {
                                        if (pDFFiledDataInspectorModel.FieldName.Contains("_9") || pDFFiledDataInspectorModel.FieldName.Contains("_10"))
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-2", "-3");
                                        }
                                    }
                                    if (T_type.Name == "EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0")
                                    {
                                        if (pDFFiledDataInspectorModel.FieldName.Contains("INSP_LEVEL"))
                                        {
                                            if (Value == "0")
                                            {
                                                objViewModel.ErrorOccured = true;

                                                ErrorList.Add(ErrorMessage + erroradd);
                                            }
                                        }

                                    }
                                    if (Value == null || Value == "" || Value == " ")
                                    {
                                        objViewModel.ErrorOccured = true;

                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                            {
                                string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                                {
                                    string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                    if (OfficeType == "J")
                                    {
                                        ErrorMessage = ErrorMessage.Replace("-3", "-4");
                                    }

                                    ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                }
                                if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName.Contains("_9") || pDFFiledDataInspectorModel.FieldName.Contains("_10"))
                                    {
                                        ErrorMessage = ErrorMessage.Replace("-2", "-3");
                                    }
                                }

                                if (Value != null && Value != "")
                                {
                                    if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                    if (isdateField == true)
                                    {
                                        if (!MatchDateFormat(Convert.ToString(Value)))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                        }
                                    }
                                }

                            }
                            if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                            {
                                int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                if (T_type.Name == "EFORM_QC_OFFICIAL_DETAILS_PIU")
                                {
                                    string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                    if (OfficeType == "J")
                                    {
                                        ErrorMessage = ErrorMessage.Replace("-3", "-4");
                                    }

                                    ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                }
                                if (T_type.Name == "EFORM_PRGS_DETAILS_PIU")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName.Contains("_9") || pDFFiledDataInspectorModel.FieldName.Contains("_10"))
                                    {
                                        ErrorMessage = ErrorMessage.Replace("-2", "-3");
                                    }
                                }
                                if (Value != null)
                                {

                                    if (Convert.ToString(Value).Length > Length)
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(item.Name);
                        ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToModel()");
                        ErrorList.Add("Page-1: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                    }
                }
                return ErrorList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToModel()");
                ErrorList.Add("Error Occured while fetching data from pdf...");

                return ErrorList;
            }
            finally
            {
                eformdbContext.Dispose();

            }
        }

        public List<string> FetchPDFFilledDataToQMModel(object T, PdfLoadedForm loadedForm)
        {
            bool isdateField = false;
            bool skip = false;
            string erroradd = string.Empty;
            List<string> ErrorList = new List<string>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                FieldTypeAttribute fieldTypeAttribute = new FieldTypeAttribute();
                RoadStatusDependableAttribute roadStatusDependableAttribute = new RoadStatusDependableAttribute();

                //----Vikky-----
                IsNewTechGSDependableGSAttribute isNewTechGSDependableAttribute = new IsNewTechGSDependableGSAttribute();
                IsNewTechBL1DependableGSAttribute isNewTechBL1DependableAttribute = new IsNewTechBL1DependableGSAttribute();
                IsNewTechBL2DependableGSAttribute isNewTechBL2DependableAttribute = new IsNewTechBL2DependableGSAttribute();
                IsNewTechBL3DependableGSAttribute isNewTechBL3DependableAttribute = new IsNewTechBL3DependableGSAttribute();


                // Saurabh
                DeficiencyStatusDependable deficiencyStatusDependable = new DeficiencyStatusDependable();
                TableStatusDependable tableStatusAttribute = new TableStatusDependable();
                DelayStatusDependable delayStatusAttribute = new DelayStatusDependable();
                IsDateExtendedDependable isDateExtendedAttribute = new IsDateExtendedDependable();
                InProgressDependable inProgressDependableAttribute = new InProgressDependable();


                // Srishti
                IsNewTechUsed19ValAttribute isNewTechUsed19ValAttribute = new IsNewTechUsed19ValAttribute();
                IsNewTechQtyUsed20ValAttribute isNewTechQtyUsed20ValAttribute = new IsNewTechQtyUsed20ValAttribute();
                IsNewTechUsed22ValAttribute isNewTechUsed22ValAttribute = new IsNewTechUsed22ValAttribute();

                //Bhushan
                IsNewTechUsedNTValAttribute isNewTechUsedNTValAttribute = new IsNewTechUsedNTValAttribute();


                RegularExpressionAttribute regularExpressionAttribute = new RegularExpressionAttribute("");
                StringLengthAttribute StringLengthAttribute = new StringLengthAttribute(0);
                RequiredAttribute requiredAttribute = new RequiredAttribute();
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();



                var T_type = T.GetType();
                var AttributeType = fieldTypeAttribute.GetType();
                var RoadStatusAttributeType = roadStatusDependableAttribute.GetType();
                var newTechUsedGSAttributeType = isNewTechGSDependableAttribute.GetType();
                var newTechUsedBL1AttributeType = isNewTechBL1DependableAttribute.GetType();
                var newTechUsedBL2AttributeType = isNewTechBL2DependableAttribute.GetType();
                var newTechUsedBL3AttributeType = isNewTechBL3DependableAttribute.GetType();


                // Saurabh
                var TableTypeAttributeType = tableStatusAttribute.GetType();
                var DelayTypeAttributeType = delayStatusAttribute.GetType();
                var DateExtendedAttributeType = isDateExtendedAttribute.GetType();
                var InProgressAttributeType = inProgressDependableAttribute.GetType();
                var DeficiencyTypeAttribute = deficiencyStatusDependable.GetType();

                // Srishti 
                var newTechUsed19ValAttributeType = isNewTechUsed19ValAttribute.GetType();
                var newTechQtyUsed20ValAttributeType = isNewTechQtyUsed20ValAttribute.GetType();
                var newTechUsed22ValAttributeType = isNewTechUsed22ValAttribute.GetType();

                //Bhushan
                var NewTechUsedNTValAttribute = isNewTechUsedNTValAttribute.GetType();


                var RegexAttributeType = regularExpressionAttribute.GetType();
                var StringLengthAttributeType = StringLengthAttribute.GetType();
                var requiredAttributeType = requiredAttribute.GetType();
                List<PropertyInfo> PropertyList = T_type.GetProperties().ToList();
                pDFFiledDataInspectorModel.LoadedForm = loadedForm;
                EFORM_QM_VIEWMODEL objViewModel = new EFORM_QM_VIEWMODEL();

                PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                int e_formId = Convert.ToInt32(eformId.Text);
                var terranType = dbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == e_formId).Select(x => x.TERRAIN_TYPE).FirstOrDefault();




                foreach (PropertyInfo item in PropertyList)
                {
                    skip = false;
                    erroradd = "";
                    isdateField = false;
                    CustomAttributeData RoadStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == RoadStatusAttributeType).FirstOrDefault();
                    CustomAttributeData newTechStatusGSAttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechUsedGSAttributeType).FirstOrDefault();
                    CustomAttributeData newTechStatusBL1AttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechUsedBL1AttributeType).FirstOrDefault();
                    CustomAttributeData newTechStatusBL2AttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechUsedBL2AttributeType).FirstOrDefault();
                    CustomAttributeData newTechStatusBL3AttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechUsedBL3AttributeType).FirstOrDefault();


                    // Saurabh
                    CustomAttributeData DeficincyStatusAttribute = item.CustomAttributes.Where(m => m.AttributeType == DeficiencyTypeAttribute).FirstOrDefault();
                    CustomAttributeData TableStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == TableTypeAttributeType).FirstOrDefault();
                    CustomAttributeData DelayStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == DelayTypeAttributeType).FirstOrDefault();
                    CustomAttributeData DateExtendedAttributeData = item.CustomAttributes.Where(m => m.AttributeType == DateExtendedAttributeType).FirstOrDefault();
                    CustomAttributeData InProgressAttributeData = item.CustomAttributes.Where(m => m.AttributeType == InProgressAttributeType).FirstOrDefault();


                    // Srishti
                    CustomAttributeData newTechUsed19ValAttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechUsed19ValAttributeType).FirstOrDefault();
                    CustomAttributeData newTechQtyUsed20ValAttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechQtyUsed20ValAttributeType).FirstOrDefault();
                    CustomAttributeData newTechUsed22ValAttributeData = item.CustomAttributes.Where(m => m.AttributeType == newTechUsed22ValAttributeType).FirstOrDefault();


                    //Bhushan
                    CustomAttributeData NewTechUsedNTValAttributedata = item.CustomAttributes.Where(m => m.AttributeType == NewTechUsedNTValAttribute).FirstOrDefault();




                    CustomAttributeData AttributeData = item.CustomAttributes.Where(m => m.AttributeType == AttributeType).FirstOrDefault();
                    pDFFiledDataInspectorModel.FieldType = (PDFFieldType)AttributeData.NamedArguments.FirstOrDefault().TypedValue.Value;
                    if (pDFFiledDataInspectorModel.FieldType == PDFFieldType.Skip)
                        continue;

                    pDFFiledDataInspectorModel.FieldName = item.Name;


                    #region----saurabh---

                    if (T_type.Name == "EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM")
                    {
                        //  TableStatusAttributeData, DelayStatusAttributeData,DateExtendedAttributeData, InProgressAttributeData
                        PdfLoadedRadioButtonListField ISDateExtended = loadedForm.Fields["P_IS_AS_PER_SCHEDULE"] as PdfLoadedRadioButtonListField;
                        if (ISDateExtended.SelectedValue == null)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "P_IS_AMOUNT_REFUNDED")
                            {
                                continue;
                            }
                        }

                        if (TableStatusAttributeData != null)
                        {
                            bool TableStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "Table_Status").FirstOrDefault().GetValue(T));
                            //    string Work_Status = Convert.ToString(PropertyList.Where(m => m.Name == "WORK_STATUS_32_1").FirstOrDefault().GetValue(T));
                            if (TableStatus == true)
                            {
                                continue;
                            }
                            if (DateExtendedAttributeData != null)
                            {
                                bool IsDateExtendedStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsDateExtended").FirstOrDefault().GetValue(T));
                                if (IsDateExtendedStatus == true)
                                {
                                    continue;
                                }
                            }
                            if (DelayStatusAttributeData != null)
                            {
                                bool IsDelayStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsDelay").FirstOrDefault().GetValue(T));
                                if (IsDelayStatus == true)
                                {
                                    continue;
                                }

                            }


                        }
                        else if (InProgressAttributeData != null)
                        {
                            bool TableStatusInProgress = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TableStatusInProgress").FirstOrDefault().GetValue(T));

                            if (TableStatusInProgress == true)
                            {
                                continue;
                            }

                            if (DelayStatusAttributeData != null)
                            {
                                bool IsDelayStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsDelay").FirstOrDefault().GetValue(T));
                                if (IsDelayStatus == true)
                                {
                                    continue;
                                }

                            }
                        }

                    }
                    if (T_type.Name == "EFORM_ACTION_TAKEN_PIU_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    if (T_type.Name == "EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM")
                    {
                        PdfLoadedRadioButtonListField CheckField = loadedForm.Fields["WORK_STATUS_32"] as PdfLoadedRadioButtonListField;
                        if (CheckField.SelectedValue != null)
                        {
                            if (CheckField.SelectedValue == "Y" && pDFFiledDataInspectorModel.FieldName != "WORK_STATUS_32")
                                continue;
                        }
                        if (DeficincyStatusAttribute != null)
                        {
                            string DifferenceInQM = Convert.ToString(PropertyList.Where(m => m.Name == "WORK_STATUS_CHECK").FirstOrDefault().GetValue(T));
                            if (DifferenceInQM == "Y")
                                continue;
                        }
                    }


                    if (T_type.Name == "EFORM_DIFFEENCE_IN_OBSERVATION_QM")
                    {
                        if (DeficincyStatusAttribute != null)
                        {
                            string DifferenceInQM = Convert.ToString(PropertyList.Where(m => m.Name == "DifferenceInPrevQM").FirstOrDefault().GetValue(T));
                            if (DifferenceInQM == "N")
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "COMMENT_ON_DIFFERENCE_33")
                        {
                            PdfLoadedRadioButtonListField isDiff = loadedForm.Fields["IS_DIFFERENCE_FOUND"] as PdfLoadedRadioButtonListField;
                            if (isDiff.SelectedValue == "Y")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }

                    if (T_type.Name == "EFORM_DEFICIENCY_PREPARATION_QM")
                    {
                        if (DeficincyStatusAttribute != null)
                        {
                            string DeficiencyStatus = Convert.ToString(PropertyList.Where(m => m.Name == "DEFICIENCY_STATUS").FirstOrDefault().GetValue(T));
                            if (DeficiencyStatus == "N")
                                continue;
                        }
                    }





                    if (T_type.Name == "EFORM_QM_CHILD_CUT_SLOPE_DETAIL")
                    {
                        skip = true;
                        if (terranType == "2" || terranType == "3")
                        {
                            skip = false;
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }
                    if (T_type.Name == "EFORM_QM_CHILD_SIDE_SLOPE_DETAIL")
                    {
                        skip = true;
                        if (terranType == "1")
                        {
                            skip = false;
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }
                    if (T_type.Name == "EFORM_QM_PRESENT_WORK_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        int item_id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = dbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "P" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault();

                    }
                    if (T_type.Name == "EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }


                    if (T_type.Name == "EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        if (pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_2") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_3") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_4")
                            || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_6") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_7") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_8")
                            || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_10") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_11") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_12"))
                        {
                            continue;
                        }

                    }
                    if (T_type.Name == "EFORM_QM_QC_TEST_DETAILS_Temp2_0")
                    {
                        if (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) == 1)
                        {
                            pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        }
                        else
                        {
                            pDFFiledDataInspectorModel.FieldName += "_" + (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) - 1) + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                            skip = true;
                            if (pDFFiledDataInspectorModel.FieldName.Equals("DPR_QUANTITY" + "_" + (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) - 1) + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T)))
                                || pDFFiledDataInspectorModel.FieldName.Equals("EXECUTED_QUANTITY" + "_" + (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) - 1) + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T))))
                            {
                                continue;
                            }
                        }



                        int item_id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = dbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "A" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault() + " of row " + Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }



                    if (T_type.Name == "EFORM_QM_QC_TEST_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        int item_id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = dbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "A" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault();

                    }

                    if (T_type.Name == "EFORM_QM_GEOMETRICS_OBS_DETAILS")
                    {
                        string GeometricType = Convert.ToString(PropertyList.Where(m => m.Name == "GEOMETRIC_TYPE").FirstOrDefault().GetValue(T));
                        if (GeometricType == "R")
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "C4IIA_ELEVATION_PER_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IIA_ELEVATION_PER_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IIA_ELEVATION_PER_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IIB_EXTRA_WIDENING_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IIB_EXTRA_WIDENING_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IIB_EXTRA_WIDENING_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IIIA_LONG_GRAD_PER_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IIIA_LONG_GRAD_PER_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IIIA_LONG_GRAD_PER_GRADE" || pDFFiledDataInspectorModel.FieldName == "ROAD_FROM" || pDFFiledDataInspectorModel.FieldName == "ROAD_TO")
                                continue;
                        }
                        if (GeometricType == "S")
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "C4IIIA_LONG_GRAD_PER_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IIIA_LONG_GRAD_PER_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IIIA_LONG_GRAD_PER_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IA_ROAD_WIDTH_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IA_ROAD_WIDTH_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IA_ROAD_WIDTH_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IB_CARRIAGE_WIDTH_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IB_CARRIAGE_WIDTH_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IB_CARRIAGE_WIDTH_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IC_CAMBER_PER_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IC_CAMBER_PER_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IC_CAMBER_PER_GRADE" || pDFFiledDataInspectorModel.FieldName == "ROAD_FROM" || pDFFiledDataInspectorModel.FieldName == "ROAD_TO")
                                continue;
                            //if (pDFFiledDataInspectorModel.FieldName == "ROAD_LOC")
                            //    skip = true;

                            string rowId = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                            if (rowId == "1")
                            {

                                PdfLoadedTextBoxField locRoad = loadedForm.Fields["S_ROAD_LOC_1"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField asPerDpr = loadedForm.Fields["S_C4IIA_ELEVATION_PER_DPR_1"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField actualAtSite = loadedForm.Fields["S_C4IIA_ELEVATION_PER_ACTUAL_1"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField asPerDprEWP = loadedForm.Fields["S_C4IIB_EXTRA_WIDENING_DPR_1"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField actualAtSiteEWP = loadedForm.Fields["S_C4IIB_EXTRA_WIDENING_ACTUAL_1"] as PdfLoadedTextBoxField;
                                PdfLoadedRadioButtonListField gradeElev = loadedForm.Fields["S_C4IIA_ELEVATION_PER_GRADE_1"] as PdfLoadedRadioButtonListField;
                                PdfLoadedRadioButtonListField gradeEWP = loadedForm.Fields["S_C4IIB_EXTRA_WIDENING_GRADE_1"] as PdfLoadedRadioButtonListField;
                                if (locRoad.Text == "" && asPerDpr.Text == "" && actualAtSite.Text == "" && asPerDprEWP.Text == "" && actualAtSiteEWP.Text == "" && gradeElev.SelectedValue == null && gradeEWP.SelectedValue == null)
                                {
                                    skip = true;
                                }

                            }

                        }
                        if (GeometricType == "L")
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "C4IIA_ELEVATION_PER_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IIA_ELEVATION_PER_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IIA_ELEVATION_PER_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IA_ROAD_WIDTH_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IA_ROAD_WIDTH_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IA_ROAD_WIDTH_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IB_CARRIAGE_WIDTH_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IB_CARRIAGE_WIDTH_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IB_CARRIAGE_WIDTH_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IC_CAMBER_PER_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IC_CAMBER_PER_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IC_CAMBER_PER_GRADE" || pDFFiledDataInspectorModel.FieldName == "C4IIB_EXTRA_WIDENING_DPR" || pDFFiledDataInspectorModel.FieldName == "C4IIB_EXTRA_WIDENING_ACTUAL" || pDFFiledDataInspectorModel.FieldName == "C4IIB_EXTRA_WIDENING_GRADE" || pDFFiledDataInspectorModel.FieldName == "ROAD_LOC") // ROAD_LOC
                                continue;
                            bool flag = false;
                            try
                            {
                                PdfLoadedCheckBoxField terrainChk = loadedForm.Fields["CB_TERRAIN"] as PdfLoadedCheckBoxField;
                                if (terrainChk.Checked)
                                {
                                    flag = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = true;
                                ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToQMModel()");
                            }


                            if (flag == false)
                            {
                                continue;
                            }
                        }

                        pDFFiledDataInspectorModel.FieldName = GeometricType + "_" + pDFFiledDataInspectorModel.FieldName + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));  //change on 22-07-2022
                        erroradd = " of row " + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));  //add on 22-07-2022

                    }


                    if (T_type.Name == "EFORM_QM_ARRANGEMENTS_OBS_DETAILS")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_7")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_2"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "SRI" || itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }


                    }
                    if (T_type.Name == "EFORM_QM_QUALITY_ATTENTION")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_9")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_3"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "SRI" || itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }

                    #endregion



                    #region----vikky------

                    if (T_type.Name == "EFORM_CHILD_GRANULAR_UCS_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_CHILD_GRANULAR_QOM_OBS_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }



                    if (T_type.Name == "EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER1_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }


                    if (T_type.Name == "EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER1_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }




                    if (T_type.Name == "EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }


                    if (T_type.Name == "EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }


                    if (T_type.Name == "EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER3_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }


                    if (T_type.Name == "EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER3_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }

                    if (T_type.Name == "EFORM_CDWORKS_PIPE_CULVERTS_QM")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_23")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_13"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }

                    if (T_type.Name == "EFORM_CDWORKS_SLAB_CULVERTS_QM")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_24")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_14"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }


                    if (T_type.Name == "EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }


                    if (T_type.Name == "EFORM_PROTECTION_WORK_QM")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_25")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_15"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }

                    if (T_type.Name == "EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_CRASH_BARRIERS_ROAD_SAFETY_QM")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_26")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_16"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }
                    if (T_type.Name == "EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_SIDE_AND_CATCH_DRAINS_EARTHEN_QM")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_27")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_17"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }

                    if (T_type.Name == "EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));


                    }



                    if (T_type.Name == "EFORM_GRANULAR_SUBBASE_QM")
                    {
                        if (newTechStatusGSAttributeData != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsNewTechUsedGSStatus").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_15")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_6"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }

                    if (T_type.Name == "EFORM_BASE_COURSE_I_QM")
                    {
                        if (newTechStatusBL1AttributeData != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsNewTechUsedBL1Status").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_16")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_7"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    if (T_type.Name == "EFORM_BASE_COURSE_2_QM")
                    {
                        if (newTechStatusBL2AttributeData != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsNewTechUsedBL2Status").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_17")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_8"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    if (T_type.Name == "EFORM_BASE_COURSE_3_QM")
                    {
                        if (newTechStatusBL3AttributeData != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsNewTechUsedBL3Status").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_18")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_9"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }
                    #endregion


                    #region  -----Bhushan----


                    if (T_type.Name == "EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_QM_CHILD_GROUP_SYMBOL_SOIL")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_QM_CHILD_DEGREE_OF_COMPAQ")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));


                    }
                    if (T_type.Name == "EFORM_QM_NEW_TECHNOLOGY_DETAILS")
                    {
                        if (NewTechUsedNTValAttributedata != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "IsNewTechUsedNTStatus").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_11")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["SUBITEM_GRADING_5_I"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }


                    }

                    if (T_type.Name == "EFORM_QM_QOM_EMBANKMENT")
                    {
                        try
                        {

                            if (pDFFiledDataInspectorModel.FieldName == "APPROVED_SRC_REMARKS")
                            {
                                skip = true;
                                PdfLoadedTextBoxField leadDist = loadedForm.Fields["DIST_SOE"] as PdfLoadedTextBoxField;
                                if (leadDist.Text != "")
                                {
                                    if (Convert.ToInt16(leadDist.Text) > 5)
                                    {
                                        skip = false;
                                    }
                                }
                            }
                            if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_12_1")
                            {
                                PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["SUBITEM_GRADING_5_II"] as PdfLoadedRadioButtonListField;
                                if (itemGrad.SelectedValue == "U")
                                {
                                    skip = false;
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToQMModel()");
                        }

                    }
                    if (T_type.Name == "EFORM_QM_COMPAQ_EMBANKMENT")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_12_2")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["SUBITEM_GRADING_5_III"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    if (T_type.Name == "EFORM_QM_SIDE_SLOPES")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "OBSERVATIONS")
                        {
                            PdfLoadedRadioButtonListField isAnalDone = loadedForm.Fields["IS_ANALYSIS_DONE"] as PdfLoadedRadioButtonListField;
                            if (isAnalDone.SelectedValue == "N")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_5IV")
                        {
                            PdfLoadedRadioButtonListField isAnalDone = loadedForm.Fields["SUBITEM_GRADING_5IV"] as PdfLoadedRadioButtonListField;
                            if (isAnalDone.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_5")
                        {
                            PdfLoadedRadioButtonListField isAnalDone = loadedForm.Fields["ITEM_GRADING_5"] as PdfLoadedRadioButtonListField;
                            if (isAnalDone.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    //-----Bhushan---- page No.27,28,29,30
                    if (T_type.Name == "EFORM_QM_CC_SR_PVAEMENTS")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_28")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_18"] as PdfLoadedRadioButtonListField;
                            // if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }


                    if (T_type.Name == "EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_QM_CC_PUCCA_DRAINS")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_30")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_19"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }


                    if (T_type.Name == "EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));

                    }


                    if (T_type.Name == "EFORM_QM_ROAD_FURNITURE_MARKINGS")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (!RoadStatus)
                                continue;

                            if (RoadStatus == true)
                            {   // CURRENT_STAGE ==1

                                PdfLoadedRadioButtonListField IsStage = loadedForm.Fields["CURRENT_STAGE"] as PdfLoadedRadioButtonListField;
                                if (pDFFiledDataInspectorModel.FieldName == "IS_MAINTANANCE_BOARD_FIXED_30")
                                {
                                    if (IsStage.SelectedValue == "1")
                                    {
                                        continue;
                                    }
                                }


                            }
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTIONS_31")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_20"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }


                    #endregion



                    #region ----- Srishti -----

                    if (T_type.Name == "EFORM_QM_OVERALL_GRADING")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }

                    }

                    if (T_type.Name == "EFORM_QM_QUALITY_GRADING")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }

                    }

                    if (T_type.Name == "EFORM_QM_BITUMINOUS_BASE_COURSE")
                    {
                        if (newTechUsed19ValAttributeData != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "radioButtonValue").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTION_20")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_10"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "INSP_DATE_19_af_date")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["IS_HOT_MIX_DONE_19"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "Y")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    if (T_type.Name == "EFORM_QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_QM_BITUMINOUS_SURFACE_COURSE")
                    {
                        if (newTechQtyUsed20ValAttributeData != null)
                        {

                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "radioButtonValue").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTION_22")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_11_22"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                        //add on 23-08-2022
                        if (pDFFiledDataInspectorModel.FieldName == "INVOICE_INSUFFICIENT_REASON_21")
                        {
                            PdfLoadedRadioButtonListField check_IS_BITUMEN_USED_21 = loadedForm.Fields["IS_BITUMEN_USED_21"] as PdfLoadedRadioButtonListField;
                            if (check_IS_BITUMEN_USED_21.SelectedValue == "N")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        //end here add on 23-08-2022
                    }


                    if (T_type.Name == "EFORM_QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_QM_SHOULDERS")
                    {
                        if (newTechUsed22ValAttributeData != null)
                        {
                            bool isnewTechStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "radioButtonValue").FirstOrDefault().GetValue(T));
                            if (!isnewTechStatus)
                                continue;
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVE_SUGGESTION_23")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_12"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }

                    if (T_type.Name == "EFORM_QM_SHOULDERS")
                    {
                        PdfLoadedRadioButtonListField MATERIAL_TYPE_22_radio = loadedForm.Fields["MATERIAL_TYPE_22"] as PdfLoadedRadioButtonListField;

                        if (MATERIAL_TYPE_22_radio.SelectedValue == "E" && (pDFFiledDataInspectorModel.FieldName == "MATERIAL_WIDTH_22" || pDFFiledDataInspectorModel.FieldName == "MATERIAL_THICKNESS_22"))
                        {
                            pDFFiledDataInspectorModel.FieldName += "_1";
                        }
                        else if (MATERIAL_TYPE_22_radio.SelectedValue == "M" && (pDFFiledDataInspectorModel.FieldName == "MATERIAL_WIDTH_22" || pDFFiledDataInspectorModel.FieldName == "MATERIAL_THICKNESS_22"))
                        {
                            pDFFiledDataInspectorModel.FieldName += "_2";
                        }
                        else if (MATERIAL_TYPE_22_radio.SelectedValue == "G" && (pDFFiledDataInspectorModel.FieldName == "MATERIAL_WIDTH_22" || pDFFiledDataInspectorModel.FieldName == "MATERIAL_THICKNESS_22"))
                        {
                            pDFFiledDataInspectorModel.FieldName += "_3";
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "MATERIAL_WIDTH_22" || pDFFiledDataInspectorModel.FieldName == "MATERIAL_THICKNESS_22")
                        {
                            if (MATERIAL_TYPE_22_radio.SelectedValue == null)
                            {
                                continue;
                            }

                        }
                    }

                    if (T_type.Name == "EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_QM_CHILD_SHOULDERS_MATERIAL_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowId").FirstOrDefault().GetValue(T));
                    }

                    #endregion
                    #region dateField or not
                    isdateField = false;
                    if (pDFFiledDataInspectorModel.FieldName == "INSPECTION_DATE"
             || pDFFiledDataInspectorModel.FieldName == "QCR1_DATE_OF_TEST_1"
             || pDFFiledDataInspectorModel.FieldName == "QCR1_DATE_OF_TEST_2"
             || pDFFiledDataInspectorModel.FieldName == "QCR1_DATE_OF_TEST_3"
             || pDFFiledDataInspectorModel.FieldName == "INSP_DATE_19_af_date"
             || pDFFiledDataInspectorModel.FieldName == "UPLOADING_DATE_38"


             )
                    {
                        isdateField = true;
                    }
                    #endregion
                    try
                    {
                        GetFilledDataFromPDFField(pDFFiledDataInspectorModel);
                    }
                    catch (Exception ex)
                    {

                        ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToQMModel()");
                        ErrorList.Add("Page-6: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                        objViewModel.ErrorOccured = true;
                        // return ErrorList;
                    }

                    //item.SetValue(T, pDFFiledDataInspectorModel.Value);
                    string Value = string.Empty;
                    if (pDFFiledDataInspectorModel.Value != "")
                    {
                        Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    }

                    try
                    {
                        var converter = TypeDescriptor.GetConverter(item.PropertyType);
                        List<CustomAttributeData> CustomValidatioList = item.CustomAttributes.Where(m => m.AttributeType != AttributeType).ToList();
                        try
                        {
                            if (Value != "" && Value != null)
                            {
                                T.GetType().InvokeMember(item.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                               Type.DefaultBinder, T, new object[] { converter.ConvertFromString(Value) });
                            }

                        }
                        catch (Exception e)
                        {
                            #region  catch code
                            foreach (CustomAttributeData Customitem in CustomValidatioList)
                            {
                                string errormsg = string.Empty;
                                switch (pDFFiledDataInspectorModel.FieldType)
                                {
                                    case PDFFieldType.TextBox:
                                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = txtbxField.ToolTip;
                                        break;
                                    case PDFFieldType.RadioButton:
                                        PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = radioButton.ToolTip;
                                        break;
                                    case PDFFieldType.ComboBox:
                                        PdfLoadedComboBoxField cmbbxField = (PdfLoadedComboBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = cmbbxField.ToolTip;
                                        if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                        {

                                            string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                            if (Value == "1")
                                            {
                                                objViewModel.ErrorOccured = true;
                                                ErrorList.Add(ErrorMessage + errormsg);
                                            }
                                        }
                                        break;
                                }

                                if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                {
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (skip == false)
                                    {
                                        if (Value == null || Value == "" || Value == " ")
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                                {
                                    string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (Value != null && Value != "")
                                    {
                                        if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                        if (isdateField == true)
                                        {
                                            if (!MatchDateFormat(Convert.ToString(Value)))
                                            {
                                                objViewModel.ErrorOccured = true;
                                                ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                            }
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                                {
                                    int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (Value != null)
                                    {

                                        if (Convert.ToString(Value).Length > Length)
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                            }
                            #endregion
                            // return ErrorList;
                        }

                        foreach (CustomAttributeData Customitem in CustomValidatioList)
                        {
                            string errormsg = string.Empty;
                            switch (pDFFiledDataInspectorModel.FieldType)
                            {
                                case PDFFieldType.TextBox:
                                    PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = txtbxField.ToolTip;
                                    break;
                                case PDFFieldType.RadioButton:
                                    PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = radioButton.ToolTip;
                                    break;
                                case PDFFieldType.ComboBox:
                                    PdfLoadedComboBoxField cmbbxField = (PdfLoadedComboBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = cmbbxField.ToolTip;
                                    if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                    {

                                        string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                        if (Value == "1")
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + errormsg);
                                        }
                                    }
                                    break;
                            }

                            if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                            {
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                if (skip == false)
                                {
                                    if (Value == null || Value == "" || Value == " ")
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                            {
                                string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                if (Value != null && Value != "")
                                {
                                    if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                    if (isdateField == true)
                                    {
                                        if (!MatchDateFormat(Convert.ToString(Value)))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                        }
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                            {
                                int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                if (Value != null)
                                {

                                    if (Convert.ToString(Value).Length > Length)
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        objViewModel.ErrorOccured = true;
                        ErrorList.Add("Page-6: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                        //  return ErrorList;
                    }
                }
                return ErrorList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.FetchPDFFilledDataToQMModel()");
                ErrorList.Add("Error Occured while fetching data from pdf...");
                //objViewModel.ErrorOccured = true;
                return ErrorList;
            }
            finally
            {
                dbContext.Dispose();

            }
        }


        //added by rohit on 04-08-2022
        public List<string> FetchBridgePIUPDFFilledDataToModel(object T, PdfLoadedForm loadedForm)
        {
            bool isdateField = false;
            bool skip = false;
            string erroradd = string.Empty;
            string officialType = string.Empty;
            string officialTableNo = string.Empty;
            List<string> ErrorList = new List<string>();
            PMGSYEntities eformdbContext = new PMGSYEntities();
            try
            {
                #region added by rohit on 04-08-2022

                FieldTypeAttribute fieldTypeAttribute = new FieldTypeAttribute();
                RoadStatusDependableAttribute roadStatusDependableAttribute = new RoadStatusDependableAttribute();

                RegularExpressionAttribute regularExpressionAttribute = new RegularExpressionAttribute("");
                StringLengthAttribute StringLengthAttribute = new StringLengthAttribute(0);
                RequiredAttribute requiredAttribute = new RequiredAttribute();
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                var T_type = T.GetType();
                var AttributeType = fieldTypeAttribute.GetType();
                var RoadStatusAttributeType = roadStatusDependableAttribute.GetType();


                var RegexAttributeType = regularExpressionAttribute.GetType();
                var StringLengthAttributeType = StringLengthAttribute.GetType();
                var requiredAttributeType = requiredAttribute.GetType();
                List<PropertyInfo> PropertyList = T_type.GetProperties().ToList();
                pDFFiledDataInspectorModel.LoadedForm = loadedForm;
                EFORM_BRIDGE_PIU_VIEW_MODEL objViewModel = new EFORM_BRIDGE_PIU_VIEW_MODEL();



                foreach (PropertyInfo item in PropertyList)
                {
                    skip = false;
                    erroradd = "";
                    officialType = string.Empty;
                    officialTableNo = string.Empty;
                    CustomAttributeData RoadStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == RoadStatusAttributeType).FirstOrDefault();


                    CustomAttributeData AttributeData = item.CustomAttributes.Where(m => m.AttributeType == AttributeType).FirstOrDefault();
                    pDFFiledDataInspectorModel.FieldType = (PDFFieldType)AttributeData.NamedArguments.FirstOrDefault().TypedValue.Value;
                    if (pDFFiledDataInspectorModel.FieldType == PDFFieldType.Skip)
                        continue;

                    pDFFiledDataInspectorModel.FieldName = item.Name;
                    if (T_type.Name == "EFORM_BRIDGE_PIU_PARTICULARS")
                    {
                        skip = false;
                        PdfLoadedRadioButtonListField isFootpathProv = loadedForm.Fields["IS_FOOTPATH_PROVIDED"] as PdfLoadedRadioButtonListField;// IS_RIVER_PROT_PROVIDED
                        if (isFootpathProv.SelectedValue == "No" || isFootpathProv.SelectedValue == null)
                        {
                            if (item.Name == "FOOTPATH_WIDTH")
                            {
                                continue;
                            }
                        }
                        PdfLoadedCheckBoxField ReturnCheck = loadedForm.Fields["CB_3"] as PdfLoadedCheckBoxField;  // RETURNS_R1 
                        PdfLoadedRadioButtonListField RiverProtProvided = loadedForm.Fields["IS_RIVER_PROT_PROVIDED"] as PdfLoadedRadioButtonListField;// IS_RIVER_PROT_PROVIDED
                        if (RiverProtProvided.SelectedValue == "No" || RiverProtProvided.SelectedValue == null)
                        {
                            if (item.Name == "RIVER_PROT_PROVIDED")
                            {
                                continue;
                            }
                        }
                        // SUBSTRUCTURE_PIERS , OTHER_SUBSTRUCTURE_PIERS
                        if (item.Name == "SUBSTRUCTURE_PIERS" || item.Name == "OTHER_SUBSTRUCTURE_PIERS" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P4" || pDFFiledDataInspectorModel.FieldName == "PIERS_P5" || pDFFiledDataInspectorModel.FieldName == "PIERS_P6" || pDFFiledDataInspectorModel.FieldName == "PIERS_P7" || pDFFiledDataInspectorModel.FieldName == "PIERS_P8")
                        {

                            PdfLoadedCheckBoxField check_CB_1 = loadedForm.Fields["CB_1"] as PdfLoadedCheckBoxField;
                            if (check_CB_1.Checked == false)
                            {
                                continue;
                            }
                            else
                            {
                                skip = true;
                                PdfLoadedTextBoxField FOUNDATION_PIERS9text = loadedForm.Fields["PIERS_P9"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS8text = loadedForm.Fields["PIERS_P8"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS7text = loadedForm.Fields["PIERS_P7"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS6text = loadedForm.Fields["PIERS_P6"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS5text = loadedForm.Fields["PIERS_P5"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS4text = loadedForm.Fields["PIERS_P4"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS3text = loadedForm.Fields["PIERS_P3"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS2text = loadedForm.Fields["PIERS_P2"] as PdfLoadedTextBoxField;
                                if (item.Name == "OTHER_SUBSTRUCTURE_PIERS")
                                {
                                    PdfLoadedRadioButtonListField SUBSTRUCTURE_PIERS = loadedForm.Fields["SUBSTRUCTURE_PIERS"] as PdfLoadedRadioButtonListField;// IS_RIVER_PROT_PROVIDED

                                    if (SUBSTRUCTURE_PIERS.SelectedValue != "OTHER_TYPE")
                                    {
                                        continue;
                                    }
                                }
                                if (item.Name == "SUBSTRUCTURE_PIERS")
                                {
                                    skip = false;
                                }
                                if (FOUNDATION_PIERS9text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P8" || pDFFiledDataInspectorModel.FieldName == "PIERS_P7" || pDFFiledDataInspectorModel.FieldName == "PIERS_P6" || pDFFiledDataInspectorModel.FieldName == "PIERS_P5" || pDFFiledDataInspectorModel.FieldName == "PIERS_P4" || pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }
                                }
                                if (FOUNDATION_PIERS8text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P7" || pDFFiledDataInspectorModel.FieldName == "PIERS_P6" || pDFFiledDataInspectorModel.FieldName == "PIERS_P5" || pDFFiledDataInspectorModel.FieldName == "PIERS_P4" || pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS7text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P6" || pDFFiledDataInspectorModel.FieldName == "PIERS_P5" || pDFFiledDataInspectorModel.FieldName == "PIERS_P4" || pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS6text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P5" || pDFFiledDataInspectorModel.FieldName == "PIERS_P4" || pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS5text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P4" || pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS4text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P3" || pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS3text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P2" || pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS2text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (pDFFiledDataInspectorModel.FieldName == "PIERS_P1")
                                {
                                    skip = false;
                                }

                            }


                        }


                        if (item.Name == "OTHER_SUBSTRUCTURE_ABUTMENTS")
                        {
                            PdfLoadedRadioButtonListField SUBSTRUCTURE_ABUTMENTS = loadedForm.Fields["SUBSTRUCTURE_ABUTMENTS"] as PdfLoadedRadioButtonListField;// IS_RIVER_PROT_PROVIDED

                            if (SUBSTRUCTURE_ABUTMENTS.SelectedValue != "OTHER_TYPE")
                            {
                                continue;
                            }
                        }



                        if (ReturnCheck.Checked == false)
                        {
                            if (item.Name == "RETURNS_R1" || item.Name == "RETURNS_R2")
                            {
                                continue;
                            }

                            if (item.Name == "SUBSTRUCTURE_RETURNS")
                            {
                                continue;
                            }
                            if (item.Name == "OTHER_SUBSTRUCTURE_RETURNS")
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (item.Name == "OTHER_SUBSTRUCTURE_RETURNS")
                            {
                                PdfLoadedRadioButtonListField SUBSTRUCTURE_RETURNS = loadedForm.Fields["SUBSTRUCTURE_RETURNS"] as PdfLoadedRadioButtonListField;// IS_RIVER_PROT_PROVIDED

                                if (SUBSTRUCTURE_RETURNS.SelectedValue != "OTHER_TYPE" || SUBSTRUCTURE_RETURNS.SelectedValue == null)
                                {
                                    continue;
                                }
                            }
                        }
                        PdfLoadedRadioButtonListField BearingType = loadedForm.Fields["BEARING_TYPE"] as PdfLoadedRadioButtonListField;
                        if (BearingType.SelectedValue != "OTHER_TYPE")
                        {
                            if (item.Name == "OTHER_BEARING_TYPE")
                            {
                                continue;
                            }
                        }
                        //  3.21 Type of superstructure
                        PdfLoadedRadioButtonListField SuperStructType = loadedForm.Fields["SUPERSTRUCTURE_MAIN_TYPE"] as PdfLoadedRadioButtonListField; // Type of superstructure
                        if (SuperStructType.SelectedValue == "RCC")
                        {
                            if (item.Name == "STEEL_SUB_TYPE" || item.Name == "OTHER_STEEL_SUB_TYPE")
                            {
                                continue;
                            }
                            PdfLoadedRadioButtonListField RccSubType = loadedForm.Fields["RCC_SUB_TYPE"] as PdfLoadedRadioButtonListField;       // Any other type
                            if (RccSubType.SelectedValue != "OTHER_TYPE")
                            {
                                if (item.Name == "OTHER_RCC_SUB_TYPE")
                                {
                                    continue;
                                }
                            }
                        }
                        if (SuperStructType.SelectedValue == "STEEL")
                        {
                            if (item.Name == "RCC_SUB_TYPE" || item.Name == "OTHER_RCC_SUB_TYPE")
                            {
                                continue;
                            }
                            PdfLoadedRadioButtonListField SteelSubType = loadedForm.Fields["STEEL_SUB_TYPE"] as PdfLoadedRadioButtonListField;       // Any other type
                            if (SteelSubType.SelectedValue != "OTHER_TYPE")
                            {
                                if (item.Name == "OTHER_STEEL_SUB_TYPE")
                                {
                                    continue;
                                }
                            }
                        }
                        //  3.22 Type of expansion joints
                        PdfLoadedRadioButtonListField ExpanJointType = loadedForm.Fields["EXPANSION_JNT_TYPE"] as PdfLoadedRadioButtonListField;
                        if (ExpanJointType.SelectedValue != "OTHER_TYPE")
                        {
                            if (item.Name == "OHTER_EXPANSION_JNT_TYPE")
                            {
                                continue;
                            }
                        }
                        // 3.25 Design loading
                        PdfLoadedRadioButtonListField DesignLoading = loadedForm.Fields["DESIGN_LOADING"] as PdfLoadedRadioButtonListField;
                        if (DesignLoading.SelectedValue != "OTHER_TYPE")
                        {
                            if (item.Name == "OTHER_DESIGN_LOADING")
                            {
                                continue;
                            }
                        }
                        // Whether bench marks and alignment are established      near the bridge site (give locations and RLs) 
                        PdfLoadedRadioButtonListField BenchMarkESTB = loadedForm.Fields["IS_BENCHMARKS_ESTB"] as PdfLoadedRadioButtonListField;
                        if (BenchMarkESTB.SelectedValue == "No")
                        {
                            if (item.Name == "BENCHMARK_LOC1" || item.Name == "BENCHMARK_LOC2" || item.Name == "BENCHMARK_LOC3" || item.Name == "BENCHMARK_LOC4" || item.Name == "BENCHMARK_RL1" || item.Name == "BENCHMARK_RL2" || item.Name == "BENCHMARK_RL3" || item.Name == "BENCHMARK_RL4")
                            {
                                continue;
                            }
                        }

                        if (item.Name == "OTHER_RAILING_TYPE")
                        {
                            PdfLoadedRadioButtonListField RAILING_TYPE = loadedForm.Fields["RAILING_TYPE"] as PdfLoadedRadioButtonListField;// IS_RIVER_PROT_PROVIDED

                            if (RAILING_TYPE.SelectedValue != "OTHER_TYPE")
                            {
                                continue;
                            }
                        }

                    }






                    if (T_type.Name == "EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        int item_id = Convert.ToInt32(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_BRIDGE_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "M" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault();
                    }
                    if (T_type.Name == "EFORM_BRIDGE_PIU_GENERAL_INFO")
                    {

                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }

                        /// To skip fields in case of Ongoing/In-Progress
                        PdfLoadedRadioButtonListField WorkStatus = loadedForm.Fields["WORK_STATUS"] as PdfLoadedRadioButtonListField;
                        if (WorkStatus.SelectedValue == "P")
                        {
                            if (item.Name == "ACTUAL_COMPLETION_DATE")
                            {
                                continue;
                            }
                        }

                        if (WorkStatus.SelectedValue == "P")
                        {
                            if (item.Name == "COMPLETION_COST")
                            {
                                continue;
                            }
                        }

                    }


                    if (T_type.Name == "EFORM_BRIDGE_PIU_PRGS_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        int item_id = Convert.ToInt32(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_BRIDGE_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "P" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault();
                    }

                    //if (T_type.Name == "EFORM_BRIDGE_PIU_PARTICULARS")
                    //{
                    //    PdfLoadedRadioButtonListField BridgeTypeHFL = loadedForm.Fields["BRIDGE_TYPE_HFL"] as PdfLoadedRadioButtonListField;
                    //    PdfLoadedRadioButtonListField BridgeTypeMaterial = loadedForm.Fields["BRIDGE_TYPE_MATERIAL"] as PdfLoadedRadioButtonListField;
                    //    PdfLoadedRadioButtonListField FoundationType = loadedForm.Fields["FOUNDATION_TYPE"] as PdfLoadedRadioButtonListField;

                    //}
                    if (T_type.Name == "EFORM_BRIDGE_PIU_QC_DETAILS")
                    {
                        if (RoadStatusAttributeData != null)
                        {
                            bool RoadStatus = Convert.ToBoolean(PropertyList.Where(m => m.Name == "TemplateStatus").FirstOrDefault().GetValue(T));
                            if (RoadStatus)
                                continue;
                        }

                    }
                    if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                    {
                        string tableNo = string.Empty;
                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                        if (OfficeType == "C")
                        {
                            tableNo = "IV. ";
                            if (pDFFiledDataInspectorModel.FieldName == "IDENTITY_NUMBER" || pDFFiledDataInspectorModel.FieldName == "FROM_DATE" || pDFFiledDataInspectorModel.FieldName == "TO_DATE")
                                continue;
                        }
                        if (OfficeType == "S")
                        {
                            tableNo = "VI. ";
                        }
                        if (OfficeType == "J" || OfficeType == "A" || OfficeType == "S" || OfficeType == "E")
                        {
                            tableNo = ((OfficeType == "J" ? "VIII. " : (OfficeType == "A" ? "VII. " : ((OfficeType == "S" ? "VI. " : ((OfficeType == "E" ? "V. " : "IV. ")))))));
                            if (pDFFiledDataInspectorModel.FieldName == "PAN" || pDFFiledDataInspectorModel.FieldName == "EMAIL_ID")
                                continue;
                        }
                        pDFFiledDataInspectorModel.FieldName = OfficeType + "_" + pDFFiledDataInspectorModel.FieldName + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                        PdfLoadedTextBoxField txtbxField = loadedForm.Fields[pDFFiledDataInspectorModel.FieldName] as PdfLoadedTextBoxField;
                        erroradd = txtbxField.ToolTip;

                        if (OfficeType == "C")
                        {
                            officialType = "C";
                        }

                        officialTableNo = tableNo;
                    }



                    if (T_type.Name == "EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        PMGSYEntities dbContext = new PMGSYEntities();
                        PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                        int e_formId = Convert.ToInt32(eformId.Text);
                        int count = dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS.Where(s => s.EFORM_ID == e_formId).Count();
                        int l = pDFFiledDataInspectorModel.FieldName.ToString().Split('_').Length;
                        skip = false;
                        if (Convert.ToInt16(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1]) > count || Convert.ToInt16(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1]) > 8)
                        {
                            skip = true;
                        }

                        erroradd = "row " + pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[l - 1].ToString();
                    }
                    #region dateField or not
                    isdateField = false;
                    if (pDFFiledDataInspectorModel.FieldName == "ACTUAL_COMPLETION_DATE"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "DUE_START_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "DUE_END_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_START_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_1"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_2"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_3"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_4"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_5"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_6"
             || pDFFiledDataInspectorModel.FieldName == "ACTUAL_END_DATE_7"
             || pDFFiledDataInspectorModel.FieldName == "PHOTO_UPLOAD_DATE"
             || pDFFiledDataInspectorModel.FieldName == "E_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "E_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "E_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "E_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "E_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "E_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "S_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "S_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "S_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "S_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "S_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "S_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "A_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "A_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "A_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "A_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "A_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "A_TO_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "J_FROM_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "J_FROM_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "J_FROM_DATE3"
             || pDFFiledDataInspectorModel.FieldName == "J_TO_DATE1"
             || pDFFiledDataInspectorModel.FieldName == "J_TO_DATE2"
             || pDFFiledDataInspectorModel.FieldName == "J_TO_DATE3"

             )
                    {
                        isdateField = true;
                    }
                    #endregion
                    try
                    {
                        GetFilledDataFromPDFField(pDFFiledDataInspectorModel);
                    }
                    catch (Exception ex)
                    {

                        ErrorLog.LogError(ex, "CommonFunctions.FetchBridgePIUPDFFilledDataToModel()");
                        ErrorList.Add("Page-1: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                        objViewModel.ErrorOccured = true;
                        // return ErrorList;
                    }


                    //item.SetValue(T, pDFFiledDataInspectorModel.Value);

                    string Value = string.Empty;
                    if (pDFFiledDataInspectorModel.Value != "")
                    {
                        Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);

                    }
                    //  string Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(item.PropertyType);
                        List<CustomAttributeData> CustomValidatioList = item.CustomAttributes.Where(m => m.AttributeType != AttributeType).ToList();
                        try
                        {
                            if (Value != "" && Value != null)
                            {
                                T.GetType().InvokeMember(item.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                Type.DefaultBinder, T, new object[] { converter.ConvertFromString(Value) });
                            }
                        }
                        catch (Exception e)
                        {
                            #region  catch code

                            foreach (CustomAttributeData Customitem in CustomValidatioList)
                            {
                                string errormsg = string.Empty;
                                switch (pDFFiledDataInspectorModel.FieldType)
                                {
                                    case PDFFieldType.TextBox:
                                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = txtbxField.ToolTip;
                                        break;
                                    case PDFFieldType.RadioButton:
                                        PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = radioButton.ToolTip;
                                        break;
                                }
                                if (skip == false)
                                {
                                    if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                    {
                                        string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                        if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                                        {
                                            string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                            if (OfficeType == "C")
                                            {
                                                ErrorMessage = ErrorMessage.Replace("-6", "-5");
                                            }

                                            ErrorMessage = ErrorMessage.Replace("Please enter", officialTableNo + " Please enter");
                                        }
                                        if (T_type.Name == "EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS")
                                        {
                                            if (pDFFiledDataInspectorModel.FieldName.Contains("INSP_LEVEL"))
                                            {
                                                if (Value == "0")
                                                {
                                                    objViewModel.ErrorOccured = true;

                                                    ErrorList.Add(ErrorMessage + erroradd);
                                                }
                                            }

                                        }
                                        if (Value == null || Value == "" || Value == " ")
                                        {
                                            objViewModel.ErrorOccured = true;

                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                                {
                                    string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                                    {
                                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                        if (OfficeType == "C")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-6", "-5");
                                        }

                                        ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                    }
                                    if (Value != null && Value != "")
                                    {
                                        if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                        if (isdateField == true)
                                        {
                                            if (!MatchDateFormat(Convert.ToString(Value)))
                                            {
                                                objViewModel.ErrorOccured = true;
                                                ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                            }
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                                {
                                    int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                                    {
                                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                        if (OfficeType == "C")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-6", "-5");
                                        }

                                        ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                    }
                                    if (Value != null)
                                    {
                                        if (Convert.ToString(Value).Length > Length)
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                            }
                            //  return ErrorList;
                            #endregion
                        }
                        foreach (CustomAttributeData Customitem in CustomValidatioList)
                        {
                            string errormsg = string.Empty;
                            switch (pDFFiledDataInspectorModel.FieldType)
                            {
                                case PDFFieldType.TextBox:
                                    PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = txtbxField.ToolTip;
                                    break;
                                case PDFFieldType.RadioButton:
                                    PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = radioButton.ToolTip;
                                    break;
                            }
                            if (skip == false)
                            {
                                if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                {
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                                    {
                                        string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                        if (OfficeType == "C")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("-6", "-5");
                                        }

                                        ErrorMessage = ErrorMessage.Replace("Please enter", officialTableNo + " Please enter");
                                    }
                                    if (T_type.Name == "EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS")
                                    {
                                        if (pDFFiledDataInspectorModel.FieldName.Contains("INSP_LEVEL"))
                                        {
                                            if (Value == "0")
                                            {
                                                objViewModel.ErrorOccured = true;

                                                ErrorList.Add(ErrorMessage + erroradd);
                                            }
                                        }

                                    }
                                    if (Value == null || Value == "" || Value == " ")
                                    {
                                        objViewModel.ErrorOccured = true;

                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                            {
                                string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                                {
                                    string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                    if (OfficeType == "C")
                                    {
                                        ErrorMessage = ErrorMessage.Replace("-6", "-5");
                                    }

                                    ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                }
                                if (Value != null && Value != "")
                                {
                                    if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                    if (isdateField == true)
                                    {
                                        if (!MatchDateFormat(Convert.ToString(Value)))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                        }
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                            {
                                int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                if (T_type.Name == "EFORM_BRIDGE_QC_OFFICIAL_DETAILS")
                                {
                                    string OfficeType = Convert.ToString(PropertyList.Where(m => m.Name == "OFFICIAL_TYPE").FirstOrDefault().GetValue(T));
                                    if (OfficeType == "C")
                                    {
                                        ErrorMessage = ErrorMessage.Replace("-6", "-5");
                                    }

                                    ErrorMessage = ErrorMessage.Replace("QUALITY CONTROL-", " QUALITY CONTROL-" + officialTableNo);
                                }
                                if (Value != null)
                                {
                                    if (Convert.ToString(Value).Length > Length)
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(item.Name);
                        ErrorLog.LogError(ex, "CommonFunctions.FetchBridgePIUPDFFilledDataToModel()");
                        ErrorList.Add("Page-1: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                    }
                }
                return ErrorList;
                #endregion              
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.FetchBridgePIUPDFFilledDataToModel()");
                // List<string> ErrorList = new List<string>();
                ErrorList.Add("Error Occured while fetching data from pdf...");
                return ErrorList;
            }
            finally
            {
                eformdbContext.Dispose();

            }
        }

        public List<string> FetchBridgeQMPDFFilledDataToModel(object T, PdfLoadedForm loadedForm)
        {
            bool skip = false;
            string erroradd = string.Empty;
            bool isdateField = false;
            List<string> ErrorList = new List<string>();
            PMGSYEntities eformdbContext = new PMGSYEntities();
            try
            {


                FieldTypeAttribute fieldTypeAttribute = new FieldTypeAttribute();
                RoadStatusDependableAttribute roadStatusDependableAttribute = new RoadStatusDependableAttribute();

                RegularExpressionAttribute regularExpressionAttribute = new RegularExpressionAttribute("");
                StringLengthAttribute StringLengthAttribute = new StringLengthAttribute(0);
                RequiredAttribute requiredAttribute = new RequiredAttribute();
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                var T_type = T.GetType();
                var AttributeType = fieldTypeAttribute.GetType();
                var RoadStatusAttributeType = roadStatusDependableAttribute.GetType();


                var RegexAttributeType = regularExpressionAttribute.GetType();
                var StringLengthAttributeType = StringLengthAttribute.GetType();
                var requiredAttributeType = requiredAttribute.GetType();
                List<PropertyInfo> PropertyList = T_type.GetProperties().ToList();
                pDFFiledDataInspectorModel.LoadedForm = loadedForm;
                EFORM_BRIDGE_QM_VIEWMODEL objViewModel = new EFORM_BRIDGE_QM_VIEWMODEL();



                foreach (PropertyInfo item in PropertyList)
                {
                    skip = false;
                    erroradd = "";
                    CustomAttributeData RoadStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == RoadStatusAttributeType).FirstOrDefault();
                    CustomAttributeData AttributeData = item.CustomAttributes.Where(m => m.AttributeType == AttributeType).FirstOrDefault();
                    pDFFiledDataInspectorModel.FieldType = (PDFFieldType)AttributeData.NamedArguments.FirstOrDefault().TypedValue.Value;
                    if (pDFFiledDataInspectorModel.FieldType == PDFFieldType.Skip)
                        continue;

                    pDFFiledDataInspectorModel.FieldName = item.Name;  //


                    #region vikky 9-15
                    if (T_type.Name == "EFORM_BRIDGE_QM_ARRANGEMENT_OBS_DETAIL")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_2")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_2"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "SRI" || itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }


                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_QUALITY_ATTENTION")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_3")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_3"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "SRI" || itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        if (pDFFiledDataInspectorModel.FieldName == "OTHER_REASON")
                        {
                            PdfLoadedCheckBoxField check_IS_OTHER = loadedForm.Fields["IS_OTHER"] as PdfLoadedCheckBoxField;
                            if (check_IS_OTHER.Checked)
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }


                    }
                    if (T_type.Name == "EFORM_BRIDGE_QM_QC_TEST_DETAILS")
                    {

                        if (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) == 1)
                        {
                            pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        }
                        else
                        {
                            pDFFiledDataInspectorModel.FieldName += "_" + (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) - 1) + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                            skip = true;
                            if (pDFFiledDataInspectorModel.FieldName.Equals("DPR_QUANTITY" + "_" + (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) - 1) + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T)))
                                || pDFFiledDataInspectorModel.FieldName.Equals("EXECUTED_QUANTITY" + "_" + (Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) - 1) + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T))))
                            {
                                continue;
                            }
                        }

                        int item_id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_BRIDGE_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "A" && s.ITEM_ID == item_id).Select(m => m.ITEM_DESC).FirstOrDefault() + " of row " + Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));


                    }


                    if (T_type.Name == "EFORM_BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        if (pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_2") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_3") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_4")
                            || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_6") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_7") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_8")
                            || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_10") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_11") || pDFFiledDataInspectorModel.FieldName.Equals("ROAD_LOC_12"))
                        {
                            continue;
                        }

                    }



                    if (T_type.Name == "EFORM_BRIDGE_QM_FOUNDATION")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_4")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_4"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }
                    if (T_type.Name == "EFORM_BRIDGE_QM_ONGOING_FOUNDATION")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS8" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS7" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS6" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS5" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS4" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                        {
                            PdfLoadedCheckBoxField check_cb_4_2 = loadedForm.Fields["cb_4_2"] as PdfLoadedCheckBoxField;
                            if (check_cb_4_2.Checked == false)
                            {
                                continue;
                            }
                            else
                            {
                                skip = true;
                                PdfLoadedTextBoxField FOUNDATION_PIERS9text = loadedForm.Fields["FOUNDATION_PIERS9"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS8text = loadedForm.Fields["FOUNDATION_PIERS8"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS7text = loadedForm.Fields["FOUNDATION_PIERS7"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS6text = loadedForm.Fields["FOUNDATION_PIERS6"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS5text = loadedForm.Fields["FOUNDATION_PIERS5"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS4text = loadedForm.Fields["FOUNDATION_PIERS4"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS3text = loadedForm.Fields["FOUNDATION_PIERS3"] as PdfLoadedTextBoxField;
                                PdfLoadedTextBoxField FOUNDATION_PIERS2text = loadedForm.Fields["FOUNDATION_PIERS2"] as PdfLoadedTextBoxField;

                                if (FOUNDATION_PIERS9text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS8" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS7" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS6" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS5" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS4" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }
                                }
                                if (FOUNDATION_PIERS8text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS7" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS6" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS5" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS4" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS7text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS6" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS5" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS4" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS6text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS5" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS4" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS5text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS4" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS4text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS3" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS3text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS2" || pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (FOUNDATION_PIERS2text.Text != "")
                                {
                                    if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                    {
                                        skip = false;
                                    }

                                }
                                if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_PIERS1")
                                {
                                    skip = false;
                                }

                            }




                        }


                        if (pDFFiledDataInspectorModel.FieldName == "FOUNDATION_RETURN1")
                        {
                            PdfLoadedCheckBoxField check_cb_4_3 = loadedForm.Fields["cb_4_3"] as PdfLoadedCheckBoxField;
                            if (check_cb_4_3.Checked == false)
                            {
                                continue;
                            }
                        }
                    }


                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }


                    #endregion


                    #region saurabh 15-22
                    if (T_type.Name == "EFORM_BRIDGE_QM_SUBSTRUCTURE")
                    {

                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_5")
                        {
                            PdfLoadedRadioButtonListField check_ITEM_GRADING_5 = loadedForm.Fields["ITEM_GRADING_5"] as PdfLoadedRadioButtonListField;
                            if (check_ITEM_GRADING_5.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }


                    }



                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }
                    if (T_type.Name == "EFORM_BRIDGE_QM_SUPERSTRUCTURE")
                    {
                        PdfLoadedRadioButtonListField StructureType = loadedForm.Fields["STRUCTURE_TYPE"] as PdfLoadedRadioButtonListField;
                        if (StructureType.SelectedValue == "RCC")
                        {
                            if (item.Name == "ITEM_GRADING_6_2" || item.Name == "ITEM_GRADING_6_3" || item.Name == "IMPROVEMENT_REMARK_6_2" || item.Name == "IMPROVEMENT_REMARK_6_3")
                            {
                                continue;
                            }


                            if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_6_1")
                            {
                                PdfLoadedRadioButtonListField check_ITEM_GRADING_6_1 = loadedForm.Fields["ITEM_GRADING_6_1"] as PdfLoadedRadioButtonListField;
                                if (check_ITEM_GRADING_6_1.SelectedValue == "U")
                                {
                                    skip = false;
                                }
                                else
                                {
                                    skip = true;
                                }
                            }





                        }
                        if (StructureType.SelectedValue == "STEEL")
                        {
                            if (item.Name == "ITEM_GRADING_6_1" || item.Name == "ITEM_GRADING_6_3" || item.Name == "IMPROVEMENT_REMARK_6_1" || item.Name == "IMPROVEMENT_REMARK_6_3")
                            {
                                continue;
                            }

                            if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_6_2")
                            {
                                PdfLoadedRadioButtonListField check_ITEM_GRADING_6_2 = loadedForm.Fields["ITEM_GRADING_6_2"] as PdfLoadedRadioButtonListField;
                                if (check_ITEM_GRADING_6_2.SelectedValue == "U")
                                {
                                    skip = false;
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }
                        if (StructureType.SelectedValue == "BAILEY")
                        {
                            if (item.Name == "ITEM_GRADING_6_2" || item.Name == "ITEM_GRADING_6_1" || item.Name == "IMPROVEMENT_REMARK_6_2" || item.Name == "IMPROVEMENT_REMARK_6_1")
                            {
                                continue;
                            }

                            if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_6_3")
                            {
                                PdfLoadedRadioButtonListField check_ITEM_GRADING_6_3 = loadedForm.Fields["ITEM_GRADING_6_3"] as PdfLoadedRadioButtonListField;
                                if (check_ITEM_GRADING_6_3.SelectedValue == "U")
                                {
                                    skip = false;
                                }
                                else
                                {
                                    skip = true;
                                }
                            }
                        }
                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    } // 

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    } // 

                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE")
                    {
                        string CorrosionCheck = string.Empty;
                        PdfLoadedRadioButtonListField PresenceOfCorrosion = null;
                        if (item.Name == "ATMOSPHERE_CORROSION_20" || item.Name == "CHEMICAL_CORROSION_20" || item.Name == "STRESS_CORROSION_20")
                        {
                            erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                            CorrosionCheck = "IS_PRESENCE_CORROSION_20" + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                            PresenceOfCorrosion = loadedForm.Fields[CorrosionCheck] as PdfLoadedRadioButtonListField;
                            if (PresenceOfCorrosion.SelectedValue == "No")
                            {
                                continue;
                            }
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));

                    }
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE")
                    {
                        string CorrosionCheck = string.Empty;
                        PdfLoadedRadioButtonListField PresenceOfCorrosion = null;

                        if (item.Name == "ATMOSPHERE_CORROSION_22" || item.Name == "CHEMICAL_CORROSION_22" || item.Name == "STRESS_CORROSION_22")
                        {
                            erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                            CorrosionCheck = "IS_PRESENCE_CORROSION_22" + "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                            PresenceOfCorrosion = loadedForm.Fields[CorrosionCheck] as PdfLoadedRadioButtonListField;
                            if (PresenceOfCorrosion.SelectedValue == "No")
                            {
                                continue;
                            }
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "RowID").FirstOrDefault().GetValue(T));
                    }



                    #endregion


                    #region vikky 23-28
                    #region EFORM_BRIDGE_QM_LOAD_TEST
                    if (T_type.Name == "EFORM_BRIDGE_QM_LOAD_TEST")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_7")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_7"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_BEARING
                    if (T_type.Name == "EFORM_BRIDGE_QM_BEARING")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_8")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_8"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }

                        }
                        if (pDFFiledDataInspectorModel.FieldName == "OTHER_BEARING_TYPE")
                        {
                            PdfLoadedCheckBoxField check_IS_OTHER_BEARING_TYPE = loadedForm.Fields["IS_OTHER_BEARING_TYPE"] as PdfLoadedCheckBoxField;
                            if (check_IS_OTHER_BEARING_TYPE.Checked)
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }

                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_BEARING_TYPE
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_BEARING_TYPE")
                    {
                        PdfLoadedCheckBoxField Check_IS_METALLIC_BEARING = loadedForm.Fields["IS_METALLIC_BEARING"] as PdfLoadedCheckBoxField;
                        if (Check_IS_METALLIC_BEARING.Checked == false)
                        {

                            if (pDFFiledDataInspectorModel.FieldName == "IS_RUSTED" || pDFFiledDataInspectorModel.FieldName == "IS_FUNCTIONING" || pDFFiledDataInspectorModel.FieldName == "IS_GREASE_REQUIRED" || pDFFiledDataInspectorModel.FieldName == "IS_CRACK_IN_SUPPORRT_MEMBER" || pDFFiledDataInspectorModel.FieldName == "IS_EFFECTIVE_ANCHOR_BOLT" || pDFFiledDataInspectorModel.FieldName == "OTHER_DEFECT_IN_METALLIC")
                                continue;
                        }

                        PdfLoadedCheckBoxField Check_IS_ELASTOMETRIC_BEARING = loadedForm.Fields["IS_ELASTOMETRIC_BEARING"] as PdfLoadedCheckBoxField;
                        if (Check_IS_ELASTOMETRIC_BEARING.Checked == false)
                        {

                            if (pDFFiledDataInspectorModel.FieldName == "IS_PAD_COND_BAD" || pDFFiledDataInspectorModel.FieldName == "IS_BEARING_CLEAN" || pDFFiledDataInspectorModel.FieldName == "OTHER_DEFECT_IN_ELASTOMERIC")
                                continue;
                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_EXPANSION
                    if (T_type.Name == "EFORM_BRIDGE_QM_EXPANSION")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_9")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_9"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                        if (pDFFiledDataInspectorModel.FieldName == "OTHER_OBSER_FOUND")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["OBSERVATION_FOUND"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "Yes")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_APPROACH")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_10")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_10"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }


                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_PROTECH_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_PROTECH_APPROACH")
                    {
                        skip = false;
                        PdfLoadedRadioButtonListField Check_IS_DPR_PROV_PROTE_WORKS = loadedForm.Fields["IS_DPR_PROV_PROTE_WORKS"] as PdfLoadedRadioButtonListField;

                        if (pDFFiledDataInspectorModel.FieldName == "IS_RETAINING-WALL" || pDFFiledDataInspectorModel.FieldName == "IS_BREAST_WALL" || pDFFiledDataInspectorModel.FieldName == "IS_PARAPET_WALL" || pDFFiledDataInspectorModel.FieldName == "IS_ANY_OTHER_PROT_WORK" || pDFFiledDataInspectorModel.FieldName == "IS_ANY_OTHER_PROT_WORK_A" || pDFFiledDataInspectorModel.FieldName == "IS_ANY_OTHER_PROT_WORK_B")
                        {
                            if (Check_IS_DPR_PROV_PROTE_WORKS.SelectedValue == "No")
                            {
                                continue;
                            }
                        }
                        PdfLoadedCheckBoxField Check_IS_ANY_OTHER_PROT_WORK = loadedForm.Fields["IS_ANY_OTHER_PROT_WORK"] as PdfLoadedCheckBoxField;
                        if (pDFFiledDataInspectorModel.FieldName == "IS_ANY_OTHER_PROT_WORK_A")
                        {
                            if (Check_IS_ANY_OTHER_PROT_WORK.Checked == false)
                            {
                                skip = true;
                            }

                        }

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_QOM_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_QOM_APPROACH")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #endregion


                    #region Bhushan page 29-35

                    #region EFORM_BRIDGE_QM_FURNITURE_MARKING
                    if (T_type.Name == "EFORM_BRIDGE_QM_FURNITURE_MARKINGS")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_11_1")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_11_1"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING
                    if (T_type.Name == "EFORM_BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "IMPROVEMENT_REMARK_11_2")
                        {
                            PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["ITEM_GRADING_11_2"] as PdfLoadedRadioButtonListField;
                            if (itemGrad.SelectedValue == "U" || itemGrad.SelectedValue == "SRI")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG
                    if (T_type.Name == "EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG")
                    {
                        PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["WORK_STATUS_30"] as PdfLoadedRadioButtonListField;
                        switch (itemGrad.SelectedValue)
                        {
                            case "C":
                                if (pDFFiledDataInspectorModel.FieldName == "P_IS_AS_PER_SCHEDULE" || pDFFiledDataInspectorModel.FieldName == "P_EXT_MONTHS" || pDFFiledDataInspectorModel.FieldName == "P_IS_AMOUNT_REFUNDED" || pDFFiledDataInspectorModel.FieldName == "P_AMOUNT" || pDFFiledDataInspectorModel.FieldName == "P_PANELTY_AMOUNT" || pDFFiledDataInspectorModel.FieldName == "P_COMMENT")
                                {
                                    skip = true;
                                }
                                else
                                {
                                    skip = false;
                                }
                                PdfLoadedRadioButtonListField check_C_IS_COMPLETED_WITH_DELAY = loadedForm.Fields["C_IS_COMPLETED_WITH_DELAY"] as PdfLoadedRadioButtonListField;

                                if (pDFFiledDataInspectorModel.FieldName == "C_PERIOD_OF_DELAY" || pDFFiledDataInspectorModel.FieldName == "C_AMOUNT_STATUS" || pDFFiledDataInspectorModel.FieldName == "C_AMOUNT" || pDFFiledDataInspectorModel.FieldName == "C_COMMENT")
                                {
                                    if (check_C_IS_COMPLETED_WITH_DELAY.SelectedValue == "Yes")
                                    {
                                        skip = false;
                                    }
                                    else
                                    {
                                        skip = true;
                                    }
                                }



                                break;
                            case "P":
                                if (pDFFiledDataInspectorModel.FieldName == "C_IS_COMPLETED_WITH_DELAY" || pDFFiledDataInspectorModel.FieldName == "C_PERIOD_OF_DELAY" || pDFFiledDataInspectorModel.FieldName == "C_AMOUNT_STATUS" || pDFFiledDataInspectorModel.FieldName == "C_AMOUNT" || pDFFiledDataInspectorModel.FieldName == "C_COMMENT")
                                {
                                    skip = true;
                                }
                                else
                                {
                                    skip = false;
                                }

                                PdfLoadedRadioButtonListField check_P_IS_AS_PER_SCHEDULE = loadedForm.Fields["P_IS_AS_PER_SCHEDULE"] as PdfLoadedRadioButtonListField;

                                if (pDFFiledDataInspectorModel.FieldName == "P_EXT_MONTHS" || pDFFiledDataInspectorModel.FieldName == "P_IS_AMOUNT_REFUNDED" || pDFFiledDataInspectorModel.FieldName == "P_AMOUNT" || pDFFiledDataInspectorModel.FieldName == "P_PANELTY_AMOUNT" || pDFFiledDataInspectorModel.FieldName == "P_COMMENT")
                                {
                                    if (check_P_IS_AS_PER_SCHEDULE.SelectedValue == "E")
                                    {
                                        skip = false;
                                    }
                                    else
                                    {
                                        skip = true;
                                    }
                                }

                                break;
                        }
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST
                    if (T_type.Name == "EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST")
                    {
                        PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["WORK_STATUS_31"] as PdfLoadedRadioButtonListField;
                        if (pDFFiledDataInspectorModel.FieldName == "SANCTION_COST" || pDFFiledDataInspectorModel.FieldName == "COMPLETION_COST" || pDFFiledDataInspectorModel.FieldName == "REASON_EXTRA_COST" || pDFFiledDataInspectorModel.FieldName == "ACTION_BY_PIU")
                        {
                            if (itemGrad.SelectedValue == "No")
                            {
                                skip = false;
                            }
                            else
                            {
                                skip = true;
                            }
                        }

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_ACTION_TAKEN_PIU
                    if (T_type.Name == "EFORM_BRIDGE_QM_ACTION_TAKEN_PIU")
                    {
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_DIFFERENCE_IN_OBSERVATION
                    if (T_type.Name == "EFORM_BRIDGE_QM_DIFFERENCE_IN_OBSERVATION")
                    {
                        PdfLoadedRadioButtonListField check_IS_DIFFERENCE_FOUND = loadedForm.Fields["IS_DIFFERENCE_FOUND"] as PdfLoadedRadioButtonListField;
                        if (pDFFiledDataInspectorModel.FieldName == "COMMENT_ON_DIFFERENCE")
                        {
                            skip = true;
                            if (check_IS_DIFFERENCE_FOUND.SelectedValue == "Yes")
                            {
                                skip = false;
                            }

                        }

                    }
                    #endregion

                    #endregion
                    #region dateField or not
                    isdateField = false;
                    if (pDFFiledDataInspectorModel.FieldName == "INSPECTION_DATE"
                     || pDFFiledDataInspectorModel.FieldName == "UPLOAD_DATE_34"
                     || pDFFiledDataInspectorModel.FieldName == "UPLOAD_DATE_35"

             )
                    {
                        isdateField = true;
                    }
                    #endregion

                    try
                    {
                        GetFilledDataFromPDFField(pDFFiledDataInspectorModel);
                    }
                    catch (Exception ex)
                    {

                        ErrorLog.LogError(ex, "CommonFunctions.FetchBridgeQMPDFFilledDataToModel()");
                        ErrorList.Add("Page-9: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                        objViewModel.ErrorOccured = true;
                        // return ErrorList;
                    }
                    //item.SetValue(T, pDFFiledDataInspectorModel.Value);

                    string Value = string.Empty;
                    if (pDFFiledDataInspectorModel.Value != "")
                    {
                        Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    }
                    //  string Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(item.PropertyType);
                        List<CustomAttributeData> CustomValidatioList = item.CustomAttributes.Where(m => m.AttributeType != AttributeType).ToList();
                        try
                        {
                            if (Value != "" && Value != null)
                            {
                                T.GetType().InvokeMember(item.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                Type.DefaultBinder, T, new object[] { converter.ConvertFromString(Value) });
                            }
                        }
                        catch (Exception e)
                        {
                            #region  catch code

                            foreach (CustomAttributeData Customitem in CustomValidatioList)
                            {
                                string errormsg = string.Empty;
                                switch (pDFFiledDataInspectorModel.FieldType)
                                {
                                    case PDFFieldType.TextBox:
                                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = txtbxField.ToolTip;
                                        break;
                                    case PDFFieldType.RadioButton:
                                        PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = radioButton.ToolTip;
                                        break;
                                }
                                if (skip == false)
                                {
                                    if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                    {
                                        string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);


                                        if (Value == null || Value == "" || Value == " ")
                                        {
                                            objViewModel.ErrorOccured = true;

                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                                {
                                    string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                    if (Value != null && Value != "")
                                    {
                                        if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                        if (isdateField == true)
                                        {
                                            if (!MatchDateFormat(Convert.ToString(Value)))
                                            {
                                                objViewModel.ErrorOccured = true;
                                                ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                            }
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                                {
                                    int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                    if (Value != null)
                                    {
                                        if (Convert.ToString(Value).Length > Length)
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                            }
                            // return ErrorList;
                            #endregion
                        }
                        foreach (CustomAttributeData Customitem in CustomValidatioList)
                        {
                            string errormsg = string.Empty;
                            switch (pDFFiledDataInspectorModel.FieldType)
                            {
                                case PDFFieldType.TextBox:
                                    PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = txtbxField.ToolTip;
                                    break;
                                case PDFFieldType.RadioButton:
                                    PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = radioButton.ToolTip;
                                    break;
                            }
                            if (skip == false)
                            {
                                if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                {
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);


                                    if (Value == null || Value == "" || Value == " ")
                                    {
                                        objViewModel.ErrorOccured = true;

                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                            {
                                string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                if (Value != null && Value != "")
                                {
                                    if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                    if (isdateField == true)
                                    {
                                        if (!MatchDateFormat(Convert.ToString(Value)))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                        }
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                            {
                                int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                if (Value != null)
                                {
                                    if (Convert.ToString(Value).Length > Length)
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(item.Name);
                        ErrorLog.LogError(ex, "CommonFunctions.FetchBridgeQMPDFFilledDataToModel()");
                        ErrorList.Add("Page-9: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                    }
                }
                return ErrorList;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.FetchBridgeQMPDFFilledDataToModel()");
                // List<string> ErrorList = new List<string>();
                ErrorList.Add("Error Occured while fetching data from pdf...");
                return ErrorList;
            }
            finally
            {
                eformdbContext.Dispose();

            }
        }



        #endregion


        public List<string> FetchTetsReportPDFFilledDataToModel(object T, PdfLoadedForm loadedForm)
        {
            bool skip = false;
            string erroradd = string.Empty;
            string errorRegexAdd = string.Empty;
            string posNoRegexPattern = @"^-?(?:\d{0,21}\.\d{1,3})$|^-?\d{0,21}$";
            string isPosNoregexpatternApplicable = "N";
            bool isCH1Mand = false;
            bool isCH2Mand = false;
            bool isCH3Mand = false;
            Decimal chainageValue = 0;
            bool isDropdownType = false;
            bool isdateField = false;
            List<string> ErrorList = new List<string>();
            PMGSYEntities eformdbContext = new PMGSYEntities();
            try
            {

                PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                int eformIdFrmPdf = Convert.ToInt32(eformId.Text);
                EFORM_MASTER eformModel = eformdbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIdFrmPdf && s.IS_VALID == "Y").FirstOrDefault();
                //string endChanage = eformdbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.IMS_PR_ROAD_CODE == eformModel.IMS_PR_ROAD_CODE && s.ADMIN_SCHEDULE_CODE == eformModel.ADMIN_SCHEDULE_CODE).Select(m => m.QM_INSPECTED_END_CHAINAGE).FirstOrDefault().ToString();
                //string startChanage = eformdbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.IMS_PR_ROAD_CODE == eformModel.IMS_PR_ROAD_CODE && s.ADMIN_SCHEDULE_CODE == eformModel.ADMIN_SCHEDULE_CODE).Select(m => m.QM_INSPECTED_START_CHAINAGE).FirstOrDefault().ToString();

                //if (endChanage != null && startChanage != null)
                //{
                //    chainageValue = Convert.ToDecimal(endChanage) - Convert.ToDecimal(startChanage);
                //}
                //if (chainageValue <= 1)
                //{
                //    isCH1Mand = true;
                //}
                //if (chainageValue > 1 && chainageValue <= 2)
                //{
                //    isCH1Mand = true;
                //    isCH2Mand = true;
                //}
                //if (chainageValue > 2)
                //{
                //    isCH1Mand = true;
                //    isCH2Mand = true;
                //    isCH3Mand = true;
                //}
                isCH1Mand = true;

                FieldTypeAttribute fieldTypeAttribute = new FieldTypeAttribute();
                RoadStatusDependableAttribute roadStatusDependableAttribute = new RoadStatusDependableAttribute();

                RegularExpressionAttribute regularExpressionAttribute = new RegularExpressionAttribute("");
                StringLengthAttribute StringLengthAttribute = new StringLengthAttribute(0);
                RequiredAttribute requiredAttribute = new RequiredAttribute();
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                var T_type = T.GetType();
                var AttributeType = fieldTypeAttribute.GetType();
                var RoadStatusAttributeType = roadStatusDependableAttribute.GetType();


                var RegexAttributeType = regularExpressionAttribute.GetType();
                var StringLengthAttributeType = StringLengthAttribute.GetType();
                var requiredAttributeType = requiredAttribute.GetType();
                List<PropertyInfo> PropertyList = T_type.GetProperties().ToList();
                pDFFiledDataInspectorModel.LoadedForm = loadedForm;
                EFORM_TR_VIEWMODEL objViewModel = new EFORM_TR_VIEWMODEL();



                foreach (PropertyInfo item in PropertyList)
                {
                    isDropdownType = false;
                    skip = false;
                    isdateField = false;
                    erroradd = "";
                    isPosNoregexpatternApplicable = "N";
                    CustomAttributeData RoadStatusAttributeData = item.CustomAttributes.Where(m => m.AttributeType == RoadStatusAttributeType).FirstOrDefault();
                    CustomAttributeData AttributeData = item.CustomAttributes.Where(m => m.AttributeType == AttributeType).FirstOrDefault();
                    pDFFiledDataInspectorModel.FieldType = (PDFFieldType)AttributeData.NamedArguments.FirstOrDefault().TypedValue.Value;
                    if (pDFFiledDataInspectorModel.FieldType == PDFFieldType.Skip)
                        continue;

                    pDFFiledDataInspectorModel.FieldName = item.Name;  //

                    #region Saurabh Table Read Condition 

                    #region Page : 2,3,4 Sub-Base Detail Table by Saurabh

                    // SubBase Page - 2 , Gradation Analysis of Aggregates - Granular Subbase
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 270)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_270")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }

                        if (Item_Id == 270)
                        {
                            skip = false;
                        }




                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 276)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_276")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }

                        if (Item_Id == 276)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 282)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_282")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 282)
                        {
                            skip = false;
                        }

                    }

                    // SubBase Page - 3 , Gravel SubBase 
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 291)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_291")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 291)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 300)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_300")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 300)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 309)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_309")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 309)
                        {
                            skip = false;
                        }

                    }

                    // SubBase Page - 4 , Cement Stabilised Subbase
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 318)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_318")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 318)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 327)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_327")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 327)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 336)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_336")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 336)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 6 Base Course 1st Layer 


                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_1")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_6_1")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_6_1_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_6_1_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_2")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_6_2")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_6_2_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }


                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_6_2_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_6_3")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_6_3_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }


                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_6_3_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 8 Base Course 2nd Layer

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_1")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_8_1")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_8_1_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }


                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_8_1_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_2")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_8_2")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_8_2_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_8_2_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_3")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_8_3")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_8_3_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_8_3_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 10 Base Course 3rd Layer

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_1")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_10_1")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_10_1_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_10_1_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_2")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_10_2")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_10_2_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_10_2_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_3")
                    {
                        int SubItemId = Convert.ToInt16(PropertyList.Where(m => m.Name == "SUBITEM_ID").FirstOrDefault().GetValue(T));
                        int RowId = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_10_3")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }

                        pDFFiledDataInspectorModel.FieldName += "_" + RowId;
                        if (RowId == 9)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_10_3_9")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_10_3_" + RowId] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (RowId == 9)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 12 Base Course 3rd Layer

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 516)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_516")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }

                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 516)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 525)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_525")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 525)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 534)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_534")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 534)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 13 Base Course 3rd Layer
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 543)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_543")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 543)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 552)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_552")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 552)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 561)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_561")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 561)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 14 BITUMINOUS BASE COURSE
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 569)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_569")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 569)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 577)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_577")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 577)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 585)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_585")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 585)
                        {
                            skip = false;
                        }

                    }
                    #endregion

                    #region Page : 17-18 BITUMINOUS SURFACE COURSE



                    if (T_type.Name == "EFORM_TR_TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "GRADATION_TYPE_ITEM_ID_17_1")
                        {
                            isDropdownType = true;
                        }
                    }


                    if (T_type.Name == "EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1")
                    {

                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        string fieldName = "DETAIL_ITEM_ID_17_1_" + Item_Id;
                        PdfLoadedTextBoxField detailsIdText = loadedForm.Fields[fieldName] as PdfLoadedTextBoxField;
                        int detailId = 0;
                        detailId = Convert.ToInt32(detailsIdText.Text);
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_17_1" && pDFFiledDataInspectorModel.FieldName != "PERMISSIBLE_RANGE_17_1" && pDFFiledDataInspectorModel.FieldName != "SIEVE_DESIGNATION_17_1")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 16)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_17_1_16" || pDFFiledDataInspectorModel.FieldName == "SIEVE_DESIGNATION_17_1_16" || pDFFiledDataInspectorModel.FieldName == "DETAIL_ITEM_ID_17_1_16")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";

                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";


                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_17_1_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 16)
                        {
                            skip = false;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "GRADATION_TYPE_ITEM_ID_17_2")
                        {
                            isDropdownType = true;
                        }
                    }
                    if (T_type.Name == "EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        string fieldName = "DETAIL_ITEM_ID_17_2_" + Item_Id;
                        PdfLoadedTextBoxField detailsIdText = loadedForm.Fields[fieldName] as PdfLoadedTextBoxField;
                        int detailId = Convert.ToInt32(detailsIdText.Text);
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();

                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_17_2" && pDFFiledDataInspectorModel.FieldName != "PERMISSIBLE_RANGE_17_2" && pDFFiledDataInspectorModel.FieldName != "SIEVE_DESIGNATION_17_2")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 16)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_17_2_16" || pDFFiledDataInspectorModel.FieldName == "SIEVE_DESIGNATION_17_2_16" || pDFFiledDataInspectorModel.FieldName == "DETAIL_ITEM_ID_17_2_16")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";


                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_17_2_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 16)
                        {
                            skip = false;
                        }

                    }
                    if (T_type.Name == "EFORM_TR_TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "GRADATION_TYPE_ITEM_ID_18_1")
                        {
                            isDropdownType = true;
                        }
                    }
                    if (T_type.Name == "EFORM_TR_TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        string fieldName = "DETAIL_ITEM_ID_17_2_" + Item_Id;
                        PdfLoadedTextBoxField detailsIdText = loadedForm.Fields[fieldName] as PdfLoadedTextBoxField;
                        int detailId = Convert.ToInt32(detailsIdText.Text);
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT_18_1" && pDFFiledDataInspectorModel.FieldName != "PERMISSIBLE_RANGE_18_1" && pDFFiledDataInspectorModel.FieldName != "SIEVE_DESIGNATION_18_1")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 16)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_18_1_16" || pDFFiledDataInspectorModel.FieldName == "SIEVE_DESIGNATION_18_1_16" || pDFFiledDataInspectorModel.FieldName == "DETAIL_ITEM_ID_18_1_16")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";

                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_18_1_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 16)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 20 SHOULDER 

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 591)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_591")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 591)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 597)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_597")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 597)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 603)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_603")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 603)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #region Page : 21 SHOULDER 

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_1")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 612)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_612")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 612)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_2")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 621)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_621")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 621)
                        {
                            skip = false;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_3")
                    {
                        int Item_Id = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == Item_Id).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (pDFFiledDataInspectorModel.FieldName != "SAMPLE_WEIGHT")
                        {
                            erroradd = erroradd + ". Please click on Calculate button.";
                        }
                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Item_Id == 630)
                        {
                            if (pDFFiledDataInspectorModel.FieldName == "SAMPLE_WEIGHT_630")
                            {
                                skip = false;
                                isPosNoregexpatternApplicable = "Y";
                                int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                                erroradd = " ,I. S. Sieve Designation- " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                            }
                            else
                            {
                                continue;
                            }
                        }
                        PdfLoadedTextBoxField sampleWt = loadedForm.Fields["SAMPLE_WEIGHT_" + Item_Id] as PdfLoadedTextBoxField;
                        string itemSampleWt = sampleWt.Text;
                        if (itemSampleWt != "")
                        {
                            skip = false;
                        }
                        else
                        {
                            skip = true;
                        }
                        if (Item_Id == 630)
                        {
                            skip = false;
                        }

                    }

                    #endregion

                    #endregion


                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE1_SRM_1")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_1_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_1_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "14" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "8")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "22")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "16" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "17")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_1_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_1_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_1_1_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_1_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "15")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_1_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_1_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_1_1_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 14).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_1_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "6"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "7"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "12"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "13"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "17"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "18"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "19"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "20"
                          || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "21"
                           || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "22"
                            )
                        {

                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";


                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE1_CCM_2")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_1_2" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_1_2" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "29")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "36")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "31" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "32")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_1_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_1_2"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_1_2_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_1_2_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "30")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_1_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_1_2"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_1_2_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 29).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_1_2_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "27"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "28"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "32"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "33"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "34"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "35"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "36"
                         )
                        {

                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE5_SRM_12")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_5_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_5_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "44" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "50")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "58")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "52" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "53")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_5_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_5_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_5_1_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_5_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "51")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_5_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_5_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_5_1_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 50).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_5_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "42"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "43"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "48"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "49"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "53"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "54"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "55"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "56"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "57"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "58"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_16")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_7_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_7_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "66" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "72")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "80")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "74" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "75")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_7_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_7_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_7_1_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_7_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "73")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_7_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_7_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_7_1_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 72).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_7_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "64"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "65"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "70"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "71"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "75"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "76"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "77"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "78"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "79"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "80"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }



                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_17")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_7_2" && isCH2Mand == false)
                          || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_7_2" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        PdfLoadedRadioButtonListField Base_Course_1_select = loadedForm.Fields["Base_Course_1"] as PdfLoadedRadioButtonListField;

                        if (pDFFiledDataInspectorModel.FieldName == "Type_7")
                        {
                            skip = true;
                            if (Base_Course_1_select.SelectedValue == "8")
                            {
                                skip = false;
                            }

                        }




                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_17")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "85")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "84"
                            || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "85"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }
                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE9_SRM_21")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_9_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_9_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "93" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "99")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "107")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "101" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "102")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_9_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_9_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_9_1_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_9_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "100")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_9_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_9_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_9_1_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 72).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_9_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "91"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "92"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "97"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "98"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "102"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "103"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "104"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "105"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "106"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "107"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE9_SRM_22")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_9_2" && isCH2Mand == false)
                           || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_9_2" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        PdfLoadedRadioButtonListField Base_Course_2_select = loadedForm.Fields["Base_Course_2"] as PdfLoadedRadioButtonListField;

                        if (pDFFiledDataInspectorModel.FieldName == "Type_9")
                        {
                            skip = true;
                            if (Base_Course_2_select.SelectedValue == "14")
                            {
                                skip = false;
                            }

                        }




                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_22")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "112")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "111"
                            || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "112"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            skip = true;
                        }


                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE11_SRM_26")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_11_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_11_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }



                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "120" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "126")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "134")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "128" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "129")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_11_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_11_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "127")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_11_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_11_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 72).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "118"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "119"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "124"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "125"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "129"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "130"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "131"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "132"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "133"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "134"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE11_SRM_27")
                    {


                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_11_2" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_11_2" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        PdfLoadedRadioButtonListField Base_Course_3_select = loadedForm.Fields["Base_Course_3"] as PdfLoadedRadioButtonListField;

                        if (pDFFiledDataInspectorModel.FieldName == "Type_11")
                        {
                            skip = true;
                            if (Base_Course_3_select.SelectedValue == "20")
                            {
                                skip = false;
                            }

                        }




                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_27")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "139")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "138"
                            || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "139"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            skip = true;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE15_SRM_37")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_15_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_15_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "147" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "153")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "161")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        //if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "155" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "156")
                        //{
                        //    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_15_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_15_1"] as PdfLoadedRadioButtonListField;
                        //    if (MOISTURE_CONTENT_METHOD_15_1_Value.SelectedValue == "RMM")
                        //    {
                        //        continue;
                        //    }
                        //    if (MOISTURE_CONTENT_METHOD_15_1_Value.SelectedValue == null)
                        //    {
                        //        skip = true;
                        //    }
                        //}


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        //if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "154")
                        //{
                        //    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_11_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_15_1"] as PdfLoadedRadioButtonListField;
                        //    if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == "RMM")
                        //    {
                        //        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 72).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        //    }
                        //    if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == null)
                        //    {
                        //        skip = true;
                        //    }
                        //}

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "145"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "146"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "151"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "152"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "156"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "157"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "158"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "159"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "160"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "161"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE16_CCM_38")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_15_2" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_15_2" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE16_CCM_38")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "168")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "175")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        //if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "170" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "171")
                        //{
                        //    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_15_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_15_2"] as PdfLoadedRadioButtonListField;
                        //    if (MOISTURE_CONTENT_METHOD_15_2_Value.SelectedValue == "RMM")
                        //    {
                        //        continue;
                        //    }
                        //    if (MOISTURE_CONTENT_METHOD_15_2_Value.SelectedValue == null)
                        //    {
                        //        skip = true;
                        //    }
                        //}
                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        //if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "169")
                        //{
                        //    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_15_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_15_2"] as PdfLoadedRadioButtonListField;
                        //    if (MOISTURE_CONTENT_METHOD_15_2_Value.SelectedValue == "RMM")
                        //    {
                        //        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 29).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        //    }
                        //    if (MOISTURE_CONTENT_METHOD_15_2_Value.SelectedValue == null)
                        //    {
                        //        skip = true;
                        //    }
                        //}

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "166"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "167"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "171"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "172"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "173"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "174"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "175"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }






                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE16_SRM_39")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "185")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "182"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "183"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "184"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "185"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";
                        }


                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE18_SRM_43")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "195")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "192"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "193"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "194"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "195"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }

                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE19_SRM_44")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_19_1" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_19_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "203" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "209")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "217")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "211" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "212")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_19_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_19_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_19_1_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_19_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }


                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "210")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_19_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_19_1"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_19_1_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 209).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_19_1_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "201"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "202"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "207"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "208"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "212"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "213"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "214"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "215"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "216"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "217"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }
                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE19_CCM_45")
                    {

                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH2_19_2" && isCH2Mand == false)
                            || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString()) == "MDD_CH3_19_2" && isCH3Mand == false))
                        {
                            skip = true;
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "224")
                        {
                            continue;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "231")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "226" || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "227")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_19_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_19_2"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_19_2_Value.SelectedValue == "RMM")
                            {
                                continue;
                            }
                            if (MOISTURE_CONTENT_METHOD_19_2_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "225")
                        {
                            PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_19_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_19_2"] as PdfLoadedRadioButtonListField;
                            if (MOISTURE_CONTENT_METHOD_19_2_Value.SelectedValue == "RMM")
                            {
                                erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == 224).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            }
                            if (MOISTURE_CONTENT_METHOD_19_2_Value.SelectedValue == null)
                            {
                                skip = true;
                            }
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "222"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "223"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "227"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "228"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "229"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "230"
                         || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "231"
                           )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";

                        }
                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_52")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "Layer_22_1")
                        {
                            isDropdownType = true;
                        }
                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE22_SRM_52")
                    {


                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "252")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }



                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "232"
                            // || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "233"
                            // || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "234"
                            )
                        {
                            skip = false;
                        }
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "233"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "234"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "244"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "245"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "246"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "247"
                        || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "248"
                        )
                        {
                            skip = true;
                        }

                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (erroradd.Length < 3)
                        {
                            erroradd = "Reading for No. of Blows:" + erroradd;
                        }
                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false))
                        {
                            skip = true;
                        }






                        #region Continuous Logic
                        //string temp = Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0])+"_";
                        //PdfLoadedTextBoxField blows_text_3 = loadedForm.Fields[temp + "235"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_4 = loadedForm.Fields[temp + "236"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_5 = loadedForm.Fields[temp + "237"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_6 = loadedForm.Fields[temp + "238"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_7 = loadedForm.Fields[temp + "239"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_8 = loadedForm.Fields[temp + "240"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_9 = loadedForm.Fields[temp + "241"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_10 = loadedForm.Fields[temp + "242"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_11 = loadedForm.Fields[temp + "243"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_12 = loadedForm.Fields[temp + "244"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_13 = loadedForm.Fields[temp + "245"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_14 = loadedForm.Fields[temp + "246"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_15 = loadedForm.Fields[temp + "247"] as PdfLoadedTextBoxField;
                        //PdfLoadedTextBoxField blows_text_16 = loadedForm.Fields[temp + "248"] as PdfLoadedTextBoxField;
                        //if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "244"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "245"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "246"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "247"
                        //|| Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "248"
                        //)
                        //{
                        //    skip = true;
                        //    if (blows_text_16.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "244"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "245"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "246"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "247"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_15.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "244"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "245"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "246"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_14.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "244"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "245"

                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_13.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "244"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_12.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "243"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_11.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "242"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_10.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "241"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_9.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "240"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_8.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "239"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_7.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "238"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_6.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "237"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_5.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "236"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //    if (blows_text_4.Text != "")
                        //    {
                        //        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "235"
                        //      )
                        //        {
                        //            skip = false;
                        //        }
                        //    }
                        //}

                        #endregion


                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "249"
                            || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "250"
                            || Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "251"

                            )
                        {
                            isPosNoregexpatternApplicable = "Y";
                            int detailId = Convert.ToInt16(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == detailId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                            if (erroradd.Length < 3)
                            {
                                erroradd = "Reading for No. of Blows:" + erroradd;
                            }
                            erroradd += " is required.";
                        }

                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_53")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "Layer_22_2")
                        {
                            isDropdownType = true;
                        }
                    }


                    if (T_type.Name == "EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53")
                    {

                        pDFFiledDataInspectorModel.FieldName += Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));


                        erroradd = Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T));
                        if ((Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) == "1" && isCH1Mand == false) || (Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) == "2" && isCH2Mand == false) || (Convert.ToString(PropertyList.Where(m => m.Name == "ROW_ID").FirstOrDefault().GetValue(T)) == "3" && isCH3Mand == false))
                        {
                            skip = true;
                        }


                    }


                    if (T_type.Name == "EFORM_TR_TYPEA_SUMMARY_PAGE23_SRM_54")
                    {
                        if (pDFFiledDataInspectorModel.FieldName == "CC_PAVEMENT_TYPE_CH1_23_1" || pDFFiledDataInspectorModel.FieldName == "CC_PAVEMENT_TYPE_CH2_23_1" || pDFFiledDataInspectorModel.FieldName == "CC_PAVEMENT_TYPE_CH3_23_1")
                        {
                            isDropdownType = true;
                        }
                        if ((pDFFiledDataInspectorModel.FieldName == "CC_PAVEMENT_TYPE_CH1_23_1" && isCH1Mand == false)
                            || (pDFFiledDataInspectorModel.FieldName == "CC_PAVEMENT_TYPE_CH2_23_1" && isCH2Mand == false)
                            || (pDFFiledDataInspectorModel.FieldName == "CC_PAVEMENT_TYPE_CH3_23_1" && isCH3Mand == false))
                        {
                            skip = true;
                        }
                    }

                    if (T_type.Name == "EFORM_TR_TYPEA_DETAIL_PAGE23_SRM_54")
                    {

                        pDFFiledDataInspectorModel.FieldName += "_" + Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));

                        if (Convert.ToString(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T)) == "264")
                        {
                            pDFFiledDataInspectorModel.FieldType = PDFFieldType.RadioButton;
                        }
                        int itemId = Convert.ToInt32(PropertyList.Where(m => m.Name == "DETAIL_ITEM_ID").FirstOrDefault().GetValue(T));
                        erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        if (itemId == 262)
                        {
                            skip = false;
                            isPosNoregexpatternApplicable = "Y";
                            erroradd = eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault() + " is required.";
                        }

                        if (itemId == 646 || itemId == 647 || itemId == 648 || itemId == 649 || itemId == 650 ||
                            itemId == 651 || itemId == 652 || itemId == 653 || itemId == 654
                            )
                        {

                            erroradd = " reading for SN. " + eformdbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(s => s.DETAIL_ITEM_ID == itemId).Select(m => m.DETAIL_ITEM_DESC).FirstOrDefault();
                        }


                        if ((Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH1" && isCH1Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH2" && isCH2Mand == false) || (Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH3" && isCH3Mand == false) || Convert.ToString(pDFFiledDataInspectorModel.FieldName.ToString().Split('_')[0]) == "CH4")
                        {
                            skip = true;
                        }


                    }

                    try
                    {
                        GetFilledDataFromPDFField(pDFFiledDataInspectorModel);
                    }
                    catch (Exception ex)
                    {

                        ErrorLog.LogError(ex, "CommonFunctions.FetchTetsReportPDFFilledDataToModel()");
                        ErrorList.Add("Page-1: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                        objViewModel.ErrorOccured = true;
                        // return ErrorList;
                    }
                    //item.SetValue(T, pDFFiledDataInspectorModel.Value);
                    #region dateField or not
                    isdateField = false;
                    if (pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_1_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_1_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_2_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_2_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_2_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_3_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_3_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_3_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_4_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_4_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_4_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_5_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_6_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_6_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_6_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_7_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_7_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_8_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_8_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_8_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_9_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_9_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_10_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_10_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_10_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_11_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_11_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_12_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_12_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_12_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_13_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_13_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_13_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_14_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_14_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_14_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_15_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_15_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_16_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_17_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_17_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_18_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_18_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_19_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_19_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_20_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_20_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_20_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_21_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_21_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_21_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_22_1"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_22_2"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_22_3"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_22_21"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_22_22"
             || pDFFiledDataInspectorModel.FieldName == "TESTING_DATE_22_23"

             )
                    {
                        isdateField = true;
                    }
                    #endregion
                    string Value = string.Empty;
                    if (pDFFiledDataInspectorModel.Value != "")
                    {
                        Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    }
                    //  string Value = string.IsNullOrEmpty(Convert.ToString(pDFFiledDataInspectorModel.Value)) ? null : Convert.ToString(pDFFiledDataInspectorModel.Value);
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(item.PropertyType);
                        List<CustomAttributeData> CustomValidatioList = item.CustomAttributes.Where(m => m.AttributeType != AttributeType).ToList();
                        try
                        {
                            if (Value != "" && Value != null)
                            {
                                T.GetType().InvokeMember(item.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                Type.DefaultBinder, T, new object[] { converter.ConvertFromString(Value) });
                            }
                        }
                        catch (Exception e)
                        {
                            #region  catch code

                            foreach (CustomAttributeData Customitem in CustomValidatioList)
                            {
                                string errormsg = string.Empty;
                                switch (pDFFiledDataInspectorModel.FieldType)
                                {
                                    case PDFFieldType.TextBox:
                                        PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = txtbxField.ToolTip;
                                        break;
                                    case PDFFieldType.RadioButton:
                                        PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                        errormsg = radioButton.ToolTip;
                                        break;
                                }
                                if (skip == false)
                                {
                                    if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                    {
                                        string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                        if (isPosNoregexpatternApplicable == "Y")
                                        {
                                            ErrorMessage = ErrorMessage.Replace("Please enter", "");
                                        }

                                        if (Value == null || Value == "" || Value == " " || (isDropdownType == true && Value == "1"))
                                        {
                                            objViewModel.ErrorOccured = true;

                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                                {
                                    string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (isPosNoregexpatternApplicable == "Y")
                                    {
                                        Pattern = posNoRegexPattern;
                                        ErrorMessage = ErrorMessage.Replace("Negative number is not allowed,", "").Replace("seven", "twenty-one");

                                    }

                                    if (Value != null && Value != "")
                                    {
                                        if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                        if (isdateField == true)
                                        {
                                            if (!MatchDateFormat(Convert.ToString(Value)))
                                            {
                                                objViewModel.ErrorOccured = true;
                                                ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                            }
                                        }
                                    }
                                }
                                if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                                {
                                    int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                    if (Value != null)
                                    {
                                        if (Convert.ToString(Value).Length > Length)
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd);
                                        }
                                    }
                                }
                            }
                            //  return ErrorList;
                            #endregion
                        }
                        foreach (CustomAttributeData Customitem in CustomValidatioList)
                        {
                            string errormsg = string.Empty;
                            switch (pDFFiledDataInspectorModel.FieldType)
                            {
                                case PDFFieldType.TextBox:
                                    PdfLoadedTextBoxField txtbxField = (PdfLoadedTextBoxField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = txtbxField.ToolTip;
                                    break;
                                case PDFFieldType.RadioButton:
                                    PdfLoadedRadioButtonListField radioButton = (PdfLoadedRadioButtonListField)pDFFiledDataInspectorModel.LoadedForm.Fields[pDFFiledDataInspectorModel.FieldName];
                                    errormsg = radioButton.ToolTip;
                                    break;
                            }
                            if (skip == false)
                            {
                                if (Customitem.AttributeType.Name == requiredAttributeType.Name)
                                {
                                    string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                    if (isPosNoregexpatternApplicable == "Y")
                                    {
                                        ErrorMessage = ErrorMessage.Replace("Please enter", "");
                                    }

                                    if (Value == null || Value == "" || Value == " " || (isDropdownType == true && Value == "1"))
                                    {
                                        objViewModel.ErrorOccured = true;

                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == RegexAttributeType.Name)
                            {
                                string Pattern = Convert.ToString(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);
                                if (isPosNoregexpatternApplicable == "Y")
                                {
                                    Pattern = posNoRegexPattern;
                                    ErrorMessage = ErrorMessage.Replace("Negative number is not allowed,", "").Replace("seven", "twenty-one");

                                }

                                if (Value != null && Value != "")
                                {
                                    if (!MatchRegexString(Convert.ToString(Value), Pattern))
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                    if (isdateField == true)
                                    {
                                        if (!MatchDateFormat(Convert.ToString(Value)))
                                        {
                                            objViewModel.ErrorOccured = true;
                                            ErrorList.Add(ErrorMessage + erroradd + " Please select date from date picker only in adobe acrobat reader dc");
                                        }
                                    }
                                }
                            }
                            if (Customitem.AttributeType.Name == StringLengthAttributeType.Name)
                            {
                                int Length = Convert.ToInt32(Customitem.ConstructorArguments[0].Value);
                                string ErrorMessage = Convert.ToString(Customitem.NamedArguments[0].TypedValue.Value);

                                if (Value != null)
                                {
                                    if (Convert.ToString(Value).Length > Length)
                                    {
                                        objViewModel.ErrorOccured = true;
                                        ErrorList.Add(ErrorMessage + erroradd);
                                    }
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(item.Name);
                        ErrorLog.LogError(ex, "CommonFunctions.FetchTetsReportPDFFilledDataToModel()");
                        ErrorList.Add("Page-1: Please contact OMMAS team." + ex.Message + ": " + pDFFiledDataInspectorModel.FieldName);
                    }
                }
                return ErrorList;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CommonFunctions.FetchTetsReportPDFFilledDataToModel()");
                //List<string> ErrorList = new List<string>();
                ErrorList.Add("Page-1: Error Occured while fetching data from pdf....Contact to Ommas team");
                return ErrorList;
            }
            finally
            {
                eformdbContext.Dispose();

            }
        }





    }
}