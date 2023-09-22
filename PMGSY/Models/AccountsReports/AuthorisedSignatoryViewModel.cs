using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.Extensions;
namespace PMGSY.Models.AccountsReports
{
    public class AuthorisedSignatoryViewModel
    {

        [Display(Name="Select DPIU")]
        public int DPIU { set; get; }


        /// <summary>
        /// Populate DPIU
        /// </summary>        
        public SelectList ListDPIU
        {
            get {

                List<SelectListItem> lstDPIU = new List<SelectListItem>();
                CommonFunctions objCommonFunction = new CommonFunctions();

                //populate DPIU
                TransactionParams objParam = new TransactionParams();
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                lstDPIU=objCommonFunction.PopulateDPIU(objParam);
                lstDPIU.RemoveAt(0);
                lstDPIU.Insert(0, new SelectListItem() { Value = "0", Text="All DPIU" });

                return new SelectList(lstDPIU, "Value", "Text");    
            }     
        }

        //lstAuthorisedSignatoryDetails List 
        public List<SP_ACC_RPT_LIST_AUTH_SIGNATORY_DETAILS_Result> lstAuthSignatoryDetails { get; set; }
    }
}