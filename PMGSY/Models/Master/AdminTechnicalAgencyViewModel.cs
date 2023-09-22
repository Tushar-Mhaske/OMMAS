/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :AdminTechnicalAgencyViewModel.cs
 * 
 * Author           :Rohit Jadhav 

 * Creation Date    :14/May/2013

 * Desc             :This class is used to declare the variables, lists that are used in the Details form.
 * ---------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.Master;

namespace PMGSY.Models.Master
{
    public class AdminTechnicalAgencyViewModel
    {
       
        public string EncryptedTACode { get; set; }
        public int ADMIN_TA_CODE { get; set; }

        [Required]
        [RegularExpression("[SP]", ErrorMessage = "Please select Level.")]
        [Display(Name = "Level")]
        public string ADMIN_TA_TYPE { get; set; }
 
        [Display(Name = "Technical Agency Name")]
        [Required(ErrorMessage="Technical Agency Name is required.")]
        [RegularExpression("([a-zA-Z0-9.&,-;() ]{1,255})", ErrorMessage = "Technical Agency Name is not in valid format.")]
        [StringLength(255, ErrorMessage = "Technical Agency Name must be less than 255 characters.")]
        public string ADMIN_TA_NAME { get; set; }



        [Display(Name = "Service Tax Registration No.")]
        [RegularExpression("([a-zA-Z0-9.&,-;() ]{1,255})", ErrorMessage = "Service Tax Registration No. is not in valid format.")]
        [StringLength(26, ErrorMessage = "Service Tax Registration No. must be less than 25 characters.")]
        public string ADMIN_TA_SERVICE_TAX { get; set; }


        [Display(Name = "Address")]
        [RegularExpression("([a-zA-Z 0-9 &_()/;,-.]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_TA_ADDRESS1 { get; set; }


        [Display(Name = "Address")]
        [RegularExpression("([a-zA-Z 0-9 &_()/;,-.]{1,255})", ErrorMessage = "Address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters.")]
        public string ADMIN_TA_ADDRESS2 { get; set; }


        [Display(Name = "District")]
        public Nullable<int> MAST_DISTRICT_CODE { get; set; }

        [Display(Name = "State")]
        public Nullable<int> MAST_STATE_CODE { get; set; }

        [Display(Name = "PIN Code")]
        [RegularExpression("^([0-9]{6})?$", ErrorMessage = "PIN Code should contains 6 digits only.")]
        [StringLength(6, ErrorMessage = "PIN Code must be 6 digits only.")]
        public string ADMIN_TA_PIN { get; set; }

        [Display(Name = "Phone1")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(6, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]
        public string ADMIN_TA_STD1 { get; set; }

        [Display(Name = "Phone2")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]
        public string ADMIN_TA_STD2 { get; set; }


        [Display(Name = "-")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits.", MinimumLength = 6)]
        public string ADMIN_TA_PHONE1 { get; set; }


        [Display(Name = "-")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Phone Number should contains digits only.")]
        [StringLength(8, ErrorMessage = "Phone Number must be 6 to 8 digits.", MinimumLength = 6)]
        public string ADMIN_TA_PHONE2 { get; set; }

        [Display(Name = "Fax")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "STD Code is not in valid format.")]
        [StringLength(5, ErrorMessage = "STD Code must be 3 to 5 digits.", MinimumLength = 3)]
        public string ADMIN_TA_STD_FAX { get; set; }

        [Display(Name = "Fax")]
        [RegularExpression("^([0-9 ]+)$", ErrorMessage = "Fax Number should contains digits only.")]
        [StringLength(30, ErrorMessage = "Fax Number must be less than 30 digits.")]
        public string ADMIN_TA_FAX { get; set; }

        
        [Display(Name = "Mobile")]
        [RegularExpression("^[0-9]{10,11}", ErrorMessage = "Enter 10 digits Mobile number.")]
        public string ADMIN_TA_MOBILE_NO { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
        public string ADMIN_TA_EMAIL { get; set; }



        [Display(Name = "Website")]
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "Website address is not in valid format.")]
        [StringLength(255, ErrorMessage = "Website address must be less than 255 characters.")]
        public string ADMIN_TA_WEBISTE { get; set; }

      
        [Display(Name = "Designation")]
        public int? ADMIN_TA_CONTACT_DESG { get; set; }

        [Display(Name = "Contact  Name")]
        [RegularExpression(@"^([a-zA-Z . ()/]+)$", ErrorMessage = "Contact Name is not in valid format.")]
        [StringLength(255, ErrorMessage = "Contact Name  must be less than 255 chareacters.")]
        public string ADMIN_TA_CONTACT_NAME { get; set; }

        [Display(Name = "Remark")]
        [StringLength(255, ErrorMessage = "Remark must be less than 255 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._';/,\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public string ADMIN_TA_REMARKS { get; set; }

        public string ADMIN_DESIGNATION_NAME { get; set; }
        public string ADMIN_STATE_NAME { get; set; }
        public string ADMIN_DISTRICT_NAME { get; set; }
      
        public virtual ICollection<ADMIN_TA_STATE> ADMIN_TA_STATE { get; set; }
        public virtual MASTER_DESIGNATION MASTER_DESIGNATION { get; set; }
        public virtual MASTER_DISTRICT MASTER_DISTRICT { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }

        /// <summary>
        /// To Get Designation Names
        /// </summary>
        public SelectList Designation
        {
            get
            {
                List<MASTER_DESIGNATION> stateList = new List<MASTER_DESIGNATION>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.GetDesignationTA();
                stateList.Insert(0, new MASTER_DESIGNATION { MAST_DESIG_CODE = 0, MAST_DESIG_NAME = "--Select--" });
                return new SelectList(stateList, "MAST_DESIG_CODE", "MAST_DESIG_NAME");
            }

        }
        /// <summary>
        /// To Get State Names
        /// </summary>

        public SelectList States
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.GetStatesTA();
                stateList.Insert(0, new MASTER_STATE { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });
                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }

        }

        /// <summary>
        /// To Get District Names
        /// </summary>
        public SelectList Districts
        {
            get
            {
                List<MASTER_DISTRICT> stateList = new List<MASTER_DISTRICT>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.GetDistrictNameTA(this.MAST_STATE_CODE);
                    
                stateList.Insert(0, new MASTER_DISTRICT { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });
                return new SelectList(stateList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");
            }

        }

    }
}