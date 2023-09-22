using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PMGSY.Models.MasterDataEntry
{
    public class MASTER_STATE
    {

        [UIHint("Hidden")]
    //    [Required(ErrorMessage = "State Code is required.")] 
        public string EncryptedStateCode { get; set; }
 
        public int MAST_STATE_CODE { get; set; }

        [Display(Name= "State Name")]
        [Required(ErrorMessage="State Name is required.")]
        [StringLength(50,ErrorMessage="State Name is not greater than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z ]+)$", ErrorMessage = "State Name is not in valid format.")]
        public string MAST_STATE_NAME { get; set; }

        [Display(Name = "Short Name")]
        [Required(ErrorMessage = "Short Name Name is required.")]
        [StringLength(2, ErrorMessage = "Short Name is not greater than 2 characters.")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Short Name is not in valid format.")]
        public string MAST_STATE_SHORT_NAME { get; set; }

        //[Display(Name = "State UT")]
        //[Required(ErrorMessage = "State UT is required.")]
        //[StringLength(1, ErrorMessage = "State UT is not greater than 1 character.")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "State UT is not in valid format.")]
        public string MAST_STATE_UT { get; set; }

        //[Display(Name = "State Type")]
        //[Required(ErrorMessage = "State Type is required.")]
        //[StringLength(1, ErrorMessage = "State Type is not greater than 1 character.")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "State Type is not in valid format.")]
        public string MAST_STATE_TYPE { get; set; }

        [Display(Name = "Census Code")]
        [Required(ErrorMessage = "Census Code is required.")]
        [Range(0, 2147483647, ErrorMessage = "Census Code is not in valid format.")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Census Code must be number.")]
        public Int32? MAST_NIC_STATE_CODE { get; set; }

        //[Display(Name = "Dummy State Code")]
        //[Required(ErrorMessage = "Dummy State Code is required.")]
        //[StringLength(2, ErrorMessage = "Dummy State Code is not greater than 2 character.")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Dummy State Code is not in valid format.")]
        public string DUMMY_STATE_CODE { get; set; }

        [Display(Name = "State/UT")]
        [Range(1, 2, ErrorMessage = "Please select State/UT.")]
        public byte StateUTID { get; set; }

        [Display(Name = "State Type")]
        [Range(1, 6, ErrorMessage = "Please select State Type.")]
        public byte StateTypeID { get; set; }

        public string MAST_STATE_ACTIVE { get; set; }
        
        //To get state Name 
        /// <summary>
        /// Master list of state details
        /// </summary>
        public SelectList StateUTs
        {
            get
            {
                List<StateUT> stateUTList = new List<StateUT>();
                int i=0;
                /*for (int i = 0; i < StateUT.lstStateUT.Count; i++)
                {
                    stateUTList.Insert(i, new StateUT() { StateUTID = (byte)i, StateUTDescription = StateUT.lstStateUT[i].Trim() });
                }*/

                foreach (var item in StateUT.lstStateUT)
                {
                    stateUTList.Add(new StateUT() { StateUTID = (byte)i, StateUTDescription = item.Value });
                    i++;
                }

                return new SelectList(stateUTList, "StateUTID", "StateUTDescription", this.StateUTID);
            }
        }

        public SelectList StateTypes
        {
            get
            {
                List<StateType> stateTypeList = new List<StateType>();
                int i = 0;
                //for (int i = 0; i < StateType.lstStateType.Count; i++)
                //{
                //    stateTypeList.Insert(i, new StateType() { StateTypeID = (byte)i, StateTypeDescription = StateType.lstStateType[i].Trim() });
                //}

                foreach (var item in StateType.lstStateType)
                {
                    stateTypeList.Add(new StateType() { StateTypeID = (byte)i, StateTypeDescription = item.Value });
                    i++;
                }

                return new SelectList(stateTypeList.OrderBy(o=>o.StateTypeDescription), "StateTypeID", "StateTypeDescription", this.StateUTID);
            }
        }

    }// end MASTER_STATE

    public class StateUT
    {
        public byte StateUTID { get; set; }
        public string StateUTDescription { get; set; }
       // public static readonly List<string> lstStateUT1 = new List<string>() { "--select--", "State", "Union Territory" };

        public static readonly Dictionary<string,string> lstStateUT = new Dictionary<string,string>() { {"0","--Select--"}, {"S","State"}, {"U","Union Territory"} };
    }

    public class StateType
    {
        public byte StateTypeID { get; set; }
        public string StateTypeDescription { get; set; }
        //public static readonly List<string> lstStateType = new List<string>() { "--select--", "Regular", "Island", "North East", "Hilly", "NorthEast & Hilly" };
        public static readonly Dictionary<string, string> lstStateType = new Dictionary<string, string>() { { "0", "--Select--" }, { "H", "Hilly" }, { "I", "Island" }, { "N", "North East" }, { "R", "Regular" }, { "X", "NorthEast & Hilly" },{"D","Desert"}};
    }
}