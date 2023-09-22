/*----------------------------------------------------------------------------------------
 * Project Id       :

 * Project Name     :OMMAS-II

 * File Name        :MasterContractorRegistrationViewModel.cs
 * 
 * Author           :Vikram Nandanwar

 * Creation Date    :01/May/2013

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
    public class MasterContractorRegistrationViewModel
    {

        [UIHint("hidden")]
        public string EncryptedRegCode { get; set; }

        public string EncryptedContractorCode { get; set; }


        [Required(ErrorMessage="Please Select Contractor/Supplier")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please Select Contractor/Supplier")]
        public int MAST_CON_ID { get; set; }
        public int MAST_REG_CODE { get; set; }

        //[Required(ErrorMessage = "Registration Number is required")]
        [Display(Name = "Registration Number ")]
        [RegularExpression("[a-zA-z0-9- _/():&.]{1,50}", ErrorMessage = "Registration Number is not in valid format")]
        [StringLength(50, ErrorMessage = "Registration Number must be less than 50 characters.")]            
        public string MAST_CON_REG_NO { get; set; }

        [Range(1, 2147483647, ErrorMessage = " Please select Class.")]
        [Display(Name = "Class")]
        public int MAST_CON_CLASS { get; set; }

        [Display(Name = "Valid From")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$",ErrorMessage="Start Date is not in valid format")]
        [DataType(DataType.Date)]       
        public string MAST_CON_VALID_FROM { get; set; }

        [Display(Name = "Valid To")]
        [FromToDateValidation("MAST_CON_VALID_FROM", ErrorMessage = "Valid to date must be greater than validity from date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [DataType(DataType.Date)]
        public string MAST_CON_VALID_TO { get; set; }

        [Range(1, 2147483647, ErrorMessage = " Please select State.")]
        [Display(Name = "State")]
        public int MAST_REG_STATE { get; set; }

        [Display(Name = "Office Name")]
        [RegularExpression("([a-zA-Z0-9- &._,()/]{2,255})", ErrorMessage = "Office Name is not in valid format.")]
        [StringLength(255, ErrorMessage = "Office Name must be less than 255 characters.")]   
        public string MAST_REG_OFFICE { get; set; }

        [Display(Name = "Status")]
        public string MAST_REG_STATUS { get; set; }
        
        public virtual MASTER_CON_CLASS_TYPE MASTER_CON_CLASS_TYPE { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }

        public string ContractorName { get; set; }
        public string ContractorClassType { get; set; }
        public string EncryptedContractor { get; set; }

        //Added on 22/09/2021
        [Required(ErrorMessage = "Fund Type is required")]
        [Display(Name = "Fund Type")]
        public string FUND_TYPE { get; set; }

        /// <summary>
        /// To get the types of the class
        /// </summary>
        public SelectList ClassTypes
        {
            get
            {
                List<PMGSY.Models.MASTER_CON_CLASS_TYPE> classTypeList = new List<MASTER_CON_CLASS_TYPE>();

                IMasterDAL objDAL = new MasterDAL();

                //classTypeList = objDAL.GetAllContClassByRegState(this.MAST_REG_STATE);

                classTypeList.Insert(0, new PMGSY.Models.MASTER_CON_CLASS_TYPE() { MAST_CON_CLASS = 0, MAST_CON_CLASS_TYPE_NAME = "--Select--" });

                return new SelectList(classTypeList, "MAST_CON_CLASS", "MAST_CON_CLASS_TYPE_NAME", this.MAST_CON_CLASS);
            }

        }

        /// <summary>
        /// To get the State Names
        /// </summary>
        public SelectList State
        {
            get
            {
                List<MASTER_STATE> stateList = new List<MASTER_STATE>();
                IMasterDAL objDAL = new MasterDAL();

                stateList = objDAL.getStateNameByRegStateCode();

                stateList.Insert(0, new PMGSY.Models.MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });

                return new SelectList(stateList, "MAST_STATE_CODE", "MAST_STATE_NAME");
            }

        }


    }
}