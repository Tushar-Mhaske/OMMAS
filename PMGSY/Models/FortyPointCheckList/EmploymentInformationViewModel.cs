
/*----------------------------------------------------------------------------------------
 * Project Id             :

 * Project Name           : OMMAS-II

 * File Name              : EmploymentInformationViewModel.cs
 
 * Author                 : Abhishek Kamble

 * Creation Date          : 20/Nov/2013

 * Desc                   : This class is used as model to apply server side validation for Forty point checklist - Employment Information details.
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.FortyPointChecklist;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.FortyPointCheckList
{
    public partial class EmploymentInformationViewModel
    {
        public string EncryptedEmploymentId { get; set; }
        public int MAST_STATE_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int TEND_EMPLOYMENT_ID { get; set; }
        public int MAST_CHECKLIST_POINTID { get; set; }
        
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Qualification")]       
        public int TEND_QUALIFICATION_ID { get; set; }

        [Required(ErrorMessage="Number of holders is required")]
        [RegularExpression("[0-9]{1,10}", ErrorMessage = "Number of holders should contains digits only and Maximum 10 digits are allowed.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter valid Number of holders")]               
        public int TEND_EMPLOYMENT_NUMBER { get; set; }

        public int CheckListId { get; set; }
        public string CheckListIssue { get; set; }

        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CHECKLIST_POINTS MASTER_CHECKLIST_POINTS { get; set; }
        public virtual MASTER_QUALIFICATION MASTER_QUALIFICATION { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
        
        //Populate Qualification    
        public SelectList Qualifications
        {
            get {
                List<SelectListItem> lstQualification = new List<SelectListItem>();
                IFortyPointChecklistDAL objDAL = new FortyPointChecklistDAL();
                lstQualification = objDAL.PopulateQualification();       
                return new SelectList(lstQualification, "Value", "Text");
            }                
        }
    }
}