using PMGSY.GepnicAndNrega;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PMGSY.NERAGA
{
    /// <summary>
    /// Summary description for OmmasIntegration
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "https://online.omms.nic.in/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OmmasIntegration : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetProposalData(string agencyFlag, string state, string sanctionYear, string batch, string collaboration, string propType, string scheme, out string uniqueRefNo , out int cntTotalRecords)
        {
            OmmasIntegrationDAL objDAL = new OmmasIntegrationDAL();
            return objDAL.GetProposalData(agencyFlag, state, sanctionYear, batch, collaboration, propType, scheme, out uniqueRefNo, out cntTotalRecords);
        }

        // new parameter cntTotalRecords added by anita

        //[WebMethod]
        //public string GetProposalData(string agencyFlag, string state, string sanctionYear, string batch, string collaboration, string propType, string scheme, out string uniqueRefNo)
        //{
        //    int cntTotalRecords = 0;
        //    OmmasIntegrationDAL objDAL = new OmmasIntegrationDAL();
        //    return objDAL.GetProposalData(agencyFlag, state, sanctionYear, batch, collaboration, propType, scheme, out uniqueRefNo, out cntTotalRecords);
        //}

    }
}
