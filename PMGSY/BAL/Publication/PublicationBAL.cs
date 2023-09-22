using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.Publication;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Common;


namespace PMGSY.BAL.Publication
{
    public class PublicationBAL : IPublicationBAL
    {
        PMGSY.DAL.Publication.IPublicationDAL publicationDAL;
        public PublicationBAL()
        {
            publicationDAL = new DAL.Publication.PublicationDAL();
        }

        public List<PublicationCategory> GetCategoryListBAL()
        {
            return publicationDAL.GetCategoryListDAL();
        }
        public Array GetPublicationListBAL(int publication, string published, string finalized, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            return publicationDAL.GetPublicationListDAL(publication, published, finalized, page, rows, sidx, sord, out totalRecords);
        }

        public bool PublicationAddEditBAL(PublicationViewModel pubVewModel, ref string message)
        {
            //Setting culture to get date as mm/dd/yyyy
            IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
            string strpublicationDate = string.Empty;
            if (pubVewModel.Date_Type == "D")
            {
                strpublicationDate = DateTime.Parse(pubVewModel.publicationDate, culture, System.Globalization.DateTimeStyles.AssumeLocal).ToString();
            }
            else if (pubVewModel.Date_Type == "Y")
            {
                strpublicationDate = "01/01/" + pubVewModel.Year;
            }
            else if (pubVewModel.Date_Type == "M")
            {
                strpublicationDate = "01/" + pubVewModel.Month + "/" + pubVewModel.Year;
            }

            MRD_PUBLICATIONS publication = new MRD_PUBLICATIONS
            {
                PUBLICATION_CODE = pubVewModel.publicationCode,
                PUBLICATION_AUTHOR = pubVewModel.publicationAuther,
                PUBLICATION_CAT_CODE = pubVewModel.publicationCategoryCode,
                PUBLICATION_PERIOD = pubVewModel.Date_Type,
                PUBLICATION_DATE = DateTime.Parse(strpublicationDate, culture, System.Globalization.DateTimeStyles.AssumeLocal),
                PUBLICATION_DESCRIPTION = pubVewModel.publicationDescription,
                PUBLICATION_NAME = pubVewModel.publicationName,
                PUBLICATION_PAGINATION = pubVewModel.publicationPagination,
                PUBLICATION_TITLE = pubVewModel.publicationTitle,
                PUBLICATION_VOLUME = pubVewModel.publicationVolume,
                USERID = PMGSYSession.Current.UserId,
                IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]

            };
            if (pubVewModel.Action == "A")
            {
                publication.PUBLICATION_IS_FINALIZED = "N";
                publication.PUBLICATION_STATUS = "N";
            }
            return publicationDAL.PublicationAddEditDAL(publication, pubVewModel.Action, ref message);
        }

        public PublicationViewModel GetPublicationBAL(int pubId)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            MRD_PUBLICATIONS publication = publicationDAL.GetPublicationDAL(pubId);
            string strpublicationDate = publication.PUBLICATION_DATE.ToShortDateString();
            int year = 0;
            int month = 0;
            String[] splitDate = strpublicationDate.Split('/');
            if (publication.PUBLICATION_PERIOD == "Y")
            {
                year = Convert.ToInt32(splitDate[2]);
            }
            else if (publication.PUBLICATION_PERIOD == "M")
            {
                year = Convert.ToInt32(splitDate[2]);
                month = Convert.ToInt32(splitDate[1]);
            }
            return new PublicationViewModel
            {
                publicationCode = publication.PUBLICATION_CODE,
                publicationAuther = publication.PUBLICATION_AUTHOR,
                publicationCategoryCode = publication.PUBLICATION_CAT_CODE,
                Date_Type = publication.PUBLICATION_PERIOD,
                publicationDate = publication.PUBLICATION_DATE.ToShortDateString(),
                publicationDescription = publication.PUBLICATION_DESCRIPTION,
                publicationFinalized = publication.PUBLICATION_IS_FINALIZED,
                publicationName = publication.PUBLICATION_NAME,
                publicationPagination = publication.PUBLICATION_PAGINATION,
                publicationStatus = publication.PUBLICATION_STATUS,
                publicationTitle = publication.PUBLICATION_TITLE,
                publicationVolume = publication.PUBLICATION_VOLUME,
                Year=year,
                Month=month
            };

        }
        public bool PublicationActionBAL(int pubId, string action, ref string message)
        {

            return publicationDAL.PublicationActionDAL(pubId, action, ref message);
        }

        public Array ListPublicationFileBAL(int pubId, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            return publicationDAL.ListPublicationFileDAL(pubId, page, rows, sidx, sord, out totalRecords);
        }

        public string AddPublicationFileUploadBAL(List<PMGSY.Models.Publication.PublicationUploadViewModel> publicationUploadViewModel)
        {


            //List<QUALITY_QM_INSPECTION_FILE> lst_qm_inspection_files = new List<QUALITY_QM_INSPECTION_FILE>();
            try
            {
                // Image Upload
                MRD_PUBLICATION_FILES pubFiles = new MRD_PUBLICATION_FILES();
                foreach (PublicationUploadViewModel pubModel in publicationUploadViewModel)
                {
                    pubFiles.PUBLICATION_CODE = pubModel.publicationCode.Value;
                    pubFiles.PUBLICATION_FILE_NAME = pubModel.publicationName;
                    pubFiles.PUBLICATION_UPLOAD_DATE = DateTime.Now;
                    pubFiles.USERID = PMGSYSession.Current.UserId;
                    pubFiles.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    publicationDAL.AddPublicationFileUploadDAL(pubFiles);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }

        }

    }

    public interface IPublicationBAL
    {
        List<PublicationCategory> GetCategoryListBAL();
        Array GetPublicationListBAL(int publication, string published, string finalized, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        bool PublicationAddEditBAL(PublicationViewModel publication, ref string message);
        PublicationViewModel GetPublicationBAL(int pubId);
        bool PublicationActionBAL(int pubId, string action, ref string message);
        Array ListPublicationFileBAL(int pubId, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        string AddPublicationFileUploadBAL(List<PMGSY.Models.Publication.PublicationUploadViewModel> publicationUploadViewModel);
    }
}
