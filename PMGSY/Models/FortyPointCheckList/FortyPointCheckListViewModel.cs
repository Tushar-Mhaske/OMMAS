
/*----------------------------------------------------------------------------------------
 * Project Id             :

 * Project Name           : OMMAS-II

 * File Name              : FortyPointCheckListViewModel.cs
 
 * Author                 : Abhishek Kamble

 * Creation Date          : 20/Nov/2013

 * Desc                   : This class is used as model to apply server side validation for Forty point checklist.
 
 * ---------------------------------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.FortyPointCheckList
{
    public class FortyPointCheckListViewModel
    {   
        public int MAST_STATE_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int MAST_CHECKLIST_POINTID { get; set; }

        [RegularExpression("[A-Za-z0-9- .()]{1,2000}", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string MAST_ACTION_TAKEN { get; set; }

        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CHECKLIST_POINTS MASTER_CHECKLIST_POINTS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}