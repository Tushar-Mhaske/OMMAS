using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Master;
using PMGSY.Models.Publication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.DAL.Publication
{
    public class PublicationDAL : IPublicationDAL
    {
        Models.PMGSYEntities dbContext;

        public PublicationDAL()
        {
            dbContext = new PMGSYEntities();
        }

        public List<PublicationCategory> GetCategoryListDAL()
        {

            List<PublicationCategory> categoryList = (from PubCat in dbContext.MASTER_PUBLICATION_CATEGORY
                                                      select new PublicationCategory { publicationCategoryCode = PubCat.MAST_PUB_CAT_CODE, publicationName = PubCat.MAST_PUB_CAT_NAME }).ToList<PublicationCategory>();
            categoryList.Add(new PublicationCategory { publicationCategoryCode = 0, publicationName = "All" });

            return categoryList.OrderBy(cat => cat.publicationName).ToList<PublicationCategory>(); ;

        }

        public Array GetPublicationListDAL(int publication, string published, string finalized, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            try
            {
                
              var  publicationList = (from pub in dbContext.MRD_PUBLICATIONS
                                      join cat in dbContext.MASTER_PUBLICATION_CATEGORY
                                      on pub.PUBLICATION_CAT_CODE equals cat.MAST_PUB_CAT_CODE
                                      where                                 
                                        (publication==0?1:pub.PUBLICATION_CAT_CODE)==(publication==0?1:publication) &&
                                        (published=="0"?"1": pub.PUBLICATION_STATUS) == (published=="0"?"1":published)&& 
                                        (finalized=="0"?"1": pub.PUBLICATION_IS_FINALIZED) ==(finalized=="0"?"1":finalized)                                 
                                   select new
                                   {
                                      pub.PUBLICATION_CODE,
                                      pub.PUBLICATION_TITLE,
                                      pub.PUBLICATION_AUTHOR,
                                      pub.PUBLICATION_DATE,
                                      pub.PUBLICATION_VOLUME,
                                      pub.PUBLICATION_NAME,
                                      pub.PUBLICATION_PAGINATION,
                                      pub.PUBLICATION_DESCRIPTION,
                                      pub.PUBLICATION_IS_FINALIZED,
                                      pub.PUBLICATION_STATUS,
                                      cat.MAST_PUB_CAT_NAME

                                   });


              
                totalRecords = publicationList.ToList().Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_PUB_CAT_NAME":
                                publicationList = publicationList.OrderBy(x => x.MAST_PUB_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_NAME":
                                publicationList = publicationList.OrderBy(x => x.PUBLICATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_AUTHOR":
                                publicationList = publicationList.OrderBy(x => x.PUBLICATION_AUTHOR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_DATE":
                                publicationList = publicationList.OrderBy(x => x.PUBLICATION_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_VOLUME":
                                publicationList = publicationList.OrderBy(x => x.PUBLICATION_VOLUME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                publicationList = publicationList.OrderBy(x => x.PUBLICATION_CODE).ThenBy(x => x.MAST_PUB_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                        //list = list.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_PUB_CAT_NAME":
                                publicationList = publicationList.OrderByDescending(x => x.MAST_PUB_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_NAME":
                                publicationList = publicationList.OrderByDescending(x => x.PUBLICATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_AUTHOR":
                                publicationList = publicationList.OrderByDescending(x => x.PUBLICATION_AUTHOR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_DATE":
                                publicationList = publicationList.OrderByDescending(x => x.PUBLICATION_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PUBLICATION_TITLE":
                                publicationList = publicationList.OrderByDescending(x => x.PUBLICATION_TITLE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                publicationList = publicationList.OrderByDescending(x => x.MAST_PUB_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                        //list = list.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    publicationList = publicationList.OrderBy(x => x.PUBLICATION_CODE).ThenBy(x => x.MAST_PUB_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = publicationList.Select(pub => new
                {
                    pub.PUBLICATION_CODE,
                    pub.PUBLICATION_TITLE,
                    pub.PUBLICATION_AUTHOR,
                    pub.PUBLICATION_DATE,
                    pub.PUBLICATION_VOLUME,
                    pub.PUBLICATION_NAME,
                    pub.PUBLICATION_PAGINATION,
                    pub.PUBLICATION_DESCRIPTION,
                    pub.PUBLICATION_IS_FINALIZED,
                    pub.PUBLICATION_STATUS,
                    pub.MAST_PUB_CAT_NAME
                }).ToArray();
                return result.Select(publicationDetail => new
                {

                    id = publicationDetail.PUBLICATION_CODE.ToString(),
                    cell = new[] {           
                                        
                                        publicationDetail.PUBLICATION_CODE.ToString(),
                                        publicationDetail.MAST_PUB_CAT_NAME,
                                        publicationDetail.PUBLICATION_TITLE,
                                        publicationDetail.PUBLICATION_AUTHOR,
                                        Convert.ToDateTime(publicationDetail.PUBLICATION_DATE).ToString("dd/MM/yyyy"),
                                        publicationDetail.PUBLICATION_VOLUME,
                                        publicationDetail.PUBLICATION_NAME,
                                        publicationDetail.PUBLICATION_PAGINATION,                                        
                                        publicationDetail.PUBLICATION_DESCRIPTION,
                                        publicationDetail.PUBLICATION_IS_FINALIZED=="N"?"<a href='#' title='Click here to upload publication details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ShowPubUpload(" +  publicationDetail.PUBLICATION_CODE.ToString()  +"); return false;'>Upload</a>":"<a href='#' title='Click here to view publication details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowPubUpload(" +  publicationDetail.PUBLICATION_CODE.ToString()  +"); return false;'>View</a>",
                                        publicationDetail.PUBLICATION_IS_FINALIZED=="N"?"<a href='#' title='Click here to edit publication details' class='ui-icon ui-icon-pencil ui-align-center' onClick='ShowPublication(" +  publicationDetail.PUBLICATION_CODE.ToString()  +"); return false;'>Edit</a>":"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",
                                        publicationDetail.PUBLICATION_IS_FINALIZED=="N"?"<a href='#' title='Click here to delete publication details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePublication(" +  publicationDetail.PUBLICATION_CODE.ToString()  +",\"D\"); return false;'>Delete</a>":"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",
                                        publicationDetail.PUBLICATION_IS_FINALIZED=="N"?"<a href='#' title='Click here to finalize publication details' class='ui-icon ui-icon-locked ui-align-center' onClick='FinalizePublication(" +  publicationDetail.PUBLICATION_CODE.ToString().Trim() + ",\""+ publicationDetail.PUBLICATION_IS_FINALIZED +"\",\""+publicationDetail.PUBLICATION_STATUS+"\"); return false;'>Finalize Publication</a>":"<a href='#'  title='Click here to de-finalize publication details' class='ui-icon ui-icon-unlocked ui-align-center'  onClick='FinalizePublication(" +  publicationDetail.PUBLICATION_CODE.ToString().Trim() + ",\""+ publicationDetail.PUBLICATION_IS_FINALIZED +"\",\""+publicationDetail.PUBLICATION_STATUS+"\"); return false;'>De-finalize Publication</a>",
                                        publicationDetail.PUBLICATION_STATUS=="N"?"<a href='#' title='Click here to publish publication details' class='ui-icon ui-icon-locked ui-align-center' onClick='PublishedPublication(" +  publicationDetail.PUBLICATION_CODE.ToString().Trim() + ",\""+ publicationDetail.PUBLICATION_STATUS+ "\",\""+publicationDetail.PUBLICATION_IS_FINALIZED+"\"); return false;'>Published Publication</a>":"<a href='#' title='Click here to un-publish publication details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='PublishedPublication(" +  publicationDetail.PUBLICATION_CODE.ToString().Trim() + ",\""+ publicationDetail.PUBLICATION_STATUS+ "\",\""+publicationDetail.PUBLICATION_IS_FINALIZED+"\"); return false;'>Un-publish Publication</a>"
                                        
                               }
                }).ToArray();
               
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public bool PublicationAddEditDAL(MRD_PUBLICATIONS publication, string action, ref string message)
        {
            try
            {

                if (action == "A")
                {

                    //publication.PUBLICATION_CODE  =  (from c in dbContext.MRD_PUBLICATIONS
                    //                                        select (Int32?)c.PUBLICATION_CODE ?? 1).Max() + 1;
                    publication.PUBLICATION_CODE = dbContext.MRD_PUBLICATIONS.Max(cp => (Int32?)cp.PUBLICATION_CODE) == null ? 1 : (Int32)dbContext.MRD_PUBLICATIONS.Max(cp => (Int32?)cp.PUBLICATION_CODE) + 1;
                    publication.MASTER_PUBLICATION_CATEGORY = dbContext.MASTER_PUBLICATION_CATEGORY.Where(cat => cat.MAST_PUB_CAT_CODE == publication.PUBLICATION_CAT_CODE).FirstOrDefault();
                    publication.UM_User_Master = dbContext.UM_User_Master.Where(cat => cat.UserID == publication.USERID).FirstOrDefault();


                    dbContext.MRD_PUBLICATIONS.Add(publication);
                    dbContext.SaveChanges();
                    message = "Success";
                    return true;
                }
                else if (action == "E")
                {
                    var publicationToEdit = dbContext.MRD_PUBLICATIONS.Where(pub => pub.PUBLICATION_CODE == publication.PUBLICATION_CODE).FirstOrDefault();
                    publicationToEdit.PUBLICATION_AUTHOR = publication.PUBLICATION_AUTHOR;
                    publicationToEdit.PUBLICATION_CAT_CODE = publication.PUBLICATION_CAT_CODE;
                    publicationToEdit.PUBLICATION_DATE = publication.PUBLICATION_DATE;
                    publicationToEdit.PUBLICATION_DESCRIPTION = publication.PUBLICATION_DESCRIPTION;
                    publicationToEdit.PUBLICATION_NAME = publication.PUBLICATION_NAME;
                    publicationToEdit.PUBLICATION_PAGINATION = publication.PUBLICATION_PAGINATION;
                    publicationToEdit.PUBLICATION_TITLE = publication.PUBLICATION_TITLE;
                    publicationToEdit.PUBLICATION_VOLUME = publication.PUBLICATION_VOLUME;

                    dbContext.Entry(publicationToEdit).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Success";
                    return true;
                }

                message = "Failed";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                message = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }

            catch (DbEntityValidationException dbEx)
            {
                message = dbEx.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.Current);
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }

            catch (Exception ex)
            {
                message = ex.Message;
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
        public bool PublicationActionDAL(int pubId, string action, ref string message)
        {
            try
            {
                MRD_PUBLICATIONS publication = dbContext.MRD_PUBLICATIONS.Where(pub => pub.PUBLICATION_CODE == pubId).FirstOrDefault();
                if (action == "D") // Delete Record
                {
                    foreach (MRD_PUBLICATION_FILES publicationFile in dbContext.MRD_PUBLICATION_FILES.Where(pubFile => pubFile.PUBLICATION_CODE == publication.PUBLICATION_CODE).ToList())
                    {
                        string PhysicalPath = ConfigurationManager.AppSettings["PUBLICATION_FILE_UPLOAD"];
                        //  string ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), publicationFile.PUBLICATION_FILE_NAME);
                        PhysicalPath = Path.Combine(PhysicalPath, publicationFile.PUBLICATION_FILE_NAME);
                        if (System.IO.File.Exists(PhysicalPath))
                        {
                            System.IO.File.Delete(PhysicalPath);
                            // System.IO.File.Delete(ThumbnailPath);
                        }
                        dbContext.MRD_PUBLICATION_FILES.Remove(publicationFile);

                    }
                    dbContext.SaveChanges();
                    dbContext.MRD_PUBLICATIONS.Remove(publication);

                }
                else
                {
                    switch (action)
                    {
                        case "FY":
                            publication.PUBLICATION_IS_FINALIZED = "N";// De finalized Record
                            break;
                        case "FN":
                            publication.PUBLICATION_IS_FINALIZED = "Y";// Finalized Record
                            break;
                        case "PY":
                            publication.PUBLICATION_STATUS = "N";// Published Record
                            break;
                        case "PN":
                            publication.PUBLICATION_STATUS = "Y";// Un Published Record
                            break;
                    }
                    dbContext.Entry(publication).State = System.Data.Entity.EntityState.Modified;
                }
                dbContext.SaveChanges();
                message = "Success";
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                message = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }

            catch (DbEntityValidationException dbEx)
            {
                message = dbEx.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.Current);
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }

            catch (Exception ex)
            {
                message = ex.Message;
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


        public MRD_PUBLICATIONS GetPublicationDAL(int pubId)
        {
            return dbContext.MRD_PUBLICATIONS.Where(pub => pub.PUBLICATION_CODE == pubId).FirstOrDefault();
        }

        public Array ListPublicationFileDAL(int pubId, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            try
            {
                List<MRD_PUBLICATION_FILES> publicationFileList = new List<MRD_PUBLICATION_FILES>();

                publicationFileList = dbContext.MRD_PUBLICATION_FILES.Where(pub => pub.PUBLICATION_CODE == pubId).ToList();


                totalRecords = publicationFileList.Count;
                publicationFileList = publicationFileList.OrderBy(x => x.PUBLICATION_CODE).ThenBy(x => x.PUBLICATION_FILE_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
           
                return publicationFileList.Select(pubFile => new
                {

                    id = pubFile.PUBLICATION_FILE_CODE.ToString(),
                    cell = new[] {           
                                        
                                        pubFile.PUBLICATION_FILE_CODE.ToString(),
                                        pubFile.PUBLICATION_UPLOAD_DATE.ToString("dd/MM/yyyy").Replace('/','-'),                         
                                        pubFile.PUBLICATION_FILE_NAME
                                        
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string AddPublicationFileUploadDAL(MRD_PUBLICATION_FILES lst_pub_files)
        {
            try
            {

                Int32? MaxID;


                if (dbContext.MRD_PUBLICATION_FILES.Count() == 0)
                {
                    MaxID = 0;

                }
                else
                {
                    MaxID = (from c in dbContext.MRD_PUBLICATION_FILES select (Int32?)c.PUBLICATION_FILE_CODE ?? 0).Max();

                }
                ++MaxID;
                var fileCount = (from c in dbContext.MRD_PUBLICATION_FILES
                                 where c.PUBLICATION_CODE == lst_pub_files.PUBLICATION_CODE
                                 select (Int32?)c.PUBLICATION_FILE_CODE ?? 0).Count();

                lst_pub_files.PUBLICATION_FILE_CODE = Convert.ToInt32(MaxID);


                dbContext.MRD_PUBLICATION_FILES.Add(lst_pub_files);

                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Your Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

    }

    public interface IPublicationDAL
    {
        List<PublicationCategory> GetCategoryListDAL();
        Array GetPublicationListDAL(int publication, string published, string finalized, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        bool PublicationAddEditDAL(MRD_PUBLICATIONS publication, string action, ref string message);
        MRD_PUBLICATIONS GetPublicationDAL(int pubId);
        bool PublicationActionDAL(int pubId, string action, ref string message);
        Array ListPublicationFileDAL(int pubId, int? page, int? rows, string sidx, string sord, out Int32 totalRecords);
        string AddPublicationFileUploadDAL(MRD_PUBLICATION_FILES lst_pub_files);

    }
}