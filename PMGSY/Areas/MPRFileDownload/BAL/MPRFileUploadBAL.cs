using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Areas.MPRFileDownload.Models;
using PMGSY.Areas.MPRFileDownload.DAL;
namespace PMGSY.Areas.MPRFileDownload.BAL
{
    public class MPRFileUploadBAL : IMPRFileUploadBAL
    {
        private IMPRFileUploadDAL objDAL = null;
         
        private IMPRFileUploadBAL objBAL = null;

        public Array ListMPRFileUpload(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new MPRFileUploadDAL();
            return objDAL.ListMPRFileUploadDAL(stateCode, page, rows, sidx, sord, out totalRecords);

        }


    }

    public interface IMPRFileUploadBAL
    {
        //ListHabs
        Array ListMPRFileUpload(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);


    }
}