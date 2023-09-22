using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Proposal
{
    public class RevisedCostLengthViewModel
    {
        public int IMS_PR_ROAD_CODE { get; set; }
        public int IMS_REVISION_CODE { get; set; }


        [Display(Name="Bridge/Road Length")]
        public decimal IMS_OLD_LENGTH { get; set; }

        [Display(Name="Old Pavement Cost")]
        public decimal IMS_OLD_PAV_COST { get; set; }

        [Display(Name = "Old CDWorks Cost")]
        public decimal IMS_OLD_CD_COST { get; set; }

        [Display(Name = "Old Protection Works Cost")]
        public decimal IMS_OLD_PW_COST { get; set; }

        [Display(Name = "Old Other Works Cost")]
        public decimal IMS_OLD_OW_COST { get; set; }

        [Display(Name = "Old Bridge Works Cost")]
        public decimal IMS_OLD_BW_COST { get; set; }

        [Display(Name = "Old State Share Cost")]
        public decimal IMS_OLD_RS_COST { get; set; }

        [Display(Name = "Old State Share Cost")]
        public decimal IMS_OLD_BS_COST { get; set; }

        [Display(Name = "Old Higher Specification Cost")]
        public decimal IMS_OLD_HS_COST { get; set; }

        [Display(Name = "Old Furniture Cost")]
        public decimal IMS_OLD_FC_COST { get; set; }

        [Display(Name = "Old Maintenance Cost(Year 1)")]
        public decimal IMS_OLD_MAINT1 { get; set; }
        [Display(Name = "Old Maintenance Cost(Year 2)")]
        public decimal IMS_OLD_MAINT2 { get; set; }
        [Display(Name = "Old Maintenance Cost(Year 3)")]
        public decimal IMS_OLD_MAINT3 { get; set; }
        [Display(Name = "Old Maintenance Cost(Year 4)")]
        public decimal IMS_OLD_MAINT4 { get; set; }
        [Display(Name = "Old Maintenance Cost(Year 5)")]
        public decimal IMS_OLD_MAINT5 { get; set; }

        [Display(Name = "Old Renewal Cost(Year 6)")]
        public decimal IMS_OLD_MAINT6 { get; set; }

        [Display(Name = "Bridge/Road Length")]
        //[CompareLength("IMS_PR_ROAD_CODE")]
        [Range(0,999999.9999,ErrorMessage="Length is invalid.")]
        public decimal IMS_NEW_LENGTH { get; set; }

        [Display(Name = "New Pavement Cost")]
        [Range(0, 999999.9999, ErrorMessage = "Pavement Cost is invalid.")]
        public decimal IMS_NEW_PAV_COST { get; set; }

        [Display(Name = "New CDWorks Cost")]
        [Range(0, 999999.9999, ErrorMessage = "CDWorks Cost is invalid.")]
        public decimal IMS_NEW_CD_COST { get; set; }

        [Display(Name = "New Protection Works Cost")]
        [Range(0, 999999.9999, ErrorMessage = "Protection Works Cost is invalid.")]
        public decimal IMS_NEW_PW_COST { get; set; }

         [Display(Name = "New Other Works Cost")]
         [Range(0, 999999.9999, ErrorMessage = "Other Works Cost is invalid.")]
        public decimal IMS_NEW_OW_COST { get; set; }

        [Display(Name = "New Bridge Works Cost")]
        [Range(0, 999999.9999, ErrorMessage = "Bridge Works Cost is invalid.")]
        public decimal IMS_NEW_BW_COST { get; set; }

        [Display(Name = "New State Share Cost")]
        [Range(0, 999999.9999, ErrorMessage = "State Share Cost is invalid.")]
        public decimal IMS_NEW_RS_COST { get; set; }

        [Display(Name = "New Higher Specification Cost")]
        [Range(0, 999999.9999, ErrorMessage = "Higher Specification Cost is invalid.")]
        public decimal IMS_NEW_HS_COST { get; set; }

        [Display(Name = "New Furniture Cost")]
        [Range(0, 999999.9999, ErrorMessage = "Furniture Cost is invalid.")]
        public decimal IMS_NEW_FC_COST { get; set; }

        [Display(Name = "New State Share Cost")]
        [Range(0, 999999.9999, ErrorMessage = "State Share Cost is invalid.")]
        public decimal IMS_NEW_BS_COST { get; set; }

        [Display(Name = "New Maintenance Cost(Year 1)")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost(Year 1) is invalid.")]
        public decimal IMS_NEW_MAINT1 { get; set; }
        
        [Display(Name = "New Maintenance Cost(Year 2)")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost(Year 2) is invalid.")]
        public decimal IMS_NEW_MAINT2 { get; set; }
        
        [Display(Name = "New Maintenance Cost(Year 3)")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost(Year 3) is invalid.")]
        public decimal IMS_NEW_MAINT3 { get; set; }
        
        [Display(Name = "New Maintenance Cost(Year 4)")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost(Year 4) is invalid.")]
        public decimal IMS_NEW_MAINT4 { get; set; }
        
        [Display(Name = "New Maintenance Cost(Year 5)")]
        [Range(0, 999999.9999, ErrorMessage = "Maintenance Cost(Year 5) is invalid.")]
        public decimal IMS_NEW_MAINT5 { get; set; }

        [Display(Name = "New Renewal Cost(Year 6)")]
        [Range(0, 999999.9999, ErrorMessage = "Renewal Cost(Year 6) is invalid.")]
        public decimal IMS_NEW_MAINT6 { get; set; }

        [Display(Name = "Old State Share")]
        public decimal Scheme2OldStateShare { get; set; }

        [Display(Name = "Old MoRD Share")]
        public decimal Scheme2OldMoRDShare { get; set; }

        public string IMS_REVISION_DATE { get; set; }

        public string IMS_REVISION_STATUS { get; set; }

        public Int16? SharePercent { get; set; }

        public string ProposalType { get; set; }
        public string Operation { get; set; }
        public string AreaType { get; set; }
        public string UpgradeConnect { get; set; }


        public virtual IMS_SANCTIONED_PROJECTS IMS_SANCTIONED_PROJECTS { get; set; }


    }


    public class CompareLength : ValidationAttribute//, IClientValidatable
    {
        private readonly string PropertyName;
        //private readonly string PropertyLength;


        public CompareLength(string propertyName)//, string propertyLength)
        {
            this.PropertyName = propertyName;
            //this.PropertyLength = propertyLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this.PropertyName);
            //var propertyTestedLength = validationContext.ObjectType.GetProperty(this.PropertyLength);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", this.PropertyName));
            }

            int proposalCode = Convert.ToInt32(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            //decimal length = Convert.ToDecimal(propertyTestedInfo.GetValue(validationContext.ObjectInstance, null));
            decimal length = 0;
            if (value != null)
            {
                length = Convert.ToDecimal(value);
            }

            if (proposalCode != null)
            {
                PMGSYEntities db = new PMGSYEntities();
                IMS_SANCTIONED_PROJECTS sancMaster = db.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                string surfaceType = db.MASTER_STATE.Where(m => m.MAST_STATE_CODE == sancMaster.MAST_STATE_CODE).Select(m => m.MAST_STATE_TYPE).FirstOrDefault();
                decimal change = 0;
                if (sancMaster.IMS_UPGRADE_CONNECT == "N")
                {
                    if (sancMaster.IMS_PROPOSAL_TYPE == "P")
                    {
                        if (surfaceType == "R")
                        {
                            if (length > sancMaster.IMS_PAV_LENGTH)
                            {
                                change = length - sancMaster.IMS_PAV_LENGTH;
                                if (change <= (sancMaster.IMS_PAV_LENGTH / 5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length must be less than equal to 20% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_PAV_LENGTH - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length changed should be greater than 0.5 Km.");
                                }
                            }
                        }
                        else if (surfaceType == "N" || surfaceType == "X" || surfaceType == "H")
                        {
                            if (length > sancMaster.IMS_PAV_LENGTH)
                            {
                                change = length - sancMaster.IMS_PAV_LENGTH;
                                if (change <= (sancMaster.IMS_PAV_LENGTH * 3 / 10))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length must be less than equal to 30% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_PAV_LENGTH - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length changed should be greater than 500 Mtrs.");
                                }
                            }
                        }

                    }
                    else
                    {
                        if (surfaceType == "R")
                        {
                            if (length > sancMaster.IMS_BRIDGE_LENGTH)
                            {
                                change = length - sancMaster.IMS_BRIDGE_LENGTH.Value;
                                if (change <= (sancMaster.IMS_BRIDGE_LENGTH / 5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length must be less than equal to 20% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_BRIDGE_LENGTH.Value - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length changed should be greater than 500 Mtrs.");
                                }
                            }
                        }
                        else if (surfaceType == "N" || surfaceType == "X" || surfaceType == "H")
                        {
                            if (length > sancMaster.IMS_BRIDGE_LENGTH)
                            {
                                change = length - sancMaster.IMS_BRIDGE_LENGTH.Value;
                                if (change <= (sancMaster.IMS_BRIDGE_LENGTH * 3 / 10))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length must be less than equal to 30% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_BRIDGE_LENGTH.Value - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length changed should be greater than 500 Mtrs.");
                                }
                            }
                        }
                    }
                }
                else if (sancMaster.IMS_UPGRADE_CONNECT == "U")
                {
                    if (sancMaster.IMS_PROPOSAL_TYPE == "P")
                    {
                        if (surfaceType == "R")
                        {
                            if (length > sancMaster.IMS_PAV_LENGTH)
                            {
                                change = length - sancMaster.IMS_PAV_LENGTH;
                                if (change <= (sancMaster.IMS_PAV_LENGTH / 10))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length must be less than equal to 10% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_PAV_LENGTH - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length changed should be greater than 0.5 Km.");
                                }
                            }
                        }
                        else if (surfaceType == "N" || surfaceType == "X" || surfaceType == "H")
                        {
                            if (length > sancMaster.IMS_PAV_LENGTH)
                            {
                                change = length - sancMaster.IMS_PAV_LENGTH;
                                if (change <= (sancMaster.IMS_PAV_LENGTH / 5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length must be less than equal to 20% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_PAV_LENGTH - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Pavement length changed should be greater than 500 Mtrs.");
                                }
                            }
                        }

                    }
                    else
                    {
                        if (surfaceType == "R")
                        {
                            if (length > sancMaster.IMS_BRIDGE_LENGTH)
                            {
                                change = length - sancMaster.IMS_BRIDGE_LENGTH.Value;
                                if (change <= (sancMaster.IMS_BRIDGE_LENGTH / 5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length must be less than equal to 30% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_BRIDGE_LENGTH.Value - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length changed should be greater than 500 Mtrs.");
                                }
                            }
                        }
                        else if (surfaceType == "N" || surfaceType == "X" || surfaceType == "H")
                        {
                            if (length > sancMaster.IMS_BRIDGE_LENGTH)
                            {
                                change = length - sancMaster.IMS_BRIDGE_LENGTH.Value;
                                if (change <= (sancMaster.IMS_BRIDGE_LENGTH * 3 / 10))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length must be less than equal to 30% of proposal length.");
                                }
                            }
                            else
                            {
                                change = sancMaster.IMS_BRIDGE_LENGTH.Value - length;
                                if (length > Convert.ToDecimal(0.5))
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("New Bridge length changed should be greater than 500 Mtrs.");
                                }
                            }
                        }
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var rule = new ModelClientValidationRule
        //    {
        //        ErrorMessage = this.ErrorMessageString,
        //        ValidationType = "comparefinancialvalidationwork"
        //    };
        //    //rule.ValidationParameters["compareworkpayment"] = this.PropertyName;
        //    yield return rule;
        //}
    }

}